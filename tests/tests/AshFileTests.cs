using AshLib;
using AshLib.Dates;
using AshLib.AshFiles;

namespace AshLib.Tests;

public class AshFileTests{
	[Fact]
    public void ashFileTest1(){
		AshFile a = new AshFile();
		a.Set("hello", "world");
		Assert.Equal(a.GetValue<string>("hello"), "world");
    }
	
	[Fact]
    public void ashFileTest2(){
		AshFile a = new AshFile();
		a.Set("hello", "world");
		a.Initialize("hello", "aa");
		Assert.Equal(a.GetValue<string>("hello"), "world");
    }
	
	[Fact]
    public void ashFileTest3(){
		AshFile a = new AshFile();
		
		a.Set("st", "test");
		a.Set("ba", (byte) 5);
		a.Set("ba.adq", true);
		a.Set("ba.ad2", true);
		a.Set("ba.ssss.s", new bool[]{true, false, true, true, false, false});
		a.Set("ba.us", (ushort) 5000);
		a.Set("ba.ui", (uint) 5500);
		a.Set("ul", (ulong) 55008712);
		a.Set("sb.gg", (sbyte) -5);
		a.Set("s", (short) -5131);
		a.Set("i", (int) -325131);
		a.Set("l", (long) 9325131);
		a.Set("ba.c.c", new Color3(13, 14, 15));
		a.Set("f", 3.178181f);
		a.Set("d", 300.178181d);
		a.Set("v2", new Vec2(14f, 17f));
		a.Set("v3", new Vec3(14f, 17f, 19.7f));
		a.Set("v4", new Vec4(14.6f, 17f, 19.7f, 0.7f));
		a.Set("bo", false);
		a.Set("bo2", true);
		a.Set("dt", (Date)DateTime.Now);
		a.Set("ar", new List<Color3>(){new Color3("FFFFFF"), new Color3("F00F00")});
		
		AshFile b = AshFile.Parse(a.ToString());
		
		Assert.Equal(a, b);
    }
	
	[Fact]
    public void ashFileTest4(){
		AshFile a = new AshFile();
		
		a.Set("st", "test");
		a.Set("ba", (byte) 5);
		a.Set("ba.adq", true);
		a.Set("ba.ad2", true);
		a.Set("ba.ssss.s", new bool[]{true, false, true, true, false, false});
		a.Set("ba.us", (ushort) 5000);
		a.Set("ba.ui", (uint) 5500);
		a.Set("ul", (ulong) 55008712);
		a.Set("sb.gg", (sbyte) -5);
		a.Set("s", (short) -5131);
		a.Set("i", (int) -325131);
		a.Set("l", (long) 9325131);
		a.Set("ba.c.c", new Color3(13, 14, 15));
		a.Set("f", 3.178181f);
		a.Set("d", 300.178181d);
		a.Set("v2", new Vec2(14f, 17f));
		a.Set("v3", new Vec3(14f, 17f, 19.7f));
		a.Set("v4", new Vec4(14.6f, 17f, 19.7f, 0.7f));
		a.Set("bo", false);
		a.Set("bo2", true);
		a.Set("dt", (Date)DateTime.Now);
		a.Set("ar", new Color3[]{new Color3("FFFFFF"), new Color3("F00F00")});
		
		AshFileModel m = new AshFileModel(
			new ModelInstance(ModelInstanceOperation.Exists, "ba", true),
			new ModelInstance(ModelInstanceOperation.Type, "ul", "hello")
		);
		
		AshFileModel n = new AshFileModel(
			new ModelInstance(ModelInstanceOperation.Exists, "n1", 67)
		);
		
		m.Merge(n);
		
		a.ApplyModel(m);
		
		bool b = (a.GetValue<byte>("ba") == 5) && (a.GetValue<string>("ul") == "hello") && (a.GetValue<int>("n1") == 67);
		
		Assert.True(b);
    }
	
	[Fact]
    public void ashFileTest5(){
		AshFile a = new AshFile();
		
		a.Set("st", "test");
		a.Set("ba", (byte) 5);
		a.Set("ba.adq", true);
		a.Set("ba.ad2", true);
		a.Set("ba.ssss.s", new bool[]{true, false});
		a.Set("ba.us", (ushort) 5000);
		a.Set("l", (long) 9325131);
		a.Set("ba.c.c", new Color3(13, 14, 15));
		a.Set("f", 3.178181f);
		a.Set("v2", new Vec2(14f, 17f));
		a.Set("dt", (Date)DateTime.Now);
		a.Set("ar", new List<Color3>(){new Color3("FFFFFF"), new Color3("F00F00")});
		
		AshFile b = AshFile.ReadFromBytes(a.WriteToBytes());
		
		bool c = a == b; //The test will do dictionary compare, not ashfile compare
		
		Assert.True(c);
    }
	
	[Fact]
    public void ashFileTest6(){
		AshFile a = new AshFile();
		a.Set("hello", "Транквилизатор");
		Assert.Equal(a.GetValue<string>("hello"), "Транквилизатор");
    }
	
	[Fact]
    public void ashFileTest7(){
		AshFile a = new AshFile();
		a.Set("st", "test");
		a.Set("ba", (byte) 5);
		a.Set("ba.adq", true);
		a.Set("ba.ad2", true);
		a.Set("ba.ssss.s", new bool[]{true, false, true, true, false, false});
		a.Set("ba.us", (ushort) 5000);
		a.Set("ba.ui", (uint) 5500);
		a.Set("ul", (ulong) 55008712);
		a.Set("sb.gg", (sbyte) -5);
		a.Set("s", (short) -5131);
		a.Set("i", (int) -325131);
		a.Set("l", (long) 9325131);
		a.Set("ba.c.c", new Color3(13, 14, 15));
		a.Set("f", 3.178181f);
		a.Set("d", 300.178181d);
		a.Set("v2", new Vec2(14f, 17f));
		a.Set("v3", new Vec3(14f, 17f, 19.7f));
		a.Set("v4", new Vec4(14.6f, 17f, 19.7f, 0.7f));
		a.Set("bo", false);
		a.Set("bo2", true);
		a.Set("dt", (Date)DateTime.Now);
		a.Set("ar", new Color3[]{new Color3("FFFFFF"), new Color3("F00F00")});
		
		AshFile b = (AshFile) a.Where(kvp => kvp.Key.StartsWith("b")).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		
		Assert.True(b.ContainsKey("ba.adq"));
    }
	
	[Fact]
    public void ashFileTest8(){
		AshFile a = new AshFile();
		
		a.Set("st", "test");
		a.Set("ba.adq", true);
		a.Set("ba.ad2", true);
		a.Set("ba.ssss.s", new bool[]{true, false});
		a.Set("l", (long) 9325131);
		a.Set("v2", new Vec2(14f, 17f));
		a.Set("ar", new List<Color3>(){new Color3("FFFFFF"), new Color3("F00F00")});
		
		a.RemoveAll(kvp => kvp.Value is bool);
		
		Assert.Equal(5, a.Count);
    }
	
	[Fact]
    public void ashFileTest9(){
		AshFile a = new AshFile();
		
		a.Set("st", "test");
		a.Set("ba", (byte) 5);
		a.Set("ba.adq", true);
		a.Set("ba.ad2", true);
		a.Set("ba.ssss.s", new bool[]{true, false});
		a.Set("ba.us", (ushort) 5000);
		a.Set("l", (long) 9325131);
		a.Set("ba.c.c", new Color3(13, 14, 15));
		a.Set("f", 3.178181f);
		a.Set("v2", new Vec2(14f, 17f));
		a.Set("dt", (Date)DateTime.Now);
		a.Set("ar", new List<Color3>(){new Color3("FFFFFF"), new Color3("F00F00")});
		
		a.password = "passkeyy";
		
		AshFile b = AshFile.ReadFromBytes(a.WriteToBytes(), "passkeyy");
		
		bool c = a == b; //The test will do dictionary compare, not ashfile compare
		
		Assert.True(c);
    }
}
