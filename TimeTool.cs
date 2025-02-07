using System;
using System.Text;
using System.Diagnostics;

namespace AshLib.Time;

public class TimeTool{ //Used to get info about times
    Stopwatch sw;
    double breakPoint;
	
    public string[] categoryNames;
    double[] times;
	
    List<double[]> history = new List<double[]>();
	
    int timeWriting;
	
    public int maxHistory = 1000;
	
	private readonly object lockObj = new object();
	
    public TimeTool(params string[] catNames){
        sw = new Stopwatch();
        categoryNames = catNames;
    }
	
	public void Reset(){
		lock(lockObj){
			sw.Restart();
			history.Clear();
		}
    }
	
    public void TickStart(){
		lock(lockObj){
			sw.Restart();
			breakPoint = 0d;
			timeWriting = 0;
			times = new double[categoryNames.Length + 1];
		}
    }
	
    public void CatEnd(){
		lock(lockObj){
			double t = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1e9;
			times[timeWriting] += t - breakPoint;
			breakPoint = t;
			timeWriting++;
		}
    }
	
	public void CatEnd(int i){
		lock(lockObj){
			double t = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1e9;
			times[i] += t - breakPoint;
			breakPoint = t;
			timeWriting = i + 1;
		}
    }
	
    public void TickEnd(){
		lock(lockObj){
			sw.Stop();
			
			double totalTime = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1e9;
			times[categoryNames.Length] = totalTime;
			
			// Add the current times to the history
			history.Add((double[])times.Clone());
			if (history.Count > maxHistory) {
				history.RemoveAt(0); // Remove the oldest entry if over max size
			}
		}
    }
	
	public double[] LastTickInfo(){
		lock(lockObj){
			return (double[])times.Clone();
		}
	}
	
    public string LastTickString(){
		StringBuilder sb = new StringBuilder();
		
		lock(lockObj){
			int maxCategoryNameWidth = "Category".Length;
			for(int i = 0; i < categoryNames.Length; i++){
				if(categoryNames[i].Length > maxCategoryNameWidth){
					maxCategoryNameWidth = categoryNames[i].Length;
				}
			}
			maxCategoryNameWidth += 2;
			
			sb.AppendLine("Category".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13}", "Time") + " │" + String.Format("{0,12}", "Percentage"));
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 11));
			
			for(int i = 0; i < categoryNames.Length; i++){
				sb.AppendLine(categoryNames[i].PadRight(maxCategoryNameWidth) + "│" +  String.Format("{0,13:N0}", times[i]) + " │" + String.Format("{0,12:N2}%", 100d * times[i] / times[categoryNames.Length]));
			}
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 11));
			sb.AppendLine("Total".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", times[categoryNames.Length]) + " │");
		}
		
        return sb.ToString();
    }
	
	public double[] MaxInfo(){
		double[] maxTimes = new double[categoryNames.Length + 1];
		
		lock(lockObj){
			foreach(double[] record in history) {
				for(int i = 0; i < record.Length; i++){
					if(maxTimes[i] < record[i]){
						maxTimes[i] = record[i];
					}
				}
			}
		}
		
		return maxTimes;
	}
	
	public double[] MeanInfo(){
		double[] sumTimes = new double[categoryNames.Length + 1];
		double[] averageTimes = new double[categoryNames.Length + 1];
		
		lock(lockObj){
			foreach(double[] record in history) {
				for(int i = 0; i < record.Length; i++){
					sumTimes[i] += record[i];
				}
			}
			
			for(int i = 0; i < categoryNames.Length + 1; i++){
				averageTimes[i] = sumTimes[i] / history.Count;
			}
		}
		
		return averageTimes;
	}
	
    public string MeanString(){
		StringBuilder sb = new StringBuilder();
		
		lock(lockObj){
			if(history.Count == 0) return "No data in history";
			
			// Compute averages
			double[] sumTimes = new double[categoryNames.Length + 1];
			double[] averageTimes = new double[categoryNames.Length + 1];
			double[] maxTimes = new double[categoryNames.Length + 1];
			
			foreach(double[] record in history) {
				for(int i = 0; i < record.Length; i++){
					sumTimes[i] += record[i];
					
					if(maxTimes[i] < record[i]){
						maxTimes[i] = record[i];
					}
				}
			}
			
			for(int i = 0; i < categoryNames.Length + 1; i++){
				averageTimes[i] = sumTimes[i] / history.Count;
			}
			
			// Build output
			
			int maxCategoryNameWidth = "Category".Length;
			for(int i = 0; i < categoryNames.Length; i++){
				if(categoryNames[i].Length > maxCategoryNameWidth){
					maxCategoryNameWidth = categoryNames[i].Length;
				}
			}
			maxCategoryNameWidth += 2;
			
			sb.AppendLine("Category".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13}", "Max") + " │" + String.Format("{0,13}", "Average") + " │" + String.Format("{0,12}", "Percentage"));
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12));
			
			for(int i = 0; i < categoryNames.Length; i++){
				sb.AppendLine(categoryNames[i].PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", maxTimes[i]) + " │" + String.Format("{0,13:N0}", averageTimes[i]) + " │" + String.Format("{0,12:N2}%", 100d * sumTimes[i] /  sumTimes[categoryNames.Length]));
			}
			
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12) + "─┤");
			sb.AppendLine("Total".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", maxTimes[categoryNames.Length]) + " │" + String.Format("{0,13:N0}", averageTimes[categoryNames.Length]) + " │");
			sb.AppendLine(new string('─', maxCategoryNameWidth) + "┤╶" + new string('─', 12) + "─┤╶" + new string('─', 12) + "─┤");
			sb.AppendLine("Ticks".PadRight(maxCategoryNameWidth) + "│" + String.Format("{0,13:N0}", history.Count) + " │" + String.Format("{0,10:N1} hz", 1000000000d / averageTimes[categoryNames.Length]) + " │");
		}
        
        return sb.ToString();
    }
}