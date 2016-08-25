//  FastPGA_Settings.java 
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

import jmetal.metaheuristics.fastPGA.*;
import jmetal.util.comparators.FPGAFitnessComparator;
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
 * Settings class of algorithm FastPGA
 */
public class FastPGA_Settings extends Settings {
  public int populationSize_                 ; 
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
  public FastPGA_Settings(String problem) throws JMException {
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
    maxEvaluations_              = 50000000 ;
    //runTime_                     = 120000 ;
    mutationProbability_         = 1.0/problem_.getNumberOfBits() ;
    //crossoverProbability_        = 0.1   ;
    mutationDistributionIndex_   = 20.0  ;
    crossoverDistributionIndex_  = 20.0  ;
  } // FastPGA_Settings

  
  /**
   * Configure NSGAII with user-defined parameter settings
   * @return A NSGAII algorithm object
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
    algorithm = new FastPGA(problem_) ;
    
        // Parameter "termination"
    // If the preferred stopping criterium is PPR based, termination must 
    // be set to 0; otherwise, if the algorithm is intended to iterate until 
    // a give number of evaluations is carried out, termination must be set to 
    // that number
    algorithm.setInputParameter("termination",1);

    // Algorithm parameters
    algorithm.setInputParameter("maxPopSize",populationSize_);
    algorithm.setInputParameter("initialPopulationSize",populationSize_);
    algorithm.setInputParameter("maxEvaluations",maxEvaluations_);
    algorithm.setInputParameter("runTime", runTime_);
    algorithm.setInputParameter("a",20.0);
    algorithm.setInputParameter("b",1.0);
    algorithm.setInputParameter("c",20.0);
    algorithm.setInputParameter("d",0.0);

    // Mutation and Crossover for Real codification
    parameters = new HashMap() ;
    parameters.put("probability", crossoverProbability_) ;
    parameters.put("distributionIndex", crossoverDistributionIndex_) ;
    crossover = CrossoverFactory.getCrossoverOperator("FMCrossover", parameters);                   

    parameters = new HashMap() ;
    parameters.put("probability", mutationProbability_) ;
    parameters.put("distributionIndex", mutationDistributionIndex_) ;
	    parameters.put("featureModel", problem_.pfm);
	    mutation = MutationFactory.getMutationOperator("FMMutator", parameters);                              
            //parameters.put("dimacs", problem_.dfm);
	    //mutation = MutationFactory.getMutationOperator("DMBitFlipMutation", parameters);                              

    // Selection Operator 
    //parameters = null ;
    parameters = new HashMap() ; 
    selection = SelectionFactory.getSelectionOperator("BinaryTournament", parameters) ;     
    parameters.put("comparator", new FPGAFitnessComparator()) ;
    //selection = new BinaryTournament(parameters);

    // Add the operators to the algorithm
    algorithm.addOperator("crossover",crossover);
    algorithm.addOperator("mutation",mutation);
    algorithm.addOperator("selection",selection);
    
   // Creating the indicator object
   if ((paretoFrontFile_!=null) && (!paretoFrontFile_.equals(""))) {
      indicators = new QualityIndicator(problem_, paretoFrontFile_);
      algorithm.setInputParameter("indicators", indicators) ;  
   } // if
   
    return algorithm ;
  } // configure
} // FastPGA_Settings
