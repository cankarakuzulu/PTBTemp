using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace nopact.Commons.Analytics
{
    public abstract class AnalyticsProviderBase :MonoBehaviour, IAnalyticsTracker
    {
        [SerializeField] protected bool isActive;
        [SerializeField] protected bool isTrackingCustomAnalyticsEvents;
        [SerializeField] protected ushort eventClass = 0;

        public void LogEvent( string eventID, ushort eventClass )
        {
            if( eventClass != this.eventClass )
            {
                if ( eventClass == 99 && isActive )
                {
                    LogSpecialEvent( eventID);
                }
                return;
            }
            if ( isTrackingCustomAnalyticsEvents && isActive)
            {
                LogEventInternal( eventID );
            }            
        }

        public void LogEvent( string eventID, ushort eventClass = 0, params AnalyticsParameter[ ] parameters )
        {
            if ( eventClass != this.eventClass )
            {
                if ( eventClass == 99 && isActive )
                {
                    LogSpecialEvent( eventID, parameters );
                }
                return;
            }
            if ( isTrackingCustomAnalyticsEvents && isActive )
            {
                LogEventInternal( eventID, parameters );
            }            
        }

        protected virtual void Awake()
        {
            RegisterToDistributor();

            if ( isActive )
            {
                InitializeOnAwake();
            }            
        }

        protected virtual T ConvertParameters<T> ( AnalyticsParameter[] parameters) where T : class
        {
            Dictionary<string, object> parametersDict = new Dictionary<string, object>();

            foreach ( var parameter in parameters )
            {
                Type t;
                parametersDict.Add( parameter.Id, parameter.Get( out t ) );
            }

            return parametersDict as T;
        }

        private void RegisterToDistributor()
        {
            if ( !isTrackingCustomAnalyticsEvents || !isActive )
            {
                return;
            }            
            var distributor = ( IAnalyticsDistributor ) GetComponent( typeof( IAnalyticsDistributor ) );
            distributor.Register( this );
        }

        protected virtual void LogSpecialEvent( string eventID ) { }
        protected virtual void LogSpecialEvent( string eventID, params AnalyticsParameter[ ] parameters ) { }
        protected abstract void LogEventInternal( string eventID );
        protected abstract void LogEventInternal( string eventID, params AnalyticsParameter[ ] parameters );
        protected abstract void InitializeOnAwake();
    }
}
