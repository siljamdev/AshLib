using System;
using System.Text;

namespace AshLib.Logging;

public class TreeLog{ //Used for logging in some format
	private readonly object lockObj = new object();
	
	private StringBuilder log;
	
	private int level;
	
	public string GetLog(){
		lock(lockObj){
			return log.ToString();
		}
	}
	
	public void Reset(){
		lock(lockObj){
			log.Clear();
			level = 0;
		}
	}
	
	public void Deep(string s){
		Write("<#> " + s);
		level++;
	}
	
	public void Shallow(){
		level--;
		Write("<!>");
	}
	
	public void Write(string s){
		lock(lockObj){
			log.AppendLine(new string('\t', level) + s);
		}
	}
	
	public void Write(object s){
		Write(s.ToString());
	}
}