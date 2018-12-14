using UnityEngine;
using System;
using System.Collections;

namespace nopact.Commons.UI.PanelWorks
{
    // Using abstract classes rather then interfaces because of Unity's inability to serialize collections with interface elements.
    public abstract class UIAnimationHandlerBase : MonoBehaviour
    {

        public abstract event Action OnCompleted, OnPaused, OnStoped, OnCanceled;

        protected int activeAnimationIndex;
        protected bool isPlaying, isInitialized;

        public abstract void Play();
        public abstract void Hurry(); 
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Cancel();
        public abstract void OnDeactivating();
        public abstract void SetAnimation( int animationIndex );
        
        public virtual bool IsPlaying { get { return isPlaying; } }
        public virtual int ActiveAnimationIndex { get { return activeAnimationIndex; } }
    }
  
}
