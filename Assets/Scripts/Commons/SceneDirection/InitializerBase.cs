using UnityEngine;
using System;
using System.Collections;
using nopact.Commons.SceneDirection.Resources;
using nopact.Commons.Analytics;
using nopact.Commons.Persistence;

namespace nopact.Commons.SceneDirection
{
    public abstract class InitializerBase : MonoBehaviour
    {
        [SerializeField] protected GameObject analytics;
        [SerializeField] protected Scenes scenes;
        [SerializeField] protected GameManagement management;
        [SerializeField] protected GameObject resources;

        protected bool isInitialized;
        protected IResourceProvider resourceProvider;
        protected IAnalyticsTracker analyticsTracker;
        
        protected virtual void OnEnable()
        {
            SceneEvents.OnSceneLoaded += SceneEvents_OnSceneLoaded;
        }
                
        protected virtual void OnDisable()
        {
            SceneEvents.OnSceneLoaded -= SceneEvents_OnSceneLoaded;
        }

        private void SceneEvents_OnSceneLoaded( string sceneName )
        {
            if( sceneName == scenes.MainSceneName && !isInitialized )
            {
                StartCoroutine( StartDirection() );
            }
        }

        private IEnumerator StartDirection()
        {
            yield return new WaitForEndOfFrame();
            management.Initialize( scenes, resourceProvider, AnalyticsTracker  );
            isInitialized = true;
        }

        protected virtual void Awake()
        {
            Application.targetFrameRate = 60;
            if ( resourceProvider == null )
            {
                resourceProvider = ( IResourceProvider ) resources.GetComponent( typeof( IResourceProvider ) );
            }
            
            if ( analyticsTracker == null )
            {
                analyticsTracker = ( IAnalyticsTracker ) analytics.GetComponent( typeof( IAnalyticsTracker ) );
            }            
        }

        private void Start()
        {
            if ( resourceProvider != null )
            {
                resourceProvider.OnInitializationComplete += ResourceProvider_OnInitializationComplete;
                resourceProvider.Initialize();
            }
            else
            {
                scenes.LoadUnpersistentScenes();
            }
        }

        private void ResourceProvider_OnInitializationComplete()
        {
            scenes.LoadUnpersistentScenes();
        }

        public ISceneLoader SceneLoader {
            get {
                return scenes;
            } }    

        public IAnalyticsTracker AnalyticsTracker
        {
            get
            {
                return analyticsTracker;
            }
        }
    }
}
