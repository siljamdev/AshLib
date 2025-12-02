using System;
using System.Collections;
using System.Collections.Generic;

namespace AshLib.AshFiles;

public partial class AshFile{
	public object this[string key]{
		get{
			return GetValue(key);
		}
		set{
			Set(key, value);
		}
	}
	
	ICollection<string> IDictionary<string, object>.Keys {
		get{
			return ((IDictionary<string, object>) this.data).Keys;
		}
	}
	
	public ICollection<string> Keys {
		get{
			return this.data.Keys;
		}
	}
	
	ICollection<object> IDictionary<string, object>.Values {
		get{
			return ((IDictionary<string, object>) this.data).Values;
		}
	}
	
	public ICollection<object> Values {
		get{
			return this.data.Values;
		}
	}
	
	public void Clear(){
		this.data.Clear();
	}
	
	public bool Contains(KeyValuePair<string, object> kvp){
		return this.data.Contains(kvp);
	}
	
	public bool ContainsKey(string key){
		return this.data.ContainsKey(key);
	}
	
	//Set
	
	public void Set(string key, object val){
		this.data[key] = val;
	}
	
	public void Add(KeyValuePair<string, object> kvp){
		Set(kvp.Key, kvp.Value);
	}
	
	public void Add(string k, object v){
		Set(k, v);
	}

	//Initialize
	
	public void Initialize(string key, object val){
		if(this.ContainsKey(key)){
			return;
		}
		this.Set(key, val);
	}

	//Get
	
	public object GetValue(string key){
		if(!this.ContainsKey(key)){
			return null;
		}
		return this.data[key];
	}
	
	public T GetValue<T>(string key){
		if(!this.ContainsKey(key)){
			return default(T);
		}
		if(this.data[key] is T val){
			return val;
		}
		return default(T);
	}
	
	public T GetOrDefault<T>(string key, T def){
		if(!this.ContainsKey(key)){
			return def;
		}
		if(this.data[key] is T val){
			return val;
		}
		return def;
	}
	
	public bool TryGetValue(string key, out object value){
		return this.data.TryGetValue(key, out value);
	}
	
	public bool TryGetValue<T>(string key, out T val){
		if(!this.ContainsKey(key)){
			val = default(T);
			return false;
		}
		if(this.data[key] is T v){
			val = v;
			return true;
		}
		val = default(T);
		return false;
	}
	
	//get type
	
	public Type GetValueType(string key){
		if(!this.ContainsKey(key)){
			return null;
		}
		return this.data[key].GetType();
	}
	
	public bool TryGetValueType(string key, out Type t){
		if(!this.ContainsKey(key)){
			t = null;
			return false;
		}
		t = this.data[key].GetType();
		return true;
	}
	
	//Remove
	
	public bool Remove(KeyValuePair<string, object> kvp){
		foreach(KeyValuePair<string, object> kvp2 in this){
			if(kvp.Key == kvp2.Key && kvp.Value == kvp2.Value){
				Remove(kvp.Key);
				return true;
			}
		}
		return false;
	}
	
	public bool Remove(string key){
		return this.data.Remove(key);
	}
	
	public int RemoveAll(Predicate<KeyValuePair<string, object>> condition){
		if(condition == null){
			return 0;
		}
		int c = 0;
		foreach(KeyValuePair<string, object> kvp in this){
			if(condition(kvp) && this.Remove(kvp.Key)){
				c++;
			}
		}
		return c;
	}
	
	//Rename
	
	public bool Rename(string oldKey, string newKey){
		if(!this.ContainsKey(oldKey)){
			return false;
		}
		object o = this.GetValue(oldKey);
		this.Remove(oldKey);
		this.Set(newKey, o);
		return true;
	}
	
	public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex){
		((ICollection<KeyValuePair<string, object>>)this.data).CopyTo(array, arrayIndex);
	}
}