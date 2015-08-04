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

package jmetal.metaheuristics.ibea;

import java.io.IOException;

import jmetal.util.comparators.FitnessComparator;
import jmetal.operators.crossover.*;
import jmetal.operators.mutation.*;
import jmetal.operators.selection.*;
import jmetal.problems.FMr.*                  ;
import jmetal.problems.*                  ;
//import jmetal.problems.FM.*;
import jmetal.problems.ZDT.*;
import jmetal.problems.dimacs.P286;
import jmetal.problems.WFG.*;
import jmetal.problems.LZ09.* ;
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
public class IBEA_main {
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
    
    indicators = null ;
    if (args.length == 1) {
      Object [] params = {"Binary"};
      problem = (new ProblemFactory()).getProblem(args[0],params);
    } // if
    else if (args.length == 2) {
      Object [] params = {"Binary"};
      problem = (new ProblemFactory()).getProblem(args[0],params);
      indicators = new QualityIndicator(problem, args[1]) ;
    } // if
    else { // Default problem
      problem = new FM290r("Binary"); 
    } // else

    algorithm = new IBEA(problem);
    
    long[] breakpoints = {
        100, 200, 300, 400, 500, 600, 700, 800, 900, 1000,  //ms
        1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, //ms
        3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000,      //3, 4, 5, 6, 7, 8, 9, 10 sec
        11000, 12000, 13000, 14000, 15000, 16000, 17000, 18000, 19000,     //11, 12, ..., 19 sec
        20000,  30000,  40000,  50000, 60000, 70000, 80000, 90000, 100000, 110000, //20, 30, ..., 110 sec
        120000, 180000, 240000, 300000, 360000, 420000, 480000, 540000,     //2, ..., 9 min
        600000, 720000, 840000, 1000000}; //10, 12, 14, 16.67 min
        //, 960000, 1080000, 1200000, 1800000, 2400000, 3000000,//16, 18, 20, 30, 40, 50 min
        //3600000, 7200000};//, 9800000, 14400000, 18000000,                     // 1, 2, 3, 4, 5 hour
        //21600000, 25200000};//, 28800000, 32400000, 36000000, 39600000, 43200000}; // 6, 7, 8, 9, 10, 11, 12 hour

    // Algorithm parameters
    algorithm.setInputParameter("populationSize",100);
    algorithm.setInputParameter("archiveSize",100);
    algorithm.setInputParameter("maxEvaluations",20000);
    algorithm.setInputParameter("runTime",1005000);//16.67 min
    algorithm.setInputParameter("breakpoints", breakpoints);
    algorithm.setInputParameter("paretoFile", "PF//FM290_8obj.pf"); //replace with appropriate paretoFile  "PF//FM6888_2obj.pf"

    // Mutation and Crossover for Binary codification 
    parameters = new HashMap() ;
    parameters.put("probability", 0.0) ;
    parameters.put("distributionIndex", 20.0) ;
    crossover = CrossoverFactory.getCrossoverOperator("FMCrossover", parameters);                   

    parameters = new HashMap() ;
    parameters.put("probability", 1.0/290) ;
    parameters.put("distributionIndex", 20.0) ;
    mutation = MutationFactory.getMutationOperator("BitFlipMutation", parameters);         

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

    // Print the results
    logger_.info("Total execution time: "+estimatedTime + "ms");
    logger_.info("Variables values have been writen to file VAR");
    population.printVariablesToFile("VAR");    
    logger_.info("Objectives values have been writen to file FUN");
    population.printObjectivesToFile("FUN");
  
    if (indicators != null) {
      logger_.info("Quality indicators") ;
      logger_.info("Hypervolume: " + indicators.getHypervolume(population)) ;
      //logger_.info("GD         : " + indicators.getGD(population)) ;
      //logger_.info("IGD        : " + indicators.getIGD(population)) ;
      logger_.info("Spread     : " + indicators.getSpread(population)) ;
      //logger_.info("Epsilon    : " + indicators.getEpsilon(population)) ;  
    } // if
  } //main
} // IBEA_main.java
