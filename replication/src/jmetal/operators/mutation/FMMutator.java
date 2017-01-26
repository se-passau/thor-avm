package jmetal.operators.mutation;
//  BitFlipMutation.java
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



import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Properties;

import jmetal.operators.mutation.Mutation;
import jmetal.util.Configuration;
import jmetal.util.JMException;
import jmetal.util.PseudoRandom;
import jmetal.core.Solution;
import jmetal.encodings.solutionType.ArrayRealSolutionType;
import jmetal.encodings.solutionType.BinaryRealSolutionType;
import jmetal.encodings.solutionType.BinarySolutionType;
import jmetal.encodings.solutionType.IntSolutionType;
import jmetal.encodings.solutionType.RealSolutionType;
import jmetal.encodings.variable.*;

import main.*;

/**
 * This class implements a bit flip mutation operator.
 * NOTE: the operator is applied to binary or integer solutions, considering the
 * whole solution as a single variable.
 */
public class FMMutator extends Mutation {
  /**
   * Valid solution types to apply this operator 
   */
  private static List VALID_TYPES = Arrays.asList(BinarySolutionType.class, 
      BinaryRealSolutionType.class,
      IntSolutionType.class) ;

  private Double mutationProbability_ = null ;
  private ParsedFM pfm;
  
	/**
	 * Constructor
	 * Creates a new instance of the Bit Flip mutation operator
	 * @throws Exception 
	 */
	public FMMutator(HashMap<String, Object> parameters) {
		super(parameters);
		if (parameters.get("probability") != null)
			mutationProbability_ = (Double) parameters.get("probability") ;
		if (parameters.get("featureModel") != null)
			pfm = (ParsedFM) parameters.get("featureModel");
	} // FMMutator

	/**
	 * Perform the mutation operation
	 * @param probability Mutation probability
	 * @param solution The solution to mutate
	 * @throws JMException
	 */
	public void doMutation2(double probability, Solution solution) throws JMException {
		try {
			boolean flipped = false;
			int counter = 0;
			
			if ((solution.getType().getClass() == BinarySolutionType.class) ||
					(solution.getType().getClass() == BinaryRealSolutionType.class)) {
				//int numBits = ((Binary) solution.getDecisionVariables()[0]).getNumberOfBits();
				while(!flipped && counter++ < 100) {
					for (int i = 0; i < solution.getDecisionVariables().length; i++) {
						for (int j = 0; j < ((Binary) solution.getDecisionVariables()[i]).getNumberOfBits(); j++) {
							Binary variable = ((Binary) solution.getDecisionVariables()[i]);
							if (PseudoRandom.randDouble() < probability && !violatesRules(variable, j)) { //ready to flip, but does it violate rules?
								variable.bits_.flip(j); //flip
								flipped = true;
							}
						}
					}
				}

				for (int i = 0; i < solution.getDecisionVariables().length; i++) {
					((Binary) solution.getDecisionVariables()[i]).decode();
				}
			} // if
			else { // Integer representation
				for (int i = 0; i < solution.getDecisionVariables().length; i++)
					if (PseudoRandom.randDouble() < probability) {
						int value = (int) (PseudoRandom.randInt(
								(int)solution.getDecisionVariables()[i].getLowerBound(),
								(int)solution.getDecisionVariables()[i].getUpperBound()));
						solution.getDecisionVariables()[i].setValue(value);
					} // if
			} // else
		} catch (ClassCastException e1) {
			Configuration.logger_.severe("BitFlipMutation.doMutation: " +
					"ClassCastException error" + e1.getMessage());
			Class cls = java.lang.String.class;
			String name = cls.getName();
			throw new JMException("Exception in " + name + ".doMutation()");
		}
	} // doMutation
	
	public void doMutation(double probability, Solution solution) throws JMException {
		boolean flipped = false;
		int counter = 0;
		Binary variable = ((Binary) solution.getDecisionVariables()[0]);
		//System.out.println(solution.get)
		//System.out.println("doMutation FMMutator");
		while(!flipped && counter++ < 10000) {
			flipped = pfm.traverseToMutate(variable, probability);
		}
		//if(counter >= 100) System.out.println(counter + " " + flipped);
//		for (int i = 0; i < solution.getDecisionVariables().length; i++) {
//			((Binary) solution.getDecisionVariables()[i]).decode();
//		}
	}

	private boolean violatesRules(Binary variable, int j) {
		// check for "requires" rule violations
        // The root feature is mandatory
        if (j==0 && variable.bits_.get(0))
        	return true; 
        if(pfm.requiresViolation(variable, j)) { //check if there is a requires_pairs violation
//        	if(j==3) {
//        		System.out.println("requires violation at 3");
//        		System.out.println(variable);
//        	}
        	return true;
        }
        
		return false;
	}

	/**
	 * Executes the operation
	 * @param object An object containing a solution to mutate
	 * @return An object containing the mutated solution
	 * @throws JMException 
	 */
	public Object execute(Object object) throws JMException {
		Solution solution = (Solution) object;

		if (!VALID_TYPES.contains(solution.getType().getClass())) {
			Configuration.logger_.severe("BitFlipMutation.execute: the solution " +
					"is not of the right type. The type should be 'Binary', " +
					"'BinaryReal' or 'Int', but " + solution.getType() + " is obtained");

			Class cls = java.lang.String.class;
			String name = cls.getName();
			throw new JMException("Exception in " + name + ".execute()");
		} // if 

		doMutation(mutationProbability_, solution);
		return solution;
	} // execute
} // BitFlipMutation
