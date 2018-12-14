using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nopact.Commons.Analytics
{
    public class GeneralAnalyticsDistributor :MonoBehaviour, IAnalyticsDistributor, IAnalyticsTracker
    {
        private List<IAnalyticsTracker> analyticsTrackers;
        public void LogEvent( string eventID, ushort eventClass = 0 )
        {
#if UNITY_EDITOR
            Debug.Log( string.Format( "<color=yellow>[Analytics] Event logged: {0} \nClass:{1}</color>", eventID, eventClass ) );
#else
            if ( analyticsTrackers == null )
            {
                return;
            }

            for ( int i = 0; i < analyticsTrackers.Count; i++ )
            {
                analyticsTrackers[i].LogEvent( eventID, eventClass );
            }            
#endif
        }

        public void LogEvent( string eventID, ushort eventClass=0, params AnalyticsParameter[ ] parameters )
        {
#if UNITY_EDITOR
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for ( int paramIndex = 0; paramIndex < parameters.Length; paramIndex++ )
            {
                sb.AppendFormat( " {0}", parameters[ paramIndex ].ToString() );
            }
            Debug.Log( string.Format( "<color=yellow>[Analytics] \nClass:{2} Event logged: {0} \n Params:{1}</color>", eventID, sb.ToString(), eventClass ) );
#else
            if ( analyticsTrackers == null )
            {
                return;
            }
            for ( int i = 0; i < analyticsTrackers.Count; i++ )
            {
                analyticsTrackers[ i ].LogEvent( eventID, eventClass, parameters );
            }            
#endif
        }

        public void Register( IAnalyticsTracker tracker )
        {
            if ( analyticsTrackers == null )
            {
                analyticsTrackers = new List<IAnalyticsTracker>();
            }

            if ( !analyticsTrackers.Contains( tracker ) )
            {
                analyticsTrackers.Add( tracker );
            }
        }
    }
}
