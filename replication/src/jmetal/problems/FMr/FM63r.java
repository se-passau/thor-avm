package jmetal.problems.FMr;
//  FM63r.java
//
//  Author:
//       Abdel Salam Sayyad
//
//  Copyright (c) Abdel Salam Sayyad - West Virginia University
//
import jmetal.core.*;
import jmetal.encodings.solutionType.BinarySolutionType;
import jmetal.encodings.variable.Binary;
import jmetal.util.JMException;
import jmetal.util.Configuration.*;
import main.ObjectiveParser;
import main.ParsedFM;

public class FM63r extends Problem {   

  private static ParsedFM featureModel;
    
  public FM63r(String solutionType) throws ClassNotFoundException {
    this(solutionType, 63, 8, new ParsedFM("..\\trunk2\\jmetal\\problems\\FMr\\FM-63.xml"));
  }

  public FM63r(String solutionType, 
               Integer numberOfBits, 
  		         Integer numberOfObjectives,
                         	ParsedFM pfm) throws ClassNotFoundException {
    numberOfVariables_  = 1;
    numberOfObjectives_ = numberOfObjectives.intValue();
    numberOfConstraints_= 0;
    problemName_        = "FM6300r";
	
    featureModel=pfm;
    featureModel.setRequiresArray(requires_pairs);
    
    this.pfm = pfm;
        
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
    //public static final ParsedFM featureModel = new ParsedFM("src\\jmetal\\problems\\FMr\\FM-63.xml");
    public static final int FEATURES = 63;
    public static final int [] [] requires_pairs = {
                                      {37,5} , {60,32} , {10,25} , {40,5} , {35,8}
                                    , {40,11} , {39,8} , {1,11} , {36,7} , {9,25}
                                    , {38,11} , {59,8} , {1,53} , {41,6} , {52,10}
                                    , {45,5} , {61,8} , {10,53} , {15,23} , {62,8}
                                    , {1,25} , {55,5} , {9,38} , {35,11} , {25,12}
                                    , {42,4} , {38,5} , {32,8} , {15,55} , {24,25}
                                    , {23,15} , {9,10} , {25,2} , {55,15} , {43,11}
                                    , {43,8} , {54,6} , {37,11} , {10,11} , {60,8}
                                    , {55,23} , {33,8} , {36,11} , {31,4} , {53,5}
                                    , {23,55}};
    
    public static ObjectiveParser op = new ObjectiveParser("..\\trunk2\\jmetal\\problems\\FMr\\qualities63.csv");

    public static final boolean[] USED_BEFORE = op.getBools(0);

    public static final int[] DEFECTS = op.getInts(1);
    
    public static final double[] COST = op.getDoubles(2);
    
    public ParsedFM getParsedFM() {
	  return featureModel;
    }
    
  /** 
  * Evaluates a solution 
  * @param solution The solution to evaluate
   * @throws JMException 
  */    
  public void evaluate(Solution solution) throws JMException {

        Binary variable = ((Binary)solution.getDecisionVariables()[0]) ;
        int requires_viol = featureModel.requiresViolations(variable);
        int excludes_viol = 0;
        int num_features = 0;
        int num_used_before = 0;
        int num_defects = 0;
        double actual_cost = 0;
        
        // check for "excludes" rule violations
        // NO rule in this FM
        //if (variable.bits_.get(24) && variable.bits_.get(40)) excludes_viol++;
        
        // Find the total number of features in this individual
        for(int x=0; x<variable.getNumberOfBits(); x++)
            num_features += (variable.bits_.get(x) ? 1 : 0);
        
        // Find the total number of features that were used before
        for(int x=0; x<variable.getNumberOfBits(); x++)
            num_used_before += ((variable.bits_.get(x) && USED_BEFORE[x]) ? 1 : 0);

        // Find the total number of known defects in the chosen features
        for(int x=0; x<variable.getNumberOfBits(); x++)
            num_defects += (variable.bits_.get(x) ? DEFECTS[x] : 0);
        
        // Find the total cost of the chosen features
        for(int x=0; x<variable.getNumberOfBits(); x++)
            actual_cost += (variable.bits_.get(x) ? COST[x] : 0);
        
        
        // Assign objectives
        int num_violations = requires_viol + excludes_viol;
        if (numberOfObjectives_ == 8){
          // First: The correctness objective, minimize violations to
          // maximize correctness
      	
          solution.setObjective(0, num_violations);
          solution.setObjective(1, num_violations);
          solution.setObjective(2, num_violations);
          solution.setObjective(3, num_violations);

          // Second: Maximize the total number of features
          // Here: we minimize the missing features
          solution.setObjective(4, FEATURES - num_features); 

          // Third: Maximize the number of features that were used before
          // Here: we minimize the features that WERE'NT used before
          solution.setObjective(5, num_features - num_used_before); 

          // Fourth: Minimize the number of known defects in the chosen features
          solution.setObjective(6, num_defects); 

          // Fifth: Minimize the total cost
          solution.setObjective(7, actual_cost); 
      }
      else if (numberOfObjectives_ == 5){
            // First: The correctness objective, minimize violations to
            // maximize correctness
        	
            solution.setObjective(0, (requires_viol + excludes_viol));

            // Second: Maximize the total number of features
            // Here: we minimize the missing features
            solution.setObjective(1, FEATURES - num_features); 
  
            // Third: Maximize the number of features that were used before
            // Here: we minimize the features that WERE'NT used before
            solution.setObjective(2, num_features - num_used_before); 
  
            // Fourth: Minimize the number of known defects in the chosen features
            solution.setObjective(3, num_defects); 
  
            // Fifth: Minimize the total cost
            solution.setObjective(4, actual_cost); 
        }
        else if (numberOfObjectives_ == 4){
            solution.setObjective(0, requires_viol + excludes_viol);

            solution.setObjective(1, num_features - num_used_before); 
  
            solution.setObjective(2, num_defects); 
  
            solution.setObjective(3, actual_cost); 
        }
 
        else if (numberOfObjectives_ == 3){
            solution.setObjective(0, (requires_viol + excludes_viol)); 

            // Second: Maximize the total number of features
            // Here: we minimize the missing features
            solution.setObjective(1, FEATURES - num_features); 
  
            solution.setObjective(2, actual_cost); 
        }
        else if (numberOfObjectives_ == 2){
            solution.setObjective(0, (requires_viol + excludes_viol)); 

            // Second: Maximize the total number of features
            // Here: we minimize the missing features
            solution.setObjective(1, FEATURES - num_features); 
        }
  
  } // evaluate   
  
}

