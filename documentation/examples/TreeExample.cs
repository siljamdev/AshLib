using System;
using AshLib.Trees;

class TreeExample{
	static Random random = new Random();
	
	public static void Main(){
		Console.WriteLine(GenerateRandomTree(6));
	}
	
	static TreeNode<int> GenerateRandomTree(int maxDepth, int currentDepth = 0){
		if (currentDepth >= maxDepth){
			return new TreeNode<int>(random.Next(1, 100)); // Leaf node with a random value
		}
		
		int nodeValue = random.Next(1, 100); // Random value for the current node
		TreeNode<int> node = new TreeNode<int>(nodeValue);
		
		// Randomly decide how many children this node should have (0-3 children)
		int numberOfChildren = random.Next(0, 7);
		
		for (int i = 0; i < numberOfChildren; i++){
			TreeNode<int> child = GenerateRandomTree(maxDepth, currentDepth + 1);
			node.AddChild(child);
		}
		
		return node;
	}
}