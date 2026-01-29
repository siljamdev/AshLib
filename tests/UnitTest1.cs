using AshLib;
using AshLib.Dates;
using AshLib.Lists;
using AshLib.AshFiles;

namespace tests;

public class UnitTest1{
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
		
		Assert.Equal(true, b);
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
		
		Assert.Equal(true, c);
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
		
		Assert.Equal(true, b.ContainsKey("ba.adq"));
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
		
		Assert.Equal(true, c);
    }
	
	[Fact]
    public void listTest1(){
		bool b = false;
		
		ReactiveList<int> a = new ReactiveList<int>(() => {
			b = true;
		}){1,2,3};
		
		a.Remove(2);
		
		Assert.Equal(true, b);
    }
	
	[Fact]
    public void listTest2(){
		ReactiveList<int> a = new ReactiveList<int>(){0,1,2,3,4,5,6};
		ReactiveList<int> b = new ReactiveList<int>(){4,0,1,2,3,5,6};
		
		a.Move(4, 0);
		
		Assert.Equal(b, a);
    }
	
	[Fact]
    public void listTest3(){
		bool b = false;
		ReactiveList<int> rl = new ReactiveList<int>(() => {
			b = true;
		}){1,2,3};
		
		IList<int> a = rl;
		
		a.Remove(2);
		
		Assert.Equal(true, b);
    }
	
	[Fact]
    public void listTest4(){
		ReactiveList<int> rl = new ReactiveList<int>(){1,2,3};
		
		rl = new ReactiveList<int>(rl.Where(h => h > 2));
		
		Assert.Equal(rl.Length, 1);
    }
}
