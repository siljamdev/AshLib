using System;
using System.Collections;

namespace AshLib;

public struct Color3{ //Just for holding the data of a RGB color
	public byte R;
	public byte G;
	public byte B;
	
	public Color3(byte r, byte g, byte b){
		this.R = r;
		this.G = g;
		this.B = b;
	}
	
	public Color3(string hex){
		if(hex.StartsWith("#")){
            hex = hex.Substring(1);
        }
		
		if(hex.Length != 6){
            throw new Exception("Hexadecimal color string must be 6 or 7 characters long.");
        }
		
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		
		this.R = r;
		this.G = g;
		this.B = b;
	}
	
	public static implicit operator System.Drawing.Color(Color3 c){ //Can cast from and to the System.Drawing
		return System.Drawing.Color.FromArgb(c.R, c.G, c.B);
	}
	
	public static implicit operator Color3(System.Drawing.Color c){
		return new Color3(c.R, c.G, c.B);
	}
	
	public static bool operator ==(Color3 a, Color3 b){
		if(a.R == b.R && a.G == b.G && a.B == b.B){
			return true;
		}
		return false;
	}
	
	public static bool operator !=(Color3 a, Color3 b){
		return !(a == b);
	}
	
	public override bool Equals(object obj)
    {
        if (obj is Color3 other)
            return this == other;
        else
            return false;
    }
	
	public override string ToString(){
		return "#" + this.R.ToString("X2") + this.G.ToString("X2") + this.B.ToString("X2");
	}
}

public struct Vec2{ //for holding two float values together, no special funcionality
	public float X;
	public float Y;
	
	public Vec2(float x, float y){
		this.X = x;
		this.Y = y;
	}
	
	public static bool operator ==(Vec2 a, Vec2 b){
		if(a.X == b.X && a.Y == b.Y){
			return true;
		}
		return false;
	}
	
	public static bool operator !=(Vec2 a, Vec2 b){
		return !(a == b);
	}
	
	public override bool Equals(object obj)
    {
        if (obj is Vec2 other)
            return this == other;
        else
            return false;
    }
	
	public override string ToString(){
		return "(" + this.X + ", " + this.Y + ")";
	}
}

public struct Vec3{//for holding three float values together, no special funcionality
	public float X;
	public float Y;
	public float Z;
	
	public Vec3(float x, float y, float z){
		this.X = x;
		this.Y = y;
		this.Z = z;
	}
	
	public static bool operator ==(Vec3 a, Vec3 b){
		if(a.X == b.X && a.Y == b.Y && a.Z == b.Z){
			return true;
		}
		return false;
	}
	
	public static bool operator !=(Vec3 a, Vec3 b){
		return !(a == b);
	}
	
	public override bool Equals(object obj)
    {
        if (obj is Vec3 other)
            return this == other;
        else
            return false;
    }
	
	public override string ToString(){
		return "(" + this.X + ", " + this.Y + ", " + this.Z + ")";
	}
}

public struct Vec4{//for holding four float values together, no special funcionality
	public float X;
	public float Y;
	public float Z;
	public float W;
	
	public Vec4(float x, float y, float z, float w){
		this.X = x;
		this.Y = y;
		this.Z = z;
		this.W = w;
	}
	
	public static bool operator ==(Vec4 a, Vec4 b){
		if(a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W){
			return true;
		}
		return false;
	}
	
	public static bool operator !=(Vec4 a, Vec4 b){
		return !(a == b);
	}
	
	public override bool Equals(object obj)
    {
        if (obj is Vec4 other)
            return this == other;
        else
            return false;
    }
	
	public override string ToString(){
		return "(" + this.X + ", " + this.Y + ", " + this.Z + ", " + this.W + ")";
	}
}

public struct Date{ //Represents a time between the year 1488 and 2511, down to seconds
	public byte seconds {get;}
	public byte minutes {get;}
	public byte hours {get;}
	public byte days {get;}
	public byte months {get;}
	public ushort years {get{return (ushort)(_years + 1488);}}
	private ushort _years;
	
	private bool isInvalid;
	
	public static readonly Date Invalid = new Date();
	
	public Date(byte s, byte m, byte h, byte d, byte mo, ushort y){ //Numbers will be cropped
		if(s > 59){
			s = 59;
		}
		seconds = s;
		if(m > 59){
			m = 59;
		}
		minutes = m;
		if(h > 23){
			h = 23;
		}
		hours = h;
		if(d > 31){
			d = 31;
		}
		if(d < 1){
			d = 1;
		}
		days = d;
		if(mo > 12){
			mo = 12;
		}
		if(mo < 1){
			mo = 1;
		}
		months = mo;
		if(y > 2511){
			y = 2511;
		}
		if(y < 1488){
			y = 1488;
		}
		_years = (ushort) (y - 1488);
		
		this.isInvalid = false;
	}
	
	public Date(string cptf){ //Constructor directly from CPTF
		this = FromCPTF(cptf);
		
		this.isInvalid = false;
	}
	
	public Date(){
		this.isInvalid = true;
	}
	
	public string ToCPTF() //Stands for Compressed printable date format. Format to represent dates in 6 base64 charachters. 
    {
        if(this.isInvalid){
			return "Invalid";
		}
		
		// Combine all parts into a single ulong
        ulong combined = ((ulong)seconds & 0x3F) | 
                         (((ulong)minutes & 0x3F) << 6) |
                         (((ulong)hours & 0x1F) << 12) |
                         (((ulong)days & 0x1F) << 17) |
                         (((ulong)months & 0x0F) << 22) |
                         (((ulong)_years & 0x03FF) << 26);
						 

        // Convert ulong to byte array
        byte[] bytes = BitConverter.GetBytes(combined);
		
		byte[] fbytes = new byte[5];
		for(int i = 0; i < 5; i++){
			fbytes[i] = bytes[i];
		}
		
		fbytes[4] = (byte)(fbytes[4] << 4);
		
        // Encode byte array to base64
        string base64 = Convert.ToBase64String(fbytes);

        // Trim the padding (if any)
        base64 =  base64.TrimEnd('=');
        return base64.Substring(0, 6);
    }
	
	public static Date FromCPTF(string cptf) //Stands for Compressed printable date format. Format to represent dates in 6 base64 charachters. 
	{
		// Pad the base64 string to ensure it can be decoded properly
		cptf += "A";
		cptf = cptf.PadRight(8, '=');
	
		// Decode base64 to byte array
		byte[] fbytes = Convert.FromBase64String(cptf);
		
		fbytes[4] = (byte)(fbytes[4] >> 4);

		// Convert the byte array to ulong
		ulong combined = 0;
		for (int i = 0; i < 5; i++)
		{
			combined |= (ulong)fbytes[i] << (i * 8);
		}
		
	
		// Extract each component
		byte seconds = (byte)(combined & 0x3F);
		byte minutes = (byte)((combined >> 6) & 0x3F);
		byte hours = (byte)((combined >> 12) & 0x1F);
		byte days = (byte)((combined >> 17) & 0x1F);
		byte months = (byte)((combined >> 22) & 0x0F);
		ushort years = (ushort)((combined >> 26) & 0x03FF);
	
		return new Date(seconds, minutes, hours, days, months, (ushort)(years + 1488));
	}
	
	public static explicit operator DateTime(Date d){ //Can transform to and from DateTime
		return new DateTime(d.years, d.months, d.days, d.hours, d.minutes, d.seconds);
	}
	
	public static explicit operator Date(DateTime d){
		return new Date((byte) d.Second, (byte) d.Minute, (byte) d.Hour, (byte) d.Day, (byte) d.Month, (ushort) d.Year);
	}
	
	public static bool operator ==(Date a, Date b){
		if(a.seconds == b.seconds && a.minutes == b.minutes && a.hours == b.hours && a.days == b.days && a.months == b.months && a.years == b.years){
			return true;
		}
		return false;
	}
	
	public static bool operator !=(Date a, Date b){
		return !(a == b);
	}
	
	public override bool Equals(object obj)
    {
        if (obj is Date other)
            return this == other;
        else
            return false;
    }
	
	public override string ToString(){
		if(this.isInvalid){
			return "Invalid";
		}
		
		return days + "/" + months + "/" + years + " " + hours + ":" + minutes + ":" + seconds;
	}
	
}

public enum Type{ //Represents the type of value in a AshFile camp
	Invalid = -1,
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