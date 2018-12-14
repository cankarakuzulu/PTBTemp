using UnityEngine;
using System.Collections;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;

namespace nopact.Commons.Monetization.IAP
{
    public interface NPIAPListener
    {
        void OnInitResult(Product[] products, bool isSuccessful,string result );
        void OnRestoreResult( bool isSuccessful, string result );
        void OnPurchaseResult( bool isSuccessful, string id, string result );   
    }
}
#else
namespace nopact.Commons.Monetization.IAP
{
    public interface NPIAPListener
    {
        void OnInitResult( object[ ] products, bool isSuccessful, string result );
        void OnRestoreResult( bool isSuccessful, string result );
        void OnPurchaseResult( bool isSuccessful, string id, string result );
    }
}

#endif