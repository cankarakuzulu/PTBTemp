using nopact.Commons.Analytics;
using nopact.Commons.Input;
using nopact.Commons.Persistence;
using nopact.Commons.SceneDirection;
using nopact.Commons.SceneDirection.PlaySession;
using nopact.Commons.SceneDirection.Resources;
using UnityEngine;

namespace nopact.Game.SceneDirection
{
    public class DirectorBase : MonoBehaviour, IDirector
    {
        private ISceneLoader loader;
        private IResourceProvider resources;
        private IAnalyticsTracker analytics;
        
        public void Initialize(ISceneLoader loader, IResourceProvider resourceProvider, IAnalyticsTracker analyticsTracker)
        {
            this.loader = loader;
            resources = resourceProvider;
            analytics = analyticsTracker;
            WillInitialize();
            DirectionEvents.InitializeDirection( analyticsTracker, this );
        }

        public virtual void RegisterCameraPosition(Transform t, string id)
        {
            
        }

        public virtual void Kill()
        {
            
        }

        public virtual void OnControlUpdate<T>(T inputState) where T : InputStateBase
        {
            
        }

        public virtual void UiIngameStateUpdate(bool isSuccessful)
        {
            
        }

        public virtual void UiPauseCommand()
        {
            
        }

        public virtual void UiPlayCommand()
        {
            
        }

        public virtual void UiNextCommand()
        {
            
        }

        public virtual void UiRetryCommand()
        {
            
        }

        protected virtual void WillInitialize()
        {
            
        }
        protected  IAnalyticsTracker Analytics => analytics;
        protected IResourceProvider Resources => resources;
        protected ISceneLoader Loader => loader;
        public virtual PlaySessionBase PlaySession { get; private set; }
    }
}