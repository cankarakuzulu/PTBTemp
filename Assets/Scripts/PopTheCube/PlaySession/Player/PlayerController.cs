using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace nopact.PopTheCube.PlaySession.Player
{
	public class PlayerController : MonoBehaviour
	{
		public event Action<float, bool> OnDash;
		[SerializeField] protected TrailRenderer dashTrail;
		private const float  AIR_DRAG = 0.3f;
		private Transform xform;
		private Limits limits = new Limits(-0.5f, 10.5f);
		private Limits xAlignments = new Limits( -2.4f, 2.4f);
		private MotionTypes motionState;
		private Tween dashTween, rotationTween;
		private float maxSpeed = 15;
		private bool isInmotion = false;
		private bool isInLeft = true;

		public void Initialize()
		{
			YPosition = limits.Middle;
			SetPosition();
			motionState = MotionTypes.Entry;

			dashTween = xform.DOMoveX((isInLeft ? xAlignments.Lower : xAlignments.Upper) * 2, 1.0f).From();
			dashTween.SetEase(Ease.OutSine);
			dashTween.OnComplete(OnInitializatonComplete);
		}

		public void Dash()
		{
			if (motionState == MotionTypes.Dash || motionState == MotionTypes.Entry )
			{
				return;
			}

			dashTrail.emitting = true;
			motionState = MotionTypes.Dash;
			var target = isInLeft ? xAlignments.Lower -1.0f : xAlignments.Upper + 1.0f;
			dashTween = xform.DOMoveX(target, 0.05f);
			dashTween.SetEase(Ease.InSine);
			dashTween.OnComplete(OnRetractionComplete);
			rotationTween = xform.DORotate( Vector3.forward * 180, 0.3f);
			rotationTween.SetRelative(true);
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
		
		#region UnityEvents

		private void Awake()
		{
			xform = GetComponent<Transform>();
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
			AdjustAcceleration();
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
		private void OnRetractionComplete()
		{
			OnDash?.Invoke( YPosition, isInLeft);
			var target = isInLeft ? xAlignments.Upper : xAlignments.Lower;
			dashTween = xform.DOMoveX(target, 0.2f);
			dashTween.SetEase(Ease.OutSine);
			StartCoroutine(StartRotation());
		}

		private IEnumerator StartRotation()
		{
			yield return new WaitForSeconds(0.1f);
			rotationTween = xform.DORotate(Vector3.up * 180, 0.15f);
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

			dashTrail.emitting = false;
			dashTween.Complete();
			isInLeft = !isInLeft;
			Velocity = -Velocity;
			motionState = MotionTypes.Vertical;
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
			dashTween = xform.DOMove(new Vector3(transform.position.x <0 ? xAlignments.Lower-2.0f: xAlignments.Upper + 2.0f, 15,-20.0f), 3f);
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

		private void AdjustAcceleration()
		{
			var direction = Mathf.Sign(Velocity);
			
			if (!isInmotion)
			{
				Acceleration = -maxSpeed*0.5f;
			}

			if (YPosition < limits.Lower + 1 )
			{
				if (direction == -1)
				{
					Acceleration = maxSpeed *3.5f;	
				}
				else if (Mathf.Abs(Velocity) < maxSpeed)
				{
					Acceleration = maxSpeed *3.5f;
				}

				if (!isInmotion)
				{
					isInmotion = true;
				}
			}

			if (YPosition > limits.Upper - 1)
			{
				if (direction == 1)
				{
					Acceleration = -maxSpeed * 3.5f;
				}
				else if (Mathf.Abs(Velocity) < maxSpeed)
				{
					Acceleration = -maxSpeed * 3.5f;
				}
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

		public bool IsOnLeft => isInLeft;

		public float MaxSpeed
		{
			get { return maxSpeed; }
			set { maxSpeed = value; }
		}

		public float YPosition { get; private set; }
		public float Velocity { get; private set; }
		public float Acceleration { get; private set; }
		
		public enum MotionTypes
		{
			Idle,
			Entry,
			Vertical,
			Dash,
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

