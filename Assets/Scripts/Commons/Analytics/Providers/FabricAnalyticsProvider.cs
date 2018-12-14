//#define FabricAnalytics
#if FabricAnalytics
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabric.Answers;
using System;

namespace nopact.Commons.Analytics.Providers
{
    public class FabricAnalyticsProvider :AnalyticsProviderBase
    {
        protected override void InitializeOnAwake()
        {
                       
        }

        protected override void LogEventInternal( string eventID )
        {
            Answers.LogCustom( eventID );
        }

        protected override void LogEventInternal( string eventID, params AnalyticsParameter[ ] parameters )
        {
            Answers.LogCustom( eventID, ConvertParameters<Dictionary<string,object>>(parameters) );
        }
    }
}
#endif