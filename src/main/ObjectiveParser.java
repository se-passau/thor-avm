package main;

import java.io.FileReader;
import java.util.List;

import com.opencsv.CSVReader;

public class ObjectiveParser {
	
	List<String[]> rows;
	
	public ObjectiveParser() {
		rows = null;
	}
	
	public ObjectiveParser(String in) {
		parseCSV(in);
	}
	
	public void parseCSV(String in) {
		try {
			CSVReader reader = new CSVReader(new FileReader(in), ',');
			rows = reader.readAll();
//			for(String[] row: csvEntries) {
//				System.out.println("cost: " + row[row.length-1] + " defects: " + row[row.length-2] + " used before: " + row[row.length-3]);
//			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public double[] getDoubles(int index) {
		double[] objectives = new double[rows.size()];
		for(int i=0;i<rows.size();i++) {
			if(rows.get(i).length>index+2)
				objectives[i] = Double.parseDouble(rows.get(i)[index+2]);
		}
		return objectives;
	}
	
	public int[] getInts(int index) {
		int[] objectives = new int[rows.size()];
		for(int i=0;i<rows.size();i++) {
			if(rows.get(i).length>index+2)
				objectives[i] = Integer.parseInt(rows.get(i)[index+2]);
		}
		return objectives;
	}
	
	public boolean[] getBools(int index) {
		boolean[] objectives = new boolean[rows.size()];
		for(int i=0;i<rows.size();i++) {
			if(rows.get(i).length>index+2)
				objectives[i] = Boolean.parseBoolean(rows.get(i)[index+2]);
		}
		return objectives;
	}
	
	public static void main(String[] args) {
		ObjectiveParser op = new ObjectiveParser();
		op.parseCSV("qualities43.csv");
	}

}
