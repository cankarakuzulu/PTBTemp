using UnityEngine;
using System.Collections;
namespace nopact.Commons.Monetization.Ads
{
    public interface IAdEvent
    {
        string AdUnitID
        {
            get;
        }
        AdTypes Type
        {
            get;
        }
    }
}
