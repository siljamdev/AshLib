using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AshLib.AshFiles;

public partial class AshFile
{
	public Dictionary<string, CampValue> data {get; private set;}
	public string? path;
	public byte format;
	
	private const int currentVersion = 2;
	
	private static string conversionErrorLog;
	private static ulong conversionErrorCount;
	
	//Constructors
	
	public AshFile(Dictionary<string, CampValue> d){
		this.data = d;
		this.format = currentVersion;
	}
	
	public AshFile(string path){
		this.path = path;
		if(!File.Exists(path)){
			FileStream f = File.Create(path);
			f.Close();
		}
		data = ReadFromFile(path, out byte fo);
		this.format = fo;
	}
	
	public AshFile(){
		this.data = new Dictionary<string, CampValue>();
		this.format = currentVersion;
	}
	
	//Other stuff
	
	public string AsString(){
		string s = "";
		foreach (KeyValuePair<string, CampValue> kvp in this.data){
			s += kvp.Key + ": " + kvp.Value + "\n";
		}
		return s;
	}
	
	//================
	
	public void Load(string path){
		this.path = path;
		data = ReadFromFile(path, out byte f);
		this.format = f;
	}
	
	public void Load(){
		if(path == null){
			throw new AshFileException("Path has not been initialized", 1);
		}
		data = ReadFromFile(path, out byte f);
		this.format = f;
	}
	
	public void Save(string path){
		this.path = path;
		WriteToFile(path, data, this.format);
	}
	
	public void Save(){
		if(path == null){
			throw new AshFileException("Path has not been initialized", 1);
		}
		WriteToFile(path, data, this.format);
	}
	

	
	//^^THINGS THE USER WILL USE^^ vvTHINGS THE USER COULD BUT WONT USEvv
	
	public static Dictionary<string, CampValue> ReadFromFile(string path, out byte f){
		if(!File.Exists(path)){
			throw new AshFileException("File in the path of \"" + path + "\" can't be found", 2);
		}
		byte[] fileBytes = File.ReadAllBytes(path);
		Dictionary<string, CampValue> d = ReadFromBytes(fileBytes, out byte fo);
		f = fo;
		return d;
	}
	
	public static Dictionary<string, CampValue> ReadFromBytes(byte[] fileBytes, out byte f){
		if (fileBytes.Length == 0){
			f = currentVersion;
			return new Dictionary<string, CampValue>();
		}
		f = fileBytes[0];
		switch(fileBytes[0]){
			case 1:
				return V1.Read(fileBytes);
			case 2:
				return V2.Read(fileBytes);
			default:
				throw new AshFileException("Invalid or unknown file version", 3);
		}
	}
	
	public static void WriteToFile(string path, Dictionary<string, CampValue> dictionary, byte format){
		try{
			File.WriteAllBytes(path, WriteToBytes(dictionary, format));
		} catch(Exception e){
			throw new AshFileException("Exception occured when writing to file in path \"" + path + "\"", 5, e);
		}
	}
	
	public static byte[] WriteToBytes(Dictionary<string, CampValue> dictionary, byte format){
		switch(format){
			case 1:
				return V1.Write(dictionary);
			case 2:
				return V2.Write(dictionary);
			default:
				throw new AshFileException("Invalid or unknown file version", 3);
		}
	}
	
	public byte[] WriteToBytes(){
		return WriteToBytes(this.data, this.format);
	}
	
	//ERROR HANDLING
	
	private static void HandleException(Exception e, string message){
		if(e is AshFileException a){
			conversionErrorCount++;
			conversionErrorLog += message + "\n";
			conversionErrorLog += a.GetFullMessage();
		} else{
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
	
	public static AshFile DeepCopy(AshFile a){
		AshFile o = new AshFile();
		
		if(a.path != null){
			o.path = new string(a.path);
		}
		
		if(a.format != null){
			o.format = a.format;
		}
		
		if(a.data != null){
			o.data = new Dictionary<string, CampValue>();
			foreach (KeyValuePair<string, CampValue> kvp in a.data){
				dynamic v = kvp.Value.GetValue();
				o.data.Add(new string(kvp.Key), new CampValue(v));
			}
		}
		
		return o;
	}
	
	public static explicit operator Dictionary<string, CampValue>(AshFile af){
		return af.data;
	}
	
	public static implicit operator AshFile(Dictionary<string, CampValue> d){
		return new AshFile(d);
	}
	
	public static AshFile operator +(AshFile a1, AshFile a2){
		AshFile o = DeepCopy(a1);
		
		foreach (KeyValuePair<string, CampValue> kvp in a2.data)
        {
            if (o.data.TryGetValue(kvp.Key, out CampValue v1) && !(v1.Equals(kvp.Value))){
				o.data[kvp.Key] = kvp.Value;
			} else if (!o.data.TryGetValue(kvp.Key, out CampValue v)){
				o.data.Add(kvp.Key, kvp.Value);
			}
        }
		
		return o;
	}
	
	public static bool operator ==(AshFile a1, AshFile a2){
		if(ReferenceEquals(a1, a2)) return true;
		if(a1 is null && a2 is null) return true;
		if(a1 is null || a2 is null) return false;
		
		if(a1.data.Count != a2.data.Count)return false;

        // Check each key-value pair in dict1
        foreach (KeyValuePair<string, CampValue> kvp in a1.data){
            // If the key is not present in dict2, or the corresponding value is not equal, return false
            if (!a2.data.TryGetValue(kvp.Key, out CampValue val) || !val.Equals(kvp.Value))
                return false;
        }

        // If all key-value pairs in dict1 are present in dict2 and have the same values, return true
        return true;
	}
	
	public static bool operator !=(AshFile a1, AshFile a2){
		return !(a1 == a2);
	}
	
	public override bool Equals(object obj)
    {
        if (obj is AshFile other)
            return this == other;
        else
            return false;
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