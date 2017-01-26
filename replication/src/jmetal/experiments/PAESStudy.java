//  PAESStudy.java
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
 * Class implementing an example of experiment using FastPGA as base algorithm.
 * The experiment consisting in studying the effect of the crossover probability
 * in FastPGA.
 */
public class PAESStudy extends Experiment {
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
/*
      parameters[0].put("mutationProbability_", 0.01);
      parameters[1].put("mutationProbability_", 0.02);
      parameters[2].put("mutationProbability_", 0.03);
      parameters[3].put("mutationProbability_", 0.04); 
      parameters[4].put("mutationProbability_", 0.05);
      parameters[5].put("mutationProbability_", 0.1);
      parameters[6].put("mutationProbability_", 0.2);
      parameters[7].put("mutationProbability_", 0.3); 
      parameters[8].put("mutationProbability_", 0.4);
      parameters[9].put("mutationProbability_", 0.5);
      parameters[10].put("mutationProbability_", 0.6);
      parameters[11].put("mutationProbability_", 0.7); 
      parameters[12].put("mutationProbability_", 0.8); 
      parameters[13].put("mutationProbability_", 0.9); 
      parameters[14].put("mutationProbability_", 0.95); 

      parameters[0].put("crossoverProbability_", 0.01);
      parameters[1].put("crossoverProbability_", 0.02);
      parameters[2].put("crossoverProbability_", 0.03);
      parameters[3].put("crossoverProbability_", 0.04); 
      parameters[4].put("crossoverProbability_", 0.05);
      parameters[5].put("crossoverProbability_", 0.06);
      parameters[6].put("crossoverProbability_", 0.07);
      parameters[7].put("crossoverProbability_", 0.08); 
      parameters[8].put("crossoverProbability_", 0.09);
      parameters[9].put("crossoverProbability_", 0.1);
      parameters[10].put("crossoverProbability_", 0.15);
      parameters[11].put("crossoverProbability_", 0.2); 
      parameters[12].put("crossoverProbability_", 0.3); 
      parameters[13].put("crossoverProbability_", 0.4); 
      parameters[14].put("crossoverProbability_", 0.5); 
      parameters[15].put("crossoverProbability_", 0.6);
      parameters[16].put("crossoverProbability_", 0.7); 
      parameters[17].put("crossoverProbability_", 0.8); 
      parameters[18].put("crossoverProbability_", 0.9); 
      parameters[19].put("crossoverProbability_", 0.95); 
      
 /*     
      parameters[0].put("populationSize_", 20);
      parameters[1].put("populationSize_", 40);
      parameters[2].put("populationSize_", 60);
      parameters[3].put("populationSize_", 80); 
      parameters[4].put("populationSize_", 100);
      parameters[5].put("populationSize_", 200);
      parameters[6].put("populationSize_", 300);
      parameters[7].put("populationSize_", 400); 
      parameters[8].put("populationSize_", 500);
      parameters[9].put("populationSize_", 600);
      parameters[10].put("populationSize_", 700);
      parameters[11].put("populationSize_", 800);
    
      parameters[0].put("archiveSize_", 20);
      parameters[1].put("archiveSize_", 40);
      parameters[2].put("archiveSize_", 60);
      parameters[3].put("archiveSize_", 80); 
      parameters[4].put("archiveSize_", 100);
      parameters[5].put("archiveSize_", 200);
      parameters[6].put("archiveSize_", 300);
      parameters[7].put("archiveSize_", 400); 
      parameters[8].put("archiveSize_", 500);
      parameters[9].put("archiveSize_", 600);
      parameters[10].put("archiveSize_", 700);
      parameters[11].put("archiveSize_", 800);
*/
 
      for (int i = 0; i < numberOfAlgorithms; i++){
            parameters[i].put("runTime_", 300000); // 1 sec
            parameters[i].put("populationSize_", 100);
            parameters[i].put("archiveSize_", 100);
            parameters[i].put("mutationProbability_", 0.01);
            //parameters[i].put("crossoverProbability_", 0.1);
      }

      if ((!paretoFrontFile_[problemIndex].equals("")) || 
      		(paretoFrontFile_[problemIndex] == null)) {
        for (int i = 0; i < numberOfAlgorithms; i++)
          parameters[i].put("paretoFrontFile_",  paretoFrontFile_[problemIndex]);
      } // if
 
      for (int i = 0; i < numberOfAlgorithms; i++)
        algorithm[i] = new PAES_Settings(problemName).configure(parameters[i]);
      
    } catch (IllegalArgumentException ex) {
      Logger.getLogger(PAESStudy.class.getName()).log(Level.SEVERE, null, ex);
    } catch (IllegalAccessException ex) {
      Logger.getLogger(PAESStudy.class.getName()).log(Level.SEVERE, null, ex);
    } catch (JMException ex) {
      Logger.getLogger(PAESStudy.class.getName()).log(Level.SEVERE, null, ex);
    }
  } // algorithmSettings
  
  public static void main(String[] args) throws JMException, IOException {
    PAESStudy exp = new PAESStudy() ; // exp = experiment
    
    exp.experimentName_  = "PAESStudy" ;
    exp.algorithmNameList_   = new String[] {"PAES"};
      /*"IBEA0", "IBEA1", "IBEA2", "IBEA3", "IBEA4", "IBEA5", "IBEA6", "IBEA7", 
        "IBEA8", "IBEA9", "IBEA10", "IBEA11", "IBEA12", "IBEA13", "IBEA14", "IBEA15",
        "IBEA16", "IBEA17", "IBEA18", "IBEA19"} ;*/
    exp.problemList_     = new String[] {
      //"FM43r", "FM43Br", "FM44r", "FM45r", "FM46r", "FM52r", "FM53r", "FM60r", "FM61r", "FM63r"};//, 
      "FM66r", "FM67r", "FM70r", "FM72r", "FM72Br", "FM88r", "FM94r", "FM97r", "FM137r", "FM290r"} ;
    exp.paretoFrontFile_ = new String[] {
      //"FM43_5obj.pf", "FM43B_5obj.pf", "FM44_5obj.pf", "FM45_5obj.pf", "FM46_5obj.pf", 
      //"FM52_5obj.pf", "FM53_5obj.pf", "FM60_5obj.pf", "FM61_5obj.pf", "FM63_5obj.pf"};//, 
      "FM66_5obj.pf", "FM67_5obj.pf", "FM70_5obj.pf", "FM72_5obj.pf", "FM72B_5obj.pf", 
      "FM88_5obj.pf", "FM94_5obj.pf", "FM97_5obj.pf", "FM137_5obj.pf", "FM290_5obj.pf"} ;
    exp.indicatorList_   = new String[] {"HV", "SPREAD", "PCORRECT"} ;
    
    int numberOfAlgorithms = exp.algorithmNameList_.length ;

    exp.experimentBaseDirectory_ = "" +
                                   exp.experimentName_;
    exp.paretoFrontDirectory_ = "PF";
    
    exp.algorithmSettings_ = new Settings[numberOfAlgorithms] ;
    
    exp.independentRuns_ = 10 ;
    
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
} // PAESStudy


