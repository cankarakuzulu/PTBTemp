using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Utility.Coroutines
{
	public class JobManager : MonoBehaviour {

		static JobManager instance = null;
	
		public static JobManager Instance{
			get{
			
				if(instance == null){
				
					instance = FindObjectOfType(typeof(JobManager)) as JobManager;
				
					if(instance == null){
					
						var obj = new GameObject("JobManaber");
						instance = obj.AddComponent<JobManager>();
					
					}
				
				}
			
				return instance;
			
			}
		}
	
		void OnApplicationQuit(){
		
			instance = null;
		
		}
	}

	public class Job{
	
		public event System.Action<bool> jobComplete;
	
		private bool running;
		public bool Running{
			get {return running;}
		}
	
		private bool paused;
		public bool Paused{
			get {return paused;}
		}
	
		private IEnumerator coroutine;
		private bool jobWasKilled;
		private Stack<Job> childJobStack;
	
		#region constructors
		public Job(IEnumerator coroutine) : this(coroutine, true)
		{}
	
		public Job(IEnumerator coroutine, bool shouldStart){
		
			this.coroutine = coroutine;
		
			if(shouldStart){
			
				Start();
			
			}
		
		}
		#endregion
	
		#region static Job makers
		public static Job Make(IEnumerator coroutine){
		
			return new Job(coroutine);
		
		}
	
		public static Job Make(IEnumerator coroutine, bool shouldStart){
		
			return new Job(coroutine, shouldStart);
		
		}
		#endregion
	
		#region public API
		public Job CreateAndAddChildJob(IEnumerator coroutine){
		
			Job job = new Job(coroutine, false);
			AddChildJob(job);
		
			return job;
		
		}
	
		public void AddChildJob(Job childJob){
		
			if(childJobStack == null){
			
				childJobStack = new Stack<Job>();
			
			}
		
			childJobStack.Push(childJob);
		
		}
	
		public void RemoveChildJob(Job childJob){
		
			if(childJobStack.Contains(childJob)){
			
				Stack<Job> childStack = new Stack<Job>(childJobStack.Count - 1);
				var allCurrentChildren = childJobStack.ToArray();
				System.Array.Reverse(allCurrentChildren);
			
				for(int i = 0; i < allCurrentChildren.Length; i++){
				
					Job j = allCurrentChildren[i];
				
					if(j != childJob){
					
						childStack.Push(j);
					
					}
				
				}
			
				childJobStack = childStack;
			}
		
		}
	
		public void Start(){
		
			running = true;
			JobManager.Instance.StartCoroutine(DoWork());
		
		}
	
		public IEnumerator StartAsCoroutine(){
		
			running = true;
			yield return JobManager.Instance.StartCoroutine(DoWork());
		
		}
	
		public void Pause(){
		
			paused = true;
		
		}
	
		public void Unpause(){
		
			paused = false;
		
		}
	
		public void Kill(){
		
			jobWasKilled = true;
			running = false;
			paused = false;
		
		}
	
		public void Kill(float delayInSeconds){
		
			int delay = (int) delayInSeconds * 1000;
		
			new System.Threading.Timer(obj => {
			
				lock(this){
				
					Kill();
				
				}
			}, null, delay, System.Threading.Timeout.Infinite);
		
		}
		#endregion
	
		private IEnumerator DoWork(){
		
			//null out the first run through in case started in paused state
			yield return null;
		
			while(running){
			
				if(paused){
				
					yield return null;
				
				}
				else{
				
					//run the next iteration and stop if done
					if(coroutine.MoveNext()){
					
						yield return coroutine.Current;
					
					}
					else{
					
						//run child jobs if any exists
						if(childJobStack != null){
						
							yield return JobManager.Instance.StartCoroutine(RunChildJobs());
						
						}
					
						running = false;
					}
				
				}
						
			}
		
			//fire off complete event
			if(jobComplete != null){
			
				jobComplete(jobWasKilled);
			
			}
		
		}
	
		private IEnumerator RunChildJobs(){
		
			if(childJobStack != null && childJobStack.Count > 0){
			
				do{
				
					Job childJob = childJobStack.Pop();
					yield return JobManager.Instance.StartCoroutine(childJob.StartAsCoroutine());
				
				}
				while(childJobStack.Count > 0);
			}
		
		}
	}
}