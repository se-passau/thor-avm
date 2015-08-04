//  SinglePointCrossover.java
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

package main;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Properties;

import jmetal.core.*;
import jmetal.encodings.solutionType.ArrayRealSolutionType;
import jmetal.encodings.solutionType.BinaryRealSolutionType;
import jmetal.encodings.solutionType.BinarySolutionType;
import jmetal.encodings.solutionType.IntSolutionType;
import jmetal.encodings.solutionType.RealSolutionType;
import jmetal.encodings.variable.*;
import jmetal.operators.crossover.Crossover;
import jmetal.util.Configuration;
import jmetal.util.JMException;
import jmetal.util.PseudoRandom;
import jmetal.util.Configuration.*;

/**
 * This class allows to apply a Single Point crossover operator using two parent
 * solutions.
 */
public class FMCrossover extends Crossover {
  /**
   * Valid solution types to apply this operator 
   */
  private static List VALID_TYPES = Arrays.asList(BinarySolutionType.class, 
  		                                            BinaryRealSolutionType.class,
  		                                            IntSolutionType.class) ;

  private Double crossoverProbability_ = null;

  /**
   * Constructor
   * Creates a new instance of the single point crossover operator
   */
  public FMCrossover(HashMap<String, Object> parameters) {
  	super(parameters) ;
  	if (parameters.get("probability") != null)
  		crossoverProbability_ = (Double) parameters.get("probability") ;  		
  } // SinglePointCrossover


  /**
   * Constructor
   * Creates a new instance of the single point crossover operator
   */
  //public SinglePointCrossover(Properties properties) {
  //    this();
  //} // SinglePointCrossover


  /**
   * Executes the operation
   * @param object An object containing an array of two solutions
   * @return An object containing an array with the offSprings
   * @throws JMException
   */
  public Object execute(Object object) throws JMException {
    Solution[] parents = (Solution[]) object;

//    if (!(VALID_TYPES.contains(parents[0].getType().getClass())  &&
//        VALID_TYPES.contains(parents[1].getType().getClass())) ) {
//
//      Configuration.logger_.severe("SinglePointCrossover.execute: the solutions " +
//              "are not of the right type. The type should be 'Binary' or 'Int', but " +
//              parents[0].getType() + " and " +
//              parents[1].getType() + " are obtained");
//
//      Class cls = java.lang.String.class;
//      String name = cls.getName();
//      throw new JMException("Exception in " + name + ".execute()");
//    } // if
//
//    if (parents.length < 2) {
//      Configuration.logger_.severe("SinglePointCrossover.execute: operator " +
//              "needs two parents");
//      Class cls = java.lang.String.class;
//      String name = cls.getName();
//      throw new JMException("Exception in " + name + ".execute()");
//    } 
    
    Solution[] offSpring = new Solution[2];
    offSpring[0] = new Solution(parents[0]);
    offSpring[1] = new Solution(parents[1]);
//    
//    //-> Update the offSpring solutions
//    for (int i = 0; i < offSpring.length; i++) {
//      offSpring[i].setCrowdingDistance(0.0);
//      offSpring[i].setRank(0);
//    }
    return offSpring;
  } // execute
} // SinglePointCrossover
