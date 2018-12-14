using System.Collections;
using System.Collections.Generic;
using nopact.Commons.Camera;
using nopact.Commons.Utility.Math;
using UnityEngine;

namespace nopact.Game.Camera
{

    public class SimpleCameraTracker :MonoBehaviour, ICameraTracker
    {
        [SerializeField] protected Vector3 smoothing;
        [SerializeField] protected Vector3 extraOffset;
        [SerializeField] protected float rotationSmoothing;
        private Vector3 speed;
        private Transform target;
        private Transform uTransform;
        private Vector3 offset, cameraSizeOffset;
                
        public Vector3 StartTracking( Transform target )
        {
            offset = extraOffset;
            this.target = target;
            return cameraSizeOffset;
        }

        public void StopTracking()
        {
            target = null;
        }
                
        void Start()
        {
            UCamera = GetComponentInChildren<UnityEngine.Camera>();
            uTransform = GetComponent<Transform>();
        }
                
        void LateUpdate()
        {
            if ( target != null )
            {
                var normalizedPosition = uTransform.position - offset;
                uTransform.position = new Vector3()
                {
                    x = Dampening.Smooth( normalizedPosition.x, target.position.x, ref speed.x, smoothing.x, Time.deltaTime ),
                    y = Dampening.Smooth( normalizedPosition.y, target.position.y, ref speed.y, smoothing.y, Time.deltaTime ),
                    z = Dampening.Smooth( normalizedPosition.z, target.position.z, ref speed.z, smoothing.z, Time.deltaTime )
                } +
                offset;

                if ( Mathf.Abs(Quaternion.Dot(uTransform.rotation, target.rotation ))<= 1 + Mathf.Epsilon )
                {
                    uTransform.rotation = Quaternion.Slerp( uTransform.rotation, target.rotation, 1 - Mathf.Pow( rotationSmoothing, Time.deltaTime) );
                }                
                else
                {
                    uTransform.rotation = target.rotation;
                }
            }         
        }

        public void SetOffset( Vector3 offset )
        {
            cameraSizeOffset = offset;
        }

        public UnityEngine.Camera UCamera { get; private set; }
    }
}
