//#define FBAnalytics
#if FBAnalytics
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Facebook.Unity;
using System;

namespace nopact.Commons.Analytics.Providers
{
    public class FacebookAnalyticsProvider :AnalyticsProviderBase
    {
        protected override void InitializeOnAwake()
        {
            FB.Init( () => OnInitComplete(), (isGameShown) => OnHideUnity(isGameShown) );
        }
        
        protected override void LogEventInternal( string eventID )
        {
            FB.LogAppEvent( eventID );
        }

        protected override void LogEventInternal( string eventID, params AnalyticsParameter[ ] parameters )
        {
            FB.LogAppEvent( eventID, null,  ConvertParameters<Dictionary<string, object>>( parameters ) );
        }

        private void OnInitComplete()
        {
            string logMessage = string.Format(
                           "[FacebookProvider] OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
                           FB.IsLoggedIn,
                           FB.IsInitialized );
            Debug.Log( logMessage );
            if ( AccessToken.CurrentAccessToken != null )
            {
                Debug.Log( string.Format( "[FacebookProvider] Access token: {0}", AccessToken.CurrentAccessToken.ToString()) );
            }
            if ( FB.IsInitialized )
            {
                FB.ActivateApp();
            }
            else
            {
                Debug.LogWarning( "[FacebookProvider] FB failed to initialize." );
            }

        }

        private void OnHideUnity( bool isGameShown )
        {
            if ( !isGameShown )
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}
#endif