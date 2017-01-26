//  BinarySolutionType.java
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

package jmetal.encodings.solutionType;

import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.Scanner;

import main.DimacsFM;
import main.FMProblem;
import main.ParsedFM;
import jmetal.core.Problem;
import jmetal.core.SolutionType;
import jmetal.core.Variable;
import jmetal.encodings.variable.Binary;
//import jmetal.problems.FM290r;

/**
 * Class representing the solution type of solutions composed of Binary 
 * variables
 */
public class BinarySolutionType extends SolutionType {

	/**
	 * Constructor
	 * @param problem
	 * @throws ClassNotFoundException 
	 */
	public BinarySolutionType(Problem problem) throws ClassNotFoundException {
		super(problem) ;
	} // Constructor
	
	/**
	 * Creates the variables of the solution
	 * @param decisionVariables
	 */
	public Variable[] createVariables() {
		Variable[]  variables = new Variable[problem_.getNumberOfVariables()];
		ParsedFM pfm = problem_.pfm;
		DimacsFM dfm = problem_.dfm;
		//System.out.println("dfm?: " + problem_.dfm);
		if(dfm != null) { //dfm
			//System.out.print("dfm ");
			for (int var = 0; var < problem_.getNumberOfVariables(); var++) {
		    	Binary b = new Binary(dfm.getInitialString());
		    	variables[var] = b;
			}
		}
		else if(pfm==null) { //not Tree Mutation
			for (int var = 0; var < problem_.getNumberOfVariables(); var++)
		    	variables[var] = new Binary(problem_.getLength(var)); 
		}
		else { //Tree Mutation
			//System.out.println("Binary Solution Type: tree mutation check");
		    for (int var = 0; var < problem_.getNumberOfVariables(); var++) {
		    	Binary b = new Binary(pfm.getInitialString());
		    	variables[var] = b;
		    	//System.out.println(b);
		    	//System.out.println(pfm.correctString(b));
		    }
		}
		return variables ;
	} // createVariables
	
	public Variable[] createVariables(String sol) {
		Variable[]  variables = new Variable[problem_.getNumberOfVariables()];
		DimacsFM dfm = problem_.dfm;
		//System.out.println("dfm?: " + problem_.dfm);
		if(dfm != null) { //dfm
			//System.out.print("dfm ");
			for (int var = 0; var < problem_.getNumberOfVariables(); var++) {
		    	Binary b = new Binary(sol);
		    	variables[var] = b;
			}
		}
		return variables ;
	} // createVariables
} // BinarySolutionType
