using System;
using AshLib;

class CPTFExample{
	public static void Main(){
		Console.WriteLine("Please, enter your birth date (DD/MM/YY): "); //First, ask for the date
		string bd = Console.ReadLine();
		
		string[] a = bd.Split("/"); //Split it in the separators
		if(a.Length < 3){ //If if its not long enough the format is invalid
			Console.WriteLine("The date wasn't correctly formatted.");
			Environment.Exit(0);
		}
		
		byte day; //Get the day, ensuring error checking
		if(!Byte.TryParse(a[0], out day)){
			Console.WriteLine("The day wasn't correctly formatted.");
			Environment.Exit(0);
		}
		
		byte month; //Get the month, ensuring error checking
		if(!Byte.TryParse(a[1], out month)){
			Console.WriteLine("The month wasn't correctly formatted.");
			Environment.Exit(0);
		}
		
		ushort year; //Get the year, ensuring error checking
		if(!UInt16.TryParse(a[2], out year)){
			Console.WriteLine("The year wasn't correctly formatted.");
			Environment.Exit(0);
		}
		
		Date bornDate = new Date(0, 0, 0, day, month, year); //Initialize the date, and then convert it into cptf format
		string cptf = bornDate.ToCPTF();
		
		Console.WriteLine("The day you were born in CPTF format is: " + cptf);
	}
}