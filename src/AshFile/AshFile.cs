using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AshLib.AshFiles;

public partial class AshFile : IDictionary<string, object>, ICloneable, IEquatable<AshFile>
{
	
	public const int latestFormat = 4;
	
	public static bool writeUseLatestFormat = false;
	
	Dictionary<string, object> data; //internal dict
	public string? path;
	
	public byte format = latestFormat;
	public bool compactBools = true;
	public bool maskCampNames = true;
	public bool maskStrings = true;
	public string? password;
	
	public AshFileFormatOptions FormatOptions{get{
		return new AshFileFormatOptions(format, compactBools, maskCampNames, maskStrings, password);
	}}
	
	public int Count{
		get{
			return data.Count;
		}
	}
	
	public bool IsReadOnly{
		get{
			return false;
		}
	}
	
	public bool IsFixedSize{
		get{
			return false;
		}
	}
	
	//Constructors
	
	public AshFile(){
		this.data = new Dictionary<string, object>();
	}
	
	public AshFile(IDictionary<string, object> d){
		this.data = new Dictionary<string, object>(d);
	}
	
	//Clone constructor
	public AshFile(AshFile a) : this(){
		if(a.path != null){
			this.path = new string(a.path);
		}
		
		if(a.format != null){
			this.format = a.format;
		}
		
		this.ImportFormatOptions(a.FormatOptions);
		
		if(a.data != null){
			foreach(KeyValuePair<string, object> kvp in a){
				if(kvp.Value is ICloneable cloneable){
					this.data.Add(new string(kvp.Key), cloneable.Clone());
				}else{
					this.data.Add(new string(kvp.Key), kvp.Value);
				}
			}
		}
	}
	
	public AshFile(string path, string passw = null){
		this.path = path;
		if(!File.Exists(path)){
			this.data = new Dictionary<string, object>();
			this.format = latestFormat;
		}else{
			this.data = ReadFromFile(path, out AshFileFormatOptions conf, passw);
			this.ImportFormatOptions(conf);
		}
	}
	
	//Other stuff
	
	public void ImportFormatOptions(AshFileFormatOptions conf){
		this.format = conf.format;
		this.compactBools = conf.compactBools;
		this.maskCampNames = conf.maskCampNames;
		this.maskStrings = conf.maskStrings;
		this.password = conf.password;
	}
	
	public IEnumerator<KeyValuePair<string, object>> GetEnumerator(){
		return data.GetEnumerator();
	}
	
	//IEnumerable method
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
		return GetEnumerator();
	}
	
	//================
	
	public void Load(string path, string passw = null){
		this.path = path;
		data = ReadFromFile(path, out AshFileFormatOptions conf, passw);
		this.ImportFormatOptions(conf);
	}
	
	public void Load(string passw = null){
		if(path == null){
			throw new AshFileException("Path has not been initialized", 1);
		}
		
		data = ReadFromFile(path, out AshFileFormatOptions conf, passw);
		this.ImportFormatOptions(conf);
	}
	
	public void Save(string path){
		this.path = path;
		WriteToFile(path, data, this.FormatOptions);
	}
	
	public void Save(){
		if(path == null){
			throw new AshFileException("Path has not been initialized", 1);
		}
		
		WriteToFile(path, data, this.FormatOptions);
	}
	
	
	
	//^^THINGS THE USER WILL USE^^ vvTHINGS THE USER COULD BUT WONT USEvv
	
	//Read
	
	public static Dictionary<string, object> ReadFromFile(string path, out AshFileFormatOptions conf, string passw = null){
		if(!File.Exists(path)){
			throw new AshFileException("File in the path of \"" + path + "\" can't be found", 2);
		}
		
		byte[] fileBytes = File.ReadAllBytes(path);
		Dictionary<string, object> d = ReadFromBytes(fileBytes, out conf, passw);
		return d;
	}
	
	public static Dictionary<string, object> ReadFromBytes(byte[] fileBytes, out AshFileFormatOptions conf, string passw = null){
		if(fileBytes.Length == 0){
			throw new AshFileException("Empty file bytes", 2);
		}
		
		switch(fileBytes[0]){
			case 1:
				conf = AshFileFormatOptions.Default with {format = 1};
				return V1.Read(fileBytes);
			case 2:
				conf = AshFileFormatOptions.Default with {format = 2};
				return V2.Read(fileBytes);
			case 3:
				return V3.Read(fileBytes, out conf);
			case 4:
				return V4.Read(fileBytes, passw, out conf);
			default:
				throw new AshFileException("Invalid or unknown file format: " + fileBytes[0], 3);
		}
	}
	
	public static AshFile ReadFromBytes(byte[] fileBytes, string passw = null){
		Dictionary<string, object> d = ReadFromBytes(fileBytes, out AshFileFormatOptions conf, passw);
		
		AshFile a = new AshFile(d);
		a.ImportFormatOptions(conf);
		
		return a;
	}
	
	//Write
	
	public static void WriteToFile(string path, Dictionary<string, object> dictionary, AshFileFormatOptions conf){
		File.WriteAllBytes(path, WriteToBytes(dictionary, conf));
	}
	
	public static byte[] WriteToBytes(Dictionary<string, object> dictionary, AshFileFormatOptions conf){
		if(writeUseLatestFormat){
			return V4.Write(dictionary, conf);
		}
		
		switch(conf.format){
			case 1:
				return V1.Write(dictionary);
			case 2:
				return V2.Write(dictionary);
			case 3:
				return V3.Write(dictionary, conf);
			case 4:
				return V4.Write(dictionary, conf);
			default:
				throw new AshFileException("Invalid or unknown file format: " + conf.format, 3);
		}
	}
	
	public byte[] WriteToBytes(){
		return WriteToBytes(this.data, this.FormatOptions);
	}
	
	//OPERATOR THINGS
	
	//Iclonable
	public object Clone(){
		return Clone(this);
	}
	
	public static AshFile Clone(AshFile a){
		return new AshFile(a);
	}
	
	public static AshFile Merge(AshFile a1, AshFile a2){
		AshFile o = Clone(a1);
		
		foreach(KeyValuePair<string, object> kvp in a2){
			o[kvp.Key] = kvp.Value;
        }
		
		return o;
	}
	
	public static AshFile operator +(AshFile a1, AshFile a2){
		return Merge(a1, a2);
	}
	
	public static explicit operator AshFile(Dictionary<string, object> d){
		return new AshFile(d);
	}
	
	public static bool operator ==(AshFile a1, AshFile a2){
		if(ReferenceEquals(a1, a2)) return true;
		if(a1 is null && a2 is null) return true;
		if(a1 is null || a2 is null) return false;
		
		if(a1.Count != a2.Count) return false;

        // Check each key-value pair in dict1
        foreach (KeyValuePair<string, object> kvp in a1){
            // If the key is not present in dict2, or the corresponding value is not equal, return false
            if(!a2.data.TryGetValue(kvp.Key, out object val) || !AreValuesEqual(val, kvp.Value))
                return false;
        }

        // If all key-value pairs in dict1 are present in dict2 and have the same values, return true
        return true;
	}
	
	public static bool operator !=(AshFile a1, AshFile a2){
		return !(a1 == a2);
	}
	
	public override bool Equals(object obj){
        if (obj is AshFile other)
            return this == other;
        else
            return false;
    }
	
	public bool Equals(AshFile? af){
		return this == af;
    }
	
	private static bool AreValuesEqual(object value1, object value2){
		// Handle null cases
		if(value1 == null && value2 == null){
			return true;
		}
	
		if(value1 == null || value2 == null){
			return false;
		}
	
		// If one or both values are arrays, handle them differently
		if(value1 is IEnumerable array1 && value1 is not string && value2 is IEnumerable array2 && value2 is not string){
			IList<object> a1 = array1.Cast<object>().ToList();
			IList<object> a2 = array2.Cast<object>().ToList();
			if(a1.Count != a2.Count)
				return false;
	
			// Compare each element of the arrays
			for(int i = 0; i < a1.Count; i++){
				if(!AreValuesEqual(a1[i], a2[i])){
					return false;
				}
			}
			return true;
		}
		
		// If the types are the same, we can compare them directly
		if(value1.GetType() == value2.GetType()){
			dynamic v1 = value1;
			dynamic v2 = value2;
			return v1 == v2;
		}
	
		// If the values are of different types, return false
		return false;
	}
}

public record AshFileFormatOptions(byte format, bool compactBools, bool maskCampNames, bool maskStrings, string password){
	public static readonly AshFileFormatOptions Default = new AshFileFormatOptions(AshFile.latestFormat, true, true, true, null);
	
	public AshFileFormatOptions(byte ver, byte compactByte)
		: this(ver, (compactByte & 1) == 1, (compactByte & 2) == 2, (compactByte & 4) == 4, (compactByte & 8) == 8 ? "def" : null){
		
	}
	
	public byte ToByte(){
		byte b = 0;
		if(compactBools){
			b |= 1;
		}
		
		if(maskCampNames){
			b |= 2;
		}
		
		if(maskStrings){
			b |= 4;
		}
		
		if(password != null){
			b |= 8;
		}
		
		return b;
	}
}

[Serializable]
internal class AshFileException : Exception{
	public int errorCode {get; private set;}
	
	internal AshFileException()
    {
    }
	
	internal AshFileException(int e)
    {
		this.errorCode = e;
    }
	
    internal AshFileException(string message) : base(message)
    {
    }
	
	internal AshFileException(string message, int e) : base(message)
    {
		this.errorCode = e;
    }

    internal AshFileException(string message, Exception innerException) : base(message, innerException)
    {
    }
	
	internal AshFileException(string message, int e, Exception innerException) : base(message, innerException)
    {
		this.errorCode = e;
    }
	
	internal protected AshFileException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        if (info != null)
        {
            errorCode = info.GetInt32(nameof(errorCode));
        }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        if (info != null)
        {
            info.AddValue(nameof(errorCode), errorCode);
        }
    }
}