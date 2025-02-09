using System;
using AshLib.AshFiles;

class AshFileExample{
	public static void Main(){
		AshFile af = new AshFile("C:/ashFileTest/messages.ash");
		af.InitializeCamp("numberOfMessages", (uint) 1);
		af.InitializeCamp("0", "Seems to not be any messages mate!");
		af.Save();
		
		uint numberOfMessages = 0;
		if(!af.CanGetCamp("numberOfMessages", out numberOfMessages)){
			Console.WriteLine("The file was in an incorrect format.");
			return;
		}
		Console.WriteLine(numberOfMessages);
		
		Random rand = new Random();
		int messageToShow = rand.Next((int) numberOfMessages);
		
		string quoteOfTheDay = "";
		if(!af.CanGetCamp(messageToShow.ToString(), out quoteOfTheDay)){
			Console.WriteLine("The file was in an incorrect format.");
			return;
		}
		
		Console.WriteLine("Quote of the day: " + quoteOfTheDay);
	}
}