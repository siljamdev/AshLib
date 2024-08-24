using System;
using System.Collections;

namespace AshLib;

public class CampValue{ //AshFile campValue
	public Type type {get;}
	
	//each possible of the 27 types
	private byte[]? ByteArray;
	private string? String;
	private byte? Byte;
	private ushort? Ushort;
	private uint? Uint;
	private ulong? Ulong;
	private sbyte? Sbyte;
	private short? Short;
	private int? Int;
	private long? Long;
	private Color3? Color;
	private float? Float;
	private double? Double;
	private Vec2? Vec2;
	private Vec3? Vec3;
	private Vec4? Vec4;
	private bool? Bool;
	private byte[]? UbyteArray;
	private ushort[]? UshortArray;
	private uint[]? UintArray;
	private ulong[]? UlongArray;
	private sbyte[]? SbyteArray;
	private short[]? ShortArray;
	private int[]? IntArray;
	private long[]? LongArray;
	private float[]? FloatArray;
	private double[]? DoubleArray;
	private Date? Date;
	
	//Null one, for errors and so
	public static readonly CampValue Null = new CampValue(Type.Invalid);
	
	private static readonly HashSet<Type> EnumerableTypes = new HashSet<Type>
	{
		Type.ByteArray,
		Type.UbyteArray,
		Type.UshortArray,
		Type.UintArray,
		Type.UlongArray,
		Type.SbyteArray,
		Type.ShortArray,
		Type.IntArray,
		Type.LongArray,
		Type.FloatArray,
		Type.DoubleArray
	};
	
	//private constructor for null
	private CampValue(Type t){
		this.type = t;
	}
	
	//Constructors for the 27
	public CampValue(byte[] p)
    {
        type = Type.ByteArray;
        ByteArray = p;
    }

    public CampValue(string p)
    {
        type = Type.String;
        String = p;
    }

    public CampValue(byte p)
    {
        type = Type.Byte;
        Byte = p;
    }

    public CampValue(ushort p)
    {
        type = Type.Ushort;
        Ushort = p;
    }

    public CampValue(uint p)
    {
        type = Type.Uint;
        Uint = p;
    }

    public CampValue(ulong p)
    {
        type = Type.Ulong;
        Ulong = p;
    }

    public CampValue(sbyte p)
    {
        type = Type.Sbyte;
        Sbyte = p;
    }

    public CampValue(short p)
    {
        type = Type.Short;
        Short = p;
    }

    public CampValue(int p)
    {
        type = Type.Int;
        Int = p;
    }

    public CampValue(long p)
    {
        type = Type.Long;
        Long = p;
    }

    public CampValue(Color3 p)
    {
        type = Type.Color;
        Color = p;
    }

    public CampValue(float p)
    {
        type = Type.Float;
        Float = p;
    }

    public CampValue(double p)
    {
        type = Type.Double;
        Double = p;
    }

    public CampValue(Vec2 p)
    {
        type = Type.Vec2;
        Vec2 = p;
    }

    public CampValue(Vec3 p)
    {
        type = Type.Vec3;
        Vec3 = p;
    }

    public CampValue(Vec4 p)
    {
        type = Type.Vec4;
        Vec4 = p;
    }

    public CampValue(bool p)
    {
        type = Type.Bool;
        Bool = p;
    }

    public CampValue(ushort[] p)
    {
        type = Type.UshortArray;
        UshortArray = p;
    }

    public CampValue(uint[] p)
    {
        type = Type.UintArray;
        UintArray = p;
    }

    public CampValue(ulong[] p)
    {
        type = Type.UlongArray;
        UlongArray = p;
    }

    public CampValue(sbyte[] p)
    {
        type = Type.SbyteArray;
        SbyteArray = p;
    }

    public CampValue(short[] p)
    {
        type = Type.ShortArray;
        ShortArray = p;
    }

    public CampValue(int[] p)
    {
        type = Type.IntArray;
        IntArray = p;
    }

    public CampValue(long[] p)
    {
        type = Type.LongArray;
        LongArray = p;
    }

    public CampValue(float[] p)
    {
        type = Type.FloatArray;
        FloatArray = p;
    }

    public CampValue(double[] p)
    {
        type = Type.DoubleArray;
        DoubleArray = p;
    }
	
	public CampValue(Date p){
		type = Type.Date;
		Date = p;
	}
	
	//For getting the value
	public object GetValue(){
		switch (type){
			case Type.ByteArray:
				return ByteArray;
			case Type.String:
				return String;
			case Type.Byte:
				return Byte;
			case Type.Ushort:
				return Ushort;
			case Type.Uint:
				return Uint;
			case Type.Ulong:
				return Ulong;
			case Type.Sbyte:
				return Sbyte;
			case Type.Short:
				return Short;
			case Type.Int:
				return Int;
			case Type.Long:
				return Long;
			case Type.Color:
				return Color;
			case Type.Float:
				return Float;
			case Type.Double:
				return Double;
			case Type.Vec2:
				return Vec2;
			case Type.Vec3:
				return Vec3;
			case Type.Vec4:
				return Vec4;
			case Type.Bool:
				return Bool;
			case Type.UbyteArray:
				return UbyteArray;
			case Type.UshortArray:
				return UshortArray;
			case Type.UintArray:
				return UintArray;
			case Type.UlongArray:
				return UlongArray;
			case Type.SbyteArray:
				return SbyteArray;
			case Type.ShortArray:
				return ShortArray;
			case Type.IntArray:
				return IntArray;
			case Type.LongArray:
				return LongArray;
			case Type.FloatArray:
				return FloatArray;
			case Type.DoubleArray:
				return DoubleArray;
			case Type.Date:
				return Date;
			default:
				return null;
		}
	}
	
	//CanGet... methods, with an out arg for output
	public bool CanGetByteArray(out byte[] p){
		if(type == Type.ByteArray){
			p = (byte[])ByteArray;
			return true;
		}
		p = (byte[])ByteArray;
		return false;
	}
	
	public bool CanGetString(out string p){
		if(type == Type.String){
			p = (string)String;
			return true;
		}
		p = (string)String;
		return false;
	}
	
	public bool CanGetByte(out byte p){
		if(type == Type.Byte){
			p = (byte)Byte;
			return true;
		}
		p = (byte)0;
		return false;
	}
	
	public bool CanGetUshort(out ushort p){
		if(type == Type.Ushort){
			p = (ushort)Ushort;
			return true;
		}
		p = (ushort)0;
		return false;
	}
	
	public bool CanGetUint(out uint p){
		if(type == Type.Uint){
			p = (uint)Uint;
			return true;
		}
		p = (uint)0;
		return false;
	}
	
	public bool CanGetUlong(out ulong p){
		if(type == Type.Ulong){
			p = (ulong)Ulong;
			return true;
		}
		p = (ulong)0;
		return false;
	}
	
	public bool CanGetSbyte(out sbyte p){
		if(type == Type.Sbyte){
			p = (sbyte)Sbyte;
			return true;
		}
		p = (sbyte)0;
		return false;
	}
	
	public bool CanGetShort(out short p){
		if(type == Type.Short){
			p = (short)Short;
			return true;
		}
		p = (short)0;
		return false;
	}
	
	public bool CanGetInt(out int p){
		if(type == Type.Int){
			p = (int)Int;
			return true;
		}
		p = (int)0;
		return false;
	}
	
	public bool CanGetLong(out long p){
		if(type == Type.Long){
			p = (long)Long;
			return true;
		}
		p = (long)0;
		return false;
	}
	
	public bool CanGetColor(out Color3 p){
		if(type == Type.Color){
			p = (Color3)Color;
			return true;
		}
		p = (Color3)new Color3(0, 0, 0);
		return false;
	}
	
	public bool CanGetFloat(out float p){
		if(type == Type.Float){
			p = (float)Float;
			return true;
		}
		p = (float)0;
		return false;
	}
	
	public bool CanGetDouble(out double p){
		if(type == Type.Double){
			p = (double)Double;
			return true;
		}
		p = (double)0;
		return false;
	}
	
	public bool CanGetVec2(out Vec2 p){
		if(type == Type.Vec2){
			p = (Vec2)Vec2;
			return true;
		}
		p = (Vec2)new Vec2(0f, 0f);
		return false;
	}
	
	public bool CanGetVec3(out Vec3 p){
		if(type == Type.Vec3){
			p = (Vec3)Vec3;
			return true;
		}
		p = (Vec3)new Vec3(0f, 0f, 0f);
		return false;
	}
	
	public bool CanGetVec4(out Vec4 p){
		if(type == Type.Vec4){
			p = (Vec4)Vec4;
			return true;
		}
		p = (Vec4)new Vec4(0f, 0f, 0f, 0f);
		return false;
	}
	
	public bool CanGetBool(out bool p){
		if(type == Type.Bool){
			p = (bool)Bool;
			return true;
		}
		p = (bool)false;
		return false;
	}
	
	public bool CanGetUbyteArray(out byte[] p){
		if(type == Type.UbyteArray){
			p = (byte[])UbyteArray;
			return true;
		}
		p = (byte[])UbyteArray;
		return false;
	}
	
	public bool CanGetUshortArray(out ushort[] p){
		if(type == Type.UshortArray){
			p = (ushort[])UshortArray;
			return true;
		}
		p = (ushort[])UshortArray;
		return false;
	}
	
	public bool CanGetUintArray(out uint[] p){
		if(type == Type.UintArray){
			p = (uint[])UintArray;
			return true;
		}
		p = (uint[])UintArray;
		return false;
	}
	
	public bool CanGetUlongArray(out ulong[] p){
		if(type == Type.UlongArray){
			p = (ulong[])UlongArray;
			return true;
		}
		p = (ulong[])UlongArray;
		return false;
	}
	
	public bool CanGetSbyteArray(out sbyte[] p){
		if(type == Type.SbyteArray){
			p = (sbyte[])SbyteArray;
			return true;
		}
		p = (sbyte[])SbyteArray;
		return false;
	}
	
	public bool CanGetShortArray(out short[] p){
		if(type == Type.ShortArray){
			p = (short[])ShortArray;
			return true;
		}
		p = (short[])ShortArray;
		return false;
	}
	
	public bool CanGetIntArray(out int[] p){
		if(type == Type.IntArray){
			p = (int[])IntArray;
			return true;
		}
		p = (int[])IntArray;
		return false;
	}
	
	public bool CanGetLongArray(out long[] p){
		if(type == Type.LongArray){
			p = (long[])LongArray;
			return true;
		}
		p = (long[])LongArray;
		return false;
	}
	
	public bool CanGetFloatArray(out float[] p){
		if(type == Type.FloatArray){
			p = (float[])FloatArray;
			return true;
		}
		p = (float[])FloatArray;
		return false;
	}
	
	public bool CanGetDoubleArray(out double[] p){
		if(type == Type.DoubleArray){
			p = (double[])DoubleArray;
			return true;
		}
		p = (double[])DoubleArray;
		return false;
	}
	
	public bool CanGetDate(out Date p){
		if(type == Type.Date){
			p = (Date)Date;
			return true;
		}
		p = (Date)AshLib.Date.Invalid;
		return false;
	}
	
	//For printing arrays
	private string PrintEnumerable(){
		IEnumerable e = (IEnumerable) this.GetValue();
		string s = "";
		foreach(var i in e){
			s += i + " ";
		}
		return s;
	}
	
	public static bool operator ==(CampValue a, CampValue b){
		if(a.type != b.type){
			return false;
		}
		dynamic c = a.GetValue();
		dynamic d = b.GetValue();
		
		return (c == d);
	}
	
	public static bool operator !=(CampValue a, CampValue b){
		return !(a == b);
	}
	
	public override bool Equals(object obj)
    {
        if (obj is CampValue other)
            return this == other;
        else
            return false;
    }
	
	public override string ToString(){
		if (EnumerableTypes.Contains(this.type)){
			return this.PrintEnumerable();
		}
		return this.GetValue().ToString();
	}
}