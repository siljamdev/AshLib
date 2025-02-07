using System;

namespace AshLib.Dates;

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