//#define GameAnalytics
using System.Collections;
using System.Collections.Generic;
using nopact.Commons.Analytics;
using UnityEngine;
using System;

#if GameAnalytics
using GameAnalyticsSDK;

namespace nopact.Commons.Analytics.Providers
{
    public class GameAnalyticsProvider : AnalyticsProviderBase
    {

        protected override void InitializeOnAwake()
        {
            GameAnalytics.Initialize();
        }

        protected override void LogEventInternal( string eventID)
        {
            GameAnalytics.NewDesignEvent( eventID );
        }

        protected override void LogEventInternal( string eventID, params AnalyticsParameter[ ] parameters )
        {
            // GameAnalytics API only supports one parameter of float value in custom events.
            if ( parameters.Length<1)
            {
                GameAnalytics.NewDesignEvent( eventID );
                return;
            }

            System.Type type;
            object ctx = parameters[ 0 ].Get( out type );
            if ( type == typeof(float))
            {
                float parameterValue = Convert.ToSingle( ctx );
                GameAnalytics.NewDesignEvent( eventID, parameterValue );
                return;
            }

            Debug.LogWarning( string.Format("[GA] Design elements doesn't support any other parameter than parameters of type float. Event id: {0}", eventID) );
            GameAnalytics.NewDesignEvent( eventID );
        }

        // Voodoo special event suport.
        protected override void LogSpecialEvent( string eventID )
        {
            base.LogSpecialEvent( eventID );

            if ( eventID == "LEVEL_STARTED" )
            {
                GameAnalytics.NewProgressionEvent( GAProgressionStatus.Start, "game" );
            }            
        }

        protected override void LogSpecialEvent( string eventID, params AnalyticsParameter[ ] parameters )
        {
            base.LogSpecialEvent( eventID, parameters );
            if ( eventID == "LEVEL_COMPLETED" )
            {
                System.Type type;
                GameAnalytics.NewProgressionEvent( GAProgressionStatus.Complete, "game", Convert.ToInt32( parameters[ 0 ].Get( out type ) ));
            }
            else if ( eventID == "LEVEL_FAILED" )
            {
                GameAnalytics.NewProgressionEvent( GAProgressionStatus.Fail, "game" );
            }
        }
    }
}
#endif