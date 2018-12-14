using UnityEngine;

namespace nopact.Commons.Animations.StateMachineUtilities
{
	public class ExecuteParticleSignal : MonoBehaviour
	{
		[SerializeField] protected ParticleSystem ps;
		[SerializeField] protected string triggerIDOnEnter;
		[SerializeField] protected bool enterTriggersPlay;
		[SerializeField] protected string triggerIDOnExit;
		[SerializeField] protected bool exitTriggersPlay;

		public void TriggerOnExit(string id)
		{
			if (id == triggerIDOnExit)
			{
				if (ps != null)
				{
					if (exitTriggersPlay)
					{
						ps.Play();
					}
					else
					{
						ps.Stop();
					}
				}
			}
		}

		public void TriggerOnEnter(string id)
		{
			if (id == triggerIDOnEnter)
			{
				if (enterTriggersPlay)
				{
					ps.Play();
				}
				else
				{
					ps.Stop();
				}
			}
		}

		private void OnEnable()
		{
			InvokeOnStateChange.OnEnter += TriggerOnEnter;
			InvokeOnStateChange.OnExit += TriggerOnExit;
		}

		private void OnDisable()
		{
			InvokeOnStateChange.OnEnter -= TriggerOnEnter;
			InvokeOnStateChange.OnExit -= TriggerOnExit;
		}
	}
}
