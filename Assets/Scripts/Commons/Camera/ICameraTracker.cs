using UnityEngine;
using System.Collections;

namespace nopact.Commons.Camera
{
    public interface ICameraTracker
    {
        void SetOffset( Vector3 offset );
        Vector3 StartTracking( Transform target );
        void StopTracking();
        UnityEngine.Camera UCamera{ get; }
    }
}
