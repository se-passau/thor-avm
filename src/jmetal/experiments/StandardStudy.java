//  StandardStudy.java
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

import java.util.logging.Level;
import java.util.logging.Logger;
import java.io.IOException;
import java.util.HashMap;
import java.util.Properties;

import jmetal.core.Algorithm;
import jmetal.core.Problem;
import jmetal.experiments.settings.FastPGA_Settings;
import jmetal.experiments.settings.GDE3_Settings;
import jmetal.experiments.settings.IBEA_Settings;
import jmetal.experiments.settings.MOCell_Settings;
import jmetal.experiments.settings.NSGAII_Settings;
import jmetal.experiments.settings.PAES_Settings;
import jmetal.experiments.settings.PESA2_Settings;
import jmetal.experiments.settings.SPEA2_Settings;
import jmetal.experiments.settings.SMPSO_Settings;
import jmetal.experiments.util.RBoxplot;
import jmetal.experiments.util.RWilcoxon;
import jmetal.util.JMException;

/**
 * Class implementing a typical experimental study. Five algorithms are 
 * compared when solving the ZDT, DTLZ, and WFG benchmarks, and the hypervolume,
 * spread and additive epsilon indicators are used for performance assessment.
 */
public class StandardStudy extends Experiment {

  /**
   * Configures the algorithms in each independent run
   * @param problemName The problem to solve
   * @param problemIndex
   * @throws ClassNotFoundException 
   */
  public void algorithmSettings(String problemName, 
  		                          int problemIndex, 
  		                          Algorithm[] algorithm) throws ClassNotFoundException {
    try {
      int numberOfAlgorithms = algorithmNameList_.length;

      HashMap[] parameters = new HashMap[numberOfAlgorithms];
      

      for (int i = 0; i < numberOfAlgorithms; i++) {
        parameters[i] = new HashMap();
      } // for
      
      for (int i = 0; i < numberOfAlgorithms; i++){
            parameters[i].put("runTime_", 300000); // 5 MIN
            parameters[i].put("populationSize_", 100);
            parameters[i].put("archiveSize_", 100);
            parameters[i].put("crossoverProbability_", 0.0);
    }

      if (!paretoFrontFile_[problemIndex].equals("")) {
        for (int i = 0; i < numberOfAlgorithms; i++)
          parameters[i].put("paretoFrontFile_", paretoFrontFile_[problemIndex]);
        } // if

        algorithm[0] = new NSGAII_Settings(problemName).configure(parameters[0]);
        algorithm[1] = new SPEA2_Settings(problemName).configure(parameters[1]);
        algorithm[2] = new FastPGA_Settings(problemName).configure(parameters[2]);
        algorithm[3] = new IBEA_Settings(problemName).configure(parameters[3]);
        algorithm[4] = new MOCell_Settings(problemName).configure(parameters[4]);
        
      } catch (IllegalArgumentException ex) {
      Logger.getLogger(StandardStudy.class.getName()).log(Level.SEVERE, null, ex);
    } catch (IllegalAccessException ex) {
      Logger.getLogger(StandardStudy.class.getName()).log(Level.SEVERE, null, ex);
    } catch  (JMException ex) {
      Logger.getLogger(StandardStudy.class.getName()).log(Level.SEVERE, null, ex);
    }
  } // algorithmSettings

  /**
   * Main method
   * @param args
   * @throws JMException
   * @throws IOException
   */
  public static void main(String[] args) throws JMException, IOException {
    StandardStudy exp = new StandardStudy();

    exp.experimentName_ = "StandardStudy";
    exp.algorithmNameList_ = new String[]{
                                "NSGAII", "SPEA2", "FastPGA", "IBEA", "MOCell"};
    exp.problemList_ = new String[]{
        "FM43r", "FM43Br", "FM44r", "FM45r", "FM46r", "FM52r", "FM53r", "FM60r", "FM61r", "FM63r", 
        "FM66r", "FM67r", "FM70r", "FM72r", "FM72Br", "FM88r", "FM94r", "FM97r", "FM137r", "FM290r"} ;
    exp.paretoFrontFile_ = new String[]{
 /*       "FM43_5obj.pf", "FM43B_5obj.pf", "FM44_5obj.pf", "FM45_5obj.pf", "FM46_5obj.pf", 
        "FM52_5obj.pf", "FM53_5obj.pf", "FM60_5obj.pf", "FM61_5obj.pf", "FM63_5obj.pf",
        "FM66_5obj.pf", "FM67_5obj.pf", "FM70_5obj.pf", "FM72_5obj.pf", "FM72B_5obj.pf", 
        "FM88_5obj.pf", "FM94_5obj.pf", "FM97_5obj.pf", "FM137_5obj.pf", "FM290_5obj.pf"} ; */
        "FM43_8obj.pf", "FM43B_8obj.pf", "FM44_8obj.pf", "FM45_8obj.pf", "FM46_8obj.pf", 
        "FM52_8obj.pf", "FM53_8obj.pf", "FM60_8obj.pf", "FM61_8obj.pf", "FM63_8obj.pf",
        "FM66_8obj.pf", "FM67_8obj.pf", "FM70_8obj.pf", "FM72_8obj.pf", "FM72B_8obj.pf", 
        "FM88_8obj.pf", "FM94_8obj.pf", "FM97_8obj.pf", "FM137_8obj.pf", "FM290_8obj.pf"} ;
 

    exp.indicatorList_   = new String[] {"HV", "SPREAD", "PCORRECT", "TimeToAnyC", "TimeTo50C", "TimeTo100C"} ;
    
    int numberOfAlgorithms = exp.algorithmNameList_.length ;

    exp.experimentBaseDirectory_ = "" +
                                   exp.experimentName_;
    exp.paretoFrontDirectory_ = "PF/";
    
    exp.algorithmSettings_ = new Settings[numberOfAlgorithms] ;
    
    exp.independentRuns_ = 10 ;

    // Run the experiments
    int numberOfThreads = 1;
    exp.runExperiment(numberOfThreads) ;

    // Generate latex tables
    exp.generateLatexTables() ;

    // Configure the R scripts to be generated
//    int rows  ;
//    int columns  ;
//    String prefix ;
//    String [] problems ;
//    boolean notch ;
//
//    // Configuring scripts for ZDT
//    rows = 3 ;
//    columns = 2 ;
//    prefix = new String("ZDT");
//    problems = new String[]{"ZDT1", "ZDT2","ZDT3", "ZDT4","ZDT6"} ;
//    
//    exp.generateRBoxplotScripts(rows, columns, problems, prefix, notch = false, exp) ;
//    exp.generateRWilcoxonScripts(problems, prefix, exp) ;
//
//    // Configure scripts for DTLZ
//    rows = 3 ;
//    columns = 3 ;
//    prefix = new String("DTLZ");
//    problems = new String[]{"DTLZ1","DTLZ2","DTLZ3","DTLZ4","DTLZ5",
//                                    "DTLZ6","DTLZ7"} ;
//
//    exp.generateRBoxplotScripts(rows, columns, problems, prefix, notch=false, exp) ;
//    exp.generateRWilcoxonScripts(problems, prefix, exp) ;
//
//    // Configure scripts for WFG
//    rows = 3 ;
//    columns = 3 ;
//    prefix = new String("WFG");
//    problems = new String[]{"WFG1","WFG2","WFG3","WFG4","WFG5","WFG6",
//                            "WFG7","WFG8","WFG9"} ;
//
//    exp.generateRBoxplotScripts(rows, columns, problems, prefix, notch=false, exp) ;
//    exp.generateRWilcoxonScripts(problems, prefix, exp) ;
  } // main
} // StandardStudy


