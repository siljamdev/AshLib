using AshLib;
using AshLib.Lists;

namespace AshLib.Tests;

public class ReactiveListTests{
	[Fact]
    public void listTest1(){
		bool b = false;
		
		ReactiveList<int> a = new ReactiveList<int>(() => {
			b = true;
		}){1,2,3};
		
		a.Remove(2);
		
		Assert.True(b);
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
		
		Assert.True(b);
    }
	
	[Fact]
    public void listTest4(){
		ReactiveList<int> rl = new ReactiveList<int>(){1,2,3};
		
		rl = new ReactiveList<int>(rl.Where(h => h > 2));
		
		Assert.Equal(rl.Length, 1);
    }
}
