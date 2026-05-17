using AshLib;
using AshLib.Formatting;

namespace AshLib.Tests;

public class FormatStringTests{
	[Fact]
    public void formatstringTest1(){
		FormatString[] fs = new FormatString("I have always\r\nhated french\r\n\r\npeople\r\n", new CharFormat(Color3.Red)).SplitIntoLines();
		Assert.Equal(fs.Length, 4);
    }
	
	[Fact]
    public void formatstringTest2(){
		FormatString[] fs = new FormatString("I have always\r\nhated french\r\n\r\npeople\r\n", new CharFormat(Color3.Red)).SplitIntoLines();
		Assert.Equal(fs.Sum(h => h.Length), 31);
    }
}
