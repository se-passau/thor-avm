package main;
import java.util.ArrayList;
import java.util.BitSet;
import java.util.Collection;
import java.util.Collections;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.List;

import jmetal.encodings.variable.Binary;
import jmetal.util.PseudoRandom;

import constraints.CNFClause;
import constraints.PropositionalFormula;

import fm.FeatureGroup;
import fm.FeatureModel;
import fm.FeatureModelStatistics;
import fm.FeatureTreeNode;
import fm.RootNode;
import fm.SolitaireFeature;
import fm.XMLFeatureModel;


public class ParsedFM {
	public HashMap<Integer, int[]> requires_pairs;
	public ArrayList<int[]> requires_array;
	public int require_rules;
	public int total_rules;
	public HashMap<Integer, FeatureGroup> feature_groups;
	public ArrayList<FeatureGroup> group_array;
	public FeatureModel featureTree;
	private int index;
	public HashMap<Integer, Integer> parentIndex;
	boolean flipped;
//	int cntr;
//	BitSet[] subTrees;
	
	public ParsedFM() {
		requires_pairs = new HashMap<Integer, int[]>();
		require_rules=0;
		total_rules=0;
		feature_groups = new HashMap<Integer, FeatureGroup>();
		parentIndex = new HashMap<Integer, Integer>();
//		flipped = false;
//		cntr = -999;
	}
	
	public ParsedFM(String fileName) {
		requires_pairs = new HashMap<Integer, int[]>();
		requires_array = new ArrayList<int[]>();
		require_rules=0;
		total_rules=0;
		feature_groups = new HashMap<Integer, FeatureGroup>();
		group_array = new ArrayList<FeatureGroup>();
		parentIndex = new HashMap<Integer, Integer>();
		parse(fileName);
	}

	public int getFeatureCount() {
		return featureTree.countFeatures();
	}
	
	public void setRequiresArray(int[][] rp) {
		requires_array = new ArrayList<int[]>();
		addCTC(rp);
	}
	
	public void parse(String fileName) {
		try {
			FeatureModel featureModel = new XMLFeatureModel(fileName, XMLFeatureModel.USE_VARIABLE_NAME_AS_ID);
			
			// Load the XML file and creates the feature model
			featureModel.loadModel();
			
			featureTree = featureModel;
		
			index=0;
			traverseDFS(featureModel.getRoot()); //get features constraints
			require_rules = requires_array.size();// + featureModel.getConstraints().size();
			total_rules = require_rules + 1 + featureModel.getConstraints().size() + group_array.size();
			//System.out.println(require_rules + " " + featureModel.getConstraints().size() + " " + group_array.size() + " " + total_rules);
			//subTrees = new BitSet[getFeatureCount()];
//			for( PropositionalFormula formula : featureModel.getConstraints() ) { //get extra constraints
//				//add formula
//			}
			
		} catch (Exception e) {
			System.out.println(e.getMessage());
		}
	}
	
	private void traverseDFS(FeatureTreeNode node) {
		node.attachData(new NodeData(index, 0, 0)); //give each node an index
		if(index>0)
			parentIndex.put(index, ((NodeData)getParentFeature(node).getAttachedData()).getIndex()); //quick lookup of a node's parent's index
		// Solitaire Feature
		
		if (node instanceof SolitaireFeature) {
			// Mandatory
			if (!((SolitaireFeature)node).isOptional()) { 
				int[] pair = new int[2];
				FeatureTreeNode parent = (FeatureTreeNode) node.getParent();
				pair[0] = ((NodeData) parent.getAttachedData()).getIndex(); //the feature that requires
				pair[1] = index; //the required feature
				addPair(pair);
				requires_array.add(pair);
			}
			if(index>0) {
				int[] parentChild = {index, ((NodeData)getParentFeature(node).getAttachedData()).getIndex()};
				//addPair(parentChild);
				requires_array.add(parentChild);
			}
		}
		// Feature Group
		else if (node instanceof FeatureGroup) {
			group_array.add((FeatureGroup) node);
			addGroup(index, (FeatureGroup)node);
			index--;
		}
		else { //Grouped Feature
			if(index>0) {
				int[] parentChild = {index, ((NodeData)getParentFeature(node).getAttachedData()).getIndex()};
				addPair(parentChild);
				requires_array.add(parentChild);
			}
			FeatureGroup p = (FeatureGroup) node.getParent();
			if(p!=null) {
				addGroup(index, p);
			}
		}
		index++;
		for( int i = 0 ; i < node.getChildCount() ; i++ ) {
			traverseDFS((FeatureTreeNode )node.getChildAt(i));
		}
	}
	
	public String getInitialString() {
		index=0;
		return generateString(featureTree.getRoot(), 0);
	}
	
	public String generateString(FeatureTreeNode node, int tab) {
		String variable="";
		NodeData nd = new NodeData(index, 1, 0);
//		for( int j = 0 ; j < tab ; j++ ) {
//			System.out.print("\t");
//		}
		FeatureTreeNode fParent = getParentFeature(node);
		FeatureTreeNode parent = (FeatureTreeNode) node.getParent();
		// Root Feature
		if ( node instanceof RootNode ) {
			node.attachData(nd); //root always required
			variable+=1;
		}
		// Feature Group
		else if (node instanceof FeatureGroup) {
			index--;
			FeatureGroup fg = (FeatureGroup) node;
			//System.out.print("Feature Group[" + fg.getMin() + "," + fg.getMax() + "]");
			if(fg.getMax()==-1)
				nd.setCounter(-1);
			else
				nd.setCounter(randInRange(fg.getMin(), fg.getMax()));
			node.attachData(nd); //attach counter
		}
		else if(((NodeData) fParent.getAttachedData()).getSelected()==0) {  //parent is dead
			nd.setSelected(0);
			node.attachData(nd); //so the children are too
			variable+=0;
		}
		// Solitaire Feature
		else if(node instanceof SolitaireFeature) {
			// Mandatory
			if (!((SolitaireFeature)node).isOptional()) {
				//System.out.print("Mandatory");
				node.attachData(nd);
				variable+=1;
			}
			//Optional
			else {
				//System.out.print("Optional");
				nd.setSelected(zeroOrOne());
				node.attachData(nd);
				variable+=nd.getSelected();
			}
		}
		else { //Grouped Feature
			//System.out.print("Grouped");
			int data = ((NodeData) parent.getAttachedData()).getCounter();
			int min = ((FeatureGroup) parent).getMin();
			if(data==-1) { //1 to many
				node.attachData(nd); //allow first feature so at least one is selected
				variable+=1;
				NodeData pData = (NodeData) parent.getAttachedData();
				pData.setCounter(randInRange(min, node.getSiblingCount()-1));
				parent.attachData(pData);
			}
			else if(data>=min) { //more features needed
				node.attachData(nd);
				variable+=1;
				NodeData pData = (NodeData)parent.getAttachedData();
				pData.setCounter(pData.getCounter()-1);
				parent.attachData(pData);
			}
			else { //no more features for this group
				nd.setSelected(0);
				node.attachData(nd);
				variable+=0;
			}
		}
		index++;
		int d;
		if(node instanceof FeatureGroup)
			d=nd.getCounter();
		else
			d=nd.getSelected();
		//System.out.print( "(ID=" + node.getID() + ", NAME=" + node.getName() + ") " + d + "\r\n");
		for( int i = 0 ; i < node.getChildCount() ; i++ ) {
			variable+=generateString((FeatureTreeNode )node.getChildAt(i), tab+1);
		}
		return variable;
	}
	
	private int zeroOrOne() {
		return (int) Math.round(Math.random());
	}
	
	private int randInRange(int min, int max) {
		return min + (int)(Math.random()*(max-min+1));
	}
	
	private Object getParentData(FeatureTreeNode node) {
		FeatureTreeNode parent = (FeatureTreeNode) node.getParent();
		return parent.getAttachedData();
	}

	public int getRequireRules() {
		return require_rules;
	}

	public void setRequireRules(int require_rules) {
		this.require_rules = require_rules;
	}

	public int getTotalRules() {
		return total_rules;
	}

	public void setTotalRules(int total_rules) {
		this.total_rules = total_rules;
	}

	public HashMap<Integer, int[]> getRequiresPairs() {
		return requires_pairs;
	}
	
	public void addPair(int[] pair) {
		requires_pairs.put(pair[1], pair);
		
	}
	
	public void addGroup(int index, FeatureGroup node) {
		feature_groups.put(index, node);
		total_rules++;
	}
	
	public String toString() {
		int count=0;
		String s="";
		s+="[";
//		for(int[] pair: requires_pairs) {
//			s+="("+pair[0]+", " + pair[1] + "), ";
//			if(++count%5==0)
//				s+="\n";
//		}
		s+="]\n";
		s+="Require Rules: " + require_rules + "\n";
		s+="Feature Groups: ";
		count=0;
		for(FeatureGroup group: feature_groups.values()) {
			s+="["+ group.getMin() +", " + group.getMax() + "], ";
			if(++count%5==0)
				s+="\n";
		}
		return s;
	}
	
	private FeatureTreeNode nodeAt(int index, FeatureTreeNode node) throws Exception {
		FeatureTreeNode foundNode = node;
		if(((NodeData)node.getAttachedData()).getIndex()==index)
			return node;
		for( int i = 0 ; i < node.getChildCount() ; i++ ) {
			foundNode = nodeAt(index, (FeatureTreeNode )node.getChildAt(i));
		}
		return foundNode;
	}

	public boolean requiresViolation(Binary variable, int j) { //PROBLEM IS IN HERE
		int[] pair;
		int pIndex = 0;
		if(parentIndex.containsKey(j)) {
			pIndex = parentIndex.get(j);
			if(!variable.bits_.get(pIndex) && !variable.bits_.get(j)) { //current bit will be selected but parent is not
				return true;
			}
		}
		if(requires_pairs.containsKey(j)) {
			pair = requires_pairs.get(j);
			if (variable.bits_.get(pair[0])) { //feature that requires the other is present
				//feature that is required is j and is present (will become not present when flipped)
				if (variable.bits_.get(pair[1])) 
					return true;
			}
		}
		FeatureGroup fg;
		if(feature_groups.containsKey(j)) {
			fg = feature_groups.get(j); //0 is included, 1 is index
			int max = fg.getMax();
			int min = fg.getMin();
			int count=0;
			for(int i=0;i<fg.getChildCount();i++) {
				NodeData temp = (NodeData) ((FeatureTreeNode) fg.getChildAt(i)).getAttachedData();
				int index = temp.getIndex();
				if((variable.bits_.get(index) && index!=j) || (index==j && !variable.bits_.get(index)))
					count++;
//				if(j==3)
//					System.out.println(variable.bits_.get(index) + " " + index + " " + count);
			}
			if(count<min)  //less than minimum cardinality
				return true;
			else if(count>max && max!=-1) //not * and maxCardinality violated
				return true;
//			if(j==3)
//				System.out.println(variable.bits_.get(3) + " " + variable.bits_.get(4));
		}
		return false;
	}
	
	private FeatureTreeNode getParentFeature(FeatureTreeNode node) {
		FeatureTreeNode parent = (FeatureTreeNode) node.getParent();
		if(parent instanceof FeatureGroup)
			parent = (FeatureTreeNode) parent.getParent();
		return parent;	
	}
	
	public int requiresViolations(Binary variable) {
		int requires_viol = 0;
		if(!variable.bits_.get(0)) requires_viol++; //root
		//System.out.println(requires_array.size());
		for (int[] pair: requires_array) {
                    if (variable.bits_.get(pair[0]))
                        if (!variable.bits_.get(pair[1]))
                            requires_viol++;
                }
		return requires_viol;
	}
	
	public int groupViolations(Binary variable) {
		int group_viol=0;
		for(FeatureGroup fg: group_array) {
			NodeData fgData = (NodeData) ((FeatureTreeNode) fg.getParent()).getAttachedData();
			if(variable.bits_.get(fgData.getIndex())) {
				int max = fg.getMax();
				int min = fg.getMin();
				int count=0;
				for(int i1=0;i1<fg.getChildCount();i1++) {
					NodeData temp = (NodeData) ((FeatureTreeNode) fg.getChildAt(i1)).getAttachedData();
					int index = temp.getIndex();
					if(variable.bits_.get(index))
						count++;
				}
				if(count<min) group_viol++;
				else if(count>max && max!=-1) group_viol++;
				//System.out.println("mine: " + count + " " + min + " " + max);
			}
		}
		//System.out.println(variable);
		return group_viol;
	}
	
	public boolean correctString(Binary variable) {
		if(!variable.bits_.get(0)) {
			System.out.println("no root");
			return false;
		}
		for(int i=0;i<variable.getNumberOfBits();i++) {
			int[] pair;
			if(parentIndex.containsKey(i)) {
				int pIndex = parentIndex.get(i);
				if(!variable.bits_.get(pIndex)) {
					if(variable.bits_.get(i)) {
						System.out.println(i + ":where's my parent " + pIndex);
						return false;
					}
					else 
						continue; //no parent and thus no child. all is right
				}
			}
			if(requires_pairs.containsKey(i)) {
				pair = requires_pairs.get(i);
				if (variable.bits_.get(pair[0])) { //feature that requires the other is present
					//feature that is required is j and is not present
					if (!variable.bits_.get(pair[1])) {
						System.out.println("requires pair violation" + i);
						System.out.println(pair[0] + " " + pair[1]);
						return false;
					}
				}
			}
			FeatureGroup fg;
			if(feature_groups.containsKey(i)) {
				fg = feature_groups.get(i); //0 is included, 1 is index
				int max = fg.getMax();
				int min = fg.getMin();
				int count=0;
				for(int i1=0;i1<fg.getChildCount();i1++) {
					NodeData temp = (NodeData) ((FeatureTreeNode) fg.getChildAt(i1)).getAttachedData();
					int index = temp.getIndex();
					//System.out.println(index + " " + variable.bits_.get(index));
					if(variable.bits_.get(index))
						count++;
				}
				if(count<min) { //less than minimum cardinality 
					System.out.println("Less than minimum cardinality " + fg.getChildCount() + " " + i + " [" + fg.getMin() + ", " + fg.getMax() + "]");
					return false;
				}
				else if(count>max && max!=-1) { //not * and maxCardinality violated
					System.out.println("not * and maxCardinality violated " + i);
					return false;
				}
			}
		}
		return true;
	}
	
	public void reselect(FeatureTreeNode node, Binary variable, int j) {
//		if(subTrees[j] != null) {
//			BitSet subTree = subTrees[j];
//			for(int i=j;i<getToIndex(node);i++) {
//				variable.bits_.set(i, subTree.get(i));
//			}
//		}
		//else { //selecting for the first time
			//System.out.println("haven't been cached " + j);
			for( int i = 0 ; i < node.getChildCount() ; i++ ) {
				reselector((FeatureTreeNode)node.getChildAt(i), variable);
			}
		//}
	}
	
	private void reselector(FeatureTreeNode node, Binary variable) {
		NodeData nd = (NodeData) node.getAttachedData();
		FeatureTreeNode fParent = getParentFeature(node);
		FeatureTreeNode parent = (FeatureTreeNode) node.getParent();
		int ind = nd.getIndex();
		ArrayList<FeatureTreeNode> children = Collections.list(node.children());
		// Feature Group
		if (node instanceof FeatureGroup) {
			FeatureGroup fg = (FeatureGroup) node;
			int max = fg.getMax();
			if(max==-1)
				max = fg.getChildCount();
			children = randNodes(fg.children(), fg.getMin(), max, fg.getChildCount());
		}
		else if(!isSelected(fParent, variable)) {  //parent is dead
			nd.setSelected(0);
			node.attachData(nd); //so the children are too
			variable.bits_.set(ind, false);
		}
		// Solitaire Feature
		else if(node instanceof SolitaireFeature) {
			// Mandatory
			if (!((SolitaireFeature)node).isOptional()) {
				//System.out.print("Mandatory");
				nd.setSelected(1);
				node.attachData(nd);
				variable.bits_.set(ind, true);
			}
			//Optional
			else {
				//System.out.print("Optional");
				nd.setSelected(0);
				node.attachData(nd);
				variable.bits_.set(ind, false);
			}
		}
		else { //Grouped Feature
			variable.bits_.set(ind, true);
		}
		//System.out.print( "(ID=" + node.getID() + ", NAME=" + node.getName() + ") " + d + "\r\n");
		for(FeatureTreeNode child: children) {
			//System.out.println(child);
			reselector(child, variable);
		}
	}
	
	private ArrayList<FeatureTreeNode> randNodes(Enumeration nodes, int min, int max, int numChildren) {
		ArrayList<FeatureTreeNode> children = Collections.list(nodes);
		Collections.shuffle(children);
		ArrayList<FeatureTreeNode> chosen = new ArrayList<FeatureTreeNode>();
		for(int i=0;i<randInRange(min, max);i++) {
			chosen.add(children.get(i));
		}
		return chosen;
	}
	
	private void deselect(FeatureTreeNode node, Binary variable) {
		NodeData nd = (NodeData) node.getAttachedData();
		variable.bits_.set(nd.getIndex(), false);
		for( int i = 0 ; i < node.getChildCount() ; i++ ) {
			deselect((FeatureTreeNode)node.getChildAt(i), variable);
		}
	}

	public boolean traverseToMutate(Binary variable, double probability) {
		flipped = false;
		treeMutation(featureTree.getRoot(), variable, probability);
		return flipped;
	}
	
	public void treeMutation(FeatureTreeNode node, Binary variable, double probability) {
		NodeData nd = (NodeData) node.getAttachedData();
		int j = nd.getIndex();
		if(!(node instanceof FeatureGroup)) {
			if(PseudoRandom.randDouble() < probability && !(j==0 && variable.bits_.get(0)) && !requiresViolation(variable, j)) {
				flipped = true;
				if(variable.bits_.get(j)) { //about to deselect a feature
					//subTrees[j] = variable.bits_.get(j, getToIndex(node)); //cache tree
					deselect(node, variable);
				}
				else {
					variable.bits_.flip(j);
					nd.setSelected(1);
					node.attachData(nd);
					reselect(node, variable, j);
				}
				//variable.bits_.flip(j);
			}
			if(variable.bits_.get(j)) {
				for( int i = 0 ; i < node.getChildCount() ; i++ ) {
					treeMutation((FeatureTreeNode)node.getChildAt(i), variable, probability);
				}
			}
		}
	}
	
	public boolean isSelected(FeatureTreeNode node, Binary variable) {
		NodeData nd = (NodeData) node.getAttachedData();
		return variable.bits_.get(nd.getIndex());
	}
	
	public int getToIndex(FeatureTreeNode node) {
		NodeData nd = (NodeData) node.getAttachedData();
		return nd.getIndex() + node.getChildCount() + 1;
	}
	
	public void printPairs() {
		int i=1;
		for (int[] pair: requires_array) {
            System.out.print("[" + pair[0] + ", " + pair[1] + "]  ");
            if(i++%5==0) System.out.println();
        }
	}
	
	public void printGroups() {
		int i=1;
		for (FeatureGroup fg: group_array) {
            System.out.print(fg + " ");
            if(i++%5==0) System.out.println();
        }
	}
	
	public void addCTC(int[][] requiresPairs) {
		for(int[] pair: requiresPairs)
			requires_array.add(pair);
	}
	
	public static void main(String[] args) throws Exception {
		for(int i=0;i<50;i++) System.out.println(PseudoRandom.randInt());
		ParsedFM pfm = new ParsedFM("..\\Research2\\trunk2\\jmetal\\problems\\FMr\\FM-52.xml");
		String solution = "1111111111111011110100011101110111011111101111101010";
		Binary b=new Binary(solution);
		System.out.println(pfm.correctString(b));
		
	}

	

	
}
