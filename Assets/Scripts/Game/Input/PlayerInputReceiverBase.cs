using System.Collections;
using System.Collections.Generic;
using nopact.Commons.Analytics;
using nopact.Commons.SceneDirection;
using nopact.Game.SceneDirection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace nopact.Game.Input
{
    public class PlayerInputReceiverBase : MonoBehaviour
    {
        private IAnalyticsTracker tracker;
        private IDirector director;
        
        private void OnEnable()
        {
            DirectionEvents.OnInitializeDirection+= DirectionEventsOnOnInitializeDirection;
        }
        private void OnDisable()
        {
            DirectionEvents.OnInitializeDirection-= DirectionEventsOnOnInitializeDirection;
        }
        private void DirectionEventsOnOnInitializeDirection(IAnalyticsTracker tracker, IDirector director)
        {
            this.tracker = tracker;
            this.director = director;
            SetInputState();
        }
        
        protected  virtual void SetInputState(){}
        
        protected IAnalyticsTracker AnalyticsTracker
        {
            get { return tracker; }
        }

        protected IDirector Director
        {
            get { return director; }
        }
    }
}
