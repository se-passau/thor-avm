//  PESA2_Settings.java 
//
//  Authors:
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

package jmetal.experiments.settings;

import java.util.HashMap;

import jmetal.metaheuristics.pesa2.*;
import jmetal.operators.crossover.Crossover;
import jmetal.operators.crossover.CrossoverFactory;
import jmetal.operators.mutation.Mutation;
import jmetal.operators.mutation.MutationFactory;
import jmetal.operators.selection.Selection;
import jmetal.operators.selection.SelectionFactory;
import jmetal.problems.ProblemFactory;
import jmetal.core.*;
import jmetal.experiments.Settings;
import jmetal.qualityIndicator.QualityIndicator;
import jmetal.util.JMException;

/**
 * Settings class of algorithm NSGA-II (real encoding)
 */
public class PESA2_Settings extends Settings {
  public int populationSize_                 ; 
  public int archiveSize_                    ;
  public int maxEvaluations_                 ;
  public int runTime_      ;
  public double mutationProbability_         ;
  public double crossoverProbability_        ;
  public double mutationDistributionIndex_   ;
  public double crossoverDistributionIndex_  ;
  
  /**
   * Constructor
   * @throws JMException 
   */
  public PESA2_Settings(String problem) throws JMException {
    super(problem) ;
    
    Object [] problemParams = {"Binary"};
    try {
	    problem_ = (new ProblemFactory()).getProblem(problemName_, problemParams);
    } catch (JMException e) {
	    // TODO Auto-generated catch block
	    e.printStackTrace();
    }  
    // Default settings
    //populationSize_              = 60   ;
    //archiveSize_                 = 60   ;
    maxEvaluations_              = 50000 ;
    //runTime_                     = 120000 ;
    //mutationProbability_         = 0.01;//1.0/problem_.getNumberOfVariables() ;
    //crossoverProbability_        = 0.1   ;
    mutationDistributionIndex_   = 20.0  ;
    crossoverDistributionIndex_  = 20.0  ;
  } // PESA2_Settings

  
  /**
   * Configure PESA2 with user-defined parameter settings
   * @return A PESA2 algorithm object
   * @throws jmetal.util.JMException
   */
  public Algorithm configure() throws JMException {
    Algorithm algorithm ;
    Selection  selection ;
    Crossover  crossover ;
    Mutation   mutation  ;

    HashMap  parameters ; // Operator parameters

    QualityIndicator indicators ;
    
    // Creating the algorithm.
    algorithm = new PESA2(problem_) ;
    
    // Algorithm parameters
    algorithm.setInputParameter("populationSize",populationSize_);
    algorithm.setInputParameter("archiveSize",archiveSize_);
    algorithm.setInputParameter("bisections",5);
    algorithm.setInputParameter("maxEvaluations",maxEvaluations_);
    algorithm.setInputParameter("runTime", runTime_);

    // Mutation and Crossover for Binary codification
    parameters = new HashMap() ;
    parameters.put("probability", crossoverProbability_) ;
    parameters.put("distributionIndex", crossoverDistributionIndex_) ;
    crossover = CrossoverFactory.getCrossoverOperator("FMCrossover", parameters);                   

    parameters = new HashMap() ;
    parameters.put("probability", mutationProbability_) ;
    parameters.put("distributionIndex", mutationDistributionIndex_) ;
    parameters.put("featureModel", problem_.pfm);
    mutation = MutationFactory.getMutationOperator("FMMutator", parameters);                         

    // Selection Operator 
    //parameters = null ;
    //selection = SelectionFactory.getSelectionOperator("BinaryTournament2", parameters) ;     

    // Add the operators to the algorithm
    algorithm.addOperator("crossover",crossover);
    algorithm.addOperator("mutation",mutation);
    //algorithm.addOperator("selection",selection);
    
   // Creating the indicator object
   if ((paretoFrontFile_!=null) && (!paretoFrontFile_.equals(""))) {
      indicators = new QualityIndicator(problem_, paretoFrontFile_);
      algorithm.setInputParameter("indicators", indicators) ;  
   } // if
   
    return algorithm ;
  } // configure
} // PESA2_Settings
