using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace nopact.Commons.UI.PanelWorks
{
    [Serializable]
    public abstract class TweenTimelineBase<T,G> where T: TweenWrapperBase<G> where G : class
    {
                
        protected Queue<T> executionQueue;
        protected T currentTween;
        protected bool isPlaying = false, isForcingComplete = false;
        protected Action onComplete;

        public abstract void Execute();
        public abstract void Play();
        public abstract void Pause();
        public abstract void ForceComplete();
        public abstract void Stop();

        public void SetOnComplete( Action callback )
        {
            this.onComplete = callback;
        }

        protected void ProcessNext()
        {
            if ( executionQueue.Count > 0 )
            {

                currentTween = executionQueue.Dequeue();
                currentTween.Render( () => OnTweenExecutionCompleted() );              
                if( currentTween != null )
                {
                    currentTween.Play();
                }                
              
                if ( isForcingComplete )
                {
                    currentTween.ForceComplete();
                }

            }
            else if (isPlaying)
            {

                isPlaying = false;
                currentTween = null;
                if ( onComplete != null)
                {
                    onComplete();
                    onComplete = null;
                }                

            }

        }

        private void OnTweenExecutionCompleted()
        {
            ProcessNext();
        }

        public abstract float Duration
        {
            get;
        }
    }
}
