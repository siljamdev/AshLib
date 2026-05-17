using AshLib;
namespace AshLib.Tests;

public class ColorTests{
    [Fact]
    public void colorTest1(){
		Color3 a = new Color3(100, 16, 0);
		Color3 b = new Color3("#641000");
		Assert.Equal(a, b);
    }
	
	[Fact]
    public void colorTest2(){
		Color3 a = new Color3(255, 51, 0);
		Vec3 b = new Vec3(1f, 0.2f, 0f);
		Assert.Equal(a.ToVec(), b);
    }
	
	[Fact]
    public void colorTest3(){
		Color3 a = Color3.FromHSV(65, 100, 60);
		Color3 b = new Color3(140, 153, 0);
		Assert.Equal(a, b);
    }
}
