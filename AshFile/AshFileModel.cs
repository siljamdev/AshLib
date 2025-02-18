using System;
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
	public static AshFile operator *(AshFile b, AshFileModel m){
		AshFile a = AshFile.DeepCopy(b);
		if(m.instances != null){
			foreach(ModelInstance i in m.instances){
				switch(i.operation){
					case ModelInstanceOperation.Delete:
						a.DeleteCamp(i.name);
						break;
					case ModelInstanceOperation.Exists:
						a.InitializeCamp(i.name, i.value);
						break;
					case ModelInstanceOperation.Type:
						if(a.CanGetCampType(i.name, out Type t)){
							if(t != i.value.GetType()){
								a.SetCamp(i.name, i.value);
							}
						}else{
							a.SetCamp(i.name, i.value);
						}
						break;
					case ModelInstanceOperation.TypeCast:
						if(a.CanGetCampType(i.name, out Type y)){
							if(!CanWorkTogether(i.value.GetType(), y)){
								a.SetCamp(i.name, i.value);
							}
						}else{
							a.SetCamp(i.name, i.value);
						}
						break;
					case ModelInstanceOperation.Value:
						a.SetCamp(i.name, i.value);
						break;
				}
			}	
		}
		
		
		if(!m.allowUnsupportedTypes){
			foreach(KeyValuePair<string, object> kvp in a.data){
				if(kvp.Value.GetType().IsArray){
					if(GetFileTypeFromType(kvp.Value.GetType().GetElementType()) == AshFileType.Default){
						a.DeleteCamp(kvp.Key);
					}
				}else{
					if(GetFileTypeFromType(kvp.Value.GetType()) == AshFileType.Default){
						a.DeleteCamp(kvp.Key);
					}
				}
			}
		}
		
		if(m.deleteNotMentioned && m.instances != null){
			List<string> names = new List<string>(m.instances.Count);
			foreach(ModelInstance i in m.instances){
				names.Add(i.name);
			}
			
			foreach(KeyValuePair<string, object> kvp in a.data){
				if(!names.Contains(kvp.Key)){
					a.DeleteCamp(kvp.Key);
				}
			}
		}
		
		if(m.actions != null){
			foreach(KeyValuePair<string, object> kvp in a.data){
				foreach(Action<AshFile, string, object> act in m.actions){
					act(a, kvp.Key, kvp.Value);
				}
			}
		}
		
		return a;
	}
	
	public static AshFile ApplyModel(AshFile a, AshFileModel m){
		return a * m;
	}
	
	private static bool CanWorkTogether(Type type1, Type type2){
        if (type1 == type2){
            return true;
        }

        if (type1.IsAssignableFrom(type2)){
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
	
	private static AshFileType GetFileTypeFromType(Type type){
		if(type == typeof(string)) return AshFileType.String;
		if(type == typeof(byte)) return AshFileType.Byte;
		if(type == typeof(ushort)) return AshFileType.Ushort;
		if(type == typeof(uint)) return AshFileType.Uint;
		if(type == typeof(ulong)) return AshFileType.Ulong;
		if(type == typeof(sbyte)) return AshFileType.Sbyte;
		if(type == typeof(short)) return AshFileType.Short;
		if(type == typeof(int)) return AshFileType.Int;
		if(type == typeof(long)) return AshFileType.Long;
		if(type == typeof(Color3)) return AshFileType.Color3;
		if(type == typeof(float)) return AshFileType.Float;
		if(type == typeof(double)) return AshFileType.Double;
		if(type == typeof(Vec2)) return AshFileType.Vec2;
		if(type == typeof(Vec3)) return AshFileType.Vec3;
		if(type == typeof(Vec4)) return AshFileType.Vec4;
		if(type == typeof(bool)) return AshFileType.Bool;
		if(type == typeof(Date)) return AshFileType.Date;
	
		return AshFileType.Default; // Default case if no matching type
	}
}