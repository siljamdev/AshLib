using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using AshLib.Dates;

namespace AshLib.AshFiles;

public partial class AshFile{
	public override string ToString(){
		StringBuilder sb = new StringBuilder();
		foreach(KeyValuePair<string, object> kvp in data){
			sb.Append("<");
			sb.Append(kvp.Key);
			sb.Append(">:");
			
			if(kvp.Value is Array array){
				AshFileTypeV3 t = GetFileTypeFromType(array.GetType().GetElementType());
				if(t == AshFileTypeV3.Default){
					continue;
				}
				sb.Append(GetTypeIdentifier(t));
				sb.Append(": [");
				bool b = false;
				foreach(object i in array){
					if(b){
						sb.Append("; ");
					}
					WriteObjectType(sb, i);
					b = true;
				}
				sb.Append("]; ");
			}else{
				AshFileTypeV3 t = GetFileTypeFromType(kvp.Value.GetType());
				sb.Append(GetTypeIdentifier(t));
				sb.Append(": ");
				if(t == AshFileTypeV3.Default){
					continue;
				}
				WriteObjectType(sb, kvp.Value);
				sb.Append("; ");
			}
		}
		
		return sb.ToString();
	}
	
	private static void WriteObjectType(StringBuilder sb, object o){
		switch(o){
			case string s:
				sb.Append("\"");
				sb.Append(s.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\n").Replace("\r\n", "\\n"));
				sb.Append("\"");
				break;
			case Vec2 v2:
				sb.Append(v2.X);
				sb.Append(", ");
				sb.Append(v2.Y);
				break;
			case Vec3 v3:
				sb.Append(v3.X);
				sb.Append(", ");
				sb.Append(v3.Y);
				sb.Append(", ");
				sb.Append(v3.Z);
				break;
			case Vec4 v4:
				sb.Append(v4.X);
				sb.Append(", ");
				sb.Append(v4.Y);
				sb.Append(", ");
				sb.Append(v4.Z);
				sb.Append(", ");
				sb.Append(v4.W);
				break;
			case bool b:
				sb.Append(b ? "true" : "false");
				break;
			case Date d:
				sb.Append(d.days);
				sb.Append("/");
				sb.Append(d.months);
				sb.Append("/");
				sb.Append(d.years);
				sb.Append("/");
				sb.Append(d.hours);
				sb.Append("/");
				sb.Append(d.minutes);
				sb.Append("/");
				sb.Append(d.seconds);
				break;
			default:
				dynamic y = o;
				sb.Append(y.ToString());
				break;
		}
	}
	
	private static string GetTypeIdentifier(AshFileTypeV3 fileType){
		switch(fileType){
			case AshFileTypeV3.String: return "@";
			case AshFileTypeV3.Byte: return "ub";
			case AshFileTypeV3.Ushort: return "us";
			case AshFileTypeV3.Uint: return "ui";
			case AshFileTypeV3.Ulong: return "ul";
			case AshFileTypeV3.Sbyte: return "sb";
			case AshFileTypeV3.Short: return "s";
			case AshFileTypeV3.Int: return "i";
			case AshFileTypeV3.Long: return "l";
			case AshFileTypeV3.Color3: return "#"; // Example for Color3 (need a proper type here)
			case AshFileTypeV3.Float: return "f";
			case AshFileTypeV3.Double: return "d";
			case AshFileTypeV3.Vec2: return "v2"; // Example for Vec2
			case AshFileTypeV3.Vec3: return "v3"; // Example for Vec3
			case AshFileTypeV3.Vec4: return "v4"; // Example for Vec4
			case AshFileTypeV3.Bool: return "b";
			case AshFileTypeV3.Date: return "dt";
			default: return "ERROR"; // Default case if no matching type
		}
	}
	
	private static AshFileTypeV3 GetTypeFromIdentifier(string identifier){
		switch(identifier){
			case "@": return AshFileTypeV3.String;
			case "ub": return AshFileTypeV3.Byte;
			case "us": return AshFileTypeV3.Ushort;
			case "ui": return AshFileTypeV3.Uint;
			case "ul": return AshFileTypeV3.Ulong;
			case "sb": return AshFileTypeV3.Sbyte;
			case "s": return AshFileTypeV3.Short;
			case "n":
			case "i": return AshFileTypeV3.Int;
			case "l": return AshFileTypeV3.Long;
			case "#": return AshFileTypeV3.Color3;
			case "f": return AshFileTypeV3.Float;
			case "d": return AshFileTypeV3.Double;
			case "v2": return AshFileTypeV3.Vec2;
			case "v3": return AshFileTypeV3.Vec3;
			case "v4": return AshFileTypeV3.Vec4;
			case "b": return AshFileTypeV3.Bool;
			case "dt": return AshFileTypeV3.Date;
			default: return AshFileTypeV3.Default;
		}
	}
	
	private static Type GetTypeFromEnum(AshFileTypeV3 fileType){
		switch (fileType){
			case AshFileTypeV3.String: return typeof(string);
			case AshFileTypeV3.Byte: return typeof(byte);
			case AshFileTypeV3.Ushort: return typeof(ushort);
			case AshFileTypeV3.Uint: return typeof(uint);
			case AshFileTypeV3.Ulong: return typeof(ulong);
			case AshFileTypeV3.Sbyte: return typeof(sbyte);
			case AshFileTypeV3.Short: return typeof(short);
			case AshFileTypeV3.Int: return typeof(int);
			case AshFileTypeV3.Long: return typeof(long);
			case AshFileTypeV3.Color3: return typeof(Color3); // Example for Color3 (need a proper type here)
			case AshFileTypeV3.Float: return typeof(float);
			case AshFileTypeV3.Double: return typeof(double);
			case AshFileTypeV3.Vec2: return typeof(Vec2); // Example for Vec2
			case AshFileTypeV3.Vec3: return typeof(Vec3); // Example for Vec3
			case AshFileTypeV3.Vec4: return typeof(Vec4); // Example for Vec4
			case AshFileTypeV3.Bool: return typeof(bool);
			case AshFileTypeV3.Date: return typeof(Date);
			default: return typeof(object); // Default case if no matching type
		}
	}
	
	public static bool TryParse(string s, out AshFile a){
		try{
			a = Parse(s);
			return true;
		}catch{
			a = null;
			return false;
		}
	}
	
	public static AshFile Parse(string s){
		int index = 0;
		AshFile af = new AshFile();
		while(index < s.Length){
			ParseWhite(s, ref index);
			if(ParseOpenName(s, ref index)){
				string name = ParseName(s, ref index);
				ParseCloseName(s, ref index);
				ParseWhite(s, ref index);
				ParseColon(s, ref index);
				ParseWhite(s, ref index);
				string type = ParseUntilColon(s, ref index);
				AshFileTypeV3 t = GetTypeFromIdentifier(type);
				if(t == AshFileTypeV3.Default){
					ParseWhite(s, ref index);
					ParseColon(s, ref index);
					ParseWhite(s, ref index);
					if(ParseOpenArray(s, ref index)){
						ParseNextCloseArray(s, ref index);
						ParseWhite(s, ref index);
						ParseSemiColon(s, ref index);
						continue;
					}else{
						ParseNextSemiColon(s, ref index, false);
						ParseSemiColon(s, ref index);
						continue;
					}
				}
				ParseWhite(s, ref index);
				ParseColon(s, ref index);
				ParseWhite(s, ref index);
				if(ParseOpenArray(s, ref index)){
					Type y = GetTypeFromEnum(t);
					
					IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(y));
					
					while(true){
						ParseWhite(s, ref index);
						object o = ParseValue(s, ref index, t, true);
						if(o != null){
							dynamic d = o;
							list.Add(d);
						}
						ParseWhite(s, ref index);
						if(ParseCloseArray(s, ref index)){
							break;
						}else{
							ParseSemiColon(s, ref index);
						}
					}
					
					Array array = Array.CreateInstance(y, list.Count);
					list.CopyTo(array, 0);
					
					dynamic a = array;
					if(!af.ExistsCamp(name)){
						af.SetCamp(name, a);
					}
					ParseWhite(s, ref index);
					ParseSemiColon(s, ref index);
				}else{
					object o = ParseValue(s, ref index, t, false);
					if(o != null && !af.ExistsCamp(name)){
						af.SetCamp(name, o);
					}
					ParseWhite(s, ref index);
					ParseSemiColon(s, ref index);
				}
			}
		}
		return af;
	}
	
	private static object ParseValue(string s, ref int index, AshFileTypeV3 t, bool array){
		switch(t){
			case AshFileTypeV3.String:
				if(ParseQuote(s, ref index)){
					return ParseString(s, ref index).Replace("\\\"", "\"").Replace("\\n", Environment.NewLine);
				}else{
					ParseNextSemiColon(s, ref index, array);
					return null;
				}
			case AshFileTypeV3.Byte:
				string p = ParseUntilSemicolon(s, ref index, array);
				if(byte.TryParse(p, out byte j2)){
					return j2;
				}else{
					return null;
				}
			case AshFileTypeV3.Ushort:
				p = ParseUntilSemicolon(s, ref index, array);
				if(ushort.TryParse(p, out ushort j3)){
					return j3;
				}else{
					return null;
				}
			case AshFileTypeV3.Uint:
				p = ParseUntilSemicolon(s, ref index, array);
				if(uint.TryParse(p, out uint j4)){
					return j4;
				}else{
					return null;
				}
			case AshFileTypeV3.Ulong:
				p = ParseUntilSemicolon(s, ref index, array);
				if(ulong.TryParse(p, out ulong j5)){
					return j5;
				}else{
					return null;
				}
			case AshFileTypeV3.Sbyte:
				p = ParseUntilSemicolon(s, ref index, array);
				if(sbyte.TryParse(p, out sbyte j6)){
					return j6;
				}else{
					return null;
				}
			case AshFileTypeV3.Short:
				p = ParseUntilSemicolon(s, ref index, array);
				if(short.TryParse(p, out short j7)){
					return j7;
				}else{
					return null;
				}
			case AshFileTypeV3.Int:
				p = ParseUntilSemicolon(s, ref index, array);
				if(int.TryParse(p, out int j8)){
					return j8;
				}else{
					return null;
				}
			case AshFileTypeV3.Long:
				p = ParseUntilSemicolon(s, ref index, array);
				if(long.TryParse(p, out long j9)){
					return j9;
				}else{
					return null;
				}
			case AshFileTypeV3.Color3:
				p = ParseUntilSemicolon(s, ref index, array);
				if(Color3.TryParse(p, out Color3 j10)){
					return j10;
				}else{
					return null;
				}
			case AshFileTypeV3.Float:
				p = ParseUntilSemicolon(s, ref index, array);
				if(float.TryParse(p, out float j11)){
					return j11;
				}else{
					return null;
				}
			case AshFileTypeV3.Double:
				p = ParseUntilSemicolon(s, ref index, array);
				if(double.TryParse(p, out double j12)){
					return j12;
				}else{
					return null;
				}
			case AshFileTypeV3.Vec2:
				float j13a, j13b;
				byte c = 0;
				
				p = ParseUntilComma(s, ref index);
				if(float.TryParse(p, out j13a)){
					c++;
				}
				
				ParseWhite(s, ref index);
				if(!ParseComma(s, ref index)){
					ParseNextSemiColon(s, ref index, array);
					return null;
				}
				ParseWhite(s, ref index);
				
				p = ParseUntilSemicolon(s, ref index, array);
				if(float.TryParse(p, out j13b)){
					c++;
				}
				
				if(c == 2){
					return new Vec2(j13a, j13b);
				}else{
					return null;
				}
			case AshFileTypeV3.Vec3:
				float j14a, j14b, j14c;
				c = 0;
				
				p = ParseUntilComma(s, ref index);
				if(float.TryParse(p, out j14a)){
					c++;
				}
				
				ParseWhite(s, ref index);
				if(!ParseComma(s, ref index)){
					ParseNextSemiColon(s, ref index, array);
					return null;
				}
				ParseWhite(s, ref index);
				
				p = ParseUntilComma(s, ref index);
				if(float.TryParse(p, out j14b)){
					c++;
				}
				
				ParseWhite(s, ref index);
				if(!ParseComma(s, ref index)){
					ParseNextSemiColon(s, ref index, array);
					return null;
				}
				ParseWhite(s, ref index);
				
				p = ParseUntilSemicolon(s, ref index, array);
				if(float.TryParse(p, out j14c)){
					c++;
				}
				
				if(c == 3){
					return new Vec3(j14a, j14b, j14c);
				}else{
					return null;
				}
			case AshFileTypeV3.Vec4:
				float j15a, j15b, j15c, j15d;
				c = 0;
				
				p = ParseUntilComma(s, ref index);
				if(float.TryParse(p, out j15a)){
					c++;
				}
				
				ParseWhite(s, ref index);
				if(!ParseComma(s, ref index)){
					ParseNextSemiColon(s, ref index, array);
					return null;
				}
				ParseWhite(s, ref index);
				
				p = ParseUntilComma(s, ref index);
				if(float.TryParse(p, out j15b)){
					c++;
				}
				
				ParseWhite(s, ref index);
				if(!ParseComma(s, ref index)){
					ParseNextSemiColon(s, ref index, array);
					return null;
				}
				ParseWhite(s, ref index);
				
				p = ParseUntilComma(s, ref index);
				if(float.TryParse(p, out j15c)){
					c++;
				}
				
				ParseWhite(s, ref index);
				if(!ParseComma(s, ref index)){
					ParseNextSemiColon(s, ref index, array);
					return null;
				}
				ParseWhite(s, ref index);
				
				p = ParseUntilSemicolon(s, ref index, array);
				if(float.TryParse(p, out j15d)){
					c++;
				}
				
				if(c == 4){
					return new Vec4(j15a, j15b, j15c, j15d);
				}else{
					return null;
				}
			case AshFileTypeV3.Bool:
				p = ParseUntilSemicolon(s, ref index, array);
				if(p == "true"){
					return true;
				}else if(p == "false"){
					return false;
				}else{
					return null;
				}
			case AshFileTypeV3.Date:
				p = ParseUntilSemicolon(s, ref index, array);
				string[] d = p.Split("/");
				if(d.Length != 6){
					return null;
				}
				if(byte.TryParse(d[0], out byte day) && byte.TryParse(d[1], out byte month) && ushort.TryParse(d[2], out ushort year) &&
				byte.TryParse(d[3], out byte hour) && byte.TryParse(d[4], out byte minute) && byte.TryParse(d[5], out byte second)){
					return new Date(second, minute, hour, day, month, year);
				}else{
					return null;
				}
			default:
				return null;
		}
	}
	
	private static string ParseString(string s, ref int index){
		StringBuilder sb = new StringBuilder();
		
		bool previousEscape = false;
		
		while(true){
			if(index >= s.Length){
				throw new AshFileException("Expected '\"' and found end of string");
			}
			
			if(s[index] == '"'){
				if(previousEscape){
					sb.Append(s[index]);
					previousEscape = false;
					index++;
					continue;
				}else{
					index++;
					return sb.ToString();
				}
			}
			
			if(s[index] == '\\'  && !previousEscape){
				previousEscape = true;
			}else{
				previousEscape = false;
			}
			sb.Append(s[index]);
			index++;
		}
	}
	
	private static bool ParseCloseArray(string s, ref int index){
		if(index >= s.Length){
			throw new AshFileException("Expected ']' and found end of string");
		}else if(s[index] == ']'){
			index++;
			return true;
		}else{
			return false;
		}
	}
	
	private static bool ParseComma(string s, ref int index){
		if(index >= s.Length){
			throw new AshFileException("Expected ',' and found end of string");
		}else if(s[index] == ','){
			index++;
			return true;
		}else{
			return false;
		}
	}
	
	private static bool ParseQuote(string s, ref int index){
		if(index >= s.Length){
			throw new AshFileException("Expected '\"' and found end of string");
		}else if(s[index] == '"'){
			index++;
			return true;
		}else{
			return false;
		}
	}
	
	private static bool ParseOpenArray(string s, ref int index){
		if(index >= s.Length){
			throw new AshFileException("Expected value of array start and found end of string");
		}else if(s[index] == '['){
			index++;
			return true;
		}else{
			return false;
		}
	}
	
	private static void ParseSemiColon(string s, ref int index){
		if(index >= s.Length){
			throw new AshFileException("Expected ';' and found end of string");
		}else if(s[index] == ';'){
			index++;
			return;
		}else{
			throw new AshFileException("Expected ';' and found '" + s[index] + "'");
		}
	}
	
	private static void ParseColon(string s, ref int index){
		if(index >= s.Length){
			throw new AshFileException("Expected ':' and found end of string");
		}else if(s[index] == ':'){
			index++;
			return;
		}else{
			throw new AshFileException("Expected ':' and found '" + s[index] + "'");
		}
	}
	
	private static string ParseName(string s, ref int index){
		StringBuilder sb = new StringBuilder();
		
		bool previousEscape = false;
		
		while(true){
			if(index >= s.Length){
				throw new AshFileException("Expected camp name and found end of string");
			}
			
			if(s[index] == '>'){
				if(previousEscape){
					sb.Append(s[index]);
					previousEscape = false;
					index++;
					continue;
				}else{
					return sb.ToString();
				}
			}
			
			if(s[index] == '\\' && !previousEscape){
				previousEscape = true;
			}else{
				previousEscape = false;
			}
			sb.Append(s[index]);
			index++;
		}
	}
	
	private static void ParseCloseName(string s, ref int index){
		if(index >= s.Length){
			throw new AshFileException("Expected '>' and found end of string");
		}else if(s[index] == '>'){
			index++;
			return;
		}else{
			throw new AshFileException("Expected '>' and found '" + s[index] + "'");
		}
	}
	
	private static bool ParseOpenName(string s, ref int index){
		if(index >= s.Length){
			return false;
		}
		if(s[index] == '<'){
			index++;
			return true;
		}
		throw new AshFileException("Expected '<' and found '" + s[index] + "'");
	}
	
	private static void ParseNextCloseArray(string s, ref int index){
		while(true){
			if(index >= s.Length){
				throw new AshFileException("Expected ']' and found end of string");
			}
			
			if(s[index] == ']'){
				index++;
				return;
			}
			
			index++;
		}
	}
	
	private static void ParseNextSemiColon(string s, ref int index, bool array){
		while(true){
			if(index >= s.Length){
				throw new AshFileException("Expected ';' and found end of string");
			}
			
			if(s[index] == ';' || (array && s[index] == ']')){
				return;
			}
			
			index++;
		}
	}
	
	private static string ParseUntilComma(string s, ref int index){
		StringBuilder sb = new StringBuilder();
		
		while(true){
			if(index >= s.Length){
				return sb.ToString();
			}
			
			if(char.IsWhiteSpace(s[index]) || s[index] == ','){
				return sb.ToString();
			}
			
			sb.Append(s[index]);
			index++;
		}
	}
	
	private static string ParseUntilColon(string s, ref int index){
		StringBuilder sb = new StringBuilder();
		
		while(true){
			if(index >= s.Length){
				return sb.ToString();
			}
			
			if(char.IsWhiteSpace(s[index]) || s[index] == ':'){
				return sb.ToString();
			}
			
			sb.Append(s[index]);
			index++;
		}
	}
	
	private static string ParseUntilSemicolon(string s, ref int index, bool array){
		StringBuilder sb = new StringBuilder();
		
		while(true){
			if(index >= s.Length){
				return sb.ToString();
			}
			
			if(char.IsWhiteSpace(s[index]) || s[index] == ';' || (array && s[index] == ']')){
				return sb.ToString();
			}
			
			sb.Append(s[index]);
			index++;
		}
	}
	
	private static void ParseWhite(string s, ref int index){
		while(index < s.Length && char.IsWhiteSpace(s[index])){
			index++;
		}
	}
}