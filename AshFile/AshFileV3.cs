using System;
using System.Text;
using System.Collections;
using AshLib.Dates;

namespace AshLib.AshFiles;

public partial class AshFile{
	protected internal class V3{
		#region read
		//Read
		public static Dictionary<string, object> Read(byte[] fileBytes, out AshFileFormatConfig conf){
			Dictionary<string, object> dic = new Dictionary<string, object>();
			
			ulong index = 5; //We start at 5 because 0 is version, and then %ASH comes
			AshFileFormatConfig config = new AshFileFormatConfig(fileBytes[index]);
			conf = config;
			index++;
			if(config.compactBools){
				try{
					ulong boolNum = ReadEHFL(fileBytes, ref index); //Read the number of bools
					
					List<string> names = new List<string>((int)boolNum);
					
					for(ulong i = 0; i < boolNum; i++){
						names.Add(ReadCampName(fileBytes, ref index, config.maskCampNames));
					}
					
					byte[] y = new byte[(boolNum + 8 - 1) / 8];
					Array.Copy(fileBytes, (int) index, y, 0, y.Length);
					index += (ulong) y.Length;
					bool[] bools = UnCompactBoolArray(y, (int) boolNum);
					
					for(int i = 0; i < (int) boolNum; i++){
						dic.Add(names[i], bools[i]);
					}
				}catch(Exception e){
					AshFile.HandleException(e, "####An error occurred while reading booldict!####");
				}
			}
			ulong campNum = ReadEHFL(fileBytes, ref index); //Read the number of camps
			
			for(ulong i = 0; i < campNum; i++){
				try{
					string campName = ReadCampName(fileBytes, ref index, config.maskCampNames);
					
					object campValue = ReadCampValue(fileBytes, ref index, config.maskStrings, config.compactBools);
					
					if(dic.ContainsKey(campName)){
						throw new AshFileException("The dictionary already has a camp named " + campName, 4);
					}
					dic.Add(campName, campValue);
				} catch(Exception e){
					AshFile.HandleException(e, "####An error occurred while reading!####");
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
				
				Type y = GetTypeFromEnum((AshFileType) t);
				if(y == typeof(object)){
					return null;
				}
				
				ulong length = ReadHFL(fileBytes, ref index);
				
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
					ulong length = ReadHFL(fileBytes, ref index);
					
					byte[] b = new byte[length];
					Buffer.BlockCopy(fileBytes, (int) index, b, 0, (int) length);
					index += length;
					return b;
				case AshFileType.String:
					length = ReadEHFL(fileBytes, ref index);
					
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
			}
		}
		
		private static float ReadFloating4(byte[] fileBytes, ref ulong index){			
			ulong n1 = index;
			byte[] b1 = EnsureEndianess(fileBytes, ref n1, 4);
			float f = BitConverter.ToSingle(b1, (int) n1);
			index += 4;
			return f;
		}
		
		private static Type GetTypeFromEnum(AshFileType fileType){
			switch (fileType){
				case AshFileType.String: return typeof(string);
				case AshFileType.Byte: return typeof(byte);
				case AshFileType.Ushort: return typeof(ushort);
				case AshFileType.Uint: return typeof(uint);
				case AshFileType.Ulong: return typeof(ulong);
				case AshFileType.Sbyte: return typeof(sbyte);
				case AshFileType.Short: return typeof(short);
				case AshFileType.Int: return typeof(int);
				case AshFileType.Long: return typeof(long);
				case AshFileType.Color3: return typeof(Color3); // Example for Color3 (need a proper type here)
				case AshFileType.Float: return typeof(float);
				case AshFileType.Double: return typeof(double);
				case AshFileType.Vec2: return typeof(Vec2); // Example for Vec2
				case AshFileType.Vec3: return typeof(Vec3); // Example for Vec3
				case AshFileType.Vec4: return typeof(Vec4); // Example for Vec4
				case AshFileType.Bool: return typeof(bool);
				case AshFileType.Date: return typeof(Date);
				default: return typeof(object); // Default case if no matching type
			}
		}
		
		private static string ReadCampName(byte[] fileBytes, ref ulong index, bool mask){
			ulong length = ReadEHFL(fileBytes, ref index);
			
			byte[] b = new byte[(int) length];
			Array.Copy(fileBytes, (int) index, b, 0, (int) length);
			
			if(mask){
				b = Unscramble(b);
			}
			
			string s = Encoding.UTF8.GetString(b);
			index += length;
			return s;
		}
		
		private static ulong ReadHFL(byte[] fileBytes, ref ulong index){
			byte size = fileBytes[index];
			index++;
			return ReadUNumberSize(fileBytes, ref index, size);
		}
		
		private static ulong ReadEHFL(byte[] fileBytes, ref ulong index){
			if(fileBytes[index] == 0){
				byte size = fileBytes[index + 1];
				index += 2;
				return ReadUNumberSize(fileBytes, ref index, size);
				
			}
			ulong l = (ulong) fileBytes[index];
			index++;
			return l;
		}
		
		private static ulong ReadUNumberSize(byte[] fileBytes, ref ulong index, byte size){
			switch(size){
				case 1:
				byte b1 = fileBytes[index];
				index++;
				return (ulong) b1;
				
				case 2:
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, (ulong) size);
				ushort us = BitConverter.ToUInt16(b2, (int) n2);
				index += 2;
				return (ulong) us;
				
				case 4:
				ulong n3 = index;
				byte[] b3 = EnsureEndianess(fileBytes, ref n3, (ulong) size);
				uint ui = BitConverter.ToUInt32(b3, (int) n3);
				index += 4;
				return (ulong) ui;
				
				case 8:
				ulong n4 = index;
				byte[] b4 = EnsureEndianess(fileBytes, ref n4, (ulong) size);
				ulong ul = BitConverter.ToUInt64(b4, (int) n4);
				index += 8;
				return (ulong) ul;
				
				default:
				ulong u = 0;
				for(ulong i = 0; i < size; i++){
					u += (ulong) fileBytes[index + i] << (8 * (int) i);
				}
				index += (ulong) size;
				return u;
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
		#endregion
		
		#region write
		
		//Write
		public static byte[] Write(IDictionary<string, object> d, AshFileFormatConfig conf){
			List<byte> bytes = new List<byte>();
			List<byte> temp = new List<byte>();
			
			ulong campNum = 0;
			
			List<KeyValuePair<string, object>> dictionary = new List<KeyValuePair<string, object>>((ICollection<KeyValuePair<string, object>>)d);
			//((ICollection<KeyValuePair<string, object>>)d).CopyTo(dictionary, 0);
			
			Dictionary<string, bool> bools = new Dictionary<string, bool>();
			
			try{
				if(conf.compactBools){
					for(ulong i = 0; i < (ulong) dictionary.Count; i++){
						if(dictionary[(int) i].Value is bool b){
							bools.Add(dictionary[(int) i].Key, b);
							dictionary.RemoveAt((int) i);
							i--;
						}
					}
				}
			}catch(Exception e){
				AshFile.HandleException(e, "####An error occurred while writing booldict!####");
			}
			
			for(ulong i = 0; i < (ulong) dictionary.Count; i++){
				try{
					temp.Clear();
					if(WriteCampValue(temp, dictionary[(int)i].Value, conf.maskStrings, conf.compactBools)){
						campNum++;
						WriteCampName(bytes, dictionary[(int)i].Key, conf.maskCampNames);
						bytes.AddRange(temp);
					}
				} catch(Exception e){
					AshFile.HandleException(e, "####An error occurred while writing!####");
				}
			}
			temp.Clear();
			temp.Add(3);
			temp.AddRange(Encoding.UTF8.GetBytes("%ASH"));
			temp.Add(conf.ToByte()); //Config
			
			if(conf.compactBools){
				WriteEHFL(temp, (ulong) bools.Count);
				foreach(string s in bools.Keys){
					WriteCampName(temp, s, conf.maskCampNames);
				}
				
				temp.AddRange(CompactBoolArray(bools.Values.ToList()));
			}
			
			WriteEHFL(temp, (ulong) campNum);
			temp.AddRange(bytes);
			
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
					WriteHFL(bytes, (ulong) l.Count);
					bytes.AddRange(CompactBoolArray(l));
					return true;
				}
				
				int count = 0;
				foreach(object i in array){
					if(WriteCampType(tempVals, i, mask)){
						count++;
					}
				}
				WriteHFL(bytes, (ulong) count);
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
					
					WriteEHFL(bytes, (ulong) b.Length);
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
			
			WriteEHFL(bytes, (ulong) b.Length);
			bytes.AddRange(b);
		}
		
		private static void WriteEHFL(List<byte> bytes, ulong length){
			if(length == 0){
				bytes.Add(0);
				bytes.Add(0);
				return;
			}
			
			if(length < 256){
				bytes.Add((byte) length);
				return;
			}
			bytes.Add(0);
			
            if(length < 65536){
                bytes.Add(2);
				byte[] l = BitConverter.GetBytes(length);
				byte[] e = EnsureEndianess(l, 2);
                bytes.AddRange(e);
            }else if (length < 4294967296){
                bytes.Add(4);
				byte[] l = BitConverter.GetBytes(length);
				byte[] e = EnsureEndianess(l, 4);
                bytes.AddRange(e);
            }else{
                bytes.Add(8);
				byte[] l = BitConverter.GetBytes(length);
				byte[] e = EnsureEndianess(l, 8);
                bytes.AddRange(e);
            }
			
		}
		
		private static void WriteHFL(List<byte> bytes, ulong length){			
            if (length < 256){
                bytes.Add(1);
				byte[] l = BitConverter.GetBytes(length);
				byte[] e = EnsureEndianess(l, 1);
                bytes.Add(e[0]);
            } else if (length < 65536){
                bytes.Add(2);
				byte[] l = BitConverter.GetBytes(length);
				byte[] e = EnsureEndianess(l, 2);
                bytes.AddRange(e);
            } else if (length < 4294967296){
                bytes.Add(4);
				byte[] l = BitConverter.GetBytes(length);
				byte[] e = EnsureEndianess(l, 4);
                bytes.AddRange(e);
            } else{
                bytes.Add(8);
				byte[] l = BitConverter.GetBytes(length);
				byte[] e = EnsureEndianess(l, 8);
                bytes.AddRange(e);
            }
		}
		
		private static byte[] EnsureEndianess(byte[] bytes, ulong size){
			byte[] result = new byte[size];

			if (BitConverter.IsLittleEndian){
				for (ulong i = 0; i < size; i++){
					result[i] = bytes[i];
				}
				return result;
			}
			for (ulong i = 0; i < size; i++){
				result[i] = bytes[(ulong) bytes.Length - i - 1];
			}
			return result;
		}
		
		private static byte[] Scramble(byte[] b){
			for(int i = 0; i < b.Length; i++){
				b[i] = (byte) (b[i] + 13 + i);
			}
			return b;
		}
		
		private static byte[] Unscramble(byte[] b){
			for(int i = 0; i < b.Length; i++){
				b[i] = (byte) (b[i] - 13 - i);
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
		#endregion
	}
}