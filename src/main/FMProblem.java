package main;
//  FM43.java
//
//  Author:
//       Abdel Salam Sayyad
//
//  Copyright (c) Abdel Salam Sayyad - West Virginia University
//


import fm.FeatureModel;
import jmetal.core.*;
import jmetal.encodings.solutionType.BinarySolutionType;
import jmetal.encodings.variable.Binary;
import jmetal.util.JMException;
import jmetal.util.Configuration.*;

/** 
 * Class representing problem DTLZ1 
 */
public class FMProblem extends Problem {
	
	private ParsedFM pfm;
    
 /** 
  * Creates a default FM43 problem (43 variables and 5 objectives)
  * @param solutionType The solution type must "Binary". 
  */
  public FMProblem(String solutionType) throws ClassNotFoundException {
    this(solutionType, 43, 4, null);
  } // FM43   
    
  /** 
  * Creates a FM43 problem instance
  * @param numberOfVariables Number of variables
  * @param numberOfObjectives Number of objective functions
  * @param solutionType The solution type must "Binary". 
  */
  public FMProblem(String solutionType, 
               Integer numberOfBits, 
  		         Integer numberOfObjectives, ParsedFM pfm) throws ClassNotFoundException {
    numberOfVariables_  = 1;
    numberOfObjectives_ = numberOfObjectives.intValue();
    numberOfConstraints_= 0;
    problemName_        = "FM";
    
    this.pfm=pfm;
    
    features=numberOfBits;
        
    lowerLimit_ = new double[numberOfVariables_];
    upperLimit_ = new double[numberOfVariables_];        
    //for (int var = 0; var < numberOfBits.intValue(); var++){
      lowerLimit_[0] = 0.0;
      upperLimit_[0] = 1.0;
   // } //for
        
    if (solutionType.compareTo("Binary") == 0)
    	solutionType_ = new BinarySolutionType(this) ;
    else {
    	System.out.println("Error: solution type " + solutionType + " invalid") ;
    	System.exit(-1) ;
    }
        length_    = new int[numberOfVariables_];
        length_[0] = numberOfBits.intValue();
}            

    public int features;
//    public static final int [] [] requires_pairs = { {1,0} , {2,1} , {3,2} , {2,3} , {4,2}
//                                   , {5,1} , {6,5} , {7,5} , {7,8} , {8,7}
//                                   , {9,7} , {10,1} , {11,10} , {10,11} , {12,10}
//                                   , {13,10} , {10,13} , {13,14} , {14,13} , {15,13}
//                                   , {16,10} , {17,0} , {0,17} , {18,17} , {21,17}
//                                   , {25,17} , {17,25} , {26,25} , {25,26} , {27,25}
//                                   , {32,0} , {35,0} , {39,0} , {19,18} , {20,18}
//                                   , {22,21} , {23,21} , {24,21} , {28,27} , {29,27}
//                                   , {30,27} , {31,27} , {33,32} , {34,32} , {36,35}
//                                   , {37,35} , {38,35} , {40,39} , {41,39} , {42,39}
//                // These are Cross-Tree Constraints
//                                   , {16,7} , {9,27} , {19,34} , {37,24} , {20,23}
//                                    };
//    
//    public static final int REQUIRE_RULES = 55;
//    public static final int TOTAL_RULES = 63;
//
//    public static final boolean[] USED_BEFORE = { false,
//                false,true,false,true,false,false,true,false,true,false,false,
//                false,true,true,true,true,false,true,true,false,false,false,
//                true,true,true,false,false,false,true,true,true,true,false,
//                true,true,false,true,false,false,false,false,true};
//
//    public static final int[] DEFECTS = {0,
//                0, 7, 0, 5, 0, 0, 7, 0, 5, 0, 0, 0, 5, 7, 2, 7,
//                0, 6, 5, 0, 0, 0, 4, 5, 3, 0, 0, 0, 3, 5, 4, 6,
//                0, 5, 3, 0, 3, 0, 0, 0, 0, 7 };
    
    //public static final int TOTAL_DEFECTS = 101;
    
//    public static final double[] COST = { 0,
//                5, 15, 9.437944916309213, 8.974708471019492, 15,
//                5, 12.208244241051922, 6.50083671587239, 14.110958023422786, 15,
//                5, 15, 12.076337566423613, 15, 9.800129483196427,
//                5, 5.581909279923921, 9.795752516630904, 11.44435770281072, 15,
//                5, 10.24593954448829, 8.185037631158725, 7.767459553000039, 5,
//                13.09760717470945, 5, 5.616340573945801, 11.897564657905031, 5.819198528310675,
//                5.558447390213202, 11.100553816341085, 5, 12.959441183284374, 6.263798773081946,
//                13.664147283553582, 5.800419393172235, 7.591085104466932, 11.139306972848676, 14.646086288312482,
//                8.153020813199923, 7.403858817222789};
    
    //public static final double TOTAL_COST = 396.8404924;
    
  public ParsedFM getParsedFM() {
	  return pfm;
  }
  
  /** 
  * Evaluates a solution 
  * @param solution The solution to evaluate
   * @throws JMException 
  */    
  public void evaluate(Solution solution) throws JMException {
	  
	  int selectedFeatures=0;
	  Binary variable = ((Binary)solution.getDecisionVariables()[0]) ;
	  for(int x=0; x<variable.getNumberOfBits(); x++)
          selectedFeatures += (variable.bits_.get(x) ? 1 : 0);
       
	  solution.setObjective(0, features - selectedFeatures); 
  
  } // evaluate   
  
}

