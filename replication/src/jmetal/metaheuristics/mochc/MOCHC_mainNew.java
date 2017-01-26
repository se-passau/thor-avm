//  MOCHC_main.java
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

package jmetal.metaheuristics.mochc;

import java.util.HashMap;

import main.ParsedFM;

import jmetal.core.*;
import jmetal.operators.crossover.*;
import jmetal.operators.mutation.*;
import jmetal.operators.selection.*;
import jmetal.problems.FMr.*;
import jmetal.problems.ZDT.*;
import jmetal.problems.WFG.*;
/**
 * This class executes the algorithm described in:
 * A.J. Nebro, E. Alba, G. Molina, F. Chicano, F. Luna, J.J. Durillo 
 * "Optimal antenna placement using a new multi-objective chc algorithm". 
 * GECCO '07: Proceedings of the 9th annual conference on Genetic and 
 * evolutionary computation. London, England. July 2007.
 */
public class MOCHC_mainNew {

  public static void main(String [] args) {
  	
    HashMap  parameters ; // Operator parameters

    try {                               
      //Problem problem = new ZDT5("Binary");

      Algorithm algorithm = null;
      ParsedFM featureModel = new ParsedFM();
      featureModel.parse("src\\FM-290.xml");

      Problem problem = new FM290r("Binary", featureModel.getFeatureCount(), 5, featureModel);

      algorithm = new MOCHCTM(problem);

      
      algorithm.setInputParameter("initialConvergenceCount",0.25);
      algorithm.setInputParameter("preservedPopulation",0.05);
      algorithm.setInputParameter("convergenceValue",3);
      algorithm.setInputParameter("populationSize",100);
      algorithm.setInputParameter("maxEvaluations",60000);
      algorithm.setInputParameter("runTime", 5000);
      
      Operator crossoverOperator      ;
      Operator mutationOperator       ;
      Operator parentsSelection       ;
      Operator newGenerationSelection ;
      
   // Mutation and Crossover for Real codification 
      parameters = new HashMap() ;
      parameters.put("probability", 0.9) ;
      parameters.put("distributionIndex", 20.0) ;
      crossoverOperator = CrossoverFactory.getCrossoverOperator("FMCrossover", parameters);   
      //parentsSelection = new RandomSelection();
      //newGenerationSelection = new RankingAndCrowdingSelection(problem);
      parameters = null ;
      parentsSelection = SelectionFactory.getSelectionOperator("RandomSelection", parameters) ;     
      
      parameters = new HashMap() ;
      parameters.put("problem", problem) ;
      newGenerationSelection = SelectionFactory.getSelectionOperator("RankingAndCrowdingSelection", parameters) ;   
     
      parameters = new HashMap() ;
      parameters.put("probability", 0.8) ;
      parameters.put("distributionIndex", 20.0) ;
    //add FM to mutator params
      parameters.put("featureModel", featureModel);
      mutationOperator = MutationFactory.getMutationOperator("FMMutator", parameters);               
      
      algorithm.addOperator("crossover",crossoverOperator);
      algorithm.addOperator("cataclysmicMutation",mutationOperator);
      algorithm.addOperator("parentSelection",parentsSelection);
      algorithm.addOperator("newGenerationSelection",newGenerationSelection);
      
      // Execute the Algorithm 
      long initTime = System.currentTimeMillis();
      SolutionSet population = algorithm.execute();
      long estimatedTime = System.currentTimeMillis() - initTime;
      System.out.println("Total execution time: "+estimatedTime);

      // Print results
      population.printVariablesToFile("VAR");
      population.printObjectivesToFile("FUN");
    } //try           
    catch (Exception e) {
      System.err.println(e);
      e.printStackTrace();
    } //catch    
  }//main
}
