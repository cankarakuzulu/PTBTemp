using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Analytics
{
    public interface IAnalyticsTracker
    {
        void LogEvent( string eventID, ushort eventClass = 0 );
        void LogEvent( string eventID, ushort eventClass = 0, params AnalyticsParameter[ ] parameters );
    }
}
