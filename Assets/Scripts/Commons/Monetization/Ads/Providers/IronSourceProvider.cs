//#define IronsourceSDK
using UnityEngine;
using nopact.Commons.Monetization.Ads;
using System.Collections.Generic;
using System;
using nopact.Commons.Utility.Timer;

#if IronsourceSDK

namespace nopact.Commons.Monetization.Ads.Providers
{

    public class IronSourceProvider : AdvertisementProviderBase, IAdvertisementProvider
    {
        [SerializeField] protected GameObject ironSourceEventDispenser;
        [SerializeField] protected string appKey;
        [SerializeField] protected int initialAdLoadDelay = 2, adRerequestTimer = 4;
        [SerializeField] protected float adCheckInterval = 5f;

        private bool isWaitingForAdReload = false;
        private float lastCheckDate;

        public void Init( IAdEvent[] adEvents )
        {
            this.adEvents = adEvents;
            lastCheckDate = Time.time;
#if !UNITY_EDITOR
            GameObject eventDispenser = Instantiate<GameObject>( ironSourceEventDispenser );
            IronSource.Agent.setConsent( true );
            RegisterEvents();
            
            if ( isDebugging )
            {
                //IronSource.Agent.setAdaptersDebug( true );
                //IronSource.Agent.validateIntegration();                
            }
#endif

            Initialize( adEvents );

        }

        public new bool Audit( Advertisement ad )
        {
#if UNITY_EDITOR
            if ( isDebugging )
            {
                Debug.Log( "[IronSourceProvider] Stub - Audits always return positive on editor." );
            }
            return base.Audit( ad );
#else
            return base.Audit( ad );
#endif
        }

        public bool GetAdAvailability( AdTypes adType )
        {
#if UNITY_EDITOR
            return true;
#else
            switch ( adType )
            {
                case AdTypes.Interstitial:
                    return IronSource.Agent.isInterstitialReady();

                case AdTypes.Rewarded:
                    return IronSource.Agent.isRewardedVideoAvailable();

                default:
                    return false;
            }            
#endif
        }

        protected override void Initialize( IAdEvent[ ] aEvents )
        {
            base.Initialize(aEvents);

#if !UNITY_EDITOR
            IronSource.Agent.reportAppStarted();
            List<string> units = new List<string>();
            units.AddRange( existingAdUnits );
            units.Add( IronSourceAdUnits.REWARDED_VIDEO );
            units.Add( IronSourceAdUnits.INTERSTITIAL );
            IronSource.Agent.init( appKey, units.ToArray() );
            TimerUtility.Instance.RegisterTimer( initialAdLoadDelay, Commons.Domain.Enum.CountdownScope.Game, ( id ) => {
                LoadAdType( AdTypes.Interstitial ); } );
#endif

        }

        protected override void ShowAd( string adUnit, AdTypes type )
        {
#if UNITY_EDITOR

            if ( isDebugging ) Debug.Log( "[IronSourceProvider] Editor stub. Faking successful ad display." );
            if ( currentAd != null )
            {
                switch ( currentAd.Type )
                {
                    case AdTypes.Interstitial:
                        InterstitialAdShowSucceededEvent();
                        InterstitialAdOpenedEvent();
                        InterstitialAdClosedEvent();
                        InterstitialAdReadyEvent();
                        break;
                    case AdTypes.Rewarded:
                        RewardedVideoAdStartedEvent();
                        RewardedVideoAdOpenedEvent();
                        RewardedVideoAdEndedEvent();                        
                        RewardedVideoAdClosedEvent();
                        RewardedVideoAdRewardedEvent( new IronSourcePlacement( "test", "test", 1 ) );
                        RewardedVideoAvailabilityChangedEvent( true );
                        break;
                }
            }
            return;
#else
            switch( type )
            {
                case AdTypes.Interstitial:
                    IronSource.Agent.showInterstitial( adUnit );
                    break;
                case AdTypes.Rewarded:
                    IronSource.Agent.showRewardedVideo( adUnit );
                    break;
            }
#endif
        }

        protected override void RegisterEvents()
        {
            IronSourceEvents.onSegmentReceivedEvent += IronSourceEvents_onSegmentReceivedEvent;

            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;

            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        }

        private void IronSourceEvents_onSegmentReceivedEvent( string obj )
        {
            if ( isDebugging ) Debug.Log( "Segment received:" + obj );
        }

        protected override void LoadAdType( AdTypes type )
        {
#if UNITY_EDITOR
            SetAdAvailability( type, true );
#else
            SetAdBuffering( type, true );
            switch ( type )
            {
                case AdTypes.Interstitial:                    

                    IronSource.Agent.loadInterstitial();                                        
                    break;
            }
#endif
        }


#region EventHandlers
        private void InterstitialAdClosedEvent()
        {
            if ( isDebugging ) Debug.Log( "interstitial closed" );
            if ( currentAd != null && currentAd.Callbacks.OnClosed != null )
            {
                currentAd.Callbacks.OnClosed();                
            }
            if ( currentAd != null && currentAd.Callbacks.OnCompleted != null )
            {
                currentAd.Callbacks.OnCompleted();
                currentAd.Dispose();
                currentAd = null;
            }
            isWaitingForAdReload = false;
            SetAdBuffering( AdTypes.Interstitial, false );
        }

        private void InterstitialAdOpenedEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Interstitial opened" );
            }
            if ( currentAd != null && currentAd.Callbacks.OnStarted != null )
            {
                currentAd.Callbacks.OnStarted();
            }
        }

        private void InterstitialAdClickedEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Interstitial Adclicked" );
            }
            
            if ( currentAd != null && currentAd.Callbacks.OnInterstitialClicked != null )
            {
                currentAd.Callbacks.OnInterstitialClicked();
            }
        }

        private void InterstitialAdShowFailedEvent( IronSourceError error )
        {
            if ( isDebugging )
            {
                Debug.Log( string.Format("[ironSourceEvent] Interstitial failed. Error: {0}", error.getDescription() ) );
            }
            
            if ( currentAd != null && currentAd.Callbacks.OnError != null )
            {
                currentAd.Callbacks.OnError (error.getDescription());
            }
            currentAd.Dispose();
            currentAd = null;            
        }

        private void InterstitialAdShowSucceededEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Interstitial Show succeeded." );
            }            
          
        }

        private void InterstitialAdLoadFailedEvent( IronSourceError error )
        {
            if ( isDebugging )
            {
                Debug.Log( string.Format( "[ironSourceEvent] Interstitial ad load failed. Error: {0} ", error.getDescription() ) );
            }
            SetAdBuffering( AdTypes.Interstitial, false );
            SetAdAvailability( AdTypes.Interstitial, false );
            currentAd = null;                  
        }

        private void InterstitialAdReadyEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Interstitial ad ready" );
            }
            SetAdBuffering( AdTypes.Interstitial, false );
            SetAdAvailability( AdTypes.Interstitial, true );            
        }

        private void RewardedVideoAdShowFailedEvent( IronSourceError error )
        {
            if ( isDebugging )
            {
                Debug.Log( string.Format( "[ironSourceEvent] Rewarded video show failed. Error:{0}", error.getDescription() ) );
            }            
            if ( currentAd != null && currentAd.Callbacks.OnError != null )
            {
                currentAd.Callbacks.OnError( error.getDescription() );
                currentAd.Dispose();
                currentAd = null;
            }
            SetAdAvailability( AdTypes.Interstitial, false );
        }

        private void RewardedVideoAdRewardedEvent( IronSourcePlacement placement )
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Rewarded ad. Rewarded event" );
            }            
            if ( currentAd != null && currentAd.Callbacks.OnRewardPending != null )
            {
                currentAd.Callbacks.OnRewardPending( placement.getRewardName(), placement.getRewardAmount() );
                currentAd.Dispose();
                currentAd = null;
            }
        }

        private void RewardedVideoAdEndedEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Rewarded ad ended." );
            }
                        
            if ( currentAd != null && currentAd.Callbacks.OnRewardedVideoEnded != null )
            {
                currentAd.Callbacks.OnRewardedVideoEnded();
            }
        }

        private void RewardedVideoAdStartedEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Rewarded ad started." );
            }            
            if ( currentAd != null && currentAd.Callbacks.OnRewardedVideoStarted != null )
            {
                currentAd.Callbacks.OnRewardedVideoStarted();
            }
        }

        private void RewardedVideoAvailabilityChangedEvent( bool isAvailable )
        {
            if ( isDebugging )
            {
                Debug.Log( string.Format( "[ironSourceEvent] Rewarded video availability changed to {0}", isAvailable ? "AVAILABLE": "UNAVAILABLE" ));
            }                        
            SetAdAvailability( AdTypes.Rewarded, isAvailable );
        }

        private void RewardedVideoAdOpenedEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Rewarded ad opened" );
            }
            
            if ( currentAd != null && currentAd.Callbacks.OnStarted != null )
            {
                currentAd.Callbacks.OnStarted();
            }
        }

        private void RewardedVideoAdClosedEvent()
        {
            if ( isDebugging )
            {
                Debug.Log( "[ironSourceEvent] Rewarded ad closed" );
            }

            if ( currentAd != null && currentAd.Callbacks.OnCompleted != null )
            {
                currentAd.Callbacks.OnCompleted();
            }

            if ( currentAd != null && currentAd.Callbacks.OnClosed != null )
            {
                currentAd.Callbacks.OnClosed();       
            }
        }

        private void OnApplicationPause( bool pause )
        {
            IronSource.Agent.onApplicationPause( pause );
        }

#endregion

        private void Update()
        {
            

            if ( this.adEvents == null )
            {
                return;
            }

#if !UNITY_EDITOR
            if ( Time.time - lastCheckDate > adCheckInterval )
            {

                bool isInterstitialReady = IronSource.Agent.isInterstitialReady();
                //Debug.Log ( "WFL:"+isWaitingForAdReload + "IIR:" + isInterstitialReady + "IAB" +IsAdBuffering( AdTypes.Interstitial ));

                if ( !isWaitingForAdReload && !IronSource.Agent.isInterstitialReady() && !IsAdBuffering( AdTypes.Interstitial ) )
                {
                    isWaitingForAdReload = true;
                    TimerUtility.Instance.RegisterTimer( adRerequestTimer, Commons.Domain.Enum.CountdownScope.Game, ( id ) =>
                    {
                        isWaitingForAdReload = false;
                        LoadUnavailableAd( AdTypes.Interstitial );
                    } );
                }
                lastCheckDate = Time.time;
            }
#else
            if ( !IsAdTypeAvailable( AdTypes.Interstitial ) && !isWaitingForAdReload)
            {
                isWaitingForAdReload = true;
                TimerUtility.Instance.RegisterTimer( adRerequestTimer, Commons.Domain.Enum.CountdownScope.Game, ( id ) =>
                {
                    LoadAdType( AdTypes.Interstitial );
                    isWaitingForAdReload = false;
                } );                
            }

           if ( !IsAdTypeAvailable( AdTypes.Rewarded ) && !isWaitingForAdReload  )
            {
                TimerUtility.Instance.RegisterTimer( adRerequestTimer, Commons.Domain.Enum.CountdownScope.Game, ( id ) =>
                {
                    LoadAdType( AdTypes.Rewarded );
                    isWaitingForAdReload = false;
                } );
            }
#endif
        }
    }
}
#endif