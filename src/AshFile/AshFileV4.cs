using System;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using AshLib.Dates;

namespace AshLib.AshFiles;

public partial class AshFile{
	protected internal class V4{
		#region read
		//Read
		public static Dictionary<string, object> Read(byte[] fileBytes, string password, out AshFileFormatOptions conf){
			Dictionary<string, object> dic = new Dictionary<string, object>();
			
			ulong index = 5; //We start at 5 because 0 is version, and then %ASH comes
			AshFileFormatOptions config = new AshFileFormatOptions(fileBytes[0], fileBytes[index]);
			
			index++;
			
			bool usesPassword = false;
			
			if(config.password != null){
				usesPassword = true;
				if(password == null){
					throw new AshFileException("File is password protected, and no password was provided", 3);
				}
				
				try{
					fileBytes = DecryptAES(fileBytes.Skip((int) index).ToArray(), password);
				}catch(Exception e){
					throw new AshFileException("Error occured on decryption", 3, e);
				}
				
				index = 0;
			}
			
			conf = config with {password = password};
			
			if(config.compactBools){
				try{
					ulong boolNum = ReadVarNum(fileBytes, ref index); //Read the number of bools
					
					List<string> names = new List<string>((int)boolNum);
					
					for(ulong i = 0; i < boolNum; i++){
						names.Add(ReadCampName(fileBytes, ref index, config.maskCampNames && !usesPassword));
					}
					
					byte[] y = new byte[(boolNum + 8 - 1) / 8];
					Array.Copy(fileBytes, (int) index, y, 0, y.Length);
					index += (ulong) y.Length;
					bool[] bools = UnCompactBoolArray(y, (int) boolNum);
					
					for(int i = 0; i < (int) boolNum; i++){
						dic.Add(names[i], bools[i]);
					}
				}catch(Exception e){
					throw new AshFileException("Error occured while reading V4", 3, e);
				}
			}
			ulong campNum = ReadVarNum(fileBytes, ref index); //Read the number of camps
			
			for(ulong i = 0; i < campNum; i++){
				try{
					string campName = ReadCampName(fileBytes, ref index, config.maskCampNames && !usesPassword);
					
					object campValue = ReadCampValue(fileBytes, ref index, config.maskStrings && !usesPassword, config.compactBools);
					
					if(dic.ContainsKey(campName)){
						throw new AshFileException("The dictionary already has a camp named " + campName, 3);
					}
					dic.Add(campName, campValue);
				} catch(Exception e){
					throw new AshFileException("Error occured while reading V4", 3, e);
				}
			}
			
			return dic;
		}
		
		private static object ReadCampValue(byte[] fileBytes, ref ulong index, bool mask, bool compactBools){
			byte t = fileBytes[index];
			index++;
			return ReadValueType(fileBytes, ref index, t, mask, compactBools);
		}
		
		private static object ReadValueType(byte[] fileBytes, ref ulong index, byte t, bool mask, bool compactBools){
			if(t > 127){
				t &= 127;
				
				Type y = AshFile.GetTypeFromEnum((AshFileType) t);
				if(y == typeof(object)){
					return null;
				}
				
				ulong length = ReadVarNum(fileBytes, ref index);
				
				if((AshFileType) t == AshFileType.Bool && compactBools){
					byte[] by = new byte[(length + 8 - 1) / 8];
					Array.Copy(fileBytes, (int) index, by, 0, (int) by.Length);
					index += (ulong) by.Length;
					bool[] bools = UnCompactBoolArray(by, (int) length);
					return bools;
				}
				
				Array array = Array.CreateInstance(y, (int) length);
				for(int i = 0; i < (int) length; i++){
					dynamic d = ReadValueType(fileBytes, ref index, t, mask, compactBools);
					array.SetValue(d, i);
				}
				
				dynamic a = array;
				return a;
			}
			
			switch((AshFileType) t){
				default:
				case AshFileType.Default:
					ulong length = ReadVarNum(fileBytes, ref index);
					
					byte[] b = new byte[length];
					Buffer.BlockCopy(fileBytes, (int) index, b, 0, (int) length);
					index += length;
					return b;
				case AshFileType.String:
					length = ReadVarNum(fileBytes, ref index);
					
					byte[] vv = new byte[(int) length];
					Array.Copy(fileBytes, (int) index, vv, 0, (int) length);
					
					if(mask){
						vv = Unscramble(vv);
					}
					
					string s = Encoding.UTF8.GetString(vv);
					index += length;
					return s;
				case AshFileType.Byte:
					byte b2 = fileBytes[index];
					index++;
					return b2;
				case AshFileType.Ushort:
					ulong n2 = index;
					b = EnsureEndianess(fileBytes, ref n2, 2);
					ushort us = BitConverter.ToUInt16(b, (int) n2);
					index += 2;
					return us;
				case AshFileType.Uint:
					n2 = index;
					b = EnsureEndianess(fileBytes, ref n2, 4);
					uint ui = BitConverter.ToUInt32(b, (int) n2);
					index += 4;
					return ui;
				case AshFileType.Ulong:
					n2 = index;
					b = EnsureEndianess(fileBytes, ref n2, 8);
					ulong ul = BitConverter.ToUInt64(b, (int) n2);
					index += 8;
					return ul;
				case AshFileType.Sbyte:
					sbyte b3 = (sbyte) fileBytes[index];
					index++;
					return b3;
				case AshFileType.Short:
					n2 = index;
					b = EnsureEndianess(fileBytes, ref n2, 2);
					short sh = BitConverter.ToInt16(b, (int) n2);
					index += 2;
					return sh;
				case AshFileType.Int:
					n2 = index;
					b = EnsureEndianess(fileBytes, ref n2, 4);
					int ii = BitConverter.ToInt32(b, (int) n2);
					index += 4;
					return ii;
				case AshFileType.Long:
					n2 = index;
					b = EnsureEndianess(fileBytes, ref n2, 8);
					long ll = BitConverter.ToInt64(b, (int) n2);
					index += 8;
					return ll;
				case AshFileType.Color3:
					byte rrr = fileBytes[index];
					byte ggg = fileBytes[index + 1];
					byte bbb = fileBytes[index + 2];
					index += 3;
					
					Color3 c = new Color3(rrr, ggg, bbb);
					return c;
				case AshFileType.Float:
					ulong n1 = index;
					b = EnsureEndianess(fileBytes, ref n1, 4);
					float f = BitConverter.ToSingle(b, (int) n1);
					index += 4;
					return f;
				case AshFileType.Double:
					n1 = index;
					b = EnsureEndianess(fileBytes, ref n1, 8);
					double dd = BitConverter.ToDouble(b, (int) n1);
					index += 8;
					return dd;
				case AshFileType.Vec2:
					float x = ReadFloating4(fileBytes, ref index);
					float y = ReadFloating4(fileBytes, ref index);
					
					Vec2 v2 = new Vec2(x, y);
					return v2;
				case AshFileType.Vec3:
					x = ReadFloating4(fileBytes, ref index);
					y = ReadFloating4(fileBytes, ref index);
					float z = ReadFloating4(fileBytes, ref index);
					
					Vec3 v3 = new Vec3(x, y, z);
					return v3;
				case AshFileType.Vec4:
					x = ReadFloating4(fileBytes, ref index);
					y = ReadFloating4(fileBytes, ref index);
					z = ReadFloating4(fileBytes, ref index);
					float w = ReadFloating4(fileBytes, ref index);
					
					Vec4 v4 = new Vec4(x, y, z, w);
					return v4;
				case AshFileType.Bool:
					byte bb3 = fileBytes[index];
					index++;
					bool n = (bb3 % 2 == 1 ? true : false); //0 will be false, 1 will be true, 2 will be false...
					return n;
				case AshFileType.Date:
					byte[] d = new byte[8];
					d[0] = fileBytes[index];
					d[1] = fileBytes[index + 1];
					d[2] = fileBytes[index + 2];
					d[3] = fileBytes[index + 3];
					d[4] = fileBytes[index + 4];
					index += 5;
					
					ulong n4 = 0;
					byte[] b4 = EnsureEndianess(d, ref n4, 8);
					ulong combined = BitConverter.ToUInt64(b4, 0);
					
					byte seconds = (byte)(combined & 0x3F);
					byte minutes = (byte)((combined >> 6) & 0x3F);
					byte hours = (byte)((combined >> 12) & 0x1F);
					byte days = (byte)((combined >> 17) & 0x1F);
					byte months = (byte)((combined >> 22) & 0x0F);
					ushort years = (ushort)((combined >> 26) & 0x03FF);
					
					return new Date(seconds, minutes, hours, days, months, (ushort)(years + 1488));
				case AshFileType.Decimal:					
					int[] its = new int[4];
					
					n1 = index;
					b = EnsureEndianess(fileBytes, ref n1, 4);
					its[0] = BitConverter.ToInt32(b, (int) n1);
					n1 = index + 4;
					b = EnsureEndianess(fileBytes, ref n1, 4);
					its[1] = BitConverter.ToInt32(b, (int) n1);
					n1 = index + 8;
					b = EnsureEndianess(fileBytes, ref n1, 4);
					its[2] = BitConverter.ToInt32(b, (int) n1);
					n1 = index + 12;
					b = EnsureEndianess(fileBytes, ref n1, 4);
					its[3] = BitConverter.ToInt32(b, (int) n1);
					
					decimal dec = new decimal(its);
					
					index += 16;
					
					return dec;
			}
		}
		
		private static float ReadFloating4(byte[] fileBytes, ref ulong index){			
			ulong n1 = index;
			byte[] b1 = EnsureEndianess(fileBytes, ref n1, 4);
			float f = BitConverter.ToSingle(b1, (int) n1);
			index += 4;
			return f;
		}
		
		private static string ReadCampName(byte[] fileBytes, ref ulong index, bool mask){
			ulong length = ReadVarNum(fileBytes, ref index);
			
			byte[] b = new byte[(int) length];
			
			Array.Copy(fileBytes, (int) index, b, 0, (int) length);
			
			if(mask){
				b = Unscramble(b);
			}
			
			string s = Encoding.UTF8.GetString(b);
			index += length;
			return s;
		}
		
		private static ulong ReadVarNum(byte[] fileBytes, ref ulong index){
			switch(fileBytes[index]){
				case 253:
				index++;
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, 2);
				ushort us = BitConverter.ToUInt16(b2, (int) n2);
				index += 2;
				return (ulong) us;
				
				case 254:
				index++;
				ulong n3 = index;
				byte[] b3 = EnsureEndianess(fileBytes, ref n3, 4);
				uint ui = BitConverter.ToUInt32(b3, (int) n3);
				index += 4;
				return (ulong) ui;
				
				case 255:
				index++;
				ulong n4 = index;
				byte[] b4 = EnsureEndianess(fileBytes, ref n4, 8);
				ulong ul = BitConverter.ToUInt64(b4, (int) n4);
				index += 8;
				return (ulong) ul;
				
				default:
				byte b1 = fileBytes[index];
				index++;
				return (ulong) b1;
			}
		}
		
		private static byte[] EnsureEndianess(byte[] fileBytes, ref ulong ind, ulong size){
			if(BitConverter.IsLittleEndian){
				return fileBytes;
			}
			byte[] f = new byte[size];
			for(ulong i = 0; i < size; i++){
				f[i] = fileBytes[ind + size - i - 1];
			}
			ind = 0;
			return f;
		}
		#endregion read
		
		#region write
		//Write
		public static byte[] Write(IDictionary<string, object> d, AshFileFormatOptions conf){
			List<byte> bytes = new List<byte>();
			List<byte> temp = new List<byte>();
			
			ulong campNum = 0;
			
			bool usesPassword = conf.password != null;
			
			List<KeyValuePair<string, object>> dictionary = new List<KeyValuePair<string, object>>((ICollection<KeyValuePair<string, object>>)d);
			
			//Find Bools
			Dictionary<string, bool> bools = new Dictionary<string, bool>();
			if(conf.compactBools){
				try{
					for(ulong i = 0; i < (ulong) dictionary.Count; i++){
						if(dictionary[(int) i].Value is bool b){
							bools.Add(dictionary[(int) i].Key, b);
							dictionary.RemoveAt((int) i);
							i--;
						}
					}
				}catch(Exception e){
					throw new AshFileException("Error occured while writing V4", 3, e);
				}
			}
			
			//Write other camps to bytes
			for(ulong i = 0; i < (ulong) dictionary.Count; i++){
				try{
					temp.Clear();
					if(WriteCampValue(temp, dictionary[(int)i].Value, conf.maskStrings && !usesPassword, conf.compactBools)){
						campNum++;
						WriteCampName(bytes, dictionary[(int)i].Key, conf.maskCampNames && !usesPassword);
						bytes.AddRange(temp);
					}
				}catch(Exception e){
					throw new AshFileException("Error occured while writing V4", 3, e);
				}
			}
			
			temp.Clear();
			temp.Add(4);
			temp.AddRange(Encoding.UTF8.GetBytes("%ASH")); //Magic number
			temp.Add(conf.ToByte()); //Config
			
			List<byte> temp2 = new List<byte>();
			
			//Write bool dict
			if(conf.compactBools){
				WriteVarNum(temp2, (ulong) bools.Count);
				foreach(string s in bools.Keys){
					WriteCampName(temp2, s, conf.maskCampNames && !usesPassword);
				}
				
				temp2.AddRange(CompactBoolArray(bools.Values.ToList()));
			}
			
			//Merge main body
			WriteVarNum(temp2, (ulong) campNum);
			temp2.AddRange(bytes);
			
			if(conf.password != null){
				temp2 = EncryptAES(temp2.ToArray(), conf.password).ToList();
			}
			
			temp.AddRange(temp2);
			
			return temp.ToArray();
		}
		
		private static bool WriteCampValue(List<byte> bytes, object o, bool mask, bool compactBools){
			if(o is IEnumerable array && o is not string){
				Type elementType = AshFile.GetBaseTypeOfEnumerable(array);
				AshFileType t = GetFileTypeFromType(elementType);
				if(t == AshFileType.Default){
					return false;
				}
				byte b = (byte) ((byte) t | 128);
				bytes.Add(b);
				
				List<byte> tempVals = new();
				
				if(t == AshFileType.Bool && compactBools){
					IList<bool> l = ((IEnumerable<bool>) array).ToList();
					WriteVarNum(bytes, (ulong) l.Count);
					bytes.AddRange(CompactBoolArray(l));
					return true;
				}
				
				int count = 0;
				foreach(object i in array){
					if(WriteCampType(tempVals, i, mask)){
						count++;
					}
				}
				WriteVarNum(bytes, (ulong) count);
				bytes.AddRange(tempVals);
				
				return true;
			}
			
			AshFileType t1 = GetFileTypeFromType(o.GetType());
			bytes.Add((byte) t1);
			return WriteCampType(bytes, o, mask);
		}
		
		private static bool WriteCampType(List<byte> bytes, object o, bool mask){
			switch(o){
				case string s:
					byte[] b = Encoding.UTF8.GetBytes(s);
					
					if(mask){
						b = Scramble(b);
					}
					
					WriteVarNum(bytes, (ulong) b.Length);
					bytes.AddRange(b);
					break;
				case byte by:
					bytes.Add(by);
					break;
				case ushort us:
					b = BitConverter.GetBytes(us);
					byte[] e = EnsureEndianess(b, 2);
					bytes.AddRange(e);
					break;
				case uint ui:
					b = BitConverter.GetBytes(ui);
					e = EnsureEndianess(b, 4);
					bytes.AddRange(e);
					break;
				case ulong ul:
					b = BitConverter.GetBytes(ul);
					e = EnsureEndianess(b, 8);
					bytes.AddRange(e);
					break;
				case sbyte sb:
					bytes.Add((byte) sb);
					break;
				case short ss:
					b = BitConverter.GetBytes(ss);
					e = EnsureEndianess(b, 2);
					bytes.AddRange(e);
					break;
				case int ii:
					b = BitConverter.GetBytes(ii);
					e = EnsureEndianess(b, 4);
					bytes.AddRange(e);
					break;
				case long l:
					b = BitConverter.GetBytes(l);
					e = EnsureEndianess(b, 8);
					bytes.AddRange(e);
					break;
				case Color3 c:
					bytes.Add(c.R);
					bytes.Add(c.G);
					bytes.Add(c.B);
					break;
				case float f:
					b = BitConverter.GetBytes(f);
					e = EnsureEndianess(b, 4);
					bytes.AddRange(e);
					break;
				case double d:
					b = BitConverter.GetBytes(d);
					e = EnsureEndianess(b, 8);
					bytes.AddRange(e);
					break;
				case Vec2 v2:
					WriteFloat4(bytes, v2.X);
					WriteFloat4(bytes, v2.Y);
					break;
				case Vec3 v3:
					WriteFloat4(bytes, v3.X);
					WriteFloat4(bytes, v3.Y);
					WriteFloat4(bytes, v3.Z);
					break;
				case Vec4 v4:
					WriteFloat4(bytes, v4.X);
					WriteFloat4(bytes, v4.Y);
					WriteFloat4(bytes, v4.Z);
					WriteFloat4(bytes, v4.W);
					break;
				case bool oo:
					bytes.Add((oo ? (byte)1 : (byte)0));
					break;
				case Date dd:
					ulong combined = ((ulong)dd.seconds & 0x3F) | 
								(((ulong)dd.minutes & 0x3F) << 6) |
								(((ulong)dd.hours & 0x1F) << 12) |
								(((ulong)dd.days & 0x1F) << 17) |
								(((ulong)dd.months & 0x0F) << 22) |
								(((ulong)(dd.years- 1488) & 0x03FF) << 26);
								
					byte[] dbytes = BitConverter.GetBytes(combined);
					
					byte[] fbytes = new byte[5];
					for(int i = 0; i < 5; i++){
						fbytes[i] = dbytes[i];
					}
					
					bytes.AddRange(fbytes);
					break;
				case decimal dec:
					int[] its = decimal.GetBits(dec);
					foreach(int its2 in its){
						b = BitConverter.GetBytes(its2);
						e = EnsureEndianess(b, 4);
						
						bytes.AddRange(e);
					}
					break;
				default:
					return false;
			}
			return true;
		}
		
		private static void WriteFloat4(List<byte> bytes, float b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 4);
            bytes.AddRange(e);
		}
		
		private static void WriteCampName(List<byte> bytes, string name, bool mask){			
			byte[] b = Encoding.UTF8.GetBytes(name);
			
			if(mask){
				b = Scramble(b);
			}
			
			WriteVarNum(bytes, (ulong) b.Length);
			bytes.AddRange(b);
		}
		
		private static void WriteVarNum(List<byte> bytes, ulong num){
			if(num <= 252ul){
				bytes.Add((byte) num);
			}else if(num <= 65535ul){
				bytes.Add(253);
				byte[] l = BitConverter.GetBytes((ushort) num);
				byte[] e = EnsureEndianess(l, 2);
                bytes.AddRange(e);
			}else if(num <= 4294967295ul){
				bytes.Add(254);
				byte[] l = BitConverter.GetBytes((uint) num);
				byte[] e = EnsureEndianess(l, 4);
                bytes.AddRange(e);
			}else{
				bytes.Add(255);
				byte[] l = BitConverter.GetBytes(num);
				byte[] e = EnsureEndianess(l, 8);
                bytes.AddRange(e);
			}
		}
		
		private static byte[] EnsureEndianess(byte[] bytes, ulong size){
			byte[] result = new byte[size];

			if(BitConverter.IsLittleEndian){
				for (ulong i = 0; i < size; i++){
					result[i] = bytes[i];
				}
				return result;
			}
			
			for(ulong i = 0; i < size; i++){
				result[i] = bytes[(ulong) bytes.Length - i - 1];
			}
			return result;
		}
		#endregion
		
		#region utils
		private static byte[] Scramble(byte[] b){
			for(int i = 0; i < b.Length; i++){
				b[i] = (byte) (b[i] + 21 + i);
			}
			return b;
		}
		
		private static byte[] Unscramble(byte[] b){
			for(int i = 0; i < b.Length; i++){
				b[i] = (byte) (b[i] - 21 - i);
			}
			return b;
		}
		
		private static byte[] CompactBoolArray(IList<bool> b){
			byte[] y = new byte[(b.Count + 8 - 1) / 8];
			
			for(int i = 0; i < b.Count; i++){
				y[i / 8] |= (byte) ((b[i] ? 1 : 0) << (i % 8));
			}
			
			return y;
		}
		
		private static bool[] UnCompactBoolArray(byte[] y, int count){
			bool[] b = new bool[count];
			
			for(int i = 0; i < count; i++){
				b[i] = ((y[i / 8] & (1 << (i % 8))) >> (i % 8)) == 1 ? true : false;
			}
			
			return b;
		}
		
		private static byte[] EncryptAES(byte[] body, string password){
			//Random salt for deriving key
			byte[] salt = new byte[16];
			RandomNumberGenerator.Fill(salt);
			
			//deriving key from password
			using var kdf = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
			byte[] key = kdf.GetBytes(32);
			
			//Random IV
			byte[] iv = new byte[12];
			RandomNumberGenerator.Fill(iv);
			
			//Output + auth tag
			byte[] ciphertext = new byte[body.Length];
			byte[] tag = new byte[16];
			
			using (var aes = new AesGcm(key)){
				aes.Encrypt(iv, body, ciphertext, tag);
			}
			
			List<byte> result = new(salt.Length + iv.Length + ciphertext.Length + tag.Length);
			result.AddRange(salt);
			result.AddRange(iv);
			result.AddRange(ciphertext);
			result.AddRange(tag);
			
			return result.ToArray();
		}
		
		private static byte[] DecryptAES(byte[] encryptedData, string password){
			byte[] salt = new byte[16];
			byte[] iv = new byte[12];
			byte[] tag = new byte[16];
			
			//Extract
			Buffer.BlockCopy(encryptedData, 0, salt, 0, salt.Length);
			Buffer.BlockCopy(encryptedData, salt.Length, iv, 0, iv.Length);
			Buffer.BlockCopy(encryptedData, encryptedData.Length - tag.Length, tag, 0, tag.Length);
			
			//Extract ciphertext
			int ciphertextLength = encryptedData.Length - salt.Length - iv.Length - tag.Length;
			byte[] ciphertext = new byte[ciphertextLength];
			Buffer.BlockCopy(encryptedData, salt.Length + iv.Length, ciphertext, 0, ciphertextLength);
			
			//Get key
			using var kdf = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
			byte[] key = kdf.GetBytes(32);
			
			//Decrypt
			byte[] plaintext = new byte[ciphertext.Length];
			using (var aes = new AesGcm(key)){
				aes.Decrypt(iv, ciphertext, tag, plaintext);
			}
		
			return plaintext;
		}
		#endregion
	}
}