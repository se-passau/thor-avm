//  PAES.main
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

package jmetal.metaheuristics.paes;

import java.io.IOException;

import jmetal.core.*;
import jmetal.operators.mutation.*;
import jmetal.problems.FMr.*;
import jmetal.problems.ZDT.*;
import jmetal.problems.WFG.*;
import jmetal.problems.LZ09.* ;
import jmetal.util.Configuration;
import jmetal.util.JMException;

import java.util.HashMap;
import java.util.logging.FileHandler;
import java.util.logging.Logger;

import main.ParsedFM;
import jmetal.qualityIndicator.QualityIndicator;

/**
 * Class for configuring and running the PAES algorithm
 */
public class PAES_mainNew {
  public static Logger      logger_ ;      // Logger object
  public static FileHandler fileHandler_ ; // FileHandler object

  /**
   * @param args Command line arguments. The first (optional) argument specifies 
   *             the problem to solve.
   * @throws JMException 
   * @throws IOException 
   * @throws SecurityException 
   * Usage: three options
   *      - jmetal.metaheuristics.mocell.MOCell_main
   *      - jmetal.metaheuristics.mocell.MOCell_main problemName
   *      - jmetal.metaheuristics.mocell.MOCell_main problemName ParetoFrontFile
   */
  public static void main(String [] args) throws JMException, IOException, ClassNotFoundException {
    Problem   problem   ;         // The problem to solve
    Algorithm algorithm ;         // The algorithm to use
    Operator  mutation  ;         // Mutation operator
    
    QualityIndicator indicators ; // Object to get quality indicators

    HashMap  parameters ; // Operator parameters

    // Logger object and file to store log messages
    logger_      = Configuration.logger_ ;
    fileHandler_ = new FileHandler("PAES_main.log");
    logger_.addHandler(fileHandler_) ;
    
    indicators = null ;
    ParsedFM featureModel = new ParsedFM();
    featureModel.parse("src\\FM-290.xml");

problem = new FM290r("Binary", featureModel.getFeatureCount(), 5, featureModel);

    algorithm = new PAESTM(problem);

algorithm.setInputParameter("runTime", 5000);

    
    // Algorithm parameters
    algorithm.setInputParameter("archiveSize",100);
    algorithm.setInputParameter("biSections",5);
    algorithm.setInputParameter("maxEvaluations",25000);                

    parameters = new HashMap() ;
    parameters.put("probability", 0.8) ;
    parameters.put("distributionIndex", 20.0) ;
    
    //add FM to mutator params
    parameters.put("featureModel", featureModel);
    mutation = MutationFactory.getMutationOperator("FMMutator", parameters);
    
    // Add the operators to the algorithm
    algorithm.addOperator("mutation", mutation);
    
    // Execute the Algorithm 
    long initTime = System.currentTimeMillis();
    SolutionSet population = algorithm.execute();
    long estimatedTime = System.currentTimeMillis() - initTime;
    
    // Result messages 
    // STEP 8. Print the results
    logger_.info("Total execution time: "+estimatedTime + "ms");
    logger_.info("Variables values have been writen to file VAR");
    population.printVariablesToFile("VAR");    
    logger_.info("Objectives values have been writen to file FUN");
    population.printObjectivesToFile("FUN");
  
    if (indicators != null) {
      logger_.info("Quality indicators") ;
      logger_.info("Hypervolume: " + indicators.getHypervolume(population)) ;
      logger_.info("GD         : " + indicators.getGD(population)) ;
      logger_.info("IGD        : " + indicators.getIGD(population)) ;
      logger_.info("Spread     : " + indicators.getSpread(population)) ;
      logger_.info("Epsilon    : " + indicators.getEpsilon(population)) ;  
    } // if
  }//main
} // PAES_main
