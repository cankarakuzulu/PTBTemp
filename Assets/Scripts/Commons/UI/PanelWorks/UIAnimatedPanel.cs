using UnityEngine;
using System.Collections.Generic;


namespace nopact.Commons.UI.PanelWorks
{
    public abstract class UIAnimatedPanel :UIPanelBase
    {

        [SerializeField] protected UIAnimationHandlerBase animationHandler;
        [SerializeField] protected int openAnimationIndex, closeAnimationIndex;

        private Queue<int> queuedAnimations;

        public virtual void PlayCustomTransition( bool isHurried, int animationIndex, ActivationState activationState)
        {
            switch (activationState)
            {
                case ActivationState.Inert:
                    PlayAnimation( isHurried, animationIndex );
                    break;
                case ActivationState.ActivateThenExecute:
                    Open( customAnimationIndex: animationIndex );
                    break;
                case ActivationState.DeactivateAfterCompletion:
                    Close( customAnimationIndex: animationIndex );
                    break;
            }
        }

        /// <summary>
        /// Called when panel needs to activate ( i.e right after open command ). Put set active commands here. 
        /// </summary>
        protected override void Activate()
        {
            LockControls( true );
            if ( animationHandler != null )
            {                                
                animationHandler.OnCompleted += AnimationHandler_OnCompleted;
            }
        }

        /// <summary>
        /// Called when panel needs to deactivate. Put set active(false) here, this is where pool recycling occurs.
        /// </summary>
        protected override void Deactivate()
        {
            if ( animationHandler != null )
            {
                animationHandler.OnDeactivating();
                animationHandler.OnCompleted -= AnimationHandler_OnCompleted;
            }
        }

        /// <summary>
        /// Called internally when panel needs to close. 
        /// </summary>
        /// <param name="isHurried"></param>
        protected override void PlayClose( bool isHurried, int customAnimationIndex )
        {
            LockControls( true );
            state = PanelStates.Closing;
            Closing();
            if ( animationHandler == null )
            {
                AnimationHandler_OnCompleted();
                return;
            }
            int animationIndex = customAnimationIndex >= 0 ? customAnimationIndex : closeAnimationIndex;
            PlayAnimation( isHurried, animationIndex );

        }

        /// <summary>
        /// Called after close animation is completed.
        /// </summary>
        protected override void Closed()
        {
            base.Closed();
        }

        /// <summary>
        /// Called internally when panel needs to close
        /// </summary>
        /// <param name="isHurried"></param>
        protected override void PlayOpen( bool isHurried, int customAnimationIndex )
        {            
            state = PanelStates.Opening;
            if ( animationHandler == null )
            {
                AnimationHandler_OnCompleted();
                return;
            }
            int animationIndex = customAnimationIndex >= 0 ? customAnimationIndex : openAnimationIndex;
            PlayAnimation( isHurried, animationIndex );
            
        }


        protected void PlayAnimation ( bool isHurried, int animationIndex )
        {
            if ( animationHandler == null )
            {
                return;
            }

            if ( !isHurried )
            {
                animationHandler.SetAnimation( animationIndex );
                animationHandler.Play(); 
            }
            else if ( animationHandler.IsPlaying && animationHandler.ActiveAnimationIndex != animationIndex )
            {
                animationHandler.Hurry();
                if ( queuedAnimations == null )
                {
                    queuedAnimations = new Queue<int>();
                    queuedAnimations.Enqueue( animationIndex );
                }
            }
        }
        /// <summary>
        /// Called after open animation is completed.
        /// </summary>
        protected override void Opened()
        {
            base.Opened();
        }

        /// <summary>
        /// Stops all animations
        /// </summary>
        protected override void Stop()
        {

            if ( animationHandler != null && animationHandler.IsPlaying )
            {
                animationHandler.Stop();
            }


        }

        /// <summary>
        /// Generic setup method. It is better to call this before opening a panel.
        /// </summary>
        /// <typeparam name="T">UIPanelParameter inherited parameter type</typeparam>
        /// <param name="config">Config parameters</param>
        public override abstract void Setup<T>( T config );
        
        private void AnimationHandler_OnCompleted()
        {            
            if ( state == PanelStates.Closing )
            {
                Closed();
            }
            else if ( state == PanelStates.Opening )
            {
                Opened();
            }

            if ( queuedAnimations != null && queuedAnimations.Count > 0 )
            {
                int animInQueueIndex = queuedAnimations.Dequeue();
                PlayAnimation( true, animInQueueIndex );
            }
        }

        public enum ActivationState {
            Inert,
            ActivateThenExecute,
            DeactivateAfterCompletion
            }

    }

}
