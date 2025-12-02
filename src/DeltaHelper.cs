using System;
using System.Diagnostics;
using System.Threading;

namespace AshLib.Time;

public class DeltaHelper{ //Class used for helping with deltatime, fps and those things
	private Stopwatch timer;
	private double lastTime;
	private double lastStable;
	private int stableFrameCounter;
	private double stableTime = 1000d; //Default is 1 second
	
	public double deltaTime {get; private set;} //Time between frames in seconds
	public double fps {get; private set;} //Fps, will fluctuate very rapidly
	public double stableFps {get; private set;} //Fps, is more stable, will update once every stableTime miliseconds. Thought for showing it
	
	public void Start(){ //Call to start the thing
		timer = Stopwatch.StartNew();
		
		lastTime = timer.Elapsed.TotalMilliseconds;
		lastStable = 0d;
		stableFrameCounter = 0;
		
		this.Frame();
	}
	
	public void Frame(){ //Call it at the very end of each frame
		double currentTime = timer.Elapsed.TotalMilliseconds;
		deltaTime = (currentTime - lastTime)/1000d; //in seconds
		lastTime = currentTime;
		fps = 1d/deltaTime;
		
		stableFrameCounter++;
		
		if(currentTime > lastStable + stableTime){ //Update the stable fps
			double stableDeltaTime = (currentTime - lastStable)/1000d;
			stableFps = (double) stableFrameCounter/stableDeltaTime;
			
			lastStable = currentTime;
			stableFrameCounter = 0;
		}
	}
	
	public void Target(double FPS){ //Call with argument at the end of each frame(just before Frame()) to achieve wanted fps target
		double wantedDeltaTime = 1000d/FPS; //In ms
		double realDeltaTime = (timer.Elapsed.TotalMilliseconds - lastTime); //In ms
		double extraTime = wantedDeltaTime - realDeltaTime;
		if(extraTime > 0){
			PreciseWait(extraTime); //Sleeps for needed time
		}
	}
	
	public void TargetLazy(double FPS){ //Call with argument at the end of each frame(just before Frame()) to achieve wanted fps target
		double wantedDeltaTime = 1000d/FPS; //In ms
		double realDeltaTime = (timer.Elapsed.TotalMilliseconds - lastTime); //In ms
		double extraTime = wantedDeltaTime - realDeltaTime;
		if(extraTime > 0){
			LazyWait(extraTime); //Sleeps for needed time or maybe more.
		}
	}
	
	public void SetStableUpdateTime(double milliseconds){ //For setting the stable time, in miliseconds
		this.stableTime = milliseconds;
	}
	
	public double GetTime(){ //Get total time in seconds
		return (timer.Elapsed.TotalMilliseconds/1000d);
	}
	
	private void PreciseWait(double milliseconds){  //Will busy wait, wich has a high CPU usage but its impossible to achieve precision otherwise
        double start = timer.Elapsed.TotalMilliseconds;
		while(timer.Elapsed.TotalMilliseconds < start + milliseconds){}
    }
	
	private void LazyWait(double milliseconds){  //Will busy wait, wich has a high CPU usage but its impossible to achieve precision otherwise
        Thread.Sleep((int) milliseconds);
    }
}