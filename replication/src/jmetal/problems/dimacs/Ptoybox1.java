package jmetal.problems.dimacs;

import jmetal.core.*;
import jmetal.encodings.solutionType.BinarySolutionType;
import jmetal.encodings.variable.Binary;
import static jmetal.problems.dimacs.P286.FEATURES;
import jmetal.util.JMException;
import jmetal.util.Configuration.*;
import main.DimacsFM;
import main.ObjectiveParser;
import main.ParsedFM;


import com.opencsv.CSVReader;
import java.io.FileReader;
import java.io.BufferedReader;
import java.util.List;
import java.io.IOException;
import java.util.BitSet;
import java.util.Map.Entry;
import java.util.AbstractMap.SimpleImmutableEntry;
import java.util.Arrays;
import java.util.ArrayList;
import java.util.Collections;
import java.net.URL;

public class Ptoybox1 extends Problem {

    public static final int FEATURES = 544;	
    public static DimacsFM dimacs;
    
    public static ObjectiveParser op =
	new ObjectiveParser(Ptoybox.class.getResource("/dimacs/toybox.csv").getPath());

    public static final double[] COST1 = op.getDoubles(-2);

    public static List<Entry<BitSet, Double>> INTERACT1 = loadInteract("/dimacs/interact0.csv");
    
    private static List<Entry<BitSet, Double>> loadInteract(String path) {
	URL interactFile =  Ptoybox1.class.getResource(path);
	if (interactFile == null)
	    return null;
	
	List<Entry<BitSet, Double>> interacts = new ArrayList<Entry<BitSet, Double>>();
	try (CSVReader reader =
	     new CSVReader(new BufferedReader(new FileReader(interactFile.getPath())),
			   ',', '"', 1);) {
	    List<String[]> lines = reader.readAll();
	    for (String[] line : lines) {
		int numOfFeatures = line.length-1;
		String[] interaction = Arrays.copyOfRange(line, 0, numOfFeatures);
		BitSet bs = new BitSet(numOfFeatures);
		for(int i = 0; i < numOfFeatures; i++) {
		    if (Integer.parseInt(interaction[i].trim()) != 0) {
			bs.set(i);
		    }
		}
		Entry e = new SimpleImmutableEntry(bs, Double.parseDouble(line[line.length-1]));
		interacts.add(e);
	    }
	    return Collections.unmodifiableList(interacts);
	} catch (Exception e) {
	    System.out.println("OpenCVS: " + e.getMessage());
	    return null;
	}
    }
  
    public Ptoybox1(String solutionType) throws ClassNotFoundException {
	this(solutionType, FEATURES, 3,
	     new DimacsFM(Ptoybox1.class.getResource("/dimacs/toybox.dimacs").getPath(),
			  FEATURES));
    }
  
    public Ptoybox1(String solutionType, 
		    Integer numberOfBits, 
		    Integer numberOfObjectives,
		    DimacsFM dm) throws ClassNotFoundException {
	numberOfVariables_  = 1;
	numberOfObjectives_ = numberOfObjectives.intValue();
	numberOfConstraints_= 0;
	problemName_        = "544";
      
	lowerLimit_ = new double[numberOfVariables_];
	upperLimit_ = new double[numberOfVariables_];        
	//for (int var = 0; var < numberOfBits.intValue(); var++){
	lowerLimit_[0] = 0.0;
	upperLimit_[0] = 1.0;
	// } //for
      
	this.dfm = dm;
      
	if (solutionType.compareTo("Binary") == 0)
	    solutionType_ = new BinarySolutionType(this) ;
	else {
	    System.out.println("Error: solution type " + solutionType + " invalid") ;
	    System.exit(-1) ;
	}
	length_    = new int[numberOfVariables_];
	length_[0] = numberOfBits.intValue();
     
    }

    public DimacsFM getDimacsFM() {
	return dfm;
    }
    
    public void evaluate(Solution solution) throws JMException {
	
	Binary variable = ((Binary)solution.getDecisionVariables()[0]) ;
	int num_features = 0;
	int num_violations;
	double actual_cost = 0;
	
	num_violations = dfm.numViolations(variable);
	
	// Find the total number of features in this individual
	for(int x=0; x<variable.getNumberOfBits(); x++)
	    num_features += (variable.bits_.get(x) ? 1 : 0);
       
	// First: The correctness objective, minimize violations to
	// maximize correctness
      	
	solution.setObjective(0, num_violations);
	
	// Second: Maximize the total number of features
	// Here: we minimize the missing features
	solution.setObjective(1, FEATURES - num_features); 

	// Third: Minimize the total cost
	solution.setObjective(2, computeCosts(variable, COST1, INTERACT1));
    } // evaluate   
    
    public double computeCosts(Binary variable, double[] COST,
			       List<Entry<BitSet, Double>> INTERACT) {
	double cost = 0;
	for(int x=0; x<variable.getNumberOfBits(); x++)
	    cost += (variable.bits_.get(x) ? COST[x] : 0);

	if (INTERACT != null) {
	    for (Entry<BitSet, Double> interactEntry : INTERACT) {
		/* Is interaction a subset of the solution:
		   solution & interaction = interaction? */
		BitSet interaction = interactEntry.getKey();
		BitSet sol = (BitSet)variable.bits_.clone();
		
		/* solution & interaction */
		sol.and(interaction);
		
		/* solution & interaction = interaction */
		if (interaction.equals(sol)) {
		    cost += interactEntry.getValue();
		}
	    }
	}
	return cost;
    }

}
