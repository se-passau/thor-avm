package main;

import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.io.RandomAccessFile;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Random;
import java.util.Scanner;

import jmetal.encodings.variable.Binary;
import jmetal.util.PseudoRandom;

public class DimacsFM {
	public int totalRules;
	public String[] features;
	public ArrayList<Integer>[] rules;
	public final int numFeatures;
	
	public int[] fixedIndexes;
	public boolean[] skipBits;
	public boolean[] skipRules;
	
	public int numSkipBits;
	public int numSkipRules;
	
	public DimacsFM() {
		numFeatures = 0;
	}
	
	public DimacsFM(String fileName, int numFeatures) {
		features = new String[numFeatures];
		this.numFeatures=numFeatures;
		Scanner in = null;
		try {
			in = new Scanner(new FileReader(fileName));
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
		int feature = 0;
		while(feature < numFeatures) {
			in.next();
			in.next();
			features[feature++] = in.next();
		}
		in.nextLine();
		numSkipBits = 0;
		numSkipRules = 0;
		totalRules = Integer.parseInt(in.nextLine().split(" ")[3]);
		rules = (ArrayList<Integer>[]) new ArrayList[totalRules];
		fixedIndexes = new int[this.numFeatures];
		skipBits = new boolean[this.numFeatures];
		skipRules = new boolean[totalRules];
		for(int i=0;i<this.numFeatures;i++) {
			fixedIndexes[i] = -999;
			skipBits[i] = false;
			skipRules[i] = false;
		}
		int rule = 0;
		int ind;
		int disCount = 0;
		String line;
		int single;
		int singleCount = 0;
		while(rule < totalRules) {
			ind = in.nextInt();
			single = ind;
			ArrayList<Integer> temp = new ArrayList<Integer>();
			disCount = 0;
			while(ind != 0) {
				temp.add(ind);
				ind = in.nextInt();
				disCount++;
			}
			if(disCount==1) { //fixed rules
				singleCount++;
				if(single > 0) //positive
					fixedIndexes[single-1] = 1; //required to be selected
				else //negative
					fixedIndexes[(-1*single)-1] = 0; //required to be not selected
				skipBits[Math.abs(single)-1]=true;
				skipRules[rule] = true;
				numSkipBits++;
				numSkipRules++;
			}
			rules[rule] = temp;
			rule++;
		}
		eliminateRules();   
		in.close();
//		numSkipBits = 0;
//		numSkipRules = 0;
//		for(int i=0;i<this.numFeatures;i++) {
//			if(skipBits[i])
//				numSkipBits++;
//			if(skipRules[i])
//				numSkipRules++;
//		}
//		System.out.println(fileName);
//		System.out.println("Skipped Features (Arity 1): " + singleCount);
//		System.out.println("Skipped Features: " + numSkipBits);
//		System.out.println("Skipped Rules: " + numSkipRules);
//		System.out.println("Total Features: " + numFeatures);
//		System.out.println("Total Rules: " + totalRules);
//		System.out.println();
		
//		for(int i=0;i<this.numFeatures;i++) {
//			System.out.println("Index " + i + ": " + fixedIndexes[i] + " " + skipBits[i]);
//		}
//		System.out.println(singleCount);
	}
	
	public DimacsFM(String name) {
		this("dimacs\\"+getFilename(name)+".dimacs", getNumFeatures(name));
	}
	
	private static int getNumFeatures(String name) {
		int f = 0;
		String n="";
		//switch(name) {
		if (name == "P322") {f=60072; n="2.6.32-2var";} //break;
                else if (name == "P3332") {f=62482; n="2.6.33.3-2var";}// break;
		else if (name == "PaxTLS") {f=684; n="axTLS";}// break;
		else if (name == "Pbuildroot") {f=14910; n="buildroot";}// break;
		else if (name == "Pbusybox") {f=6796; n="busybox-1.18.0";}// break;
		else if (name == "Pcoreboot") {f=12268; n="coreboot";}// break;
		else if (name == "Pecos") {f=1244; n="ecos-icse11";}// break;
		else if (name == "Pembtoolkit") {f=23516; n="embtoolkit";}// break;
		else if (name == "Pfiasco") {f=1638; n="fiasco";}// break;
		else if (name == "Pfreebsd") {f=1396; n="freebsd-icse11";}// break;
		else if (name == "Pfreetz") {f=31012; n="freetz";}// break;
		else if (name == "Ptoybox") {f=544; n="toybox";}// break;
		else if (name == "PuClinux") {f=1850; n="uClinux";}// break;
		else if (name == "PuClinuxconfig") {f=11254; n="uClinux-config";}// break;
		//}
		return f;
	}
	
	private static String getFilename(String name) { //yeah, I know
		int f = 0;
		String n="";
		//switch(name) {
		if (name == "P322") {f=60072; n="2.6.32-2var";}// break;
                else if (name == "P3332") {f=62482; n="2.6.33.3-2var";}// break;
		else if (name == "PaxTLS") {f=684; n="axTLS";}// break;
		else if (name == "Pbuildroot") {f=14910; n="buildroot";}// break;
		else if (name == "Pbusybox") {f=6796; n="busybox-1.18.0";}// break;
		else if (name == "Pcoreboot") {f=12268; n="coreboot";}// break;
		else if (name == "Pecos") {f=1244; n="ecos-icse11";} //break;
		else if (name == "Pembtoolkit") {f=23516; n="embtoolkit";}// break;
		else if (name == "Pfiasco") {f=1638; n="fiasco";}// break;
		else if (name == "Pfreebsd") {f=1396; n="freebsd-icse11";}// break;
		else if (name == "Pfreetz") {f=31012; n="freetz";}// break;
		else if (name == "Ptoybox") {f=544; n="toybox";}// break;
		else if (name == "PuClinux") {f=1850; n="uClinux";}// break;
		else if (name == "PuClinuxconfig") {f=11254; n="uClinux-config";}// break;
		//}
		return n;
	}
	
	private void eliminateRules() {
		int rule = 0;
		for(ArrayList<Integer> line: rules) {
			for(int index: line) {
				if(index > 0) { //positive
					if(fixedIndexes[index-1]==1) { //entire line is true
						skipRules[rule] = true;
						numSkipRules++;
					}
					else if(fixedIndexes[index-1]==0 && line.size()==2) { //need to fix the other bit to be 1 
						int otherIndex = line.get(flip(line.indexOf(index))); //flip to get the other index in the line
						//System.out.println("Other Index: " + otherIndex);
						if(otherIndex > 0) //positive so fix to 1
							fixedIndexes[otherIndex-1] = 1; 
						else //negative so fix to 0
							fixedIndexes[(-1*otherIndex)-1] = 0;
						skipBits[index-1] = true;
						skipRules[rule] = true;
						numSkipBits++;
						numSkipRules++;
					}
				}
				else { //negative
					int ind = Math.abs(index)-1;
					if(fixedIndexes[ind]==0) { //entire line is true
						skipRules[rule] = true;
						numSkipRules++;
					}
					else if(fixedIndexes[ind]==1 && line.size()==2) { //need to fix the other bit to be 1 
						int otherIndex = line.get(flip(line.indexOf(index))); //flip to get the other index in the line
						//System.out.println("Other Index: " + otherIndex);
						if(otherIndex > 0) //positive so fix to 1
							fixedIndexes[otherIndex-1] = 1; 
						else //negative so fix to 0
							fixedIndexes[(-1*otherIndex)-1] = 0;
						skipBits[ind] = true;
						skipRules[rule] = true;
						numSkipBits++;
						numSkipRules++;
					}
				}
			}
			rule++;
		}
		
	}
	
	private int flip(int num) {
		if(num==1) return 0;
		return 1;
	}

	public int numViolations(Binary variable) {
		int violations = 0;
		int rule = 0;
		for(ArrayList<Integer> line: rules) {
			if(!skipRules[rule])
				violations += lineViolations(variable, line);
			rule++;
		}
		return violations;
	}
	
	public int lineViolations(Binary variable, ArrayList<Integer> line) {
		for(int index: line) {
			if(index > 0) { //positive
				if(variable.bits_.get(index-1)) //dimacs file indexes start from 1
					return 0;  //one is true, so the whole line is true
			}
			else { //negative
				if(variable.bits_.get((-1*index)-1))
					return 0;
			}
		}
		return 1;  //couldn't prove line true so there's 1 violation;
	}
	
	public void generateAttributes(String name, int numFeat) {
		try {
			RandomAccessFile ListFile = 
			        new RandomAccessFile("trunk2\\dimacs\\"+name+".list", "rw");

			RandomAccessFile QualFile = 
			        new RandomAccessFile("trunk2\\dimacs\\"+name+".csv", "rw");
			
			Random randomGen = new Random();
			// Generate random values for:
            // 1) used_before {true or false}, uniform distribution
            // 2) defects, integer between 0 and 10, average 5, Gaussian distribution, 
            //      EQUALS 0 if used_before = false
            // 3) cost, double between 5 and 15, average 10, Gaussian distribution
			
			for(int i=0;i<numFeat;i++) {
        
	            String usedBefore = "false";
	            double randomNum = randomGen.nextDouble();
	            if (randomNum < 0.5){
	                usedBefore = "true";
	            }
	            ListFile.writeBytes(" " +usedBefore+ " ");
	            QualFile.writeBytes(usedBefore + ",");
	        
	            double ddefects = (randomGen.nextGaussian()*2) + 5;
	            long defects = Math.round(ddefects);
	            if ((defects < 0) || (usedBefore.equals("false"))){
	                defects = 0;
	            }
	            else if (defects > 10){
	                defects = 10;
	            }
	            String sdefects = Long.toString(defects);
	            ListFile.writeBytes(sdefects + " ");
	            QualFile.writeBytes(sdefects + ",");
	        
	            double cost = (randomGen.nextGaussian()*4) + 10;
	            if (cost < 5){
	                cost = 5;
	            }
	            else if (cost > 15){
	                cost = 15;
	            }
	            String scost = Double.toString(cost);
	            ListFile.writeBytes(scost + " ");
	            QualFile.writeBytes(scost + "\r\n");
			}
			ListFile.writeBytes(")");
			ListFile.close();
			QualFile.close();
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public boolean skipMutation(int j) {
		return skipBits[j];
	}
	
	public String getInitialString() {
		String bits = "";
		for(int i=0;i<numFeatures;i++) {
			int fi = fixedIndexes[i];
			if(fi>-1)
				bits+=fi;
			else {
				if(PseudoRandom.randDouble() < 0.5)
					bits+=1;
				else
					bits+=0;
			}
		}
		return bits;
	}
	
	public static void main(String[] args) {
		new DimacsFM("dimacs\\2.6.32-2var.dimacs", 60072);
		new DimacsFM("dimacs\\2.6.33.3-2var.dimacs", 62482);
		new DimacsFM("dimacs\\axTLS.dimacs", 684);
		new DimacsFM("dimacs\\buildroot.dimacs", 14910);
		new DimacsFM("dimacs\\busybox-1.18.0.dimacs", 6796);
		new DimacsFM("dimacs\\coreboot.dimacs", 12268);
		new DimacsFM("dimacs\\ecos-icse11.dimacs", 1244);
		new DimacsFM("dimacs\\embtoolkit.dimacs", 23516);
		new DimacsFM("dimacs\\fiasco.dimacs", 1638);
		new DimacsFM("dimacs\\freebsd-icse11.dimacs", 1396);
		new DimacsFM("dimacs\\freetz.dimacs", 31012);
		new DimacsFM("dimacs\\toybox.dimacs", 544);
		new DimacsFM("dimacs\\uClinux.dimacs", 1850);
		new DimacsFM("dimacs\\uClinux-config.dimacs", 11254);


	}

	

}
