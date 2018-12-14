#if UNITY_PURCHASING
using UnityEngine;
using UnityEngine.Purchasing;
using System;

namespace nopact.Commons.Monetization.IAP
{
    [Serializable]
    public class IAPProduct
    {
        [SerializeField] protected string name, id, storeSpecificID;
        [SerializeField] protected ProductType type;
        [SerializeField] protected int value;
        [SerializeField] protected bool isActive;
        [SerializeField] protected PayoutType payoutType;
        [SerializeField] protected string payoutSubtype;
        [SerializeField] protected double payoutQuantity;
        [SerializeField] protected string payoutData;
        [SerializeField] protected bool isCoinPack;

        private ProductMetadata metaData;

        public bool IsCoinPack
        {
            get
            {
                return isCoinPack;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }            
        }

        public string StoreSpecificID
        {
            get
            {
                return storeSpecificID;
            }
        }

        public ProductType Type
        {
            get
            {
                return type;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }

        public PayoutType PayoutType
        {
            get
            {
                return payoutType;
            }
        }

        public string PayoutSubtype
        {
            get
            {
                return payoutSubtype;
            }
        }

        public double PayoutQuantity
        {
            get
            {
                return payoutQuantity;
            }
        }

        public string PayoutData
        {
            get
            {
                return payoutData;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }
        }

        public ProductDefinition ProdDefinition
        {
            get
            {
                var payoutDefinition = new PayoutDefinition( payoutType, payoutSubtype, payoutQuantity, payoutData );
                var productDefinition = new ProductDefinition( id, storeSpecificID, type, isActive, payoutDefinition );

                return productDefinition;
            }            
        }

        public ProductMetadata MetaData
        {
            get
            {
                return metaData;
            }

            set
            {
                metaData = value;
            }
        }
    }
}
#else
using UnityEngine;
using System;
namespace nopact.Commons.Monetization.IAP
{
    [Serializable]
    public class IAPProduct
    {
    }
}
#endif