using System;
using AshLib;

class AshLibExample{
	public static void Main(){
		AshFile af = new AshFile("C:/ashFileTest/messages.ash");
		af.InitializeCamp("numberOfMessages", (uint) 1);
		af.InitializeCamp("0", "Seems to not be any messages mate!");
		af.Save();
		
		uint numberOfMessages = 0;
		if(!af.CanGetCampAsUint("numberOfMessages", out numberOfMessages)){
			Console.WriteLine("The file was in an incorrect format.");
			Environment.Exit(0);
		}
		Console.WriteLine(numberOfMessages);
		
		Random rand = new Random();
		int messageToShow = rand.Next((int) numberOfMessages);
		
		string quoteOfTheDay = "";
		if(!af.CanGetCampAsString(messageToShow.ToString(), out quoteOfTheDay)){
			Console.WriteLine("The file was in an incorrect format.");
			Environment.Exit(0);
		}
		
		Console.WriteLine("Quote of the day: " + quoteOfTheDay);
	}
}