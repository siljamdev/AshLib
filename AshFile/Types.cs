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