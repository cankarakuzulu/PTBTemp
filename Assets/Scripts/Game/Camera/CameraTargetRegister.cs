using nopact.Commons.SceneDirection;
using nopact.Game.SceneDirection;
using UnityEngine;

namespace nopact.Game.Camera
{
    public class CameraTargetRegister :MonoBehaviour
    {        
        private void OnEnable()
        {
            DirectionEvents.OnInitializeDirection += OnInitializeDirection;    
        }

        private void OnDisable()
        {
            DirectionEvents.OnInitializeDirection -= OnInitializeDirection;
        }

        private void OnInitializeDirection( Commons.Analytics.IAnalyticsTracker analytics, IDirector director )
        {
            director.RegisterCameraPosition( transform, gameObject.name );
        }
    }
}
