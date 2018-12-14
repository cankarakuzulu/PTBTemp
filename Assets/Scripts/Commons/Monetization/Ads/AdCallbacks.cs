using UnityEngine;
using System.Collections;
using System;

namespace nopact.Commons.Monetization.Ads
{
    public class AdCallbacks : IDisposable
    {        
        private Action<string> onError;
        private Action onStarted;
        private Action onCompleted;
        private Action onClosed;
        private Action<string, int> onRewardPending;
        private Action onRewardedVideoStarted, onRewardedVideoEnded, onInterstitialClicked;

        public AdCallbacks( 
            Action<string> onError, 
            Action onStarted, 
            Action onCompleted, 
            Action onClosed = null,
            Action onInterstitialClicked = null,
            Action<string, int> onRewardPending = null, 
            Action onRewardedVideoStarted = null, 
            Action onRewardedVideoEnded = null )
        {
            OnError = onError;
            OnStarted = onStarted;
            OnCompleted = onCompleted;
            OnClosed = onClosed;
            OnRewardPending = onRewardPending;
            OnRewardedVideoStarted = onRewardedVideoStarted;
            OnRewardedVideoEnded = onRewardedVideoEnded;
            OnInterstitialClicked = onInterstitialClicked;
            
        }

        public Action<string, int> OnRewardPending
        {
            get
            {
                return onRewardPending;
            }
            private set{ onRewardPending = value; }
        }

        public Action OnClosed
        {
            get
            {
                return onClosed;
            }
            private set { onClosed = value; }
        }

        public Action OnCompleted
        {
            get
            {
                return onCompleted;
            }
            private set { onCompleted = value; }                
        }

        public Action OnStarted
        {
            get
            {
                return onStarted;
            }
            private set { onStarted = value; }
        }

        public Action<string> OnError
        {
            get
            {
                return onError;
            }

            private set { onError = value; }
        }

        public Action OnRewardedVideoStarted
        {
            get
            {
                return onRewardedVideoStarted;
            }

            private set
            {
                onRewardedVideoStarted = value;
            }
        }    

        public Action OnRewardedVideoEnded
        {
            get
            {
                return onRewardedVideoEnded;
            }

            private set
            {
                onRewardedVideoEnded = value;
            }
        }

        public Action OnInterstitialClicked
        {
            get
            {
                return onInterstitialClicked;
            }

           private set
            {
                onInterstitialClicked = value;
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposedValue )
            {
                if ( disposing )
                {
                    onError = null;
                    onRewardPending = null;
                    onRewardedVideoStarted = null;
                    onRewardedVideoEnded = null;
                    onStarted = null;
                    onCompleted = null;
                    onClosed = null;
                    onInterstitialClicked = null;

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
