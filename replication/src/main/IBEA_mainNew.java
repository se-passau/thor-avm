package main;
//  IBEA_main.java
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



import java.io.IOException;
import jmetal.problems.FMr.*;
import jmetal.util.comparators.FitnessComparator;
import jmetal.metaheuristics.ibea.IBEA;
import jmetal.operators.crossover.*;
import jmetal.operators.mutation.*;
import jmetal.operators.selection.*;
import jmetal.problems.*                  ;
import jmetal.qualityIndicator.QualityIndicator;

import jmetal.util.Configuration;
import jmetal.util.JMException;

import java.util.HashMap;
import java.util.logging.FileHandler;
import java.util.logging.Logger;

import jmetal.core.*;

/**
 * Class for configuring and running the DENSEA algorithm
 */
public class IBEA_mainNew {
  public static Logger      logger_ ;      // Logger object
  public static FileHandler fileHandler_ ; // FileHandler object

  /**
   * @param args Command line arguments.
   * @throws JMException 
   * @throws IOException 
   * @throws SecurityException 
   * Usage: three choices
   *      - jmetal.metaheuristics.nsgaII.NSGAII_main
   *      - jmetal.metaheuristics.nsgaII.NSGAII_main problemName
   *      - jmetal.metaheuristics.nsgaII.NSGAII_main problemName paretoFrontFile
   */
  public static void main(String [] args) throws JMException, IOException, ClassNotFoundException {
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
    ParsedFM featureModel = new ParsedFM("..\\trunk2\\jmetal\\problems\\FMr\\FM-88.xml");
    
    indicators = null;

    problem = new FM88r("Binary", featureModel.getFeatureCount(), 5, featureModel);

    algorithm = new IBEA(problem);
    
    long[] breakpoints = {1000, 2000, 3000, 4000, 5000,     //1, 2, 3, 4, 5 sec
                6000,   7000,   8000,   9000,   10000,      //6, 7, 8, 9, 10 sec
                20000,  30000,  40000,  50000, 	60000,      //20, 30, 40, 50, 60 sec
                120000, 180000, 240000, 300000, 360000,     //2, 3, 4, 5, 6 min
                420000, 480000, 540000, 600000,              // 7, 8, 9, 10 min
                1200000, 1800000, 2400000, 3000000,         //20, 30, 40, 50 min
                3600000, 7200000, 9800000};                 // 1, 2, 3 hour

    // Algorithm parameters
    algorithm.setInputParameter("populationSize",100);
    algorithm.setInputParameter("archiveSize",100);
    algorithm.setInputParameter("maxEvaluations",25000);
    algorithm.setInputParameter("runTime", 9805000);
    algorithm.setInputParameter("clusterTime", 10000); //time for executing the population of 100 and each population of 25 (each solution gets clusterTime*2 amount of eval time)
    algorithm.setInputParameter("clustering", false); //turn clustering on or off
    algorithm.setInputParameter("breakpoints", breakpoints);
    algorithm.setInputParameter("paretoFile", "PF//FM88_5obj.pf");

    // Mutation and Crossover for Real codification 
    parameters = new HashMap() ;
    parameters.put("probability", 0.9) ;
    parameters.put("distributionIndex", 20.0) ;
    crossover = CrossoverFactory.getCrossoverOperator("FMCrossover", parameters);                   

    parameters = new HashMap() ;
    parameters.put("probability", 0.1) ;
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
  
    if (indicators != null) {
      logger_.info("Quality indicators") ;
      logger_.info("Hypervolume: " + indicators.getHypervolume(population)) ;
//      logger_.info("GD         : " + indicators.getGD(population)) ;
//      logger_.info("IGD        : " + indicators.getIGD(population)) ;
      logger_.info("Spread     : " + indicators.getSpread(population)) ;
//      logger_.info("Epsilon    : " + indicators.getEpsilon(population)) ;  
    } // if
  } //main
} // IBEA_main.java
