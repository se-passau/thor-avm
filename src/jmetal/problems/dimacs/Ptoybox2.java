package jmetal.problems.dimacs;

import jmetal.encodings.variable.Binary;

/* TODO: relocate */
import com.opencsv.CSVReader;
import java.io.FileReader;
import java.io.BufferedReader;
import java.util.List;
import java.io.IOException;
import java.util.BitSet;
import java.util.Map;
import java.util.HashMap;
import java.util.Arrays;

public class Ptoybox2 extends Ptoybox {   

    public final Map <BitSet, Double> interactions = new HashMap<BitSet, Double>();
    
    public Ptoybox2(String solutionType) throws ClassNotFoundException, IOException {
	super(solutionType);

	/* TODO relocate later, make static */
	String interactFile = Ptoybox.class.getResource("/dimacs/ivmI16.csv").getPath();
	try (CSVReader reader =
	     new CSVReader(new BufferedReader(new FileReader(interactFile)), ',', '"', 1);
	     ) {
	  
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
		this.interactions.put(bs, Double.parseDouble(line[line.length-1]));
	    }
	} catch (Exception e) {
	    System.out.println("OpenCVS: " + e.getMessage());
	    throw e;
	}
    }

    public double computeCosts(Binary variable) {
	double cost = super.computeCosts(variable);
	
	/* TODO: make it a simple list */
	
	for (Map.Entry<BitSet, Double> interactEntry : this.interactions.entrySet()) {
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
	
	return cost;
    }

}
