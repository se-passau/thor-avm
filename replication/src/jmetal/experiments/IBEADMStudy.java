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
import java.util.Properties;
import java.util.logging.Level;

import jmetal.core.Algorithm;
import jmetal.core.Problem;
import jmetal.experiments.settings.*;
import jmetal.experiments.util.RBoxplot;
import jmetal.experiments.util.RWilcoxon;
import jmetal.util.JMException;

/**
 * Class implementing an example of experiment using IBEA as base algorithm.
 * The experiment consisting in studying the effect of the crossover probability
 * in IBEA.
 */
public class IBEADMStudy extends Experiment {
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
        parameters[i] = new HashMap();
      } // for

      for (int i = 0; i < numberOfAlgorithms; i++){
            parameters[i].put("runTime_", 300000); // 5 minutes
            parameters[i].put("populationSize_", 100);
            parameters[i].put("archiveSize_", 100);
            parameters[i].put("crossoverProbability_", 0.0);
            parameters[i].put("crossoverOperator_", "FMCrossover");
      }

      if ((!paretoFrontFile_[problemIndex].equals("")) || 
      		(paretoFrontFile_[problemIndex] == null)) {
        for (int i = 0; i < numberOfAlgorithms; i++) 
          parameters[i].put("paretoFrontFile_",  paretoFrontFile_[problemIndex]);
      } // if
 
        algorithm[0] = new NSGAII_Settings(problemName).configure(parameters[0]);
        algorithm[1] = new SPEA2_Settings(problemName).configure(parameters[1]);
        algorithm[2] = new FastPGA_Settings(problemName).configure(parameters[2]);
        algorithm[3] = new IBEA_Settings(problemName).configure(parameters[3]);
        algorithm[4] = new MOCell_Settings(problemName).configure(parameters[4]);
      
    } catch (IllegalArgumentException ex) {
      Logger.getLogger(IBEAStudy.class.getName()).log(Level.SEVERE, null, ex);
    } catch (IllegalAccessException ex) {
      Logger.getLogger(IBEAStudy.class.getName()).log(Level.SEVERE, null, ex);
    } catch (JMException ex) {
      Logger.getLogger(IBEAStudy.class.getName()).log(Level.SEVERE, null, ex);
    }
  } // algorithmSettings
  
  public static void main(String[] args) throws JMException, IOException {
    IBEADMStudy exp = new IBEADMStudy() ; // exp = experiment
    
    exp.experimentName_  = "IBEADMStudy" ;
    exp.algorithmNameList_ = new String[]{
                                "NSGAII", "SPEA2", "FastPGA", "IBEA", "MOCell"};
   exp.problemList_ = new String[] {
       "Ptoybox", "PaxTLS", "Pecos", "Pfreebsd", "Pfiasco", "PuClinux", "P286"}; 
       //"Pbusybox", "PuClinuxconfig", "Pcoreboot"};
        //"Pbuildroot", "Pembtoolkit", "Pfreetz", "P322", "P3332"};
    
    exp.paretoFrontFile_ = new String[] {
        "FM544_5obj.pf", "FM684_5obj.pf", "FM1244_5obj.pf", 
        "FM1396_5obj.pf", "FM1638_5obj.pf", "FM1850_5obj.pf", "FM6888_5obj.pf"};  
//        "FM6796_5obj.pf", "FM544_5obj.pf", "FM684_5obj.pf", "FM1244_5obj.pf", 
//        "FM1396_5obj.pf", "FM1638_5obj.pf", "FM1850_5obj.pf", //"FM6796_5obj.pf", 
//        "FM6888_5obj.pf"};//, "FM11254_5obj.pf", "FM12268_5obj.pf"};
        //"FM14910_5obj.pf", "FM23516_5obj.pf", "FM31012_5obj.pf", "FM60072_5obj.pf", "FM62482_5obj.pf"};
    
    exp.indicatorList_   = new String[] {"HV", "SPREAD", "PCORRECT", "TimeToAnyC", "TimeTo50C", "TimeTo100C"} ;
    
    int numberOfAlgorithms = exp.algorithmNameList_.length ;

    exp.experimentBaseDirectory_ = "" +
                                   exp.experimentName_;
    exp.paretoFrontDirectory_ = "PF/";
    
    exp.algorithmSettings_ = new Settings[numberOfAlgorithms] ;
    
    exp.independentRuns_ = 10;
    
    // Run the experiments
    int numberOfThreads = 1;
    exp.runExperiment(numberOfThreads) ;
    
    // Generate latex tables (comment this sentence is not desired)
    exp.generateLatexTables() ;
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


