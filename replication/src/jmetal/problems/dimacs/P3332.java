package jmetal.problems.dimacs;

import jmetal.core.*;
import jmetal.encodings.solutionType.BinarySolutionType;
import jmetal.encodings.variable.Binary;
import jmetal.util.JMException;
import jmetal.util.Configuration.*;
import main.DimacsFM;
import main.ObjectiveParser;

public class P3332 extends Problem {   

  public static final int FEATURES = 62482;	
  //public static final DimacsFM dfm = new DimacsFM("dimacs\\2.6.33.3-2var.dimacs", FEATURES);	
  
  public static ObjectiveParser op = new ObjectiveParser("..\\trunk2\\dimacs\\62482B.csv");

  public static final boolean[] USED_BEFORE = op.getBools(-2); //offset the index since there are just 3 columns

  public static final int[] DEFECTS = op.getInts(-1);
  
  public static final double[] COST = op.getDoubles(0);
  
    
  public P3332(String solutionType) throws ClassNotFoundException {
	  this(solutionType, 62482, 5,new DimacsFM("..\\trunk2\\dimacs\\2.6.33.3-2var.dimacs", FEATURES));
  }
  
  public P3332(String solutionType, 
          Integer numberOfBits, 
		         Integer numberOfObjectives, DimacsFM dimacsFM) throws ClassNotFoundException {
	  numberOfVariables_  = 1;
	    numberOfObjectives_ = numberOfObjectives.intValue();
	    numberOfConstraints_= 0;
	    problemName_        = "P3332";
	    
	    this.dfm = dimacsFM;
	        
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
  
  public void evaluate(Solution solution) throws JMException {

      Binary variable = ((Binary)solution.getDecisionVariables()[0]) ;
      int num_features = 0;
      int num_violations;
      int num_used_before = 0;
      int num_defects = 0;
      double actual_cost = 0;
      
      num_violations = dfm.numViolations(variable);
      
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
      if (numberOfObjectives_ == 5){
          // First: The correctness objective, minimize violations to
          // maximize correctness
      	
          solution.setObjective(0, num_violations);

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
          solution.setObjective(0, num_violations);

          solution.setObjective(1, num_features - num_used_before); 

          solution.setObjective(2, num_defects); 

          solution.setObjective(3, actual_cost); 
      }

      else if (numberOfObjectives_ == 3){
          solution.setObjective(0, num_violations); 

          // Second: Maximize the total number of features
          // Here: we minimize the missing features
          solution.setObjective(1, FEATURES - num_features); 

          solution.setObjective(2, actual_cost); 
      }
      if (numberOfObjectives_ == 2){
          solution.setObjective(0, num_violations); 

          solution.setObjective(1, num_features); 
//          solution.setObjective(1, FEATURES - num_features); 
      }

} // evaluate   

}
