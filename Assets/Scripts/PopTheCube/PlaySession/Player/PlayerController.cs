using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace nopact.PopTheCube.PlaySession.Player
{
	public class PlayerController : MonoBehaviour
	{
		public event Action OnFailed;
		[SerializeField] protected TrailRenderer dashTrail;
		private const float  AIR_DRAG = 0.3f;
		private Transform xform;
		private Limits limits = new Limits(0f, 8f);
		private Limits xAlignments = new Limits( -2.5f, 2.5f);
		private MotionTypes motionState;
		private Tween dashTween, rotationTween;
		private float maxSpeed = 17f;
		private bool isInmotion = false;
		private bool isInLeft = true;

		public void Initialize()
		{
			YPosition = limits.Middle;
			SetPosition();
			motionState = MotionTypes.Entry;

			dashTween = xform.DOMoveX((isInLeft ? xAlignments.Lower : xAlignments.Upper) * 3, 0.25f).From();
			dashTween.SetEase(Ease.OutSine);
			dashTween.OnComplete(OnInitializatonComplete);
		}

		public void Kill()
		{
			motionState = MotionTypes.Idle;
		}

		public void Dash()
		{
			if (motionState != MotionTypes.Vertical  && motionState != MotionTypes.ChainDash)
			{
				return;
			}

			if(motionState == MotionTypes.ChainDash ) 
			{
				ChainDashCount++;
			}
			
			
			dashTrail.emitting = true;
			motionState = MotionTypes.Dash;
			rotationTween = xform.DORotate( Vector3.forward * 180, 0.05f);
			rotationTween.SetRelative(true);
			var target = isInLeft ? xAlignments.Upper : xAlignments.Lower;
			dashTween = xform.DOMoveX(target, 0.1f);
			dashTween.SetEase(Ease.OutSine);
			StartCoroutine(StartRotation());

		}

		public void FailDash()
		{
			dashTrail.emitting = true;
			motionState = MotionTypes.Freefall;
			dashTween.Pause();
			DOTween.Kill(dashTween);

			var target = isInLeft ? xAlignments.Lower -1.0f : xAlignments.Upper + 1.0f;
			dashTween = xform.DOMoveX(target, 0.05f);
			dashTween.SetEase(Ease.OutSine);
			dashTween.OnComplete(OnFRetractionComplete);
			rotationTween = xform.DORotate( Vector3.forward * 180, 0.3f);
			rotationTween.SetRelative(true);
		}
		
		public void MarkEmptyPass()
		{
			ChainDashCount = 0;
		}
		
		#region UnityEvents

		private void Awake()
		{
			xform = GetComponent<Transform>();
		}

		private void Start()
		{
			xform.position = new Vector3
			{
				x = (isInLeft ? xAlignments.Lower : xAlignments.Upper) * 3,
				y= xform.position.y,
				z = 0
			};
		}
		private void FixedUpdate()
		{
			if (motionState != MotionTypes.Vertical)
			{
				return;
			}

			if (Constraint())
			{
				Velocity = -Velocity;
			}
			
			AdjustVelocity();
			Integrate();
			SetPosition();
		}
		#endregion
		
		private void OnInitializatonComplete()
		{
			dashTrail.emitting = false;
			motionState = MotionTypes.Vertical;
		}
	
		private IEnumerator StartRotation()
		{
			yield return new WaitForSeconds(0.05f);
			rotationTween = xform.DORotate(Vector3.up * 180, 0.05f);
			rotationTween.SetEase(Ease.OutExpo);
			rotationTween.SetRelative(true);
			rotationTween.OnComplete(OnRotationComplete);
		}

		private void OnRotationComplete()
		{
			if (motionState != MotionTypes.Dash)
			{
				return;
			}
			
			dashTween.Complete();
			isInLeft = !isInLeft;
			
			if (motionState != MotionTypes.ChainDash)
			{
				motionState = MotionTypes.ChainDash;
				if (IsInvoking(nameof(ChainDashRoutine)))
				{
					CancelInvoke(nameof(ChainDashRoutine));
				}
				Invoke(nameof(ChainDashRoutine), 0.45f);	
			}
		}

		private void ChainDashRoutine()
		{
			
			if (motionState == MotionTypes.ChainDash)
			{
				motionState = MotionTypes.Vertical;
				ChainDashCount = 0;
				Velocity = -Velocity;
				dashTrail.emitting = false;
			}
		}

		private void OnFRetractionComplete()
		{
			var target = !isInLeft ? xAlignments.Upper-1.2f : xAlignments.Lower + 1.2f;
			dashTween = xform.DOMoveX(target, 0.05f);
			dashTween.SetEase(Ease.OutExpo);
			dashTween.OnComplete(OnFlyAway);
		}

		private void OnFlyAway()
		{
			dashTween = xform.DOMove(new Vector3(transform.position.x <0 ? xAlignments.Lower-2.0f: xAlignments.Upper + 2.0f, 15,-20.0f), 1.5f);
			dashTween.SetEase(Ease.OutQuad);
			dashTween.OnComplete(OnFlyAwayCompleted);
			rotationTween.Complete();
			DOTween.Kill(rotationTween);
			rotationTween = xform.DORotate(Vector3.up * 45, 0.2f);
			rotationTween.SetLoops(-1, LoopType.Incremental);
			rotationTween.SetEase(Ease.Linear);
		}

		private void OnFlyAwayCompleted()
		{
			dashTrail.emitting = false;
			rotationTween.SetLoops(1, LoopType.Restart);
			rotationTween.Complete();
			DOTween.Kill(rotationTween);
			OnFailed?.Invoke();
		}

		private void AdjustVelocity()
		{
			if (!isInmotion)
			{
				Velocity = maxSpeed;
				isInmotion = true;
			}

			if (Acceleration != 0 && Mathf.Abs(Velocity) > maxSpeed )
			{
				Acceleration = 0;
				Velocity = Mathf.Sign(Velocity) * maxSpeed;
			}
		}

		private bool Constraint()
		{
			float old = YPosition;
			YPosition = Mathf.Clamp(YPosition, limits.Lower, limits.Upper);
			return !(old == YPosition);	
		}

		private void Integrate()
		{
			Velocity += Acceleration * Time.deltaTime;
			YPosition += Velocity * Time.deltaTime - Velocity * AIR_DRAG * Time.deltaTime;
		}

		private void SetPosition()
		{
			xform.position = new Vector3()
			{
				x = isInLeft ? xAlignments.Lower : xAlignments.Upper,
				y = YPosition,
				z = xform.position.z
			};
		}
		
		public uint ChainDashCount { get; private set; }

		public bool IsOnLeft => isInLeft;

		public float MaxSpeed
		{
			get { return maxSpeed; }
			set { maxSpeed = value; }
		}

		public float YPosition { get; private set; }
		public float Velocity { get; private set; }
		public float Acceleration { get; private set; }

		public MotionTypes Motion => motionState;
	
		public enum MotionTypes
		{
			Idle,
			Entry,
			Vertical,
			Dash,
			ChainDash,
			Freefall
		}
		public struct Limits
		{
			public Limits(float lower, float upper)
			{
				Lower = lower;
				Upper = upper;
			}
			public float Upper { get;  }
			public float Lower { get; }
			public float Middle =>(Upper + Lower) * 0.5f;
		}
	}	
}

