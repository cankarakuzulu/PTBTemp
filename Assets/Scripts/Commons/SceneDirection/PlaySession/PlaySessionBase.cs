
using UnityEngine;
using System.Collections;
using nopact.Commons.SceneDirection.Resources;
using System;
using nopact.Commons.Camera;
using nopact.Commons.Input;

namespace nopact.Commons.SceneDirection.PlaySession
{    
    public abstract class PlaySessionBase
    {
        public PlaySessionBase( IResourceProvider resources, ICameraTracker cameraTracker )
        {
            this.CameraTracker = cameraTracker;
            this.Resources = resources;            
        }

        public abstract void Initialize();
        public abstract void InputInterrupt<T>( T inputState ) where T : InputStateBase;

        protected virtual void OnLevelEnd() { }

        public virtual void Kill()
        {            
            Resources = null;
            CameraTracker = null;         
        }
        protected virtual void OnDeath() { }

        protected IResourceProvider Resources { get; private set; }
        protected ICameraTracker CameraTracker { get; private set; }        
    }
}