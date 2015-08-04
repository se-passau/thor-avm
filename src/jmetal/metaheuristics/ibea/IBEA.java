//  IBEA.java
//
//  Author:
//       Antonio J. Nebro <antonio@lcc.uma.es>
//       Juan J. Durillo <durillo@lcc.uma.es>
//
//  Copyright (c) 2011 Antonio J. Nebro, Juan J. Durillo
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

package jmetal.metaheuristics.ibea;

import java.io.BufferedWriter;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;
import java.util.Scanner;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import jmetal.core.*;
import jmetal.encodings.variable.Binary;
import jmetal.util.comparators.DominanceComparator;
import jmetal.util.comparators.FitnessComparator;
import jmetal.qualityIndicator.Epsilon;
import jmetal.qualityIndicator.Hypervolume;
import jmetal.qualityIndicator.QualityIndicator;
import jmetal.util.*;

/**
 * This class implementing the IBEA algorithm
 */
public class IBEA extends Algorithm{

  /**
   * Defines the number of tournaments for creating the mating pool
   */
  public static final int TOURNAMENTS_ROUNDS = 1;

  /**
   * Stores the value of the indicator between each pair of solutions into
   * the solution set
   */
  private List<List<Double>> indicatorValues_;

  /**
   *
   */
  private double maxIndicatorValue_;
  
  private final ExecutorService executorPool = Executors.newFixedThreadPool(4);
  /**
  * Constructor.
  * Create a new IBEA instance
  * @param problem Problem to solve
  */
  public IBEA(Problem problem) {
    super (problem) ;
  } // Spea2

  /**
   * calculates the hypervolume of that portion of the objective space that
   * is dominated by individual a but not by individual b
   */
  double calcHypervolumeIndicator(Solution p_ind_a,
                                  Solution p_ind_b,
                                  int d,
                                  double maximumValues [],
                                  double minimumValues []) {
    double a, b, r, max;
    double volume = 0;
    double rho = 2.0;

    r = rho * (maximumValues[d-1] - minimumValues[d-1]);
    max = minimumValues[d-1] + r;


    a = p_ind_a.getObjective(d-1);
    if (p_ind_b == null)
      b = max;
    else
      b = p_ind_b.getObjective(d-1);

    if (d == 1)
    {
      if (a < b)
        volume = (b - a) / r;
      else
        volume = 0;
    }
    else
    {
      if (a < b)
      {
         volume = calcHypervolumeIndicator(p_ind_a, null, d - 1, maximumValues, minimumValues) *
         (b - a) / r;
         volume += calcHypervolumeIndicator(p_ind_a, p_ind_b, d - 1, maximumValues, minimumValues) *
         (max - b) / r;
      }
      else
      {
         volume = calcHypervolumeIndicator(p_ind_a, p_ind_b, d - 1, maximumValues, minimumValues) *
         (max - b) / r;
      }
    }

    return (volume);
  }



    /**
   * This structure store the indicator values of each pair of elements
   */
  public void computeIndicatorValuesHD(SolutionSet solutionSet,
                                     double [] maximumValues,
                                     double [] minimumValues) {
    SolutionSet A, B;
    // Initialize the structures
    indicatorValues_ = new ArrayList<List<Double>>();
    maxIndicatorValue_ = - Double.MAX_VALUE;

    for (int j = 0; j < solutionSet.size(); j++) {
      A = new SolutionSet(1);
      A.add(solutionSet.get(j));

      List<Double> aux = new ArrayList<Double>();
      for (int i = 0; i < solutionSet.size(); i++) {
        B = new SolutionSet(1);
        B.add(solutionSet.get(i));

        int flag = (new DominanceComparator()).compare(A.get(0), B.get(0));

        double value = 0.0;
        if (flag == -1) {
            value = - calcHypervolumeIndicator(A.get(0), B.get(0), problem_.getNumberOfObjectives(), maximumValues, minimumValues);
        } else {
            value = calcHypervolumeIndicator(B.get(0), A.get(0), problem_.getNumberOfObjectives(), maximumValues, minimumValues);
        }
        //double value = epsilon.epsilon(matrixA,matrixB,problem_.getNumberOfObjectives());

        
        //Update the max value of the indicator
        if (Math.abs(value) > maxIndicatorValue_)
          maxIndicatorValue_ = Math.abs(value);
        aux.add(value);
     }
     indicatorValues_.add(aux);
   }
  } // computeIndicatorValues



  /**
   * Calculate the fitness for the individual at position pos
   */
  public void fitness(SolutionSet solutionSet,int pos) {
      double fitness = 0.0;
      double kappa   = 0.05;
    
      for (int i = 0; i < solutionSet.size(); i++) {
        if (i!=pos) {
           fitness += Math.exp((-1 * indicatorValues_.get(i).get(pos)/maxIndicatorValue_) / kappa);
        }
      }
      solutionSet.get(pos).setFitness(fitness);
  }


  /**
   * Calculate the fitness for the entire population.
  **/
  public void calculateFitness(SolutionSet solutionSet) {
    // Obtains the lower and upper bounds of the population
    double [] maximumValues = new double[problem_.getNumberOfObjectives()];
    double [] minimumValues = new double[problem_.getNumberOfObjectives()];

    for (int i = 0; i < problem_.getNumberOfObjectives();i++) {
        maximumValues[i] = - Double.MAX_VALUE; // i.e., the minus maxium value
        minimumValues[i] =   Double.MAX_VALUE; // i.e., the maximum value
    }

    for (int pos = 0; pos < solutionSet.size(); pos++) {
        for (int obj = 0; obj < problem_.getNumberOfObjectives(); obj++) {
          double value = solutionSet.get(pos).getObjective(obj);
          if (value > maximumValues[obj])
              maximumValues[obj] = value;
          if (value < minimumValues[obj])
              minimumValues[obj] = value;
        }
    }

    computeIndicatorValuesHD(solutionSet,maximumValues,minimumValues);
    for (int pos =0; pos < solutionSet.size(); pos++) {
        fitness(solutionSet,pos);
    }
  }



  /** 
   * Update the fitness before removing an individual
   */
  public void removeWorst(SolutionSet solutionSet) {
   
    // Find the worst;
    double worst      = solutionSet.get(0).getFitness();
    int    worstIndex = 0;
    double kappa = 0.05;
     
    for (int i = 1; i < solutionSet.size(); i++) {
      if (solutionSet.get(i).getFitness() > worst) {
        worst = solutionSet.get(i).getFitness();
        worstIndex = i;
      }
    }

    //if (worstIndex == -1) {
    //    System.out.println("Yes " + worst);
    //}
    //System.out.println("Solution Size "+solutionSet.size());
    //System.out.println(worstIndex);

    // Update the population
    for (int i = 0; i < solutionSet.size(); i++) {
      if (i!=worstIndex) {
          double fitness = solutionSet.get(i).getFitness();
          fitness -= Math.exp((- indicatorValues_.get(worstIndex).get(i)/maxIndicatorValue_) / kappa);
          solutionSet.get(i).setFitness(fitness);
      }
    }

    // remove worst from the indicatorValues list
    indicatorValues_.remove(worstIndex); // Remove its own list
    Iterator<List<Double>> it = indicatorValues_.iterator();
    while (it.hasNext())
        it.next().remove(worstIndex);

    // remove the worst individual from the population
    solutionSet.remove(worstIndex);
  } // removeWorst


  /**
  * Runs of the IBEA algorithm.
  * @return a <code>SolutionSet</code> that is a set of non dominated solutions
  * as a result of the algorithm execution
  * @throws JMException
  */
  public SolutionSet execute() throws JMException, ClassNotFoundException{
    int populationSize, archiveSize, maxEvaluations, evaluations, clusterTime;
    Operator crossoverOperator, mutationOperator, selectionOperator;
    SolutionSet solutionSet, archive, offSpringSolutionSet;
    long runTime;
   
    //Read the params
    populationSize = ((Integer)getInputParameter("populationSize")).intValue();
    archiveSize    = ((Integer)getInputParameter("archiveSize")).intValue();
    maxEvaluations = ((Integer)getInputParameter("maxEvaluations")).intValue();
    
    runTime = ((Integer) getInputParameter("runTime"));
    final boolean clustering = false;
    //final boolean useBreakpoints = false;
    //clusterTime = ((Integer) getInputParameter("clusterTime"));
    //final boolean clustering = (Boolean) getInputParameter("clustering");
    //final boolean useBreakpoints = (Boolean) getInputParameter("useBreakpoints");
    
    long[] breakpoints = (long[]) getInputParameter("breakpoints");
    int breakIndex = 0;
    /* Open the file */
    BufferedWriter bw = null;
    if(breakpoints != null) {
	    try {
			FileOutputStream fos   = new FileOutputStream("OBJ.csv")     ;
			OutputStreamWriter osw = new OutputStreamWriter(fos)    ;
			bw = new BufferedWriter(osw)        ;
			bw.write("TIME,HV,SPREAD,CORRECT,TOTAL,%CORRECT,"
                                + "OBJ1-MEAN,OBJ1-STDEV,OBJ2-MEAN,OBJ2-STDEV,"
                                + "OBJ3-MEAN,OBJ3-STDEV,OBJ4-MEAN,OBJ4-STDEV,"
                                + "OBJ5-MEAN,OBJ5-STDEV\n");
                        System.out.println("TIME,HV,SPREAD,CORRECT,TOTAL,%CORRECT,"
                                + "OBJ1-MEAN,OBJ1-STDEV,OBJ2-MEAN,OBJ2-STDEV,"
                                + "OBJ3-MEAN,OBJ3-STDEV,OBJ4-MEAN,OBJ4-STDEV,"
                                + "OBJ5-MEAN,OBJ5-STDEV\n");
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
    }
    
    String pf = (String) getInputParameter("paretoFile");
    QualityIndicator indicators = null;
    if(pf!=null)
    	indicators = new QualityIndicator(problem_, pf);

    //Read the operators
    crossoverOperator = operators_.get("crossover");
    mutationOperator  = operators_.get("mutation");
    selectionOperator = operators_.get("selection");

    //Initialize the variables
    solutionSet  = new SolutionSet(populationSize);
    archive     = new SolutionSet(archiveSize);
    evaluations = 0;

    //-> Create the initial solutionSet
    int numInitial = 0; //change to 0 to make all solutions random
    Solution newSolution;
/*	String[] solutions = new String[numInitial];
	Scanner in;
	try {
                //int ones = 0;
                //StringBuilder kk = new StringBuilder();
		//in = new Scanner(new FileReader("DMCorrect\\"+problem_.getNumberOfBits()+".correct"));
		in = new Scanner(new FileReader("DMCorrect\\VAR"+problem_.getNumberOfBits()+".0"));
		//in = new Scanner(new FileReader("DMCorrect\\2.6.28.6-icse11.correct"));
		for(int i=0;i<numInitial;i++) {
                    String st = in.next();
                    //for(int j=0; j<problem_.getNumberOfBits();j++) {
                        //kk.append(st.charAt(0) == '-'? "0" : "1");
                        //if(st.charAt(0) != '-') ones++;
                   // }
                    //System.out.println("Selected features in Z3 solution = "+ones);
                    //newSolution = new Solution(problem_, kk.toString());
                    newSolution = new Solution(problem_, st);
		    problem_.evaluate(newSolution);
		    problem_.evaluateConstraints(newSolution);
		    evaluations++;
		    solutionSet.add(newSolution);
		    
		}
		in.close();
	} catch (FileNotFoundException e) {
		System.out.println(e.getMessage());
	}
        // Read next batch
   /*     Scanner in2;
	try {
 		in2 = new Scanner(new FileReader("VAR_6888_2obj_cost"));
		for(int i=0;i<60;i++) {
                    String st = in2.next();
                    newSolution = new Solution(problem_, st);
		    problem_.evaluate(newSolution);
		    problem_.evaluateConstraints(newSolution);
		    evaluations++;
		    solutionSet.add(newSolution);
		}
		in2.close();
	} catch (FileNotFoundException e) {
		System.out.println(e.getMessage());
	}
        // Read next batch
        Scanner in3;
	try {
 		in3 = new Scanner(new FileReader("VAR_6888_2obj_defects"));
		for(int i=0;i<60;i++) {
                    String st = in3.next();
                    newSolution = new Solution(problem_, st);
		    problem_.evaluate(newSolution);
		    problem_.evaluateConstraints(newSolution);
		    evaluations++;
		    solutionSet.add(newSolution);
		}
		in3.close();
	} catch (FileNotFoundException e) {
		System.out.println(e.getMessage());
	}
        // Read next batch
        Scanner in4;
	try {
 		in4 = new Scanner(new FileReader("VAR_6888_2obj_used"));
		for(int i=0;i<60;i++) {
                    String st = in4.next();
                    newSolution = new Solution(problem_, st);
		    problem_.evaluate(newSolution);
		    problem_.evaluateConstraints(newSolution);
		    evaluations++;
		    solutionSet.add(newSolution);
		}
		in4.close();
	} catch (FileNotFoundException e) {
		System.out.println(e.getMessage());
	}
       */ 
        
    for (int i = numInitial; i < populationSize; i++) {
      newSolution = new Solution(problem_);
      problem_.evaluate(newSolution);
      problem_.evaluateConstraints(newSolution);
      evaluations++;
      solutionSet.add(newSolution);
    }
    
    long anyC = -1;
    long fiftyC = -1;
    long hundredC = -1;
    
    long initTime = System.currentTimeMillis();   //timing
    long diff = 0;
    final int[] sortOrder = {0, 4, 2, 3, 1};
    Comparator<Solution> c = new Comparator<Solution>() {
        public int compare(Solution a, Solution b) {
        	for(int i: sortOrder) {
        		if(a.getObjective(i)!=b.getObjective(i))
	        		return ((Double) a.getObjective(i)).compareTo(b.getObjective(i));
        	}
        	return 0;
        }
    };
    while (diff < runTime || clustering) { //1 5 3 4 2
    //while (diff < runTime && anyC < 0){
      if(breakpoints!=null && pf!=null && breakIndex < breakpoints.length && diff > breakpoints[breakIndex]) {
    	  //System.out.println(breakpoints[breakIndex]);
    	  Ranking ranking = new Ranking(archive);
    	  SolutionSet currentSet = ranking.getSubfront(0);
    	  currentSet.printCurrentObjectivesToFile(bw, breakpoints[breakIndex], indicators.getHypervolume(currentSet), indicators.getSpread(currentSet));
    	  breakIndex++;
      }
      if(clustering && diff>clusterTime) { //cluster
    	  solutionSet.sort(c);
    	  SolutionSet s1 = new SolutionSet(25);
    	  SolutionSet s2 = new SolutionSet(25);
    	  SolutionSet s3 = new SolutionSet(25);
    	  SolutionSet s4 = new SolutionSet(25);
    	  
    	  for(int i=0;i<solutionSet.size();i++) {
    		  Solution temp = solutionSet.get(i);
    		  if(i<25)
    			  s1.add(temp);
    		  else if(i<50)
    			  s2.add(temp);
    		  else if(i<75)
    			  s3.add(temp);
    		  else
    			  s4.add(temp);
    	  }
//    	  final SolutionSet[] clusters = {s1, s2, s3, s4};
//    	  for(int i = 1;i<=clusters.length;i++) {
//    		  final int index = i;
//    		  executorPool.submit(new Runnable() {
//  				public void run() {
//  					try {
//						SolutionSet done = executeCluster(clusters[index-1]);
//						done.printObjectivesToFile("FUN" + index);
//						done.printVariablesToFile("VAR" + index);
//					} catch (Exception e) {
//						e.printStackTrace();
//					}
//  				}
//  			});
//    	  }
//    	  executorPool.shutdown();
    	  s1 = executeCluster(s1);
    	  s2 = executeCluster(s2);
    	  s3 = executeCluster(s3);
    	  s4 = executeCluster(s4);
    	  s1.printObjectivesToFile("FUN1");
    	  s2.printObjectivesToFile("FUN2");
    	  s3.printObjectivesToFile("FUN3");
    	  s4.printObjectivesToFile("FUN4");
    	  s1.printVariablesToFile("VAR1");
    	  s2.printVariablesToFile("VAR2");
    	  s3.printVariablesToFile("VAR3");
    	  s4.printVariablesToFile("VAR4");
    	  System.out.println("Done. Total execution time: "+(System.currentTimeMillis()-initTime) + "ms");
    	  System.exit(0);
    	  //solutionSet = s1.union(s2).union(s3).union(s4);  this is only if the clusters remain size 25
      }
      SolutionSet union = ((SolutionSet)solutionSet).union(archive);
      calculateFitness(union);
      archive = union;
      
      //System.out.println(union.get(0).getObjective(0));
      
      while (archive.size() > populationSize) {
        removeWorst(archive);
      }
      // Create a new offspringPopulation
      offSpringSolutionSet= new SolutionSet(populationSize);
      Solution  [] parents = new Solution[2];
      while (offSpringSolutionSet.size() < populationSize){
        int j = 0;
        do{
          j++;
          parents[0] = (Solution)selectionOperator.execute(archive);
        } while (j < IBEA.TOURNAMENTS_ROUNDS); // do-while
        int k = 0;
        do{
          k++;
          parents[1] = (Solution)selectionOperator.execute(archive);
        } while (k < IBEA.TOURNAMENTS_ROUNDS); // do-while

        //make the crossover
        Solution [] offSpring = (Solution [])crossoverOperator.execute(parents);
        mutationOperator.execute(offSpring[0]);
        problem_.evaluate(offSpring[0]);
        problem_.evaluateConstraints(offSpring[0]);
        offSpringSolutionSet.add(offSpring[0]);
        evaluations++;
        diff = System.currentTimeMillis() - initTime;
      } // while
      // End Create a offSpring solutionSet
      solutionSet = offSpringSolutionSet;
      Ranking ranking = new Ranking(archive);
      SolutionSet fSol = ranking.getSubfront(0);
      float pCorrect = fSol.getPercentCorrect();
      if(pCorrect > 0 && anyC < 0)
    	  anyC = diff;
      if(pCorrect > 50 && fiftyC < 0)
    	  fiftyC = diff;
      if(pCorrect == 100 && hundredC < 0)
    	  hundredC = diff;
    	  
    } // while
    if(breakpoints!=null) {
	    try {
			bw.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
    }
    Ranking ranking = new Ranking(archive);
    SolutionSet fSol = ranking.getSubfront(0);
    fSol.setTimeCorrect(anyC, fiftyC, hundredC);
    return fSol;
  } // execute
  
  public SolutionSet executeCluster(SolutionSet solutionSet) throws JMException, ClassNotFoundException{
	    int populationSize, archiveSize, clusterTime;
	    Operator mutationOperator, selectionOperator;
	    SolutionSet archive, offSpringSolutionSet;
	    //long runTime;
	   
	    //Read the params
	    populationSize = solutionSet.size();
	    archiveSize    = solutionSet.size();
	    //runTime = 2000;
	    clusterTime = ((Integer) getInputParameter("clusterTime"));

	    //Read the operators
	    mutationOperator  = operators_.get("mutation");
	    selectionOperator = operators_.get("selection");

	    archive     = new SolutionSet(archiveSize);
	    
	    long initTime = System.currentTimeMillis();   //timing
	    long diff = System.currentTimeMillis() - initTime;
	    int round = 0;
//	    final int[] sortOrder = {0, 4, 2, 3, 1};
//	    Comparator<Solution> c = new Comparator<Solution>() {
//	        public int compare(Solution a, Solution b) {
//	        	for(int i: sortOrder) {
//	        		if(a.getObjective(i)!=b.getObjective(i))
//		        		return ((Double) a.getObjective(i)).compareTo(b.getObjective(i));
//	        	}
//	        	return 0;
//	        }
//	    };
	    while (diff < clusterTime) { //1 5 3 4 2
	    //while (evaluations < maxEvaluations){
//	      if(diff>clusterTime) { //cluster
//	    	  clusterTime*=2;
//	    	  solutionSet.sort(c);
//	    	  SolutionSet s1 = new SolutionSet(25);
//	    	  SolutionSet s2 = new SolutionSet(25);
//	    	  SolutionSet s3 = new SolutionSet(25);
//	    	  SolutionSet s4 = new SolutionSet(25);
//	    	  for(int i=0;i<solutionSet.size();i++) {
//	    		  Solution temp = solutionSet.get(i);
//	    		  if(i<25)
//	    			  s1.add(temp);
//	    		  else if(i<50)
//	    			  s2.add(temp);
//	    		  else if(i<75)
//	    			  s3.add(temp);
//	    		  else
//	    			  s4.add(temp);
//	    	  }
//	    	  System.out.println(s1);
//	    	  System.out.println(s2);
//	    	  System.out.println(s3);
//	    	  System.out.println(s4);
//	      }
	      SolutionSet union = ((SolutionSet)solutionSet).union(archive);
	      calculateFitness(union);
	      archive = union;
	      
	      //System.out.println(union.get(0).getObjective(0));
//	      if(round<2) { //for sizing up the population
//	    	  populationSize*=2;
//	    	  archiveSize*=2;
//	    	  round++;
//	      }
//	      else {
		      while (archive.size() > populationSize) {
		        removeWorst(archive);
		      }
	     // }
	      // Create a new offspringPopulation
	      offSpringSolutionSet= new SolutionSet(populationSize);
	      Solution  [] parents = new Solution[1];
	      while (offSpringSolutionSet.size() < populationSize){
	        int j = 0;
	        do{
	          j++;
	          parents[0] = (Solution)selectionOperator.execute(archive);
	        } while (j < IBEA.TOURNAMENTS_ROUNDS); // do-while
	    /*    int k = 0;
	        do{
	          k++;
	          parents[1] = (Solution)selectionOperator.execute(archive);
	        } while (k < IBEA.TOURNAMENTS_ROUNDS); // do-while
             */
	        //no crossover
	        Solution[] offSpring = new Solution[1];
	        offSpring[0] = new Solution(parents[0]);
	        //offSpring[1] = new Solution(parents[1]);
	        mutationOperator.execute(offSpring[0]);
	        problem_.evaluate(offSpring[0]);
	        problem_.evaluateConstraints(offSpring[0]);
	        offSpringSolutionSet.add(offSpring[0]);
	      } // while
	      // End Create a offSpring solutionSet
	      solutionSet = offSpringSolutionSet;
	      diff = System.currentTimeMillis() - initTime;
	    } // while

	    Ranking ranking = new Ranking(archive);
	    return ranking.getSubfront(0);
	  } // execute
} // Spea2
