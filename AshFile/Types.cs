using System;
using System.Collections;
using AshLib.Dates;

namespace AshLib.AshFiles;

public enum AshFileTypeOld : byte{ //Represents the type of value in a AshFile camp
	ByteArray = 0,
	String = 1,
	Byte = 2,
	Ushort = 3,
	Uint = 4,
	Ulong = 5,
	Sbyte = 6,
	Short = 7,
	Int = 8,
	Long = 9,
	Color = 10,
	Float = 11,
	Double = 12,
	Vec2 = 13,
	Vec3 = 14,
	Vec4 = 15,
	Bool = 16,
	UbyteArray = 17,
	UshortArray = 18,
	UintArray = 19,
	UlongArray = 20,
	SbyteArray = 21,
	ShortArray = 22,
	IntArray = 23,
	LongArray = 24,
	FloatArray = 25,
	DoubleArray = 26,
	Date = 27
}

public enum AshFileType : byte{ //Represents the type of value in a AshFile camp
	Default = 0,
	String = 1,
	Byte = 2,
	Ushort = 3,
	Uint = 4,
	Ulong = 5,
	Sbyte = 6,
	Short = 7,
	Int = 8,
	Long = 9,
	Color3 = 10,
	Float = 11,
	Double = 12,
	Vec2 = 13,
	Vec3 = 14,
	Vec4 = 15,
	Bool = 16,
	Date = 17
}

public partial class AshFile{
	private static AshFileType GetFileTypeFromType(Type type){
		if(type == typeof(string)) return AshFileType.String;
		if(type == typeof(byte)) return AshFileType.Byte;
		if(type == typeof(ushort)) return AshFileType.Ushort;
		if(type == typeof(uint)) return AshFileType.Uint;
		if(type == typeof(ulong)) return AshFileType.Ulong;
		if(type == typeof(sbyte)) return AshFileType.Sbyte;
		if(type == typeof(short)) return AshFileType.Short;
		if(type == typeof(int)) return AshFileType.Int;
		if(type == typeof(long)) return AshFileType.Long;
		if(type == typeof(Color3)) return AshFileType.Color3;
		if(type == typeof(float)) return AshFileType.Float;
		if(type == typeof(double)) return AshFileType.Double;
		if(type == typeof(Vec2)) return AshFileType.Vec2;
		if(type == typeof(Vec3)) return AshFileType.Vec3;
		if(type == typeof(Vec4)) return AshFileType.Vec4;
		if(type == typeof(bool)) return AshFileType.Bool;
		if(type == typeof(Date)) return AshFileType.Date;
	
		return AshFileType.Default; // Default case if no matching type
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
}