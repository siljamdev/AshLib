using System;
using System.Collections;

namespace AshLib;

public struct Color3{ //Just for holding the data of a RGB color
	public byte R;
	public byte G;
	public byte B;
	
	public static readonly Color3 Black = new Color3(0, 0, 0);
	public static readonly Color3 White = new Color3(255, 255, 255);
	public static readonly Color3 Gray = new Color3(150, 150, 150);
	public static readonly Color3 Magenta = new Color3(255, 0, 255);
	public static readonly Color3 Cyan = new Color3(0, 255, 255);
	public static readonly Color3 Yellow = new Color3(255, 255, 0);
	public static readonly Color3 Blue = new Color3(0, 0, 255);
	public static readonly Color3 Green = new Color3(0, 255, 0);
	public static readonly Color3 Red = new Color3(255, 0, 0);
	
	public Color3(byte r, byte g, byte b){
		this.R = r;
		this.G = g;
		this.B = b;
	}
	
	public Color3(string hex){
		this = Parse(hex);
	}
	
	public static Color3 Parse(string hex){
		if(hex.StartsWith("#")){
            hex = hex.Substring(1);
        }
		
		if(hex.Length != 6){
            throw new Exception("Hexadecimal color string must be 6 or 7 characters long.");
        }
		
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		
		return new Color3(r, g, b);
	}
	
	public static bool TryParse(string hex, out Color3 c){
		if(hex.StartsWith("#")){
            hex = hex.Substring(1);
        }
		
		if(hex.Length != 6){
			c = new Color3(0, 0, 0);
            return false;
        }
		
		try{
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2).Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			
			c = new Color3(r, g, b);
			return true;
		}catch{
			c = new Color3(0, 0, 0);
			return false;
		}
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
	
	public override bool Equals(object obj){
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
	
	public override bool Equals(object obj){
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
	
	public override bool Equals(object obj){
        if (obj is Vec4 other)
            return this == other;
        else
            return false;
    }
	
	public override string ToString(){
		return "(" + this.X + ", " + this.Y + ", " + this.Z + ", " + this.W + ")";
	}
}