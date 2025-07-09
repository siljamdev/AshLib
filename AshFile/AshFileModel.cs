using System;
using System.Collections;
using AshLib.Dates;

namespace AshLib.AshFiles;

public class AshFileModel{
	
	public bool allowUnsupportedTypes;
	public bool deleteNotMentioned;
	public List<ModelInstance> instances{get;}
	public List<Action<AshFile, string, object>> actions{get;}
	
	public static readonly AshFileModel DeleteUnsupportedTypes = new AshFileModel();
	
	public AshFileModel(params ModelInstance[] insArray){
		instances = insArray.ToList();
		actions = new List<Action<AshFile, string, object>>();
		allowUnsupportedTypes = true;
		deleteNotMentioned = false;
	}
	
	private AshFileModel(){
		allowUnsupportedTypes = false;
		deleteNotMentioned = false;
	}
	
	public void Merge(AshFileModel m){
		instances.AddRange(m.instances);
		actions.AddRange(m.actions);
	}
	
	public static AshFileModel operator +(AshFileModel m1, AshFileModel m2){
		m1.Merge(m2);
		return m1;
	}
}

public class ModelInstance{
	public ModelInstanceOperation operation;
	public string name;
	public object value;
	
	public ModelInstance(ModelInstanceOperation o, string nam, object val){
		operation = o;
		name = nam;
		value = val;
	}
}

public enum ModelInstanceOperation{
	Delete, Exists, Type, TypeCast, Value, None
}

public partial class AshFile{	
	public void ApplyModel(AshFileModel m){
		if(m.instances != null){
			foreach(ModelInstance i in m.instances){
				switch(i.operation){
					case ModelInstanceOperation.Delete:
						this.Remove(i.name);
						break;
					case ModelInstanceOperation.Exists:
						this.Initialize(i.name, i.value);
						break;
					case ModelInstanceOperation.Type:
						if(this.TryGetValueType(i.name, out Type t)){
							if(t != i.value.GetType()){
								this.Set(i.name, i.value);
							}
						}else{
							this.Set(i.name, i.value);
						}
						break;
					case ModelInstanceOperation.TypeCast:
						if(this.TryGetValueType(i.name, out Type y)){
							if(!CanWorkTogether(i.value.GetType(), y)){
								this.Set(i.name, i.value);
							}
						}else{
							this.Set(i.name, i.value);
						}
						break;
					case ModelInstanceOperation.Value:
						this.Set(i.name, i.value);
						break;
				}
			}	
		}
		
		
		if(!m.allowUnsupportedTypes){
			foreach(KeyValuePair<string, object> kvp in this){
				if(kvp.Value is IEnumerable enu && kvp.Value is not string){
					if(GetFileTypeFromType(GetBaseTypeOfEnumerable(enu)) == AshFileType.Default){
						this.Remove(kvp.Key);
					}
				}else{
					if(GetFileTypeFromType(kvp.Value.GetType()) == AshFileType.Default){
						this.Remove(kvp.Key);
					}
				}
			}
		}
		
		if(m.deleteNotMentioned && m.instances != null){
			List<string> names = m.instances.Select(i => i.name).ToList();
			
			foreach(KeyValuePair<string, object> kvp in this){
				if(!names.Contains(kvp.Key)){
					this.Remove(kvp.Key);
				}
			}
		}
		
		if(m.actions != null){
			foreach(KeyValuePair<string, object> kvp in this){
				foreach(Action<AshFile, string, object> act in m.actions){
					act(this, kvp.Key, kvp.Value);
				}
			}
		}
	}
	
	public static AshFile ApplyModel(AshFile b, AshFileModel m){
		AshFile a = Clone(b);
		a.ApplyModel(m);
		return a;
	}
	
	public static AshFile operator *(AshFile b, AshFileModel m){
		return ApplyModel(b, m);
	}
	
	private static bool CanWorkTogether(Type type1, Type type2){
        if(type1 == type2){
            return true;
        }

        if(type1.IsAssignableFrom(type2)){
            return true;
        }

        try{
            Convert.ChangeType(Activator.CreateInstance(type1), type2);
            return true;
        }
        catch{
            return false;
        }
    }
}