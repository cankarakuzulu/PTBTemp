using System.Collections;
using System.Collections.Generic;
using nopact.Commons.Analytics;
using nopact.Commons.SceneDirection.Resources;
using UnityEngine;

namespace nopact.Commons.SceneDirection
{
    [RequireComponent( typeof( IDirector ) )]
    public class GameManagement :MonoBehaviour
    {
        private IDirector director;
        public void Initialize( ISceneLoader loader, IResourceProvider resourceProvider, IAnalyticsTracker analyticsTracker )
        {
            director = (IDirector)GetComponent( typeof( IDirector ) );

            if ( director != null )
            {
                director.Initialize( loader, resourceProvider, analyticsTracker );
            }
        }

    }
}
