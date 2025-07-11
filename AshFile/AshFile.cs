﻿using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AshLib.AshFiles;

public partial class AshFile : IDictionary<string, object>, ICloneable, IEquatable<AshFile>
{
	Dictionary<string, object> data; //internal dict
	public string? path;
	public byte format;
	
	public bool compactBools = true;
	public bool maskCampNames = true;
	public bool maskStrings = true;
	
	public AshFileFormatConfig FormatConfig{get{
		return new AshFileFormatConfig(compactBools, maskCampNames, maskStrings);
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
	
	public const int currentVersion = 3;
	
	private static string conversionErrorLog;
	private static ulong conversionErrorCount;
	
	//Constructors
	
	public AshFile(){
		this.data = new Dictionary<string, object>();
		this.format = currentVersion;
	}
	
	public AshFile(AshFile a) : this(){
		if(a.path != null){
			this.path = new string(a.path);
		}
		
		if(a.format != null){
			this.format = a.format;
		}
		
		this.ImportFormatConfig(a.FormatConfig);
		
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
	
	public AshFile(IDictionary<string, object> d){
		this.data = new Dictionary<string, object>(d);
		this.format = currentVersion;
	}
	
	public AshFile(string path){
		this.path = path;
		if(!File.Exists(path)){
			this.data = new Dictionary<string, object>();
			this.format = currentVersion;
		}else{
			this.data = ReadFromFile(path, out byte fo, out AshFileFormatConfig conf);
			this.format = fo;
			this.ImportFormatConfig(conf);
		}
	}
	
	//Other stuff
	
	public void ImportFormatConfig(AshFileFormatConfig conf){
		this.compactBools = conf.compactBools;
		this.maskCampNames = conf.maskCampNames;
		this.maskStrings = conf.maskStrings;
	}
	
	public IEnumerator<KeyValuePair<string, object>> GetEnumerator(){
		return data.GetEnumerator();
	}
	
	//IEnumerable method
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
		return GetEnumerator();
	}
	
	//================
	
	public void Load(string path){
		this.path = path;
		data = ReadFromFile(path, out byte f, out AshFileFormatConfig conf);
		this.format = f;
		this.ImportFormatConfig(conf);
	}
	
	public void Load(){
		if(path == null){
			throw new AshFileException("Path has not been initialized", 1);
		}
		data = ReadFromFile(path, out byte f, out AshFileFormatConfig conf);
		this.format = f;
		this.ImportFormatConfig(conf);
	}
	
	public void Save(string path){
		this.path = path;
		WriteToFile(path, data, this.format, this.FormatConfig);
	}
	
	public void Save(){
		if(path == null){
			throw new AshFileException("Path has not been initialized", 1);
		}
		WriteToFile(path, data, this.format, this.FormatConfig);
	}
	

	
	//^^THINGS THE USER WILL USE^^ vvTHINGS THE USER COULD BUT WONT USEvv
	
	public static Dictionary<string, object> ReadFromFile(string path, out byte f, out AshFileFormatConfig conf){
		if(!File.Exists(path)){
			throw new AshFileException("File in the path of \"" + path + "\" can't be found", 2);
		}
		byte[] fileBytes = File.ReadAllBytes(path);
		Dictionary<string, object> d = ReadFromBytes(fileBytes, out byte fo, out AshFileFormatConfig co);
		f = fo;
		conf = co;
		return d;
	}
	
	public static Dictionary<string, object> ReadFromBytes(byte[] fileBytes, out byte f, out AshFileFormatConfig conf){
		if(fileBytes.Length == 0){
			f = currentVersion;
			conf = AshFileFormatConfig.Default;
			return new Dictionary<string, object>();
		}
		f = fileBytes[0];
		try{
			switch(fileBytes[0]){
				case 1:
					conf = AshFileFormatConfig.Default;
					return V1.Read(fileBytes);
				case 2:
					conf = AshFileFormatConfig.Default;
					return V2.Read(fileBytes);
				case 3:
					return V3.Read(fileBytes, out conf);
				default:
					throw new AshFileException("Invalid or unknown file version", 3);
			}
		}catch(Exception e){
			AshFile.HandleException(e, "####An error occurred while reading!####");
			conf = AshFileFormatConfig.Default;
			return new Dictionary<string, object>();
		}
	}
	
	public static AshFile ReadFromBytes(byte[] fileBytes){
		Dictionary<string, object> d = ReadFromBytes(fileBytes, out byte f, out AshFileFormatConfig conf);
		
		AshFile a = new AshFile(d);
		a.format = f;
		a.ImportFormatConfig(conf);
		
		return a;
	}
	
	public static void WriteToFile(string path, Dictionary<string, object> dictionary, byte format, AshFileFormatConfig conf){
		try{
			File.WriteAllBytes(path, WriteToBytes(dictionary, format, conf));
		} catch(Exception e){
			throw new AshFileException("Exception occured when writing to file in path \"" + path + "\"", 5, e);
		}
	}
	
	public static byte[] WriteToBytes(Dictionary<string, object> dictionary, byte format, AshFileFormatConfig conf){
		switch(format){
			case 1:
				return V1.Write(dictionary);
			case 2:
				return V2.Write(dictionary);
			case 3:
				return V3.Write(dictionary, conf);
			default:
				throw new AshFileException("Invalid or unknown file version", 3);
		}
	}
	
	public byte[] WriteToBytes(){
		return WriteToBytes(this.data, this.format, this.FormatConfig);
	}
	
	//ERROR HANDLING
	
	private static void HandleException(Exception e, string message){
		if(e is AshFileException a){
			conversionErrorCount++;
			conversionErrorLog += message + "\n";
			conversionErrorLog += a.GetFullMessage();
		}else{
			conversionErrorCount++;
			conversionErrorLog += message + "\n";
			conversionErrorLog += "Message: " + e.Message + "\n";
			conversionErrorLog += "Source: " + e.Source + "\n";
			conversionErrorLog += "Stack Trace: " + e.StackTrace + "\n";
			conversionErrorLog += "Target Site: " + e.TargetSite + "\n";
		}
	}
	
	public static ulong GetErrorCount(){
		return AshFile.conversionErrorCount;
	}
	
	public static string GetErrorLog(){
		return AshFile.conversionErrorLog;
	}
	
	public static void EmptyErrors(){
		AshFile.conversionErrorCount = 0;
		AshFile.conversionErrorLog = "";
	}
	
	//OPERATOR THINGS
	
	//Iclonable
	public object Clone(){
		return Clone(this);
	}
	
	public static AshFile Clone(AshFile a){
		AshFile o = new AshFile();
		
		if(a.path != null){
			o.path = new string(a.path);
		}
		
		if(a.format != null){
			o.format = a.format;
		}
		
		o.ImportFormatConfig(a.FormatConfig);
		
		if(a.data != null){
			foreach(KeyValuePair<string, object> kvp in a.data){
				if (kvp.Value is ICloneable cloneable){
					o.data.Add(new string(kvp.Key), cloneable.Clone());
				}else{
					o.data.Add(new string(kvp.Key), kvp.Value);
				}
			}
		}
		
		return o;
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

public struct AshFileFormatConfig{
	public bool compactBools;
	public bool maskCampNames;
	public bool maskStrings;
	
	public static readonly AshFileFormatConfig Default = new AshFileFormatConfig(true, true, true);
	
	public AshFileFormatConfig(bool compactBoo, bool maskCampNam, bool maskStr){
		compactBools = compactBoo;
		maskCampNames = maskCampNam;
		maskStrings = maskStr;
	}
	
	public AshFileFormatConfig(byte compactByte){
		if((compactByte & 1) == 1){
			compactBools = true;
		}else{
			compactBools = false;
		}
		
		if((compactByte & 2) == 2){
			maskCampNames = true;
		}else{
			maskCampNames = false;
		}
		
		if((compactByte & 4) == 4){
			maskStrings = true;
		}else{
			maskStrings = false;
		}
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

    public override string ToString()
    {
        return GetFullMessage();
    }
	
	public string GetFullMessage(){
		string f = "";
        f += "AshFileException caught:\n";
        f += $"Error code: {this.errorCode}\n";
        f += $"Message: {this.Message}\n";
        f += $"Source: {this.Source}\n";
        f += $"Stack Trace: {this.StackTrace}\n";
        f += $"Target Site: {this.TargetSite}\n";

        if (this.Data.Count > 0)
        {
           f += "Additional Data:\n";
            foreach (var key in this.Data.Keys)
            {
                f += $"  {key}: {this.Data[key]}\n";
            }
        }

        if (this.InnerException != null)
        {
            f += "Inner Exception:\n";
            if(this.InnerException is AshFileException x){ // Recursively log inner exceptions
				f += x.GetFullMessage();
			} else{
				f += $"Message: {this.InnerException.Message}\n";
				f += $"Source: {this.InnerException.Source}\n";
				f += $"Stack Trace: {this.InnerException.StackTrace}\n";
				f += $"Target Site: {this.InnerException.TargetSite}\n";
			}
        }
		return f;
	}
}