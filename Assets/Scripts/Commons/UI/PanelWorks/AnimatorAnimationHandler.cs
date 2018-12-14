
using UnityEngine;
using System.Collections;
using System;

namespace nopact.Commons.UI.PanelWorks
{
    public class AnimatorAnimationHandler :UIAnimationHandlerBase
    {

        [SerializeField] protected Animator activeAnimator;
        [SerializeField] private string[ ] triggerIDs;
        [SerializeField] private float hurriedSpeed = 2.0f;

        public override event Action OnCompleted;
        public override event Action OnPaused;
        public override event Action OnStoped;
        public override event Action OnCanceled;

        private int[ ] triggerHashes;
        private float defaultSpeed;        
        private int animatorInstanceID;
        private int lastAnimatorStateHash;
        private bool isOnPendingMode;

        public override void Cancel()
        {
            if ( OnCanceled != null )
            {
                OnCanceled();
            }
            Stop();

        }

        public override void Hurry()
        {
            if ( isPlaying )
            {
                activeAnimator.speed = hurriedSpeed;
            }
        }

        public override void Pause()
        {
            if ( isPlaying )
            {
                activeAnimator.speed = 0;
                isPlaying = false;

                if ( OnPaused != null)
                {
                    OnPaused();
                }
            }
        }

        public override void Play( )
        {
            
            if ( !isInitialized )
            {
                Debug.LogWarning( string.Format("Set Animation call before awake. Call receiver:{0}",gameObject.name) );
                return;
            }

            if ( isPlaying )
            {
                Debug.Log( string.Format( "{0} is already Playing. Next animation added to queue.", gameObject.name) );                
            }

            if ( isInitialized && isOnPendingMode )
            {
                activeAnimator.Play( lastAnimatorStateHash );
                isOnPendingMode = false;                
            }

            if ( activeAnimationIndex >= 0 )
            {
                activeAnimator.speed = defaultSpeed;
                activeAnimator.SetTrigger( triggerHashes[ activeAnimationIndex ] );                        
                isPlaying = true;
            }
        }

        public override void SetAnimation( int animationIndex )
        { 
            if( !isInitialized )
            {
                Debug.LogWarning( "Set Animation call before awake. Call receiver:" + gameObject.name );
                return;
            }

            Debug.Assert( animationIndex >= 0 && animationIndex < triggerHashes.Length );
            this.activeAnimationIndex = animationIndex;            

        }

        public override void Stop()
        {
            activeAnimator.speed = 0;
            isPlaying = false;
            activeAnimationIndex = -1;

            if ( OnStoped != null )
            {
                OnStoped();
            }
        }

        public void ForfeitAnimationHashRestore()
        {
            this.activeAnimationIndex = -1;
            this.lastAnimatorStateHash = 0;
            isOnPendingMode = false;
        }

        private void OnEnable()
        {

            AnimatorStateTrigger.OnEnter += AnimatorStateTrigger_OnStateTrigger;
            AnimatorStateTrigger.OnExit += AnimatorStateTrigger_OnStateTrigger;            

        }
        
        private void OnDisable()
        {
            AnimatorStateTrigger.OnEnter -= AnimatorStateTrigger_OnStateTrigger;
            AnimatorStateTrigger.OnExit -= AnimatorStateTrigger_OnStateTrigger;

            
        }
        
        protected virtual void Awake()
        {
            isInitialized = true;
            animatorInstanceID = activeAnimator.GetInstanceID();            
            defaultSpeed = activeAnimator.speed;

            BuildTriggerHash( triggerIDs, out triggerHashes );
        }

        protected void BuildTriggerHash ( string[] source, out int[] output )
        {
            output = new int[ source.Length ];

            for ( int triggerIndex = 0; triggerIndex < source.Length; triggerIndex++ )
            {

                output[ triggerIndex ] = Animator.StringToHash( source[ triggerIndex ] );

            }

        }

        private void AnimatorStateTrigger_OnStateTrigger( int animatorInstanceID, string triggerID )
        {
                           
            if ( animatorInstanceID != this.animatorInstanceID || activeAnimationIndex < 0 )
            {
                return;
            }            
            int triggerHash = Animator.StringToHash( triggerID );
            if ( triggerHash.Equals( triggerHashes[activeAnimationIndex]))
            {
                CompleteAnimation();
            }            

        }

        private void CompleteAnimation()
        {         
            isPlaying = false;
            activeAnimationIndex = -1;
            
            if ( OnCompleted != null )
            { 
                OnCompleted();
            }
        }

        public override void OnDeactivating()
        {
            lastAnimatorStateHash = activeAnimator.GetCurrentAnimatorStateInfo( 0 ).fullPathHash;
            activeAnimationIndex = -1;
            isOnPendingMode = true;
        }
    }
}
