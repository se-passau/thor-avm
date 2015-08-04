//  NSGAII_main.java
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

package jmetal.metaheuristics.nsgaII;

import jmetal.core.*;
import jmetal.metaheuristics.ibea.IBEA;
import jmetal.operators.crossover.*;
import jmetal.operators.mutation.*;
import jmetal.operators.selection.*;
import jmetal.problems.FMr.*;
import jmetal.problems.ZDT.*;
import jmetal.problems.WFG.*;
import jmetal.problems.LZ09.* ;

import jmetal.util.Configuration;
import jmetal.util.JMException;
import jmetal.util.comparators.FitnessComparator;

import java.io.IOException;
import java.util.* ;

import java.util.logging.FileHandler;
import java.util.logging.Logger;

import main.ParsedFM;

import jmetal.qualityIndicator.QualityIndicator;

/** 
 * Class implementing the NSGA-II algorithm.  
 * This implementation of NSGA-II makes use of a QualityIndicator object
 *  to obtained the convergence speed of the algorithm. This version is used
 *  in the paper:
 *     A.J. Nebro, J.J. Durillo, C.A. Coello Coello, F. Luna, E. Alba 
 *     "A Study of Convergence Speed in Multi-Objective Metaheuristics." 
 *     To be presented in: PPSN'08. Dortmund. September 2008.
 *     
 *   Besides the classic NSGA-II, a steady-state version (ssNSGAII) is also
 *   included (See: J.J. Durillo, A.J. Nebro, F. Luna and E. Alba 
 *                  "On the Effect of the Steady-State Selection Scheme in 
 *                  Multi-Objective Genetic Algorithms"
 *                  5th International Conference, EMO 2009, pp: 183-197. 
 *                  April 2009)
 */

public class NSGAII_mainNew {
  public static Logger      logger_ ;      // Logger object
  public static FileHandler fileHandler_ ; // FileHandler object

  /**
   * @param args Command line arguments.
   * @throws JMException 
   * @throws IOException 
   * @throws SecurityException 
   * Usage: three options
   *      - jmetal.metaheuristics.nsgaII.NSGAII_main
   *      - jmetal.metaheuristics.nsgaII.NSGAII_main problemName
   *      - jmetal.metaheuristics.nsgaII.NSGAII_main problemName paretoFrontFile
   */
  public static void main(String [] args) throws 
                                  JMException, 
                                  SecurityException, 
                                  IOException, 
                                  ClassNotFoundException {
	  Problem   problem   ;         // The problem to solve
	    Algorithm algorithm ;         // The algorithm to use
	    Operator  crossover ;         // Crossover operator
	    Operator  mutation  ;         // Mutation operator
	    Operator  selection ;         // Selection operator

	    QualityIndicator indicators ; // Object to get quality indicators

	    HashMap  parameters ; // Operator parameters

	    // Logger object and file to store log messages
	    logger_      = Configuration.logger_ ;
	    fileHandler_ = new FileHandler("IBEA.log"); 
	    logger_.addHandler(fileHandler_) ;
	    
	    //Create the feature model and parse it
	    ParsedFM featureModel = new ParsedFM();
	    featureModel.parse("src\\FM-290.xml");
	    
	    indicators = null;
	   
	    problem = new FM290r("Binary", featureModel.getFeatureCount(), 5, featureModel);

	    algorithm = new ssNSGAIITM(problem);

	    // Algorithm parameters
	    algorithm.setInputParameter("populationSize",100);
	    algorithm.setInputParameter("archiveSize",100);
	    algorithm.setInputParameter("maxEvaluations",25000);
	    algorithm.setInputParameter("runTime", 5000);
	    algorithm.setInputParameter("clusterTime", 10000); //time for executing the population of 100 and each population of 25 (each solution gets clusterTime*2 amount of eval time)
	    algorithm.setInputParameter("clustering", false); //turn clustering on or off

	    // Mutation and Crossover for Real codification 
	    parameters = new HashMap() ;
	    parameters.put("probability", 0.9) ;
	    parameters.put("distributionIndex", 20.0) ;
	    crossover = CrossoverFactory.getCrossoverOperator("FMCrossover", parameters);                   

	    parameters = new HashMap() ;
	    parameters.put("probability", 0.8) ;
	    parameters.put("distributionIndex", 20.0) ;
	    
	    //add FM to mutator params
	    parameters.put("featureModel", featureModel);
	    mutation = MutationFactory.getMutationOperator("FMMutator", parameters);         

	    /* Selection Operator */
	    parameters = new HashMap() ; 
	    parameters.put("comparator", new FitnessComparator()) ;
	    selection = new BinaryTournament(parameters);
	    
	    // Add the operators to the algorithm
	    algorithm.addOperator("crossover",crossover);
	    algorithm.addOperator("mutation",mutation);
	    algorithm.addOperator("selection",selection);

	    // Execute the Algorithm
	    long initTime = System.currentTimeMillis();
	    SolutionSet population = algorithm.execute();
	    long estimatedTime = System.currentTimeMillis() - initTime;
	    
	    population.printVariablesToFile2("VAR", featureModel);

	    // Print the results
	    logger_.info("Total execution time: "+estimatedTime + "ms");
	    logger_.info("Variables values have been writen to file VAR");
	    //population.printVariablesToFile("VAR");    
	    logger_.info("Objectives values have been writen to file FUN");
	    population.printObjectivesToFile("FUN");
  } //main
} // NSGAII_main
