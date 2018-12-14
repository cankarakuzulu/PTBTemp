using UnityEngine;
using System.Collections;
using System;
namespace nopact.Commons.Monetization.Ads
{
    public class Advertisement : IDisposable
    {
        public static event Action<AdTypes, bool> OnAdAvailabilityChanged;

        private IAdEvent adEvent;
        private AdCallbacks adCallbacks;
        private Action onShow;    
        
        public static void UpdateAdAvailability( AdTypes ad, bool isAvailable )
        {
            if ( OnAdAvailabilityChanged != null )
            {
                OnAdAvailabilityChanged( ad, isAvailable );
            }
        }

        public void Show()
        {
            if ( onShow != null )
            {
                onShow();
            }
        }

        public void SetReadyToShow( Action onShow )
        {
            if ( this.onShow == null )
            {
                this.onShow = onShow;
            }            
        }

        public Advertisement( IAdEvent adEvent, AdCallbacks adCallbacks )
        {
            this.adEvent = adEvent;
            this.adCallbacks = adCallbacks;
        }

        public Advertisement ( IAdEvent adEvent, IAdListener listener )
        {
            this.adEvent = adEvent;
            this.adCallbacks = new AdCallbacks(
                ( error ) => listener.OnError( error ),
                () => listener.OnStarted(),
                () => listener.OnCompleted(),
                () => listener.OnClosed(),
                () => listener.OnInterstitialClicked(),
                ( reward, count ) => listener.OnRewardPending( reward, count ),
                () => listener.OnRewardedVideoStarted(),
                () => listener.OnRewardedVideoEnded() );
        }

        public string AdUnit
        {
            get { return adEvent.AdUnitID; }
        }

        public AdTypes Type
        {
            get { return adEvent.Type; }
        }

        public AdCallbacks Callbacks
        {
            get { return adCallbacks; }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposedValue )
            {
                if ( disposing )
                {
                    adCallbacks.Dispose();
                }              
                disposedValue = true;
            }
        }        
        
        public void Dispose()
        {         
            Dispose( true );         
        }
        #endregion
    }
}
