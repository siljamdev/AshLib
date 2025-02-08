using System;
using AshLib.Formatting;

class FormatStringExample{
	public static void Main(){
		FormatString s = "/[c#,ff5900]KI/[bg,100,0,0,rc]NO"; //Create a format string, with the specialthe first half has a foreground color, and the second a background color. Reference to the group KINO <3
		
		Console.WriteLine(s); //Print it to the console. It will display with colors!
	}
}