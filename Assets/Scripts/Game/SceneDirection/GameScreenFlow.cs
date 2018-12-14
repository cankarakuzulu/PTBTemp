using System;
using System.Collections.Generic;
using DG.Tweening;
using nopact.Commons.Utility;
using nopact.Commons.Utility.Timer;

using UnityEngine;

namespace nopact.Game.SceneDirection
{
    public abstract class GameScreenFlow
    {

        private Tween currentCameraTween;
        private string flowTimerID;

        protected GameScreenFlow( string cameraPositionKey, Transform cameraTarget, Ease cameraEase, float cameraMotionDelay, Dictionary<string, Transform> gamePlayCameraPositions, Action onDone, Action onActivateUI )
        {
            CameraPositionKey = cameraPositionKey;
            CameraTarget = cameraTarget;
            CameraEase = cameraEase;
            CameraMotionDuration = cameraMotionDelay;
            GamePlayCameraPositions = gamePlayCameraPositions;
            OnDone = onDone;
            OnActivateUI = onActivateUI;
            FlowTimer = new CountdownTimer( NopactUtility.GenerateId(), 0.0f, Commons.Domain.Enum.CountdownScope.Game, true, ( id ) => OnFlowTimerComplete( id ), null );
        }

        public abstract void Run( );
        public virtual void ForceEnd()
        {
            if ( currentCameraTween != null )
            {
                currentCameraTween.Complete();
            }
        }
        protected abstract void OnCameraMotionComplete();

        protected virtual void OnFlowTimer() { }
        protected virtual void MoveCameraTo ( Vector3 matchingPosition, Quaternion matchingRotation )
        {
            currentCameraTween = CameraTarget.DOMove( matchingPosition, CameraMotionDuration );
            CameraTarget.rotation = matchingRotation;
            currentCameraTween.SetEase( CameraEase );
            currentCameraTween.Play();
            currentCameraTween.OnComplete( () => OnComplete() );
        }

        private void OnFlowTimerComplete( string id )
        {
            if ( id == flowTimerID )
            {
                OnFlowTimer();
            }
        }

        private void OnComplete()
        {
            OnCameraMotionComplete();
        }

        protected string FlowTimerID
        {
            set
            {
                flowTimerID = value;
            }
        }

        protected CountdownTimer FlowTimer
        {
            get;
            private set;
        }

        protected Action OnActivateUI
        {
            get;
            private set;
        }

        protected string CameraPositionKey
        {
            get;
            private set;
        }

        protected Action OnDone
        {
            get;
            private set;
        }

        protected Transform CameraTarget
        {
            get;
            private set;            
        }

        protected Ease CameraEase
        {
            get;
            private set;
        }

        protected float CameraMotionDuration
        {
            get;
            private set;
        }

        protected Dictionary<string, Transform> GamePlayCameraPositions
        {
            get;
            private set;
        }
    }
}
