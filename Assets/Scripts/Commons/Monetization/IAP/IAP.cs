#if UNITY_PURCHASING
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections;
using System;
using UnityEngine.Purchasing.Security;

namespace nopact.Commons.Monetization.IAP
{
    public class IAP :MonoBehaviour, IStoreListener
    {

        [SerializeField] private bool isDebugging = false;
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;
        private NPIAPListener listener;

        public void Initialize( ProductDefinition[] products, NPIAPListener listener )
        {
            if ( storeController == null )
            {
                InitPurchasing( products );
                this.listener = listener;
            }
        }

        public void Initialize ( IAPProduct[] products, NPIAPListener listener )
        {
            if ( storeController == null )
            {
                this.listener = listener;
                ProductDefinition[ ] proddefs = new ProductDefinition[ products.Length ];
                for ( int prodIndex = 0; prodIndex < proddefs.Length; prodIndex++ )
                {
                    proddefs[ prodIndex ] = products[ prodIndex ].ProdDefinition;

                }
                InitPurchasing( proddefs );
            }
        }

        private void InitPurchasing( ProductDefinition[ ] products )
        {
            if ( IsInitialized )
            {
                return;
            }

            var configBuilder = ConfigurationBuilder.Instance( StandardPurchasingModule.Instance() );
            configBuilder.AddProducts( products );
            UnityPurchasing.Initialize( this, configBuilder );

        }

        public void BuyProductID( string productId )
        {
            
            if ( IsInitialized )
            {
                var product = storeController.products.WithID( productId );
                
                if ( product != null && product.availableToPurchase )
                {
                    if ( isDebugging ) Debug.Log( string.Format( "[IAP] Purchasing product asychronously: '{0}'", product.definition.id ) );                
                    storeController.InitiatePurchase( product );
                }                
                else
                {
                    if ( isDebugging ) Debug.Log( "[IAP] BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase" );
                    listener.OnPurchaseResult( false, productId, "Product not available." );
                }
            }            
            else
            {
                if ( isDebugging ) Debug.Log( "[IAP] Buy failed. Purchasing is uninitialized." );
                listener.OnPurchaseResult( false, productId, "Uninitialized." );
            }
        }

        
        public void RestorePurchases()
        {            
            if ( !IsInitialized )
            {

                if ( isDebugging ) Debug.Log( "[IAP] Restore Failed. Not initialized." );
                listener.OnRestoreResult( false, "Uninitialized" );
                return;
            }
            
            if ( Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer )
            {
                          
                var apple = extensionProvider.GetExtension<IAppleExtensions>();                
                apple.RestoreTransactions( ( result ) => {
                   
                    Debug.Log( string.Format("[IAP] RestorePurchases result: {0}" , result ));
                    listener.OnRestoreResult( result, string.Empty );
                } );
            }            
            else
            {
                if ( isDebugging ) Debug.Log( "[IAP] This platform does not require Restore.");
            }
        }

        public void OnInitialized( IStoreController controller, IExtensionProvider extensions )
        {
            if ( isDebugging ) Debug.Log( "[IAP] Store initialized." );
            storeController = controller;
            extensionProvider = extensions;
            listener.OnInitResult( storeController.products.all,true, "Successful initialize." );
            
        }

        public void OnInitializeFailed( InitializationFailureReason error )
        {
            if ( isDebugging ) Debug.Log( string.Format( "[IAP] Store intialization failed: {0}", error.ToString()) );
            listener.OnInitResult( null, false, error.ToString() );
        }

        public void OnPurchaseFailed( Product i, PurchaseFailureReason p )
        {
            if ( isDebugging ) Debug.Log( string.Format( "[IAP] Product purchase for {0} failed: {1}", i.definition.id.ToString(), p.ToString() ) );
            listener.OnPurchaseResult( false, i.definition.id, p.ToString() );
        }

        public ProductMetadata GetMetaData( string id )
        {            
            foreach ( var item in storeController.products.all )
            {
                if ( item.definition.id == id )
                {
                    return item.metadata;
                }
            }

            if ( isDebugging ) Debug.Log( string.Format( @"[IAP] Item '{0}' is missing.", id ) );
            return null;
        }


        public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs e )
        {
            var productID = e.purchasedProduct.definition.id;

            // Validate receipts using obfuscated keys. (Local validation) 
            // Read more: https://docs.unity3d.com/Manual/UnityIAPValidatingReceipts.html
            // or ask <can@nopact.com>.

            bool validPurchase = false;

            var validator = new CrossPlatformValidator( 
                GooglePlayTangle.Data(),
                AppleTangle.Data(), 
                Application.identifier );

#if UNITY_EDITOR

            if ( isDebugging ) Debug.Log( "[IAP] Receipt validation is skipped on editor." );
            validPurchase = true;            
#else
            try
            {              
                var result = validator.Validate( e.purchasedProduct.receipt );

                System.Text.StringBuilder resultStringBuilder = new System.Text.StringBuilder();
                resultStringBuilder.Append( "[IAP] Receipt is valid. Contents:" );
                foreach ( IPurchaseReceipt productReceipt in result )
                {
                    resultStringBuilder.AppendFormat( 
                        "\n id: {0} date: {1} transactionID:{2} ", 
                        productReceipt.productID, 
                        productReceipt.purchaseDate, 
                        productReceipt.transactionID 
                        );                    
                }

                if ( isDebugging ) Debug.Log( resultStringBuilder.ToString() );
                validPurchase = true;
            }
            catch ( IAPSecurityException )
            {
                Debug.LogWarning( "Invalid receipt, not unlocking content" );
                validPurchase = false;
            }            
#endif

            listener.OnPurchaseResult( validPurchase, productID, e.purchasedProduct.transactionID );
            return PurchaseProcessingResult.Complete;
        }

        public bool IsInitialized
        {
            get
            {
                return storeController != null && extensionProvider != null;
            }
        }
    }

}
#endif