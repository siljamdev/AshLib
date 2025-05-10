using System;
using System.Text;
using AshLib.Trees;
using AshLib.Formatting;

namespace AshLib.AshFiles;

public partial class AshFile{
	
	public const string DefaultSeparator = ".";
	
	public TreeNode<string> GetCampTree(string separator = DefaultSeparator){
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
	
	public TreeNode<string> GetValueTree(string separator = DefaultSeparator){
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
	
	public TreeNode<FormatString> GetFormattedValueTree(CharFormat? name, CharFormat? val, string separator = DefaultSeparator){
		TreeNode<FormatString> r = GetCampTree(separator).Clone(n => new TreeNode<FormatString>(new FormatString(n.value, name)));
		List<TreeNode<FormatString>> cs = r.GetAllNodes();
		
		Dictionary<TreeNode<FormatString>, FormatString> changes = new Dictionary<TreeNode<FormatString>, FormatString>();
		
		foreach(TreeNode<FormatString> n in cs){
			string s = GetWholeCampName(n.GetPathToRoot(), separator);
			if(ExistsCamp(s)){
				if(GetCamp(s) is Array array){
					foreach(object a in array){
						n.AddChild(new TreeNode<FormatString>(new FormatString(a.ToString(), val)));
					}
					changes.Add(n, n.value + new FormatString(":", val));
				}else{
					changes.Add(n, n.value + new FormatString(": " + GetCamp(s).ToString(), val));
				}
			}
		}
		
		foreach(KeyValuePair<TreeNode<FormatString>, FormatString> kvp in changes){
			kvp.Key.value = kvp.Value;
		}
		
		r.value = new FormatString("Root", name);
		
		return r;
	}
	
	public string VisualizeAsTree(string separator = DefaultSeparator){
		return GetValueTree(separator).ToString();
	}
	
	public FormatString VisualizeAsFormattedTree(CharFormat? line, CharFormat? name, CharFormat? val, string separator = DefaultSeparator){
		return GetFormattedValueTree(name, val, separator).ToFormattedString(line, val);
	}
	
	private string GetWholeCampName(List<TreeNode<string>> p, string separator = DefaultSeparator) {
		return string.Join(separator, p.Skip(1).Select(node => node.value));
	}
	
	private string GetWholeCampName(List<TreeNode<FormatString>> p, string separator = DefaultSeparator) {
		return string.Join(separator, p.Skip(1).Select(node => node.value.content));
	}
	
	public string Visualize(){
		StringBuilder s = new StringBuilder();
		bool f = false;
		foreach(KeyValuePair<string, object> kvp in this.data){
			if(kvp.Value is Array a){
				if(f){
					s.Append(Environment.NewLine);
				}
				string tab = new string(' ', kvp.Key.Length + 2);
				s.Append(kvp.Key);
				s.Append(": ");
				
				bool b = false;
				
				foreach(object o in a){
					if(b){
						s.Append(Environment.NewLine);
						s.Append(tab);
					}
					
					string val = o.ToString();
					string[] lines = val.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
					
					if(lines.Length > 1){
						bool b2 = true;
						foreach(string l in lines){
							if(b2){
								b2 = false;
							}else{
								s.Append(Environment.NewLine);
								s.Append(tab);
							}
							s.Append(l);
						}
					}else{
						s.Append(val);
					}
					
					b = true;
				}
			}else{
				if(f){
					s.Append(Environment.NewLine);
				}
				s.Append(kvp.Key);
				s.Append(": ");
				string tab = new string(' ', kvp.Key.Length + 2);
				
				string val = kvp.Value.ToString();
				string[] lines = val.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
				
				if(lines.Length > 1){
					bool b2 = true;
					foreach(string l in lines){
						if(b2){
							b2 = false;
						}else{
							s.Append(Environment.NewLine);
							s.Append(tab);
						}
						s.Append(l);
					}
				}else{
					s.Append(val);
				}
			}
			f = true;
		}
		return s.ToString();
	}
	
	public FormatString VisualizeFormatted(CharFormat? name, CharFormat? val){
		FormatString s = new FormatString();
		bool f = false;
		foreach(KeyValuePair<string, object> kvp in this.data){
			if(kvp.Value is Array a){
				if(f){
					s.Append(Environment.NewLine);
				}
				string tab = new string(' ', kvp.Key.Length + 2);
				s.Append(kvp.Key, name);
				s.Append(": ", val);
				
				bool b = false;
				
				foreach(object o in a){
					if(b){
						s.Append(Environment.NewLine);
						s.Append(tab);
					}
					
					string val2 = o.ToString();
					string[] lines = val2.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
					
					if(lines.Length > 1){
						bool b2 = true;
						foreach(string l in lines){
							if(b2){
								b2 = false;
							}else{
								s.Append(Environment.NewLine);
								s.Append(tab);
							}
							s.Append(l, val);
						}
					}else{
						s.Append(val2, val);
					}
					
					b = true;
				}
			}else{
				if(f){
					s.Append(Environment.NewLine);
				}
				s.Append(kvp.Key, name);
				s.Append(": ", val);
				string tab = new string(' ', kvp.Key.Length + 2);
				
				string val2 = kvp.Value.ToString();
				string[] lines = val2.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
				
				if(lines.Length > 1){
					bool b2 = true;
					foreach(string l in lines){
						if(b2){
							b2 = false;
						}else{
							s.Append(Environment.NewLine);
							s.Append(tab);
						}
						s.Append(l, val);
					}
				}else{
					s.Append(val2, val);
				}
			}
			f = true;
		}
		return s;
	}
}