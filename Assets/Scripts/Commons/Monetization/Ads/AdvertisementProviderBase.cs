using UnityEngine;
using System.Collections.Generic;
using System.Text;
using nopact.Commons.Utility.Timer;
using System;

namespace nopact.Commons.Monetization.Ads
{
    public abstract class AdvertisementProviderBase : MonoBehaviour
    {
        [SerializeField] protected bool isDebugging;        

        protected Advertisement currentAd;
        protected IAdEvent[ ] adEvents;
        protected string[ ] existingAdUnits;
        private bool[ ] adTypeAvailabilityRecord, adBufferingRecord;        
        private int adBufferTimeout = 5;

        protected bool Audit( Advertisement ad )
        {
            if ( currentAd == null && IsAdUnitRegistered( ad.AdUnit ) )
            {
                if (!IsAdTypeAvailable( ad.Type ))
                {
                    LoadAdType( ad.Type );
                }
                currentAd = ad;
                currentAd.SetReadyToShow( () => OnShow() );
                return true;
            }

            return false;
        }
                
        private void DisposeAd()
        {
            if ( currentAd != null )
            {
                currentAd.Dispose();
                currentAd = null;
            }
        }
                       
        protected abstract void RegisterEvents();
        protected abstract void ShowAd( string adUnit , AdTypes type );
        protected abstract void LoadAdType( AdTypes type );

        protected virtual void Initialize( IAdEvent[] aEvents )
        {
            existingAdUnits = ExtractAdUnits( aEvents );
            string[ ] adNames = System.Enum.GetNames( typeof( AdTypes ) );
            adTypeAvailabilityRecord = new bool[ adNames.Length ];
            adBufferingRecord = new bool[ adNames.Length ];
        }

        protected string[ ] ExtractAdUnits( IAdEvent[ ] adEvents )
        {
            string[ ] existingAdUnits = new string[ adEvents.Length ];

            StringBuilder sb = new StringBuilder();
            sb.Append( "Ad Unit list: " );
            for ( int unitIndex = adEvents.Length - 1; unitIndex > -1; unitIndex-- )
            {

                sb.AppendFormat( " {0} ", adEvents[ unitIndex ].AdUnitID );                
                existingAdUnits[ unitIndex ] = adEvents[ unitIndex ].AdUnitID;
            }

            if ( isDebugging )
            {
                Debug.Log( sb.ToString() );
            }

            return existingAdUnits;
        }

        protected bool IsAdUnitRegistered ( string adUnit )
        {
            if ( adEvents == null )
            {
                return false;
            }
            for ( int unitIndex = adEvents.Length - 1; unitIndex > -1; unitIndex-- )
            {
                if ( adEvents[ unitIndex ].AdUnitID == adUnit ) {
                    return true;
                }
            }

            return false;
        }

        protected bool IsAdTypeAvailable ( AdTypes type )
        {
            int adIndex = ( int ) type;
            return adTypeAvailabilityRecord[ adIndex ];
        }

        protected void SetAdAvailability ( AdTypes type, bool isAvailable )
        {
            int adIndex = ( int ) type;
#if !UNITY_EDITOR
            if ( adTypeAvailabilityRecord[ adIndex ] != isAvailable )
            {
                Advertisement.UpdateAdAvailability( type, isAvailable );
                adTypeAvailabilityRecord[ adIndex ] = isAvailable;
            }
#else
                Advertisement.UpdateAdAvailability( type, isAvailable );
                adTypeAvailabilityRecord[ adIndex ] = isAvailable;
#endif
        }

        protected bool IsAdBuffering( AdTypes type )
        {
            int adIndex = ( int ) type;
            return adBufferingRecord[ adIndex ];
        }

        protected void SetAdBuffering( AdTypes type, bool isBuffering )
        {
            if ( !IsAdBuffering(type) && isBuffering )
            {
                int adIndex = ( int ) type;
                adBufferingRecord[ adIndex ] = true;
                string id = string.Empty;

                id = TimerUtility.Instance.RegisterTimer( adBufferTimeout, Domain.Enum.CountdownScope.Game, ( s ) =>
                {
                    if ( s != id || !IsAdBuffering( type ) )
                    {
                        return;
                    }
                    SetAdBuffering( type, false );
                    OnAdBufferTimeout( type );
                } );
            }
            else
            {
                int adIndex = ( int ) type;
                adBufferingRecord[ adIndex ] = false;
            }
        }

        protected virtual void OnAdBufferTimeout( AdTypes type )
        {
            
        }

        protected void LoadUnavailableAd( AdTypes type )
        {         
               LoadAdType( type );      
        }

        private void OnShow()
        {
            if ( currentAd != null )
            {
                if ( isDebugging )
                {
                    Debug.Log( "Showing ad:" + currentAd.AdUnit );
                }                
                ShowAd( currentAd.AdUnit, currentAd.Type );
            }
        }
    }
}
