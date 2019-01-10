using UnityEngine;
using DG.Tweening;
	
namespace nopact.ShapeCutter.Animations
{
	public class LoopedMoveTween : MonoBehaviour
	{
		[SerializeField] protected Transform affectedTransform;
		[SerializeField] protected Vector3 relativeMotionTarget;
		[SerializeField] protected Ease motionEase;
		[SerializeField] protected LoopType loopType;
		[SerializeField] protected float loopTime;
		private Tween t;
		
		void Start ()
		{
			t = affectedTransform.DOMove(relativeMotionTarget, loopTime);
			t.SetRelative();
			t.SetEase(motionEase);
			t.SetLoops(-1, loopType);
			t.Play();
			
		}
	}
}
