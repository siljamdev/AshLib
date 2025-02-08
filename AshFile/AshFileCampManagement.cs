using System;
using AshLib.Dates;

namespace AshLib.AshFiles;

public partial class AshFile
{
	public bool ExistsCamp(string name){
		if(this.data.ContainsKey(name)){
			return true;
		}
		return false;
	}
	
	//Set
	
	public void SetCamp(string name, object val){
		this.data[name] = val;
	}

	//Initialize
	
	public void InitializeCamp(string name, object val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, val);
	}

	//Get
	
	public object GetCamp(string name){
		if(!this.ExistsCamp(name)){
			return null;
		}
		return this.data[name];
	}
	
	public bool CanGetCamp(string name, out object val){
		if(!this.ExistsCamp(name)){
			val = null;
			return false;
		}
		val = this.data[name];
		return true;
	}
	
	public T GetCamp<T>(string name){
		if(!this.ExistsCamp(name)){
			return default(T);
		}
		if(this.data[name] is T val){
			return val;
		}
		return default(T);
	}
	
	public T GetCampOrDefault<T>(string name, T def){
		if(!this.ExistsCamp(name)){
			return def;
		}
		if(this.data[name] is T val){
			return val;
		}
		return def;
	}
	
	public bool CanGetCamp<T>(string name, out T val){
		if(!this.ExistsCamp(name)){
			val = default(T);
			return false;
		}
		if(this.data[name] is T v){
			val = v;
			return true;
		}
		val = default(T);
		return false;
	}
	
	//get type
	
	public Type GetCampType(string name){
		if(!this.ExistsCamp(name)){
			return null;
		}
		return this.data[name].GetType();
	}
	
	public bool CanGetCampType(string name, out Type t){
		if(!this.ExistsCamp(name)){
			t = null;
			return false;
		}
		t = this.data[name].GetType();
		return true;
	}
	
	//delete
	
	public void DeleteCamp(string name){
		if(!ExistsCamp(name)){
			return;
		}
		this.data.Remove(name);
	}
	
	public bool CanDeleteCamp(string name){
		if(!this.ExistsCamp(name)){
			return false;
		}
		this.data.Remove(name);
		return true;
	}
	
	//Rename
	
	public void RenameCamp(string oldName, string newName){
		if(!this.ExistsCamp(oldName)){
			return;
		}
		object o = this.GetCamp(oldName);
		this.DeleteCamp(oldName);
		this.SetCamp(newName, o);
	}
	
	public bool CanRenameCamp(string oldName, string newName){
		if(!this.ExistsCamp(oldName)){
			return false;
		}
		object o = this.GetCamp(oldName);
		this.DeleteCamp(oldName);
		this.SetCamp(newName, o);
		return true;
	}
}