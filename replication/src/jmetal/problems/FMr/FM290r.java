package jmetal.problems.FMr;
//  FM290r.java
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

public class FM290r extends Problem {   

  private static ParsedFM featureModel;
    
  public FM290r(String solutionType) throws ClassNotFoundException {
      this(solutionType, 290, 5 /* 8 */, new ParsedFM(FM290r.class.getResource("FM-290.xml").getPath()));
  }

  public FM290r(String solutionType, 
               Integer numberOfBits, 
  		         Integer numberOfObjectives,
                         	ParsedFM pfm) throws ClassNotFoundException {
    numberOfVariables_  = 1;
    numberOfObjectives_ = numberOfObjectives.intValue();
    numberOfConstraints_= 0;
    problemName_        = "FM290AAr";
	
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
    //public static final ParsedFM featureModel = new ParsedFM("src\\jmetal\\problems\\FMr\\FM-290.xml");
    public static final int FEATURES = 290;
    public static final int [] [] requires_pairs = {                 
				// These are Cross-Tree Constraints
				  {91,92} , {44,65} , {45,65} , {44,203} , {40,178}
                                , {108,38} , {211,33} , {46,206} , {45,186} , {216,91}
				, {107,14} , {107,12} , {111,188} , {7,231} , {67,264}
				, {45,66} , {80,69} , {95,11} , {93,11}
				};
    
    public static ObjectiveParser op = new ObjectiveParser(FM290r.class.getResource("qualities290.csv").getPath());

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
        int other_viol = 0;
        int num_features = 0;
        int num_used_before = 0;
        int num_defects = 0;
        double actual_cost = 0;
        
        // check for other rule violations
        // 2 CTC rules (c13b and c18) 
        if (!variable.bits_.get(11) && !variable.bits_.get(92)) other_viol++;
        if (variable.bits_.get(217) && !(variable.bits_.get(180) || variable.bits_.get(181))) other_viol++;
        
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
        int num_violations = requires_viol + other_viol;
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
        	
            solution.setObjective(0, requires_viol + other_viol);

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
            solution.setObjective(0, requires_viol + other_viol);

            solution.setObjective(1, num_features - num_used_before); 
  
            solution.setObjective(2, num_defects); 
  
            solution.setObjective(3, actual_cost); 
        }
 
        else if (numberOfObjectives_ == 3){
            solution.setObjective(0, (requires_viol + other_viol)); 

            // Second: Maximize the total number of features
            // Here: we minimize the missing features
            solution.setObjective(1, FEATURES - num_features); 
  
            solution.setObjective(2, actual_cost); 
        }
        else if (numberOfObjectives_ == 2){
            solution.setObjective(0, (requires_viol + other_viol)); 

            // Second: Maximize the total number of features
            // Here: we minimize the missing features
            solution.setObjective(1, FEATURES - num_features); 
        }
  
  } // evaluate   
  
}

