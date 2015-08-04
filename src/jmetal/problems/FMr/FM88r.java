package jmetal.problems.FMr;
//  FM88r.java
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

public class FM88r extends Problem {   

  private static ParsedFM featureModel;
    
  public FM88r(String solutionType) throws ClassNotFoundException {
    this(solutionType, 88, 8, new ParsedFM("..\\trunk2\\jmetal\\problems\\FMr\\FM-88.xml"));
  }

  public FM88r(String solutionType, 
               Integer numberOfBits, 
  		         Integer numberOfObjectives,
                         	ParsedFM pfm) throws ClassNotFoundException {
    numberOfVariables_  = 1;
    numberOfObjectives_ = numberOfObjectives.intValue();
    numberOfConstraints_= 0;
    problemName_        = "FM8800r";
	
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
    //public static final ParsedFM featureModel = new ParsedFM("src\\jmetal\\problems\\FMr\\FM-88.xml");
    public static final int FEATURES = 88;
    public static final int [] [] requires_pairs = {{71,23} , {72,23} , {6,73} , {6,74} , {51,36}
                                            , {36,87} , {13,77} , {32,47} , {13,76} , {6,43}
                                            , {13,75} , {32,20} , {32,19} , {13,73} , {9,34}
                                            , {39,25} , {32,21} , {39,24} , {83,31} , {34,75}
                                            , {34,77} , {34,76} , {79,30} , {78,30} , {81,23}
                                            , {80,23} , {36,38} , {36,67} , {66,29} , {32,29}
                                            , {68,37} , {38,18} , {1,36}  , {36,84} , {70,37}
                                            , {69,37} , {87,37} , {84,37} , {51,37} , {38,17} 
                                            , {1,37}  , {61,27} , {1,27}  , {58,24} , {15,57} 
                                            , {60,26} , {17,59} , {87,28} , {19,63} , {19,64} 
                                            , {20,65} , {62,28} , {82,31} , {34,81} , {36,81} 
                                            , {36,84} , {19,56} , {6,23} , {32,51}};
     
    public static ObjectiveParser op = new ObjectiveParser("..\\trunk2\\jmetal\\problems\\FMr\\qualities88.csv");

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
        // no rule in this FM
        //if (variable.bits_.get(24) && variable.bits_.get(40)) excludes_viol++;
        
        // Find the total number of features in this individual
        for(int x=0; x<FEATURES; x++)
            num_features += (variable.bits_.get(x) ? 1 : 0);
        
        // Find the total number of features that were used before
        for(int x=0; x<FEATURES; x++)
            num_used_before += ((variable.bits_.get(x) && USED_BEFORE[x]) ? 1 : 0);

        // Find the total number of known defects in the chosen features
        for(int x=0; x<FEATURES; x++)
            num_defects += (variable.bits_.get(x) ? DEFECTS[x] : 0);
        
        // Find the total cost of the chosen features
        for(int x=0; x<FEATURES; x++)
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

