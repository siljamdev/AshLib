using System;
using System.Text;
using AshLib.Trees;

namespace AshLib.AshFiles;

public partial class AshFile{
	
	public const string DefaultSeparator = ".";
	
	public TreeNode<string> GetCampTree(string separator){
		TreeNode<string> root = new TreeNode<string>(null);
		
		foreach(KeyValuePair<string, object> kvp in this.data){
			string[] p = kvp.Key.Split(separator);
			TreeNode<string> c = root;
			int i = 0;
			while(i < p.Length){
				TreeNode<string>? t = c.FindChildNode(p[i]);
				if(t == null){
					break;
				}
				c = t;
				i++;
			}
			
			for(; i < p.Length; i++){
				TreeNode<string> v = new TreeNode<string>(p[i]);
				c.AddChild(v);
				c = v;
			}
		}
		
		return root;
	}
	
	public TreeNode<string> GetValueTree(string separator){
		TreeNode<string> r = GetCampTree(separator);
		List<TreeNode<string>> cs = r.GetAllNodes();
		
		Dictionary<TreeNode<string>, string> changes = new Dictionary<TreeNode<string>, string>();
		
		foreach(TreeNode<string> n in cs){
			string s = GetWholeCampName(n.GetPathToRoot(), separator);
			if(ExistsCamp(s)){
				if(GetCamp(s) is Array array){
					foreach(object a in array){
						n.AddChild(new TreeNode<string>(a.ToString()));
					}
					changes.Add(n, n.value + ":");
				}else{
					changes.Add(n, n.value + ": " + GetCamp(s).ToString());
				}
			}
		}
		
		foreach(KeyValuePair<TreeNode<string>, string> kvp in changes){
			kvp.Key.value = kvp.Value;
		}
		
		r.value = "Root";
		
		return r;
	}
	
	public string VisualizeAsTree(string separator){
		return GetValueTree(separator).ToString();
	}
	
	public string VisualizeAsTree(){
		return GetValueTree(DefaultSeparator).ToString();
	}
	
	private string GetWholeCampName(List<TreeNode<string>> p, string separator) {
		return string.Join(separator, p.Skip(1).Select(node => node.value));
	}
}