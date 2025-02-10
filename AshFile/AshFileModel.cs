using System;
using AshLib.Dates;

namespace AshLib.AshFiles;

public class AshFileModel{
	
	public bool allowUnsupportedTypes;
	public bool deleteNotMentioned;
	public ModelInstance[] instances;
	
	public static readonly AshFileModel DeleteUnsupportedTypes = new AshFileModel();
	
	public AshFileModel(params ModelInstance[] m){
		instances = m;
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
	
	public ModelInstance(ModelInstanceOperation o, string n, object v){
		operation = o;
		name = n;
		value = v;
	}
}

public enum ModelInstanceOperation{
	Delete, Exists, Type, TypeCast, Value
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
					if(GetFileTypeFromType(kvp.Value.GetType().GetElementType()) == AshFileTypeV3.Default){
						a.DeleteCamp(kvp.Key);
					}
				}else{
					if(GetFileTypeFromType(kvp.Value.GetType()) == AshFileTypeV3.Default){
						a.DeleteCamp(kvp.Key);
					}
				}
			}
		}
		
		if(m.deleteNotMentioned && m.instances != null){
			List<string> names = new List<string>(m.instances.Length);
			foreach(ModelInstance i in m.instances){
				names.Add(i.name);
			}
			
			foreach(KeyValuePair<string, object> kvp in a.data){
				if(!names.Contains(kvp.Key)){
					a.DeleteCamp(kvp.Key);
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
	
	private static AshFileTypeV3 GetFileTypeFromType(Type type){
		if(type == typeof(string)) return AshFileTypeV3.String;
		if(type == typeof(byte)) return AshFileTypeV3.Byte;
		if(type == typeof(ushort)) return AshFileTypeV3.Ushort;
		if(type == typeof(uint)) return AshFileTypeV3.Uint;
		if(type == typeof(ulong)) return AshFileTypeV3.Ulong;
		if(type == typeof(sbyte)) return AshFileTypeV3.Sbyte;
		if(type == typeof(short)) return AshFileTypeV3.Short;
		if(type == typeof(int)) return AshFileTypeV3.Int;
		if(type == typeof(long)) return AshFileTypeV3.Long;
		if(type == typeof(Color3)) return AshFileTypeV3.Color3;
		if(type == typeof(float)) return AshFileTypeV3.Float;
		if(type == typeof(double)) return AshFileTypeV3.Double;
		if(type == typeof(Vec2)) return AshFileTypeV3.Vec2;
		if(type == typeof(Vec3)) return AshFileTypeV3.Vec3;
		if(type == typeof(Vec4)) return AshFileTypeV3.Vec4;
		if(type == typeof(bool)) return AshFileTypeV3.Bool;
		if(type == typeof(Date)) return AshFileTypeV3.Date;
	
		return AshFileTypeV3.Default; // Default case if no matching type
	}
}