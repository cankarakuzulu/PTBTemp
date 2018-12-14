//#define Dev2DevAnalytics
using System.Linq;
using UnityEngine;
using System;
#if Dev2DevAnalytics
using DevToDev;
using DTDEditor;

namespace nopact.Commons.Analytics.Providers
{
    public class Dev2DevAnalyticsProvider :AnalyticsProviderBase
    {
        [SerializeField] protected DTDCredentials[ ] Credentials;

        protected override void InitializeOnAwake()
        {
#if UNITY_ANDROID
			InitializeAnalytics(DTDPlatform.Android);
#elif UNITY_IOS
            InitializeAnalytics( DTDPlatform.iOS );
#endif
        }

        private void InitializeAnalytics( DTDPlatform platform )
        {
            var targetCredential = Credentials.FirstOrDefault( item => item.Platform == platform );
            if ( targetCredential != null )
            {
                DevToDev.Analytics.Initialize( targetCredential.Key, targetCredential.Secret );
            }
        }

        protected override void LogEventInternal( string eventID )
        {
            DevToDev.Analytics.CustomEvent( eventID );
        }

        protected override void LogEventInternal( string eventID, params AnalyticsParameter[ ] parameters )
        {
            DevToDev.Analytics.CustomEvent( eventID, ConvertParameters<CustomEventParams>( parameters ) );
        }

        protected override T ConvertParameters<T>( AnalyticsParameter[ ] parameters )
        {
            CustomEventParams dev2DevParams = new CustomEventParams();
            for( int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex ++ )
            {
                var param = parameters[ parameterIndex ];
                System.Type type;
                object value = param.Get( out type );

                if( value == null )
                {
                    continue;
                }

                if( type == typeof( int ))
                {
                    dev2DevParams.AddParam( param.Id, ( int ) value );
                }
                else if ( type == typeof( float ))
                {
                    dev2DevParams.AddParam( param.Id, Convert.ToDouble(value) );
                }
                else if ( type == typeof( string ) )
                {
                    dev2DevParams.AddParam( param.Id, ( string ) value );
                }
                else
                {
                    throw new System.ArgumentException( "[Analytics] Analytics system can only handle int, long and string types." );
                }
            }
            return dev2DevParams as T;
        }
    }

}
#endif
