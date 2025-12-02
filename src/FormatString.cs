using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace AshLib.Formatting;

public class CharFormat{
	public byte? density{get;} //0 is normal, 1 is bold, 2 is thin
	public bool? italic{get;} 
	public byte? underline{get;}  //0 is not, 1 is single, 2 is double
	public bool? strikeThrough{get;} 
	
	public Color3? foreground{get;} //null is skip
	public bool foregroundReset{get;}
	public Color3? background{get;} //null is skip
	public bool backgroundReset{get;}
	
	public static CharFormat ResetAll = new CharFormat(0, false, 0, false, null, true, null, true);
	
	public CharFormat(byte? dens, bool? ital, byte? uline, bool? sthrough, Color3? fgcolor, bool fgreset, Color3? bgcolor, bool bgreset){
		density = dens;
		italic = ital;
		underline = uline;
		strikeThrough = sthrough;
		foreground = fgcolor;
		foregroundReset = fgreset;
		background = bgcolor;
		backgroundReset = bgreset;
	}
	
	public CharFormat(Color3? fgcolor, bool fgreset, Color3? bgcolor, bool bgreset){
		density = null;
		italic = null;
		underline = null;
		strikeThrough = null;
		foreground = fgcolor;
		foregroundReset = fgreset;
		background = bgcolor;
		backgroundReset = bgreset;
	}
	
	public CharFormat(Color3? fgcolor, Color3? bgcolor){
		density = null;
		italic = null;
		underline = null;
		strikeThrough = null;
		foreground = fgcolor;
		foregroundReset = false;
		background = bgcolor;
		backgroundReset = false;
	}
	
	public CharFormat(Color3? fgcolor, bool fgreset){
		density = null;
		italic = null;
		underline = null;
		strikeThrough = null;
		foreground = fgcolor;
		foregroundReset = fgreset;
		background = null;
		backgroundReset = false;
	}
	
	public CharFormat(Color3? fgcolor){
		density = null;
		italic = null;
		underline = null;
		strikeThrough = null;
		foreground = fgcolor;
		foregroundReset = false;
		background = null;
		backgroundReset = false;
	}
	
	public CharFormat(){
		density = null;
		italic = null;
		underline = null;
		strikeThrough = null;
		foreground = null;
		foregroundReset = false;
		background = null;
		backgroundReset = false;
	}
	
	public override string ToString(){
		return "#CharFormat# Density: " + (density == null ? "Default" : (density == 0 ? "Normal" : (density == 1 ? "Bold" : (density == 2 ? "Thin" : "Invalid or Unknown"))))
		+ " Italic: " + (italic == null ? "Default" : (italic == true ? "Activated" : "Deactivated"))
		+ " Underlined: " + (underline == null ? "Default" : (underline == 0 ? "Normal" : (underline == 1 ? "Single" : (underline == 2 ? "Double" : "Invalid or Unknown"))))
		+ " Strike-Through: " + (strikeThrough == null ? "Default" : (strikeThrough == true ? "Activated" : "Deactivated"))
		+ " Foreground Color: " + (foreground == null ? "Default" : (foregroundReset == true ? "Reset" : foreground.ToString()))
		+ " Background Color: " + (background == null ? "Default" : (foregroundReset == true ? "Reset" : background.ToString()));
	}
	
	public static bool operator ==(CharFormat a, CharFormat b){
		// Check for null references
		if(ReferenceEquals(a, b)) return true;
		if(a is null && b is null) return true;
		if(a is null || b is null) return false;

		// Compare all properties
		return a.density == b.density &&
			a.italic == b.italic &&
			a.underline == b.underline &&
			a.strikeThrough == b.strikeThrough &&
			a.foreground == b.foreground &&
			a.background == b.background;
    }
	
	public static bool operator !=(CharFormat a, CharFormat b){
        return !(a==b);
    }
	
	public override bool Equals(object obj){
        if (obj is CharFormat other)
            return this == other;
        else
            return false;
    }
}

public class FormatString : ICollection<(char, CharFormat?)>, ICloneable, IEquatable<FormatString>
{
	
	//WINDOWS TERMINAL THINGS
	#region W
    private const int  STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_PROCESSED_OUTPUT = 0x0001;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
	
	[DllImport("kernel32")]
	private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

	[DllImport("kernel32")]
	private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

	[DllImport("kernel32")]
	private static extern IntPtr GetStdHandle(int nStdHandle);
	
	//================================
	
	private static bool hasConsoleBeenPrepared = false;
	#endregion W
	
	public static bool usesColors = true;
	
	public bool addFinalReset = true;
	
	public string content{get{
		return new string(privateContent.ToArray());
	}}
	private List<char> privateContent{get;}
	public List<CharFormat?> format{get;}
	
	private string _built;
	public string built{get{
		if(!hasConsoleBeenPrepared && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
			PrepareConsole();
			hasConsoleBeenPrepared = true;
		}
		
		if(flagToBuild){
			Build();
		}
		return _built;
	}}
	
	public int Length{
		get{
			return privateContent.Count;
		}
	}
	
	public int Count{
		get{
			return privateContent.Count;
		}
	}
	
	public bool IsReadOnly{
		get{
			return false;
		}
	}
	
	private bool flagToBuild;
	
	static FormatString(){
		string noColor = Environment.GetEnvironmentVariable("NO_COLOR");
		if(noColor != null){
			usesColors = false;
		}
	}
	
	public FormatString(){
		privateContent = new List<char>();
		format = new List<CharFormat?>();
		flagToBuild = true;
	}
	
	public FormatString(FormatString fs) : this(){
		Append(fs);
	}
	
	public FormatString(ICollection<(char, CharFormat?)> fs) : this(){
		Append(fs);
	}
	
	public FormatString(string s) : this(){
		Append(s);
	}
	
	public FormatString(string s, CharFormat? f) : this(){
		Append(s, f);
	}
	
	public FormatString(params (string, CharFormat?)[] b) : this(){
		foreach((string s, CharFormat? f) in b){
			Append(s, f);
		}
	}
	
	#region IEnumerator
	public IEnumerator<(char, CharFormat?)> GetEnumerator(){
		for(int i = 0; i < privateContent.Count; i++){
			yield return (privateContent[i], format[i]);
		}
	}
	
	//IEnumerable method
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
		return GetEnumerator();
	}
	#endregion IEnumerator
	
	private static void PrepareConsole(){
		var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
        var _ = GetConsoleMode(iStdOut, out var outConsoleMode)
        && SetConsoleMode(iStdOut, outConsoleMode | ENABLE_PROCESSED_OUTPUT | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
	}
	
	public void Clear(){
		privateContent.Clear();
		format.Clear();
		flagToBuild = true;
	}
	
	public object Clone(){
		return Clone(this);
	}
	
	public static FormatString Clone(FormatString fs){
		FormatString f = new FormatString(fs);
		return f;
	}
	
	private void Build(){
		CharFormat lastFormat = new CharFormat();
		StringBuilder final = new StringBuilder();
		
		if(privateContent.Count > format.Count){
			_built = new string(privateContent.ToArray());
			flagToBuild = false;
			return;
		}
		
		for(int i = 0; i < privateContent.Count; i++){
			CharFormat current = format[i];
			
			if(current == null){
				lastFormat = current;
				final.Append(privateContent[i]);
				continue;
			}
			
			if(current == lastFormat){
				lastFormat = current;
				final.Append(privateContent[i]);
				continue;
			}
			
			bool formatChanges = false;
			List<string> changes = new List<string>();
			
			if(lastFormat == null || current.density != lastFormat.density){
				switch(current.density){
					case 0:
					formatChanges = true;
					changes.Add("22");
					break;
					
					case 1:
					formatChanges = true;
					changes.Add("1");
					break;
					
					case 2:
					formatChanges = true;
					changes.Add("2");
					break;
				}
			}
			
			if(lastFormat == null || current.italic != lastFormat.italic){
				switch(current.italic){
					case true:
					formatChanges = true;
					changes.Add("3");
					break;
					
					case false:
					formatChanges = true;
					changes.Add("23");
					break;
				}
			}
			
			if(lastFormat == null || current.underline != lastFormat.underline){
				switch(current.underline){
					case 0:
					formatChanges = true;
					changes.Add("24");
					break;
					
					case 1:
					formatChanges = true;
					changes.Add("4");
					break;
					
					case 2:
					formatChanges = true;
					changes.Add("21");
					break;
				}
			}
			
			if(lastFormat == null || current.strikeThrough != lastFormat.strikeThrough){
				switch(current.strikeThrough){
					case true:
					formatChanges = true;
					changes.Add("9");
					break;
					
					case false:
					formatChanges = true;
					changes.Add("29");
					break;
				}
			}
			
			if((lastFormat == null || (current.foreground != lastFormat.foreground && current.foreground != null) || current.foregroundReset == true) && usesColors){
				if(current.foregroundReset == true){
					formatChanges = true;
					changes.Add("39");
				}else if(current.foreground != null){
					formatChanges = true;
					changes.Add("38");
					changes.Add("2");
					Color3 foregroundColor = current.foreground.Value;
					changes.Add(foregroundColor.R.ToString());
					changes.Add(foregroundColor.G.ToString());
					changes.Add(foregroundColor.B.ToString());
				}
			}
			
			if((lastFormat == null || (current.background != lastFormat.background && current.background != null) || current.backgroundReset == true) && usesColors){
				if(current.backgroundReset == true){
					formatChanges = true;
					changes.Add("49");
				}else if(current.background != null){
					formatChanges = true;
					changes.Add("48");
					changes.Add("2");
					Color3 backgroundColor = current.background.Value;
					changes.Add(backgroundColor.R.ToString());
					changes.Add(backgroundColor.G.ToString());
					changes.Add(backgroundColor.B.ToString());
				}
			}
			lastFormat = current;
			
			if(formatChanges){
				final.Append("\x1b[");
				for(int j = 0; j < changes.Count; j++){
					final.Append(changes[j]);
					if(j != changes.Count - 1){
						final.Append(";");
					}
				}
				final.Append("m");
			}
			final.Append(privateContent[i]);
		}
		if(addFinalReset){
			final.Append("\x1b[0m");
		}
		flagToBuild = false;
		_built = final.ToString();
	}
	
	public void Append(string s, CharFormat? f){
		if(string.IsNullOrEmpty(s)){
			return;
		}
		privateContent.AddRange(s.ToCharArray());
		for(int i = 0; i < s.Length; i++){
			format.Add(f);
		}
		flagToBuild = true;
	}
	
	public void Append(object s, CharFormat? f){
		if(s == null){
			return;
		}
		string o = s.ToString();
		if(string.IsNullOrEmpty(o)){
			return;
		}
		privateContent.AddRange(o.ToCharArray());
		for(int i = 0; i < o.Length; i++){
			format.Add(f);
		}
		flagToBuild = true;
	}
	
	public void Append(string s, IEnumerable<CharFormat?> f){
		if(string.IsNullOrEmpty(s) || f == null){
			return;
		}
		if(s.Length != f.Count()){
			return;
		}
		privateContent.AddRange(s.ToCharArray());
		format.AddRange(f);
		flagToBuild = true;
	}
	
	public void Append(FormatString f){
		if(f == null){
			return;
		}
		privateContent.AddRange(f.privateContent.ToArray());
		format.AddRange(f.format.ToArray());
		flagToBuild = true;
	}
	
	public void Append(ICollection<(char, CharFormat?)> f){
		if(f == null){
			return;
		}
		privateContent.AddRange(f.Select(n => n.Item1));
		format.AddRange(f.Select(n => n.Item2));
		flagToBuild = true;
	}
	
	public void Append((char, CharFormat?) f){
		privateContent.Add(f.Item1);
		format.Add(f.Item2);
		flagToBuild = true;
	}
	
	public void Add((char, CharFormat?) f){
		Append(f);
	}
	
	/* public void Append(string s, params object[] objs){
		if(string.IsNullOrEmpty(s) || objs == null){
			return;
		}
		StringBuilder sb = new StringBuilder();
		for(int i = 0; i < s.Length; i++){
			if(s[i] == '{' && s[i+1] == '$'){
				int x = i + 2;
				string n = UntilNextCurlyBrace(s, ref x);
				int k;
				if(int.TryParse(n, out k) && k > -1 && objs != null && k < objs.Length){
					sb.Append(objs[k].ToString());
					i = x - 1;
				}
			}else{
				sb.Append(s[i]);
			}
		}
		Append(sb.ToString());
		flagToBuild = true;
	} */
	
	public void Append(string s){
		if(string.IsNullOrEmpty(s)){
			return;
		}
		CharFormat lastFormat = null;
		for(int i = 0; i < s.Length; i++){
			if(s[i] == '/' && i + 1 < s.Length && s[i+1] == '[' && (i == 0 || s[i-1] != '\\')){
				int x = i;
				x += 2;
				
				CharFormat f = new CharFormat();
				bool hasChanged = false;
				
				while(true){
					bool increment = false;
					switch(UntilNextComma(s, ref x).ToUpper()){
						case "B": //Bold density
						f = new CharFormat(1, f.italic, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						hasChanged = true;
						increment = true;
						break;
						
						case "T": //Thin density
						f = new CharFormat(2, f.italic, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						hasChanged = true;
						increment = true;
						break;
						
						case "RT":
						case "RD":
						case "D": //Normal density
						f = new CharFormat(0, f.italic, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						hasChanged = true;
						increment = true;
						break;
						
						case "I": //Italic
						f = new CharFormat(f.density, true, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "RI": //No italic
						f = new CharFormat(f.density, false, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "U": //Underlined
						f = new CharFormat(f.density, f.italic, 1, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "DU": //Double underline
						f = new CharFormat(f.density, f.italic, 2, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "RU": //No underline
						f = new CharFormat(f.density, f.italic, 0, f.strikeThrough, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "S": //Strike-Through
						f = new CharFormat(f.density, f.italic, f.underline, true, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "RS": //No strikeThrough
						f = new CharFormat(f.density, f.italic, f.underline, false, f.foreground, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "C":
						case "F": //Foreground color in RGB
						byte r;
						if(!byte.TryParse(UntilNextComma(s, ref x), out r)){
							break;
						}
						byte g;
						if(!byte.TryParse(UntilNextComma(s, ref x), out g)){
							break;
						}
						byte b;
						if(!byte.TryParse(UntilNextComma(s, ref x), out b)){
							break;
						}
						f = new CharFormat(f.density, f.italic, f.underline, f.strikeThrough, new Color3(r, g, b), f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "C#":
						case "F#": //Foreground color in hex
						string h = UntilNextComma(s, ref x);
						Color3 v;
						if(!Color3.TryParse(h, out v)){
							break;
						}
						f = new CharFormat(f.density, f.italic, f.underline, f.strikeThrough, v, f.foregroundReset, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "RC":
						case "RF": //Reset foreground color
						f = new CharFormat(f.density, f.italic, f.underline, f.strikeThrough, f.foreground, true, f.background, f.backgroundReset);
						increment = true;
						break;
						
						case "BG": //background color RGB
						if(!byte.TryParse(UntilNextComma(s, ref x), out r)){
							break;
						}
						if(!byte.TryParse(UntilNextComma(s, ref x), out g)){
							break;
						}
						if(!byte.TryParse(UntilNextComma(s, ref x), out b)){
							break;
						}
						f = new CharFormat(f.density, f.italic, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, new Color3(r, g, b), f.backgroundReset);
						increment = true;
						break;
						
						case "BG#": //Background color hex
						h = UntilNextComma(s, ref x);
						if(!Color3.TryParse(h, out v)){
							break;
						}
						f = new CharFormat(f.density, f.italic, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, v, f.backgroundReset);
						increment = true;
						break;
						
						case "RB": //Reset background color
						f = new CharFormat(f.density, f.italic, f.underline, f.strikeThrough, f.foreground, f.foregroundReset, f.background, true);
						increment = true;
						break;
						
						case "0": //Reset all
						f = CharFormat.ResetAll;
						increment = true;
						break;
						
					}
					
					if(increment){
						hasChanged = true;
					}
					
					if(x >= s.Length || s[x] == ']'){
						break;
					}
				}
				
				if(hasChanged){
					x++;
					
					i = x;
					lastFormat = f;
				}
			}
			
			if(s[i] == '\\' && i + 2 < s.Length && s[i+1] == '/' && s[i+2] == '['){
				continue;
			}
			
			if(i > s.Length - 1){
				continue;
			}
			privateContent.Add(s[i]);
			format.Add(lastFormat);
		}
		flagToBuild = true;
	}
	
	private string UntilNextComma(string s, ref int index){
		StringBuilder sb = new StringBuilder();
		int ind = index;
		while(true){
			if(ind > s.Length - 1){
				return sb.ToString();
			}
			if(s[ind] == ','){
				ind++;
				index = ind;
				return sb.ToString();
			}
			if(s[ind] == ']'){
				index = ind;
				return sb.ToString();
			}
			sb.Append(s[ind]);
			ind++;
		}
	}
	
	/* private string UntilNextCurlyBrace(string s, ref int index){
		StringBuilder sb = new StringBuilder();
		int ind = index;
		while(true){
			if(ind > s.Length - 1){
				return sb.ToString();
			}
			if(s[ind] == '}'){
				ind++;
				index = ind;
				return sb.ToString();
			}
			sb.Append(s[ind]);
			ind++;
		}
	} */
	
	public bool Contains((char, CharFormat?) f){
		for(int i = 0; i < privateContent.Count; i++){
			if(privateContent[i] == f.Item1 && format[i] == f.Item2){
				return true;
			}
		}
		return false;
	}
	
	public bool Remove((char, CharFormat?) f){
		for(int i = 0; i < privateContent.Count; i++){
			if(privateContent[i] == f.Item1 && format[i] == f.Item2){
				privateContent.RemoveAt(i);
				format.RemoveAt(i);
				return true;
			}
		}
		return false;
	}
	
	public void CopyTo((char, CharFormat?)[] array, int index){
		(char, CharFormat?)[] f= privateContent.Zip(format, (first, second) => (first, second)).ToArray();
		f.CopyTo(array, index);
	}
	
	public FormatString Substring(int si, int n){
		if(si < 0 || n < 0 || si >= Length){
			return new FormatString();
		}
		
		if(si + n > Length){
			n = Length - si;
		}
		
		FormatString f = new FormatString();
		f.Append(new string(privateContent.GetRange(si, n).ToArray()), format.GetRange(si, n).ToArray());
		
		return f;
	}
	
	public FormatString[] SplitIntoLines(){
		List<FormatString> l = new();
		
		int s = 0;
		
		int i;
		for(i = 0; i < privateContent.Count; i++){
			if(privateContent[i] == '\r'){
				if(i + 1 < privateContent.Count && privateContent[i + 1] == '\n'){
					i++;
					FormatString f = new FormatString();
					f.Append(new string(privateContent.GetRange(s, i - s).ToArray()), format.GetRange(s, i - s).ToArray());
					l.Add(f);
					s = i + 1;
				}else{
					FormatString f = new FormatString();
					f.Append(new string(privateContent.GetRange(s, i - s).ToArray()), format.GetRange(s, i - s).ToArray());
					l.Add(f);
					s = i + 1;
				}
			}else if(privateContent[i] == '\n'){
				FormatString f = new FormatString();
				f.Append(new string(privateContent.GetRange(s, i - s).ToArray()), format.GetRange(s, i - s).ToArray());
				l.Add(f);
				s = i + 1;
			}
		}
		
		if(i != s){
			FormatString f = new FormatString();
			f.Append(new string(privateContent.GetRange(s, i - s).ToArray()), format.GetRange(s, i - s).ToArray());
			l.Add(f);
		}
		
		return l.ToArray();
	}
	
	public bool RemoveFromStart(int n){
		if(n < 0){
			return false;
		}
		
		if(n > Length){
			n = Length;
		}
		
		privateContent.RemoveRange(0, n);
		format.RemoveRange(0, n);
		flagToBuild = true;
		return true;
	}
	
	public bool RemoveFromEnd(int n){
		if(n < 0){
			return false;
		}
		
		if(n > Length){
			n = Length;
		}
		
		int l = Length;
		
		privateContent.RemoveRange(l - n, n);
		format.RemoveRange(l - n, n);
		flagToBuild = true;
		return true;
	}
	
	public bool RemoveRange(int si, int n){
		if(si < 0 || n < 0 || si >= Length){
			return false;
		}
		
		if(si + n > Length){
			n = Length - si;
		}
		
		privateContent.RemoveRange(si, n);
		format.RemoveRange(si, n);
		flagToBuild = true;
		return true;
	}
	
	public override string ToString(){
		return built;
	}
	
	public static FormatString operator + (FormatString a, FormatString b){
		FormatString c = new FormatString();
		
		c.Append(a);
		c.Append(b);
		
		return c;
	}
	
	public static FormatString operator + (string a, FormatString b){
		FormatString c = new FormatString();
		
		c.Append(a);
		c.Append(b);
		
		return c;
	}
	
	public static FormatString operator + (FormatString a, string b){
		FormatString c = new FormatString();
		
		c.Append(a);
		c.Append(b);
		
		return c;
	}
	
	public static FormatString operator + (char a, FormatString b){
		FormatString c = new FormatString();
		
		c.Append(a, null);
		c.Append(b);
		
		return c;
	}
	
	public static FormatString operator + (FormatString a, char b){
		FormatString c = new FormatString();
		
		c.Append(a);
		c.Append(b, null);
		
		return c;
	}
	
	public static FormatString operator * (FormatString a, CharFormat? b){
		FormatString c = new FormatString();
		
		c.Append(a.content, b);
		
		return c;
	}
	
	public static implicit operator FormatString(string s){
		FormatString c = new FormatString(s);
		
		return c;
	}
	
	public static bool operator ==(FormatString a, FormatString b){
		// Check for null references
		if(ReferenceEquals(a, b)) return true;
		if(a is null && b is null) return true;
		if(a is null || b is null) return false;

		// Compare all properties
		if(a.content != b.content) return false;
		if(a.format.Count != b.format.Count) return false;
		for(int i = 0; i < a.format.Count; i++){
			if(a.format[i] != b.format[i]) return false;
		}
		return true;
    }
	
	public static bool operator !=(FormatString a, FormatString b){
        return !(a==b);
    }
	
	public override bool Equals(object obj){
        if (obj is FormatString other)
            return this == other;
        else
            return false;
    }
	
	public bool Equals(FormatString? fs){
		return this == fs;
    }
	
	/* public static void test(){
		string t = "/[c#,ff5900]";
		FormatString s = new FormatString();
		s.Append("{$0}Ки/[bg,100,0,0,rc]но", t);
		Console.WriteLine(s);
	} */
}