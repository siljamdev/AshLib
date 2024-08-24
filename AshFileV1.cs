using System;

namespace AshLib;

public partial class AshFile{
	protected internal class V1{
		public static Dictionary<string, CampValue> Read(byte[] fileBytes){
			long byteIndex = 1; //will be used as pointer of the reading byte. Starts at 1 because 0 is version and has already been read
			
			long fileLength = GetLength(fileBytes, ref byteIndex);
			long headerLength = byteIndex;
			
			if(fileBytes.Length < fileLength + headerLength){
				throw new AshFileException("Byte array length smaller than that specified in file");
			}
			
			Dictionary<string, CampValue> output = new Dictionary<string, CampValue>(); //Will be the final product
			
			//We can start with camps
			
			long campNameLength;
			string campName;
			long typeOfValue;
			long valueLength;
			CampValue val;
			
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
					
					string t1 = "";
					for(int i = 0; i < valueLength; i++){
						t1 = t1 + (char) fileBytes[byteIndex + i];
					}
					val = new CampValue(t1);
					byteIndex = byteIndex + valueLength;
					break;
					
					case 2:
					valueLength = GetLength(fileBytes, ref byteIndex);
					
					ulong t2 = 0;
					for(int i = 0; i < valueLength; i++){
						t2 = t2 + fileBytes[byteIndex + i] * (ulong) Math.Pow(256, valueLength - 1 - i);
					}
					val = new CampValue(t2);
					byteIndex = byteIndex + valueLength;
					break;
					
					case 3:
					bool t3 = false;
					if(fileBytes[byteIndex] == 1){
						t3 = true;
					}
					val = new CampValue(t3);
					byteIndex++;
					break;
					
					default:
					throw new AshFileException("Invalid or unknown data type in file");
				}
				output.Add(campName, val);
			}
			
			return output;
		}
		
		public static byte[] Write(Dictionary<string, CampValue> dictionary){
			List<byte> output = new List<byte>(); //Final output
			
			List<KeyValuePair<string, CampValue>> dictionaryList = dictionary.ToList(); //Transform it to a list so it can be easily iterated through
			
			char[] name;
			CampValue val;
			
			Type t;
			
			for(int i = 0; i < dictionaryList.Count(); i++){ //For each of the dictionary elements, a camp will be written
				name = dictionaryList[i].Key.ToCharArray();
				val = dictionaryList[i].Value;
				
				t = val.type;
				
				WriteLength(name.Length, ref output);
				
				for(int j = 0; j < name.Length; j++){
					output.Add((byte)name[j]);
				}
				
				if(t == Type.String){
					output.Add(1);
					
					string s = (string) val.GetValue();
					
					char[] valueString = s.ToCharArray();
					
					WriteLength(valueString.Length, ref output);
					
					for(int j = 0; j < valueString.Length; j++){
						output.Add((byte) valueString[j]);
					}
				} else if(t == Type.Ulong){
					output.Add(2);
					
					ulong n = (ulong) val.type;
					
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
				} else if(t == Type.Bool){
					output.Add(3);
					
					bool b = (bool) val.GetValue();
					
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
	}
}