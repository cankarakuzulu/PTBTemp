using UnityEngine;
using System.Collections;

namespace nopact.Commons.Monetization.Ads
{
    public interface IAdListener
    {
        void OnError( string error );
        void OnStarted();
        void OnCompleted();
        void OnClosed();
        void OnInterstitialClicked();
        void OnRewardPending(string id, int count);
        void OnRewardedVideoStarted();
        void OnRewardedVideoEnded();        
    }
}
