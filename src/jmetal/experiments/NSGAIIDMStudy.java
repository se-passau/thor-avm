//  IBEAStudy.java
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

package jmetal.experiments;

import java.util.logging.Logger;
import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Arrays;
import java.util.Properties;
import java.util.logging.Level;

import jmetal.core.Algorithm;
import jmetal.core.Problem;
import jmetal.experiments.settings.NSGAII_Settings;
import jmetal.experiments.settings.IBEA_Settings;
import jmetal.experiments.util.RBoxplot;
import jmetal.experiments.util.RWilcoxon;
import jmetal.util.JMException;


public class NSGAIIDMStudy extends Experiment {
  /**
   * Configures the algorithms in each independent run
   * @param problem The problem to solve
   * @param problemIndex
   * @param algorithm Array containing the algorithms to run
   * @throws ClassNotFoundException 
   */
  public synchronized void algorithmSettings(String problemName, 
  		                                       int problemIndex, 
  		                                       Algorithm[] algorithm) 
    throws ClassNotFoundException {  	
  	try {
	    int numberOfAlgorithms = algorithmNameList_.length;
	    
	    HashMap[] parameters = new HashMap[numberOfAlgorithms];
	    
	    for (int i = 0; i < numberOfAlgorithms; i++) {
		HashMap params = new HashMap();
		params.put("runTime_", 1800000); //30 minutes
		params.put("populationSize_", 300);
		params.put("archiveSize_", 300);
		params.put("mutationProbability_", 0.001); 
		params.put("mutationOperator_", "DMBitFlipMutation");
		params.put("crossoverProbability_", 0.05);
		params.put("crossoverOperator_", "SinglePointCrossover");
		
		if ((!paretoFrontFile_[problemIndex].equals("")) || 
		    (paretoFrontFile_[problemIndex] == null)) {
		    params.put("paretoFrontFile_",  paretoFrontFile_[problemIndex]);
		}
		
		switch (algorithmNameList_[i]) {
		case "IBEA":
		    algorithm[i] = new IBEA_Settings(problemName).configure(params);
		    break;
		case "NSGAII":
		    algorithm[i] = new NSGAII_Settings(problemName).configure(params);
		    break;
		default:
		    throw new IllegalArgumentException("Unsupported algorithm.");
		}
	    }
	    
	} catch (IllegalArgumentException ex) {
	    Logger.getLogger(NSGAIIDMStudy.class.getName()).log(Level.SEVERE,
								null,
								ex);
	} catch (IllegalAccessException ex) {
	    Logger.getLogger(NSGAIIDMStudy.class.getName()).log(Level.SEVERE,
								null,
								ex);
	} catch (JMException ex) {
	    Logger.getLogger(NSGAIIDMStudy.class.getName()).log(Level.SEVERE,
								null,
								ex);
	}
  } // algorithmSettings
    
    private static String[] paretoFrontFiles = {
	    "FM544_5obj.pf",
	    "FM544_5obj.pf",
	    "toybox.pf",
	    "FM684_5obj.pf",
	    "FM1244_5obj.pf",
	    "FM1396_5obj.pf",
	    "FM1638_5obj.pf",
	    "FM1850_5obj.pf",
	    "FM6888_5obj.pf"
	};
    
    private static List<String> availProblems = Arrays.asList(
	"Ptoybox",
	"Ptoybox2",
	"Ptoybox3",
	"PaxTLS",
	"Pecos",
	"Pfreebsd",
	"Pfiasco",
	"PuClinux", 
	"P286");
    
    private static String[] getParetoFF (String[] probs) {
	String res[] = new String[probs.length];
	for (int i = 0; i < probs.length; i++) {
	    res[i] = paretoFrontFiles[availProblems.indexOf(probs[i])];
	}
	return res;
    }

    
  public static void main(String[] args) throws JMException, IOException {
    NSGAIIDMStudy exp = new NSGAIIDMStudy() ; // exp = experiment
    int numberOfThreads = -1, numberOfRuns = -1;
    String[] probsList = null, algoList = null;
	
    exp.experimentName_  = "NSGAIIDMStudy" ;
    
    if (args.length == 5) {
	try {
	    algoList = args[0].split(":");
	    probsList = args[1].split(":");
	    numberOfRuns = Integer.parseInt(args[2]);
	    numberOfThreads = Integer.parseInt(args[3]);
	    if (args[4] != "") {
		exp.experimentName_ = args[4];
	    }
	} catch (Exception e) {
	    System.err.println("Malformatted arguments.");
	    System.exit(1);
	}
    } else {
	System.err.println("Wrong numbers of arguments.");
	System.exit(1);
    }
   
    exp.algorithmNameList_  = algoList; // new String[] {"IBEA", "NSGAII" };

    exp.problemList_ = probsList;
    exp.paretoFrontFile_ = getParetoFF(probsList);

    exp.indicatorList_ = new String[] {
	"HV",
	"SPREAD",
	"PCORRECT",
	"TimeToAnyC",
	"TimeTo50C",
	"TimeTo100C"
    };
    
    int numberOfAlgorithms = exp.algorithmNameList_.length ;

    exp.experimentBaseDirectory_ = "" +
                                   exp.experimentName_;
    exp.paretoFrontDirectory_ = "PF/";
    
    exp.algorithmSettings_ = new Settings[numberOfAlgorithms] ;
    
    exp.independentRuns_ = numberOfRuns;
    
    // Run the experiments
    exp.runExperiment(numberOfThreads) ;
    
    // Generate latex tables (comment this sentence is not desired)
    // exp.generateLatexTables() ;
   /* 
    // Configure the R scripts to be generated
    int rows  ;
    int columns  ;
    String prefix ;
    String [] problems ;

    rows = 2 ;
    columns = 3 ;
    prefix = new String("Problems");
    problems = new String[]{"ZDT1", "ZDT2","ZDT3", "ZDT4", "DTLZ1", "WFG2"} ;

    boolean notch ;
    exp.generateRBoxplotScripts(rows, columns, problems, prefix, notch = true, exp) ;
    exp.generateRWilcoxonScripts(problems, prefix, exp) ;  */
  } // main
} // IBEAStudy


