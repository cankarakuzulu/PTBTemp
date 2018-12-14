using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nopact.Commons.Analytics
{
    public interface IAnalyticsDistributor 
    {
        void Register( IAnalyticsTracker tracker );
    }
}
