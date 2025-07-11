using System;
using System.Text;
using AshLib.Formatting;

namespace AshLib.Trees;

public class TreeNode<T>{
	public List<TreeNode<T>> children{get;}
	public T value;
	public TreeNode<T>? parent{get; private set;}
	
	public bool isLeaf{get{
		return children.Count == 0;
	}}
	
	public bool isRoot{get{
		return parent == null;
	}}
	
	public TreeNode(T val){
		value = val;
		children = new List<TreeNode<T>>();
	}
	
	public TreeNode(T val, List<TreeNode<T>> childlist){
		value = val;
		children = childlist;
	}
	
	public TreeNode<T> FindRoot(){
		if(isRoot){
			return this;
		}else{
			return parent.FindRoot();
		}
	}
	
	public void AddChild(TreeNode<T> node){
		node.parent = this;
		children.Add(node);
	}
	
	public bool RemoveChild(TreeNode<T> node){
		node.parent = null;
		return children.Remove(node);
	}
	
	public void ClearChildren(){
		foreach(TreeNode<T> n in children){
			n.parent = null;
		}
		children.Clear();
	}
	
	public int CountChildren(){
		return children.Count;
	}
	
	public int CountDescendants(){
		int c = 0;
		foreach(TreeNode<T> n in children){
			c++;
			c += n.CountDescendants();
		}
		return c;
	}
	
	public int CountLeafs(){
		int c = 0;
		foreach(TreeNode<T> n in children){
			if(n.isLeaf){
				c++;
			}else{
				c += n.CountLeafs();
			}
		}
		return c;
	}
	
	public int DetermineTreeDepth(){
		int c = 0;
		foreach(TreeNode<T> n in children){
			int d = n.DetermineTreeDepth();
			if(d > c){
				c = d;
			}
		}
		return c + 1;
	}
	
	public List<TreeNode<T>> GetAllNodes(){
		List<TreeNode<T>> list = new List<TreeNode<T>>();
		list.Add(this);
		
		foreach(TreeNode<T> n in children){
			list.AddRange(n.GetAllNodes());
		}
		
		return list;
	}
	
	public List<TreeNode<T>> GetAllLeafNodes(){
		List<TreeNode<T>> list = new List<TreeNode<T>>();
		
		if(isLeaf){
			list.Add(this);
		}else{
			foreach(TreeNode<T> n in children){
				list.AddRange(n.GetAllLeafNodes());
			}
		}
		
		return list;
	}
	
	public void TraversePreOrder(Action<TreeNode<T>> action){
		action(this);
		foreach(TreeNode<T> n in children){
			n.TraversePreOrder(action);
		}
	}
	
	public void TraversePostOrder(Action<TreeNode<T>> action){
		foreach(TreeNode<T> n in children){
			n.TraversePostOrder(action);
		}
		action(this);
	}
	
	public void TraverseLevelOrder(Action<TreeNode<T>> action){
		Queue<TreeNode<T>> queue = new Queue<TreeNode<T>>();
		queue.Enqueue(this);
		
		while(queue.Count > 0){
			TreeNode<T> n = queue.Dequeue();
			action(n);
			
			foreach(TreeNode<T> c in n.children){
				queue.Enqueue(c);
			}
		}
	}
	
	public void TraverseLeafsOnly(Action<TreeNode<T>> a){
		if(isLeaf){
			a(this);
		}
		else{
			foreach(TreeNode<T> n in children){
				n.TraverseLeafsOnly(a);
			}	
		}
	}
	
	public List<TreeNode<T>> GetPathToRoot(){
		List<TreeNode<T>> p = new List<TreeNode<T>>();
		
		TreeNode<T>? c = this;
		while(c != null){
			p.Add(c);
			c = c.parent;
		}
		
		p.Reverse();
		return p;
	}
	
	public int GetDepth(){
		int d = 0;
		
		TreeNode<T>? c = this.parent;
		while(c != null){
			d++;
			c = c.parent;
		}
		
		return d;
	}
	
	public TreeNode<T>? FindNode(T target){
		if(EqualityComparer<T>.Default.Equals(value, target)){
			return this;
		}
		
		foreach(TreeNode<T> n in children){
			TreeNode<T>? f = n.FindNode(target);
			if(f != null){
				return f;
			}
		}
		
		return null;
	}
	
	public TreeNode<T>? FindChildNode(T target){		
		foreach(TreeNode<T> n in children){
			if(EqualityComparer<T>.Default.Equals(n.value, target)){
				return n;
			}
		}
		
		return null;
	}
	
	public TreeNode<T>? FindNode(Predicate<TreeNode<T>> condition){
		if(condition(this)){
			return this;
		}
		
		foreach(TreeNode<T> n in children){
			TreeNode<T>? f = n.FindNode(condition);
			if(f != null){
				return f;
			}
		}
		
		return null;
	}
	
	public TreeNode<T>? FindNode(Predicate<T> condition){
		if(condition(value)){
			return this;
		}
		
		foreach(TreeNode<T> n in children){
			TreeNode<T>? f = n.FindNode(condition);
			if(f != null){
				return f;
			}
		}
		
		return null;
	}
	
	public TreeNode<T>? FindChildNode(Predicate<TreeNode<T>> condition){
		foreach(TreeNode<T> n in children){
			if(condition(n)){
				return n;
			}
		}
		
		return null;
	}
	
	public TreeNode<T>? FindChildNode(Predicate<T> condition){
		foreach(TreeNode<T> n in children){
			if(condition(n.value)){
				return n;
			}
		}
		
		return null;
	}
	
	public TreeNode<T> Clone(){
		TreeNode<T> n = new TreeNode<T>(value);
		
		foreach (TreeNode<T> c in children){
			n.AddChild(c.Clone());
		}
		
		return n;
	}
	
	public TreeNode<T2> Clone<T2>(Converter<TreeNode<T>, TreeNode<T2>> f){
		TreeNode<T2> n = f.Invoke(this);
		
		foreach(TreeNode<T> c in children){
			n.AddChild(c.Clone(f));
		}
		
		return n;
	}
	
	public string ToStringFormat(){
		StringBuilder sb = new StringBuilder();
		
		sb.Append(">\"");
		if(value != null){
			string s = value.ToString();
			sb.Append(s.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\n").Replace("\r\n", "\\n"));
		}
		sb.Append("\"");
		if(!isLeaf){
			sb.Append(":");
		}
		
		for(int i = 0; i < children.Count; i++){
			sb.Append(children[i].ToStringFormat(out bool b));
			if(!b && i != children.Count - 1){
				sb.Append(",");
			}
		}
		
		sb.Append(";");
		return sb.ToString();
	}
	
	private string ToStringFormat(out bool b){
		StringBuilder sb = new StringBuilder();
		
		if(value != null){
			string s = value.ToString();
			if(s.Contains(";") || s.Contains(",") || s.Contains(":")){
				sb.Append("\"");
			}
			sb.Append(s.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\n").Replace("\r\n", "\\n"));
			if(s.Contains(";") || s.Contains(",") || s.Contains(":")){
				sb.Append("\"");
			}
		}else{
			sb.Append("\"\"");
		}
		
		if(!isLeaf){
			sb.Append(":");
		}
		
		for(int i = 0; i < children.Count; i++){
			sb.Append(children[i].ToStringFormat(out bool y));
			if(!y && i != children.Count - 1){
				sb.Append(",");
			}
		}
		
		if(!isLeaf){
			sb.Append(";");
			b = true;
		}else{
			b = false;
		}
		return sb.ToString();
	}
	
	public static bool TryParseStringFormat(string s, out TreeNode<string> tree){
		try{
			tree = ParseStringFormat(s);
			return true;
		}catch{
			tree = null;
			return false;
		}
	}
	
	public static TreeNode<string> ParseStringFormat(string s){
		s = s.Trim();
		if(s.Length < 4){
			throw new FormatException("Too short string");
		}
		int index = 0;
		if(s[index] != '>'){
			throw new FormatException("Expected '>', instead found " + s[index]);
		}
		index++;
		string f = ParseString(s, ref index);
		
		TreeNode<string> r = new TreeNode<string>(f);
		TreeNode<string> n = r;
		
		while(true){
			if(s[index] == ';'){
				if(n.isLeaf){
					if(n == r || n.parent == r){
						break;
					}
					n = n.parent.parent;
				}else{
					if(n == r){
						break;
					}
					n = n.parent;
				}
				index++;
			}else if(s[index] == ':'){
				index++;
				string g = ParseString(s, ref index);
				TreeNode<string> w = new TreeNode<string>(g);
				n.AddChild(w);
				n = w;
			}else if(s[index] == ','){
				index++;
				n = n.parent;
				string g = ParseString(s, ref index);
				TreeNode<string> w = new TreeNode<string>(g);
				n.AddChild(w);
				n = w;
			}else{
				string g = ParseString(s, ref index);
				TreeNode<string> w = new TreeNode<string>(g);
				n.AddChild(w);
				n = w;
			}
		}
		
		return r;
	}
	
	public static bool TryParseStringFormat(string s, Func<string, T> parseFunc, out TreeNode<T> tree){
		try{
			tree = ParseStringFormat(s, parseFunc);
			return true;
		}catch{
			tree = null;
			return false;
		}
	}
	
	public static TreeNode<T> ParseStringFormat(string s, Func<string, T> parseFunc){
		s = s.Trim();
		if(s.Length < 4){
			throw new FormatException("Too short string");
		}
		int index = 0;
		if(s[index] != '>'){
			throw new FormatException("Expected '>', instead found " + s[index]);
		}
		index++;
		string f = ParseString(s, ref index);
		T val = parseFunc(f);
		
		TreeNode<T> r = new TreeNode<T>(val);
		TreeNode<T> n = r;
		
		while(true){
			if(s[index] == ';'){
				if(n.isLeaf){
					if(n == r || n.parent == r){
						break;
					}
					n = n.parent.parent;
				}else{
					if(n == r){
						break;
					}
					n = n.parent;
				}
				index++;
			}else if(s[index] == ':'){
				index++;
				string g = ParseString(s, ref index);
				val = parseFunc(g);
				
				TreeNode<T> w = new TreeNode<T>(val);
				n.AddChild(w);
				n = w;
			}else if(s[index] == ','){
				index++;
				n = n.parent;
				string g = ParseString(s, ref index);
				val = parseFunc(g);
				TreeNode<T> w = new TreeNode<T>(val);
				n.AddChild(w);
				n = w;
			}else{
				string g = ParseString(s, ref index);
				val = parseFunc(g);
				TreeNode<T> w = new TreeNode<T>(val);
				n.AddChild(w);
				n = w;
			}
		}
		
		return r;
	}
	
	private static string ParseString(string s, ref int index){		
		if(index >= s.Length){
			throw new FormatException("Expected '\"' and found end of string");
		}
		
		StringBuilder sb = new StringBuilder();
		
		if(s[index] != '"'){
			while(s[index] != ';' && s[index] != ':' && s[index] != ','){
				sb.Append(s[index]);
				index++;
			}
			return sb.ToString().Replace("\\\"", "\"").Replace("\\n", Environment.NewLine);
		}
		
		index++;
		
		bool previousEscape = false;
		
		while(true){
			if(index >= s.Length){
				throw new FormatException("Expected '\"' and found end of string");
			}
			
			if(s[index] == '"'){
				if(previousEscape){
					sb.Remove(sb.Length - 1, 1);
					sb.Append(s[index]);
					previousEscape = false;
					index++;
					continue;
				}else{
					index++;
					return sb.ToString().Replace("\\\"", "\"").Replace("\\n", Environment.NewLine);
				}
			}
			
			if(s[index] == '\\'  && !previousEscape){
				previousEscape = true;
			}else{
				previousEscape = false;
			}
			sb.Append(s[index]);
			index++;
		}
	}
	
	public override string ToString(){
		return ToStringHelper(this, 0);
	}
	
	private static string ToStringHelper(TreeNode<T> n, int level){
		string indent = "";
		if(level > 0){
			indent = string.Concat(Enumerable.Repeat("│  ", level - 1));
			indent += "├──";
		}
		//string indent = new string(' ', level * 2);
		StringBuilder sb = new StringBuilder();
		sb.Append(indent);
		
		string val2 = n.value.ToString();
		string[] lines = val2.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
		
		if(lines.Length > 0){
			bool b = true;
			string t = string.Concat(Enumerable.Repeat("│  ", level));
			foreach(string str in lines){
				if(b){
					b = false;
				}else{
					sb.Append(Environment.NewLine);
					sb.Append(t);
				}
				sb.Append(str);
			}
		}else{
			sb.Append(val2);
		}
		
		foreach(TreeNode<T> c in n.children){
			sb.Append(Environment.NewLine);
			sb.Append(ToStringHelper(c, level + 1));
		}
		
		return sb.ToString();
	}
	
	public FormatString ToFormattedString(CharFormat? lines, CharFormat? val){
		return ToFormattedStringHelper(this, 0, lines, val);
	}
	
	private static FormatString ToFormattedStringHelper(TreeNode<T> n, int level, CharFormat? lines, CharFormat? v){
		string indent = "";
		if(level > 0){
			indent = string.Concat(Enumerable.Repeat("│  ", level - 1));
			indent += "├──";
		}
		//string indent = new string(' ', level * 2);
		FormatString fs = new FormatString();
		fs.Append(indent, lines);
		
		if(n.value is FormatString nfs){
			FormatString[] liness = nfs.SplitIntoLines();
			if(liness.Length > 0){
				bool b = true;
				FormatString t = new FormatString(string.Concat(Enumerable.Repeat("│  ", level)), lines);
				foreach(FormatString str in liness){
					if(b){
						b = false;
					}else{
						fs.Append(Environment.NewLine);
						fs.Append(t);
					}
					fs.Append(str);
				}
			}else{
				fs.Append(nfs);
			}
		}else{
			string val2 = n.value.ToString();
			string[] liness = val2.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
			
			if(liness.Length > 0){
				bool b = true;
				FormatString t = new FormatString(string.Concat(Enumerable.Repeat("│  ", level)), lines);
				foreach(string str in liness){
					if(b){
						b = false;
					}else{
						fs.Append(Environment.NewLine);
						fs.Append(t);
					}
					fs.Append(str, v);
				}
			}else{
				fs.Append(val2, v);
			}
		}
		
		foreach(TreeNode<T> c in n.children){
			fs.Append(Environment.NewLine);
			fs.Append(ToFormattedStringHelper(c, level + 1, lines, v));
		}
		
		return fs;
	}
}