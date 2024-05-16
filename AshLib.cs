using System;
using System.Diagnostics;

namespace AshLib;

public class AshFile
{
	public Dictionary<string, object> data;
	public string path;
	
	public AshFile(Dictionary<string, object> a){
		this.data = a;
	}
	
	public AshFile(string p){
		path = p;
		if(!File.Exists(path)){
			FileStream f = File.Create(path);
			f.Close();
		}
		data = ReadFile(path);
	}
	
	public AshFile(){
		this.data = new Dictionary<string, object>();
	}
	
	public Dictionary<string, object> getAsDictionary(){
		return this.data;
	}
	
	public void SetCamp(string index, object val){
		this.data[index] = val;
	}
	
	public void InitializeCamp(string index, object val){
		if(this.data.ContainsKey(index)){
			return;
		}
		this.data[index] = val;
	}
	
	public object GetCamp(string index){
		return this.data[index];
	}
	
	public string AsString(){
		string s = "";
		foreach (KeyValuePair<string, object> kvp in this.data){
			s += kvp.Key + ": " + kvp.Value + "\n";
		}
		return s;
	}
	
	public void Load(string p){
		path = p;
		data = ReadFile(path);
	}
	
	public void Load(){
		if(path == null){
			throw new AshFileException("Path has not been initialized");
		}
		data = ReadFile(path);
	}
	
	public void Save(string p){
		path = p;
		WriteFile(path, data);
	}
	
	public void Save(){
		if(path == null){
			throw new AshFileException("Path has not been initialized");
		}
		WriteFile(path, data);
	}
	
	public static Dictionary<string, object> ReadFile(string path){
		if(!File.Exists(path)){
			throw new AshFileException("File in the path of \"" + path + "\" can't be found");
		}
		byte[] fileBytes = File.ReadAllBytes(path);
		return Read(fileBytes);
	}
	
	public static Dictionary<string, object> Read(byte[] fileBytes){
		if (fileBytes.Length == 0){
			return new Dictionary<string, object>();
		}
		switch(fileBytes[0]){
			case 1:
				return ReadV1(fileBytes);
			default:
				throw new AshFileException("Invalid or unknown file version");
		}
	}
	
	public static void WriteFile(string path, Dictionary<string, object> dictionary){
		File.WriteAllBytes(path, Write(dictionary));
	}
	
	public static byte[] Write(Dictionary<string, object> dictionary){
		return WriteV1(dictionary);
	}
	
	private static Dictionary<string, object> ReadV1(byte[] fileBytes){
		long byteIndex = 1; //will be used as pointer of the reading byte. Starts at 1 because 0 is version and has already been read
		
		long fileLength = GetLength(fileBytes, ref byteIndex);
		long headerLength = byteIndex;
		
		if(fileBytes.Length < fileLength + headerLength){
			throw new AshFileException("Byte array length smaller than that specified in file");
		}
		
		Dictionary<string, object> output = new Dictionary<string, object>(); //Will be the final product
		
		//We can start with camps
		
		long campNameLength;
		string campName;
		long typeOfValue;
		long valueLength;
		object val;
		
		while(true){ //each iteration will be a camp
			if(byteIndex > fileLength + headerLength - 1){
				break;
			}
			
			campNameLength = GetLength(fileBytes, ref byteIndex); //get the length of the name
			
			campName = "";
			for(int i = 0; i < campNameLength; i++){
				campName = campName + (char) fileBytes[byteIndex + i]; //get the name
			}
			byteIndex = byteIndex + campNameLength;
			
			typeOfValue = fileBytes[byteIndex]; //get type of value
			byteIndex++;
			
			switch (typeOfValue){ //act accordingly by type of value
				case 1:
				valueLength = GetLength(fileBytes, ref byteIndex);
				
				val = "";
				for(int i = 0; i < valueLength; i++){
					val = (string) val + (char) fileBytes[byteIndex + i];
				}
				byteIndex = byteIndex + valueLength;
				break;
				
				case 2:
				valueLength = GetLength(fileBytes, ref byteIndex);
				
				val = (ulong) 0;
				for(int i = 0; i < valueLength; i++){
					val = (ulong) val + fileBytes[byteIndex + i] * (ulong) Math.Pow(256, valueLength - 1 - i);
				}
				byteIndex = byteIndex + valueLength;
				break;
				
				case 3:
				val = false;
				if(fileBytes[byteIndex] == 1){
					val = true;
				}
				byteIndex++;
				break;
				
				default:
				throw new AshFileException("Invalid or unknown data type in file");
			}
			output.Add(campName, val);
		}
		
		return output;
	}
	
	private static byte[] WriteV1(Dictionary<string, object> dictionary){
		List<byte> output = new List<byte>(); //Final output
		
		List<KeyValuePair<string, object>> dictionaryList = dictionary.ToList(); //Transform it to a list so it can be easily iterated through
		
		char[] name;
		object val;
		
		for(int i = 0; i < dictionaryList.Count(); i++){ //For each of the dictionary elements, a camp will be written
			name = dictionaryList[i].Key.ToCharArray();
			val = dictionaryList[i].Value;
			
			WriteLength(name.Length, ref output);
			
			for(int j = 0; j < name.Length; j++){
				output.Add((byte)name[j]);
			}
			
			if(val is string s){
				output.Add(1);
				
				char[] valueString = s.ToCharArray();
				
				WriteLength(valueString.Length, ref output);
				
				for(int j = 0; j < valueString.Length; j++){
					output.Add((byte) valueString[j]);
				}
			} else if(val is ulong n){
				output.Add(2);
				
				int j = 0;
				ulong num = n;
				List<byte> number = new List<byte>();
				
				while(true){
					number.Add((byte) ((num % (ulong) Math.Pow(256, j + 1)) / (ulong) Math.Pow(256, j)));
					num = num - num % (ulong) Math.Pow(256, j + 1);
					if(num == 0){
						break;
					}
					j++;
				}
				
				WriteLength(number.Count(), ref output);
				
				for(int k = 0; k < number.Count(); k++){
					output.Add(number[number.Count() - k - 1]);
				}
			} else if(val is bool b){
				output.Add(3);
				
				if(b){
					output.Add(1);
				} else {
					output.Add(0);
				}
			}
		}
		
		List<byte> o = new List<byte>(); //Additional list for putting the length before the final conversion
		o.Add(1);
		WriteLength(output.Count(), ref o);
		
		o.AddRange(output);
		
		return o.ToArray();
	}
	
	private static long GetLength(byte[] fileBytes, ref long start){
		if(fileBytes[start] == 0){
			List<byte> number = new List<byte>();
			long num = fileBytes[start + 1];
			for(int i = 0; i < num; i++){
				number.Add(fileBytes[start + 2 + i]);
			}
			byte[] length = number.ToArray();
			long result = 0;
			for(int i = 0; i < length.Length; i++){
				result += length[i] * (long) Math.Pow(256, length.Length - 1 - i);
			}
			start = start + 2 + num;
			return result;
		}
		start = start + 1;
		return (long) fileBytes[start - 1];
	}
	
	private static void WriteLength(long len, ref List<byte> dic){
		if(len > 255){
			dic.Add(0);
			
			int i = 0;
			List<byte> number = new List<byte>();
			
			while(true){
				number.Add((byte) ((len % (long) Math.Pow(256, i + 1)) / (long) Math.Pow(256, i)));
				len = len - len % (long) Math.Pow(256, i + 1);
				if(len == 0){
					break;
				}
				i++;
			}
			
			dic.Add((byte) number.Count());
			
			for(int j = 0; j < number.Count(); j++){
				dic.Add(number[number.Count() - j - 1]);
			}
			return;
		}
		dic.Add((byte) len);
	}
	
	public static AshFile DeepCopy(AshFile a){
		AshFile o = new AshFile();
		
		if(a.path != null){
			o.path = new string(a.path);
		}
		
		if(a.data != null){
			o.data = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> kvp in a.data){
				o.data.Add(new string(kvp.Key), kvp.Value);
			}
		}
		
		return o;
	}
	
	public static explicit operator Dictionary<string, object>(AshFile af){
		return af.data;
	}
	
	public static implicit operator AshFile(Dictionary<string, object> d){
		return new AshFile(d);
	}
	
	public static AshFile operator +(AshFile a1, AshFile a2){
		AshFile o = DeepCopy(a1);
		
		foreach (KeyValuePair<string, object> kvp in a2.data)
        {
            if (o.data.TryGetValue(kvp.Key, out object v1) && !(v1.Equals(kvp.Value))){
				o.data[kvp.Key] = kvp.Value;
			} else if (!o.data.TryGetValue(kvp.Key, out object v)){
				o.data.Add(kvp.Key, kvp.Value);
			}
        }
		
		return o;
	}
	
	public static bool operator ==(AshFile a1, AshFile a2){
		if (a1.data.Count != a2.data.Count)
            return false;

        // Check each key-value pair in dict1
        foreach (KeyValuePair<string, object> kvp in a1.data)
        {
            // If the key is not present in dict2, or the corresponding value is not equal, return false
            if (!a2.data.TryGetValue(kvp.Key, out object value) || !value.Equals(kvp.Value))
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

internal class AshFileException : Exception{
	public AshFileException()
        {
        }

        public AshFileException(string message)
            : base(message)
        {
        }

        public AshFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
}

public class Dependencies{ //Used for handling files in a central folder (kindof the .minecraft folder)
	public string path;
	public AshFile config;
	
	public Dependencies(string path, bool config, string[] files, string[] directories){
		this.path = path;
		checkDir(this.path);
		if(config){
			this.config = new AshFile(this.path + "/config.ash");
		}
		
		for(int i = 0; i < files.Length; i++){
			this.checkFile(files[i]);
		}
		
		for(int i = 0; i < directories.Length; i++){
			this.checkDir(directories[i]);
		}
	}
	
	private void checkDir(string p){
		if(!Directory.Exists(p)){
			Directory.CreateDirectory(p);
		}
	}
	
	private void checkFile(string p){
		if(!File.Exists(p)){
			FileStream fs = File.Create(p);
			fs.Close();
		}
	}
	
	public string ReadFile(string p){
		return File.ReadAllText(this.path + p);
	}
	
	public AshFile ReadAshFile(string p){
		return (AshFile) AshFile.ReadFile(this.path + p);
	}
	
	public void SaveFile(string p, string t){
		File.WriteAllText(this.path + p, t);
	}
	
	public void SaveAshFile(string p, AshFile a){
		AshFile.WriteFile(this.path + p, a.getAsDictionary());
	}
	
	public void CreateDir(string p){
		if(!Directory.Exists(this.path + p)){
			Directory.CreateDirectory(this.path + p);
		}
	}
}

public class DeltaHelper{
	private Stopwatch timer;
	private double lastTime;
	private double lastStable;
	private double stableTime = 1000d; //Default is 1 second
	
	public double deltaTime;
	public double fps;
	public double stableFps;
	
	public DeltaHelper(){}
	
	public void Start(){ //Call to start the thing
		timer = Stopwatch.StartNew();
		lastTime = timer.Elapsed.TotalMilliseconds;
		lastStable = 0d;
		this.Frame();
	}
	
	public void Frame(){ //Call it each frame
		double currentTime = timer.Elapsed.TotalMilliseconds;
		deltaTime = (currentTime - lastTime)/1000d;
		lastTime = currentTime;
		fps = 1d/deltaTime;
		
		if(currentTime > lastStable + stableTime){
			lastStable = currentTime;
			stableFps = fps;
		}
	}
	
	public void Target(float FPS){ //Call with argument at the end of each frame to achieve fps target
		double wantedDeltaTime = 1000d/(double)FPS; //In ms
		double realDeltaTime = (timer.Elapsed.TotalMilliseconds - lastTime)/1000d;
		double extraTime = wantedDeltaTime - realDeltaTime;
		System.Threading.Thread.Sleep((int)extraTime);
	}
	
	public void SetStableTime(float s){
		this.stableTime = s;
	}
	
	public float getTime(){ //Get total time in seconds
		return (float)(timer.Elapsed.TotalMilliseconds/1000d);
	}
}
