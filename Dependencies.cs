using System;

namespace AshLib;

public class Dependencies{ //Used for handling files in a central folder (kindof the .minecraft folder)
	public string path;
	public AshFile config;
	
	public Dependencies(string path, bool config, string[] directories, string[] files){
		this.path = path;
		checkDir(this.path);
		if(config){
			this.config = new AshFile(this.path + "/config.ash");
		}
		
		if(directories != null){
			for(int i = 0; i < directories.Length; i++){
				this.checkDir(this.path + "/" + directories[i]);
			}
		}
		
		if(files != null){
			for(int i = 0; i < files.Length; i++){
				this.checkFile(files[i]);
			}	
		}
	}

	public string ReadFileText(string p){
		if(!File.Exists(this.path + "/" + p)){
			return "";
		}
		return File.ReadAllText(this.path + "/" + p);
	}
	
	public AshFile ReadAshFile(string p){
		return new AshFile(this.path + "/" + p);
	}
	
	public void SaveFileText(string p, string t){
		File.WriteAllText(this.path + "/" + p, t);
	}
	
	public void SaveAshFile(string p, AshFile a){
		a.Save(this.path + "/" + p);
	}
	
	public void CreateDir(string p){
		if(!Directory.Exists(this.path + "/" + p)){
			Directory.CreateDirectory(this.path + "/" + p);
		}
	}
	
	private void checkDir(string p){
		if(!Directory.Exists(p)){
			Directory.CreateDirectory(p);
		}
	}
	
	private void checkFile(string p){
		if(!File.Exists(path + "/" + p)){
			FileStream fs = File.Create(path + "/" + p);
			fs.Close();
		}
	}
	
}