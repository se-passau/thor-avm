package main;

public class NodeData {
	
	public int index;
	public int selected;
	public int counter;
	
	public NodeData(int i, int select, int count) {
		index=i;
		selected=select;
		counter=count;
	}

	public int getIndex() {
		return index;
	}

	public void setIndex(int index) {
		this.index = index;
	}

	public int getSelected() {
		return selected;
	}

	public void setSelected(int selected) {
		this.selected = selected;
	}

	public int getCounter() {
		return counter;
	}

	public void setCounter(int counter) {
		this.counter = counter;
	}
	
	public String toString() {
		return "Index: " + index + "\nSelected: " + selected + "\nCounter: " + counter;
	}

}
