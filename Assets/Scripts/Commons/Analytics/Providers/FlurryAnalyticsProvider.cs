//#define Prime31Flurry
using UnityEngine;
using nopact.Commons.Analytics;
using System;
using System.Collections.Generic;

#if Prime31Flurry
using Prime31;
namespace nopact.Commons.Game.Analytics.Providers
{    
    public class FlurryAnalyticsProvider :AnalyticsProviderBase
    {
        [SerializeField] protected string flurryKeyIOS, flurryKeyAndroid;

        protected override void InitializeOnAwake()
        {
            FlurryAnalytics.startSession( flurryKeyIOS );
        }

        protected override void LogEventInternal( string eventID )
        {
            FlurryAnalytics.logEvent( eventID, false );
        }

        protected override void LogEventInternal( string eventID, params AnalyticsParameter[ ] parameters )
        {
            var parameterDictionary = ConvertParameters( parameters );
            FlurryAnalytics.logEventWithParameters( eventID, parameterDictionary, false );
        }

        protected override T ConvertParameters<T>( AnalyticsParameter[ ] parameters )
        {
            var dict = new Dictionary<string, string>();

            for ( int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++ )
            {
                var parameter = parameters[ parameterIndex ];
                dict.Add( parameter.Id, parameter.ValueToString() );
            }

            return dict as T;
        }

        private Dictionary<string, string> ConvertParameters( AnalyticsParameter[ ] parameters )
        {
            var dict = new Dictionary<string, string>();

            for ( int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++ )
            {
                var parameter = parameters[ parameterIndex ];
                dict.Add( parameter.Id, parameter.ValueToString() );
            }

            return dict;
        }

    }
}
#endif