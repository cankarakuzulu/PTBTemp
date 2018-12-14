using System.Collections;
using System.Collections.Generic;
using nopact.Commons.SceneDirection;
using nopact.Commons.Input;
using nopact.Commons.SceneDirection.Resources;
using UnityEngine;
using nopact.Commons.Analytics;
using nopact.Commons.SceneDirection.PlaySession;

namespace nopact.Commons.SceneDirection
{
    public interface IDirector
    {        
        void Initialize( ISceneLoader loader, IResourceProvider resourceProvider, IAnalyticsTracker analyticsTracker );
        void RegisterCameraPosition(Transform t, string id);
        void Kill();
        void OnControlUpdate<T>( T inputState ) where T : InputStateBase;
        PlaySessionBase PlaySession { get; }
    }
}
