using System;
using AshLib.Dates;

namespace AshLib.AshFiles;

public partial class AshFile{
	protected internal class V2{
		//Read
		public static Dictionary<string, object> Read(byte[] fileBytes){
			Dictionary<string, object> dic = new Dictionary<string, object>();
			
			ulong index = 1; //We start at 1 because 0 is version
			ulong campNum = ReadEHFL(fileBytes, ref index); //Read the number of camps
			
			for(ulong i = 0; i < campNum; i++){
				try{
					string campName = ReadCampName(fileBytes, ref index);
					
					object campValue = ReadCampValue(fileBytes, ref index);
					
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
		
		private static object ReadCampValue(byte[] fileBytes, ref ulong index){
			byte t = fileBytes[index];
			index++;
			AshFileType type = (AshFileType) t;
			switch(type){
				case AshFileType.ByteArray:
				return ReadByteArray(fileBytes, ref index);
				
				case AshFileType.String:
				return ReadString(fileBytes, ref index);
				
				case AshFileType.Byte:
				return ReadUint1(fileBytes, ref index);
				
				case AshFileType.Ushort:
				return ReadUint2(fileBytes, ref index);
				
				case AshFileType.Uint:
				return ReadUint4(fileBytes, ref index);
				
				case AshFileType.Ulong:
				return ReadUint8(fileBytes, ref index);
				
				case AshFileType.Sbyte:
				return ReadInt1(fileBytes, ref index);
				
				case AshFileType.Short:
				return ReadInt2(fileBytes, ref index);
				
				case AshFileType.Int:
				return ReadInt4(fileBytes, ref index);
				
				case AshFileType.Long:
				return ReadInt8(fileBytes, ref index);
				
				case AshFileType.Color:
				return ReadColor(fileBytes, ref index);
				
				case AshFileType.Float:
				return ReadFloat4(fileBytes, ref index);
				
				case AshFileType.Double:
				return ReadFloat8(fileBytes, ref index);
				
				case AshFileType.Vec2:
				return ReadVec2(fileBytes, ref index);
				
				case AshFileType.Vec3:
				return ReadVec3(fileBytes, ref index);
				
				case AshFileType.Vec4:
				return ReadVec4(fileBytes, ref index);
				
				case AshFileType.Bool:
				return ReadBool(fileBytes, ref index);
				
				case AshFileType.UbyteArray:
				return ReadUint1Array(fileBytes, ref index);
				
				case AshFileType.UshortArray:
				return ReadUint2Array(fileBytes, ref index);
				
				case AshFileType.UintArray:
				return ReadUint4Array(fileBytes, ref index);
				
				case AshFileType.UlongArray:
				return ReadUint8Array(fileBytes, ref index);
				
				case AshFileType.SbyteArray:
				return ReadInt1Array(fileBytes, ref index);
				
				case AshFileType.ShortArray:
				return ReadInt2Array(fileBytes, ref index);
				
				case AshFileType.IntArray:
				return ReadInt4Array(fileBytes, ref index);
				
				case AshFileType.LongArray:
				return ReadInt8Array(fileBytes, ref index);
				
				case AshFileType.FloatArray:
				return ReadFloat4Array(fileBytes, ref index);
				
				case AshFileType.DoubleArray:
				return ReadFloat8Array(fileBytes, ref index);
				
				case AshFileType.Date:
				return ReadDate(fileBytes, ref index);
				
				default:
				return ReadByteArray(fileBytes, ref index);
			}
		}
		
		//Individual camp types
		
		private static object ReadDate(byte[] fileBytes, ref ulong index){
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
		
		private static object ReadFloat8Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			double[] a = new double[length];
			
			for(ulong i = 0; i < length; i++){
				ulong n1 = index;
				byte[] b1 = EnsureEndianess(fileBytes, ref n1, 8);
				double f = BitConverter.ToDouble(b1, (int) n1);
				index += 8;
				a[i] = f;
			}
			
			return a;
		}
		
		private static object ReadFloat4Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			float[] a = new float[length];
			
			for(ulong i = 0; i < length; i++){
				float f = ReadFloating4(fileBytes, ref index);
				a[i] = f;
			}
			
			return a;
		}
		
		private static object ReadInt8Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			long[] u = new long[length];
			for(ulong i = 0; i < length; i++){
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, 8);
				long us = BitConverter.ToInt64(b2, (int) n2);
				index += 8;
				u[i] = us;
			}
			
			return u;
		}
		
		private static object ReadInt4Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			int[] u = new int[length];
			for(ulong i = 0; i < length; i++){
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, 4);
				int us = BitConverter.ToInt32(b2, (int) n2);
				index += 4;
				u[i] = us;
			}
			
			return u;
		}
		
		private static object ReadInt2Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			short[] u = new short[length];
			for(ulong i = 0; i < length; i++){
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, 2);
				short us = BitConverter.ToInt16(b2, (int) n2);
				index += 2;
				u[i] = us;
			}
			
			return u;
		}
		
		private static object ReadInt1Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			sbyte[] b = new sbyte[length];
			for(ulong i = 0; i < length; i++){
				b[i] = (sbyte) fileBytes[index + i];
			}
			index += length;
			
			return b;
		}
		
		private static object ReadUint8Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			ulong[] u = new ulong[length];
			for(ulong i = 0; i < length; i++){
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, 8);
				ulong us = BitConverter.ToUInt64(b2, (int) n2);
				index += 8;
				u[i] = us;
			}
			
			return u;
		}
		
		private static object ReadUint4Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			uint[] u = new uint[length];
			for(ulong i = 0; i < length; i++){
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, 4);
				uint us = BitConverter.ToUInt32(b2, (int) n2);
				index += 4;
				u[i] = us;
			}
			
			return u;
		}
		
		private static object ReadUint2Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			ushort[] u = new ushort[length];
			for(ulong i = 0; i < length; i++){
				ulong n2 = index;
				byte[] b2 = EnsureEndianess(fileBytes, ref n2, 2);
				ushort us = BitConverter.ToUInt16(b2, (int) n2);
				index += 2;
				u[i] = us;
			}
			
			return u;
		}
		
		private static object ReadUint1Array(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			byte[] b = new byte[length];
			for(ulong i = 0; i < length; i++){
				b[i] = fileBytes[index + i];
			}
			index += length;
			
			return b;
		}
		
		private static object ReadBool(byte[] fileBytes, ref ulong index){
			byte b = fileBytes[index];
			index++;
			bool n = (b % 2 == 1 ? true : false); //0 will be false, 1 will be true, 2 will be false...
			return n;
		}
		
		private static object ReadVec4(byte[] fileBytes, ref ulong index){
			float x = ReadFloating4(fileBytes, ref index);
			float y = ReadFloating4(fileBytes, ref index);
			float z = ReadFloating4(fileBytes, ref index);
			float w = ReadFloating4(fileBytes, ref index);
			
			Vec4 v = new Vec4(x, y, z, w);
			return v;
		}
		
		private static object ReadVec3(byte[] fileBytes, ref ulong index){
			float x = ReadFloating4(fileBytes, ref index);
			float y = ReadFloating4(fileBytes, ref index);
			float z = ReadFloating4(fileBytes, ref index);
			
			Vec3 v = new Vec3(x, y, z);
			return v;
		}
		
		private static object ReadVec2(byte[] fileBytes, ref ulong index){
			float x = ReadFloating4(fileBytes, ref index);
			float y = ReadFloating4(fileBytes, ref index);
			
			Vec2 v = new Vec2(x, y);
			return v;
		}
		
		private static object ReadFloat8(byte[] fileBytes, ref ulong index){			
			ulong n2 = index;
			byte[] b2 = EnsureEndianess(fileBytes, ref n2, 8);
			double d = BitConverter.ToDouble(b2, (int) n2);
			index += 8;
			return d;
		}
		
		private static object ReadFloat4(byte[] fileBytes, ref ulong index){			
			float f = ReadFloating4(fileBytes, ref index);
			return f;
		}
		
		private static object ReadColor(byte[] fileBytes, ref ulong index){
			byte r = fileBytes[index];
			byte g = fileBytes[index + 1];
			byte b = fileBytes[index + 2];
			index += 3;
			
			Color3 c = new Color3(r, g, b);
			return c;
		}
		
		private static object ReadInt8(byte[] fileBytes, ref ulong index){			
			ulong n4 = index;
			byte[] b4 = EnsureEndianess(fileBytes, ref n4, 8);
			long ul = BitConverter.ToInt64(b4, (int) n4);
			index += 8;
			return ul;
		}
		
		private static object ReadInt4(byte[] fileBytes, ref ulong index){			
			ulong n3 = index;
			byte[] b3 = EnsureEndianess(fileBytes, ref n3, 4);
			int ui = BitConverter.ToInt32(b3, (int) n3);
			index += 4;
			return ui;
		}
		
		private static object ReadInt2(byte[] fileBytes, ref ulong index){			
			ulong n2 = index;
			byte[] b2 = EnsureEndianess(fileBytes, ref n2, 2);
			short us = BitConverter.ToInt16(b2, (int) n2);
			index += 2;
			return us;
		}
		
		private static object ReadInt1(byte[] fileBytes, ref ulong index){			
			sbyte b = (sbyte) fileBytes[index];
			index++;
			return b;
		}
		
		private static object ReadUint8(byte[] fileBytes, ref ulong index){			
			ulong n4 = index;
			byte[] b4 = EnsureEndianess(fileBytes, ref n4, 8);
			ulong ul = BitConverter.ToUInt64(b4, (int) n4);
			index += 8;
			return ul;
		}
		
		private static object ReadUint4(byte[] fileBytes, ref ulong index){			
			ulong n3 = index;
			byte[] b3 = EnsureEndianess(fileBytes, ref n3, 4);
			uint ui = BitConverter.ToUInt32(b3, (int) n3);
			index += 4;
			return ui;
		}
		
		private static object ReadUint2(byte[] fileBytes, ref ulong index){			
			ulong n2 = index;
			byte[] b2 = EnsureEndianess(fileBytes, ref n2, 2);
			ushort us = BitConverter.ToUInt16(b2, (int) n2);
			index += 2;
			return us;
		}
		
		private static object ReadUint1(byte[] fileBytes, ref ulong index){			
			byte b = fileBytes[index];
			index++;
			return b;
		}
		
		private static object ReadString(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			string s = "";
			for(ulong i = 0; i < length; i++){
				s += (char) fileBytes[index + i];
			}
			index += length;
			return s;
		}
		
		private static object ReadByteArray(byte[] fileBytes, ref ulong index){
			ulong length = ReadHFL(fileBytes, ref index);
			
			byte[] b = new byte[length];
			for(ulong i = 0; i < length; i++){
				b[i] = fileBytes[index + i];
			}
			index += length;
			return b;
		}
		
		//Camp things
		
		private static string ReadCampName(byte[] fileBytes, ref ulong index){
			ulong length = ReadEHFL(fileBytes, ref index);
			
			string s = "";
			for(ulong i = 0; i < length; i++){
				s += (char) fileBytes[index + i];
			}
			index += length;
			return s;
		}
		
		//Lengths
		
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
		
		private static float ReadFloating4(byte[] fileBytes, ref ulong index){			
			ulong n1 = index;
			byte[] b1 = EnsureEndianess(fileBytes, ref n1, 4);
			float f = BitConverter.ToSingle(b1, (int) n1);
			index += 4;
			return f;
		}
		
		//Endianess stuff
		
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
		
		//Write
		
		private static AshFileType GetAshFileTypeFromType(Type type){
			if (type == typeof(byte[]))
				return AshFileType.ByteArray;
			else if (type == typeof(string))
				return AshFileType.String;
			else if (type == typeof(byte))
				return AshFileType.Byte;
			else if (type == typeof(ushort))
				return AshFileType.Ushort;
			else if (type == typeof(uint))
				return AshFileType.Uint;
			else if (type == typeof(ulong))
				return AshFileType.Ulong;
			else if (type == typeof(sbyte))
				return AshFileType.Sbyte;
			else if (type == typeof(short))
				return AshFileType.Short;
			else if (type == typeof(int))
				return AshFileType.Int;
			else if (type == typeof(long))
				return AshFileType.Long;
			else if (type == typeof(Color3))
				return AshFileType.Color;
			else if (type == typeof(float))
				return AshFileType.Float;
			else if (type == typeof(double))
				return AshFileType.Double;
			else if (type == typeof(bool))
				return AshFileType.Bool;
			else if (type == typeof(Date))
				return AshFileType.Date;
			else if (type == typeof(Vec2))
				return AshFileType.Vec2;
			else if (type == typeof(Vec3))
				return AshFileType.Vec3;
			else if (type == typeof(Vec4))
				return AshFileType.Vec4;
			// Add additional checks for array types (e.g., byte[], uint[], etc.)
			else if (type == typeof(byte[]))
				return AshFileType.UbyteArray;
			else if (type == typeof(ushort[]))
				return AshFileType.UshortArray;
			else if (type == typeof(uint[]))
				return AshFileType.UintArray;
			else if (type == typeof(ulong[]))
				return AshFileType.UlongArray;
			else if (type == typeof(sbyte[]))
				return AshFileType.SbyteArray;
			else if (type == typeof(short[]))
				return AshFileType.ShortArray;
			else if (type == typeof(int[]))
				return AshFileType.IntArray;
			else if (type == typeof(long[]))
				return AshFileType.LongArray;
			else if (type == typeof(float[]))
				return AshFileType.FloatArray;
			else if (type == typeof(double[]))
				return AshFileType.DoubleArray;
			
			return AshFileType.ByteArray;
		}
		
		public static byte[] Write(Dictionary<string, object> d){
			List<byte> bytes = new List<byte>();
			
			KeyValuePair<string, object>[] dictionary = new KeyValuePair<string, object>[d.Count];
			((ICollection<KeyValuePair<string, object>>)d).CopyTo(dictionary, 0);
			
			bytes.Add(2);
			
			WriteEHFL(bytes, (ulong) dictionary.Length);
			
			for(ulong i = 0; i < (ulong) dictionary.Length; i++){
				try{
					WriteCampName(bytes, dictionary[i].Key);
					bytes.Add((byte) GetAshFileTypeFromType(dictionary[i].Value.GetType()));
					WriteCampValue(bytes, GetAshFileTypeFromType(dictionary[i].Value.GetType()), dictionary[i].Value);
				} catch(Exception e){
					AshFile.HandleException(e, "####An error occurred while writing!####");
				}
			}
			
			return bytes.ToArray();
		}
		
		private static void WriteCampValue(List<byte> bytes, AshFileType t, object val){
			switch(t){
				case AshFileType.ByteArray:
				WriteByteArray(bytes, (byte[]) val);
				return;
				
				case AshFileType.String:
				WriteString(bytes, (string) val);
				return;
				
				case AshFileType.Byte:
				WriteUint1(bytes, (byte) val);
				return;
				
				case AshFileType.Ushort:
				WriteUint2(bytes, (ushort) val);
				return;
				
				case AshFileType.Uint:
				WriteUint4(bytes, (uint) val);
				return;
				
				case AshFileType.Ulong:
				WriteUint8(bytes, (ulong) val);
				return;
				
				case AshFileType.Sbyte:
				WriteInt1(bytes, (sbyte) val);
				return;
				
				case AshFileType.Short:
				WriteInt2(bytes, (short) val);
				return;
				
				case AshFileType.Int:
				WriteInt4(bytes, (int) val);
				return;
				
				case AshFileType.Long:
				WriteInt8(bytes, (long) val);
				return;
				
				case AshFileType.Color:
				WriteColor(bytes, (Color3) val);
				return;
				
				case AshFileType.Float:
				WriteFloat4(bytes, (float) val);
				return;
				
				case AshFileType.Double:
				WriteFloat8(bytes, (double) val);
				return;
				
				case AshFileType.Vec2:
				WriteVec2(bytes, (Vec2) val);
				return;
				
				case AshFileType.Vec3:
				WriteVec3(bytes, (Vec3) val);
				return;
				
				case AshFileType.Vec4:
				WriteVec4(bytes, (Vec4) val);
				return;
				
				case AshFileType.Bool:
				WriteBool(bytes, (bool) val);
				return;
				
				case AshFileType.UbyteArray:
				WriteUint1Array(bytes, (byte[]) val);
				return;
				
				case AshFileType.UshortArray:
				WriteUint2Array(bytes, (ushort[]) val);
				return;
				
				case AshFileType.UintArray:
				WriteUint4Array(bytes, (uint[]) val);
				return;
				
				case AshFileType.UlongArray:
				WriteUint8Array(bytes, (ulong[]) val);
				return;
				
				case AshFileType.SbyteArray:
				WriteInt1Array(bytes, (sbyte[]) val);
				return;
				
				case AshFileType.ShortArray:
				WriteInt2Array(bytes, (short[]) val);
				return;
				
				case AshFileType.IntArray:
				WriteInt4Array(bytes, (int[]) val);
				return;
				
				case AshFileType.LongArray:
				WriteInt8Array(bytes, (long[]) val);
				return;
				
				case AshFileType.FloatArray:
				WriteFloat4Array(bytes, (float[]) val);
				return;
				
				case AshFileType.DoubleArray:
				WriteFloat4Array(bytes, (double[]) val);
				return;
				
				case AshFileType.Date:
				WriteDate(bytes, (Date) val);
				return;
			}
		}
		
		//Different value types
		
		private static void WriteDate(List<byte> bytes, Date b){
			ulong combined = ((ulong)b.seconds & 0x3F) | 
                         (((ulong)b.minutes & 0x3F) << 6) |
                         (((ulong)b.hours & 0x1F) << 12) |
                         (((ulong)b.days & 0x1F) << 17) |
                         (((ulong)b.months & 0x0F) << 22) |
                         (((ulong)(b.years- 1488) & 0x03FF) << 26);
						 
			byte[] dbytes = BitConverter.GetBytes(combined);
		
			byte[] fbytes = new byte[5];
			for(int i = 0; i < 5; i++){
				fbytes[i] = dbytes[i];
			}
			
			bytes.AddRange(fbytes);
		}
		
		private static void WriteFloat4Array(List<byte> bytes, double[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteFloat8(bytes, b[i]);
			}
		}
		
		private static void WriteFloat4Array(List<byte> bytes, float[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteFloat4(bytes, b[i]);
			}
		}
		
		private static void WriteInt8Array(List<byte> bytes, long[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteInt8(bytes, b[i]);
			}
		}
		
		private static void WriteInt4Array(List<byte> bytes, int[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteInt4(bytes, b[i]);
			}
		}
		
		private static void WriteInt2Array(List<byte> bytes, short[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteInt2(bytes, b[i]);
			}
		}
		
		private static void WriteInt1Array(List<byte> bytes, sbyte[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				bytes.Add((byte) b[i]);
			}
		}
		
		private static void WriteUint8Array(List<byte> bytes, ulong[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteUint8(bytes, b[i]);
			}
		}
		
		private static void WriteUint4Array(List<byte> bytes, uint[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteUint4(bytes, b[i]);
			}
		}
		
		private static void WriteUint2Array(List<byte> bytes, ushort[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				WriteUint2(bytes, b[i]);
			}
		}
		
		private static void WriteUint1Array(List<byte> bytes, byte[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				bytes.Add(b[i]);
			}
		}
		
		private static void WriteBool(List<byte> bytes, bool b){
			bytes.Add((b ? (byte)1 : (byte)0));
		}
		
		private static void WriteVec4(List<byte> bytes, Vec4 b){
			WriteFloat4(bytes, b.X);
			WriteFloat4(bytes, b.Y);
			WriteFloat4(bytes, b.Z);
			WriteFloat4(bytes, b.W);
		}
		
		private static void WriteVec3(List<byte> bytes, Vec3 b){
			WriteFloat4(bytes, b.X);
			WriteFloat4(bytes, b.Y);
			WriteFloat4(bytes, b.Z);
		}
		
		private static void WriteVec2(List<byte> bytes, Vec2 b){
			WriteFloat4(bytes, b.X);
			WriteFloat4(bytes, b.Y);
		}
		
		private static void WriteFloat8(List<byte> bytes, double b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 8);
            bytes.AddRange(e);
		}
		
		private static void WriteFloat4(List<byte> bytes, float b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 4);
            bytes.AddRange(e);
		}
		
		private static void WriteColor(List<byte> bytes, Color3 b){
            bytes.Add(b.R);
            bytes.Add(b.G);
            bytes.Add(b.B);
		}
		
		private static void WriteInt8(List<byte> bytes, long b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 8);
            bytes.AddRange(e);
		}
		
		private static void WriteInt4(List<byte> bytes, int b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 4);
            bytes.AddRange(e);
		}
		
		private static void WriteInt2(List<byte> bytes, short b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 2);
            bytes.AddRange(e);
		}
		
		private static void WriteInt1(List<byte> bytes, sbyte b){
			bytes.Add((byte) b);
		}
		
		private static void WriteUint8(List<byte> bytes, ulong b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 8);
            bytes.AddRange(e);
		}
		
		private static void WriteUint4(List<byte> bytes, uint b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 4);
            bytes.AddRange(e);
		}
		
		private static void WriteUint2(List<byte> bytes, ushort b){
			byte[] l = BitConverter.GetBytes(b);
			byte[] e = EnsureEndianess(l, 2);
            bytes.AddRange(e);
		}
		
		private static void WriteUint1(List<byte> bytes, byte b){
			bytes.Add(b);
		}
		
		private static void WriteString(List<byte> bytes, string c){
			WriteHFL(bytes, (ulong) c.Length);
			
			char[] s = c.ToCharArray();
			
			for(int i = 0; i < s.Length; i++){
				bytes.Add((byte) s[i]);
			}
		}
		
		private static void WriteByteArray(List<byte> bytes, byte[] b){
			WriteHFL(bytes, (ulong) b.Length);
			
			for(int i = 0; i < b.Length; i++){
				bytes.Add(b[i]);
			}
		}
		
		//Other stuff?? idk what to call it
		
		private static void WriteCampName(List<byte> bytes, string name){
			WriteEHFL(bytes, (ulong) name.Length);
			
			char[] s = name.ToCharArray();
			
			for(int i = 0; i < s.Length; i++){
				bytes.Add((byte) s[i]);
			}
		}
		
		//Lengths
		
		private static void WriteEHFL(List<byte> bytes, ulong length){
			if(length < 256){
				bytes.Add((byte) length);
				return;
			}
			bytes.Add(0);
			
            if (length < 65536){
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
		
		//Endianess stuff
		
		private static byte[] EnsureEndianess(byte[] bytes, ulong size)
		{
			byte[] result = new byte[size];

			if (BitConverter.IsLittleEndian)
			{
				for (ulong i = 0; i < size; i++)
				{
					result[i] = bytes[i];
				}
				return result;
			}
			for (ulong i = 0; i < size; i++)
			{
				result[i] = bytes[(ulong) bytes.Length - i - 1];
			}
			return result;
		}
	}
}