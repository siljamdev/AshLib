using AshLib;
using AshLib.Dates;

namespace AshLib.Tests;

public class DateTests{
	[Fact]
    public void dateTest1(){
		Date a = Date.FromCPTF("AMAYZm");
		Date b = (Date) new DateTime(1897, 8, 12, 12, 0, 0);
		Assert.Equal(a, b);
    }
	
	[Fact]
    public void dateTest2(){
		Date a = new Date(0, 0, 12, 12, 8, 1897);
		Assert.Equal(a.ToString(), "12/08/1897 12:00:00");
    }
}
