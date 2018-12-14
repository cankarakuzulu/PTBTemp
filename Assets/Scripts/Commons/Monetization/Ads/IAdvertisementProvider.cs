using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Monetization.Ads
{
    public interface IAdvertisementProvider
    {        
        void Init( IAdEvent[] adEvents );                
        bool Audit( Advertisement ad );
        bool GetAdAvailability( AdTypes adType );
    }
}
