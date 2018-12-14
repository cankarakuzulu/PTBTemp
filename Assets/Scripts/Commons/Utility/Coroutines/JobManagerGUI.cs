using System.Collections;
using UnityEngine;

namespace nopact.Commons.Utility.Coroutines
{
	public class JobManagerGUI : MonoBehaviour {

		private Job job;
	
		private 
	
			void OnGUI(){
		
			int buttonHeight = 60;
		
			if(GUI.Button(new Rect(5,5,200, buttonHeight), "Make ans Start 5 iteration job")){
			
				Job j = Job.Make(writeLog(5));
				j.jobComplete += (wasKilled) => {UnityEngine.Debug.Log ("job done. was it killed? " + wasKilled);};
			
			}
		
			if(GUI.Button(new Rect(5,100,200, buttonHeight), "Make Job Paused")){
			
				job = Job.Make (endlessLog(), false);
				job.jobComplete += (wasKilled) => {UnityEngine.Debug.Log ("endless job done. was it killed? " + wasKilled);};
			
			}
		
			if(GUI.Button(new Rect(5,100 + buttonHeight,200, buttonHeight), "Start Made Job")){
			
				job.Start();	
			
			}
		
			if(GUI.Button(new Rect(5,100 + buttonHeight * 2 ,200, buttonHeight), "Pause")){
			
				job.Pause();	
			
			}
		
			if(GUI.Button(new Rect(5,100 + buttonHeight * 3 ,200, buttonHeight), "Unpause")){
			
				job.Unpause();
			
			}
		
			if(GUI.Button(new Rect(5,100 + buttonHeight * 4 ,200, buttonHeight), "Kill Immediately")){
			
				job.Kill();
			
			}
		
			if(GUI.Button(new Rect(5, 100+ buttonHeight * 5 ,200, buttonHeight), "Kill After 3s Delay")){
			
				job.Kill(3);
			
			}
		
			int xPos = Screen.width - 205;
		
			if(GUI.Button(new Rect(xPos, 5, 200, buttonHeight), "Run Job With 5 Children")){
			
				Job j = Job.Make (printAfterDelay("in the parent job", 1), false);
				j.jobComplete += (wasKilled) => {UnityEngine.Debug.Log ("parent job done");};
			
				for(int i = 0; i <= 5; i++){
				
					string text = "Job number " + i;
					j.CreateAndAddChildJob(printAfterDelay(text, 1));
				
				}
			
				j.Start();
			
			}
		
		}
	
		IEnumerator writeLog(int totalTimes){
		
			int i = 0;
			WaitForSeconds wfs = new WaitForSeconds(1);
		
			while(i <= totalTimes){
			
				UnityEngine.Debug.Log (string.Format("writing log {0} of {1}", i, totalTimes));
				i++;
				yield return wfs;
			
			}
		
		}
	
		IEnumerator endlessLog(){
		
			int i = 0;
			WaitForSeconds wfs = new WaitForSeconds(1);
		
			while(true){
			
				UnityEngine.Debug.Log ("writing endless log: " + i);
				i++;
				yield return wfs;
			
			}
		
		}
	
		IEnumerator printAfterDelay(string text, float delay){
		
			yield return new WaitForSeconds(delay);
			UnityEngine.Debug.Log ("print after delay: " + text);
		
		}
	}
}
