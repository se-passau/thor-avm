package jmetal.problems.dimacs;

import jmetal.encodings.variable.Binary;

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

public class Ptoybox2 extends Ptoybox {   

    public static List<Entry<BitSet, Double>> interactions = null;
    
    static {
	String interactFile = Ptoybox.class.getResource("/dimacs/ivmI16.csv").getPath();
	List<Entry<BitSet, Double>> interacts = new ArrayList<Entry<BitSet, Double>>();
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
		Entry e = new SimpleImmutableEntry(bs, Double.parseDouble(line[line.length-1]));
		interacts.add(e);
	    }
	    interactions = Collections.unmodifiableList(interacts);
	} catch (Exception e) {
	    System.out.println("OpenCVS: " + e.getMessage());
	}
    }
    
    public Ptoybox2(String solutionType) throws ClassNotFoundException {
	super(solutionType);
    }

    public double computeCosts(Binary variable) {
	double cost = super.computeCosts(variable);

	for (Entry<BitSet, Double> interactEntry : this.interactions) {
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
