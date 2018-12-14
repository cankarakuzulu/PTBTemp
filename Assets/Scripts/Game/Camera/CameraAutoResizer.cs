using UnityEngine;
using System.Collections;
using nopact.Commons.Utility;
using System;
using nopact.Commons.Utility.Singleton;
using nopact.Commons.Camera;

namespace nopact.Game.Camera
{
    public class CameraAutoResizer : MonoBehaviour
    {
        [SerializeField] protected UnityEngine.Camera targetCamera;
        [Header("Camera auto resizing options")]
        [SerializeField] protected bool isInterpolatingFirstZone = false;
        [SerializeField] protected bool isInterpolatingSecondZone = true;
        [Header( "HDlike Screens:" )]
        [SerializeField] protected float hdCameraSize = 2.9f;
        [SerializeField] protected float hdAspectRatio = 1.77f;
        [SerializeField] protected Vector3 hdCameraOffset;
        [Header( "iPhoneX:" )]
        [SerializeField] protected float extremeRatioCameraSize = 3.5f;
        [SerializeField] protected float extremeRatio = 2.16f;
        [SerializeField] protected Vector3 extremeCameraOffset;
        [Header( "iPadish:" )]
        [SerializeField] protected float lowRatioCameraSize = 2.2f;
        [SerializeField] protected float lowRatio = 1f;
        [SerializeField] protected Vector3 lowCameraOffset;

        private ICameraTracker cameraTracker;
        private UnityEngine.Camera uCamera;
        private float size = 0;
        private Vector3 cameraOffset;

        protected  void Start()
        {   
            cameraTracker = (ICameraTracker) GetComponent( typeof( ICameraTracker ) );
            uCamera = targetCamera;
            AutoResize();
        }

      
        private void AutoResize ()
        {
            float aspectRatio = AspectRatio;
            if ( aspectRatio < 1 )
            {
                hdAspectRatio = 1 / hdAspectRatio;
                lowRatio = 1 / lowRatio;
                extremeRatio = 1 / extremeRatio;
            }
            size = hdCameraSize;
            AspectRatioClass = AspectClass.HD;

            if ( aspectRatio > hdAspectRatio && isInterpolatingFirstZone )
            {
                var diffRatio = ( aspectRatio - hdAspectRatio ) / ( lowRatio - hdAspectRatio );
                size = Mathf.Lerp( hdCameraSize, lowRatioCameraSize, diffRatio );
                AspectRatioClass = AspectClass.Low;
                cameraOffset = Vector3.Lerp( hdCameraOffset, lowCameraOffset, diffRatio );
            }

            if ( aspectRatio < hdAspectRatio && isInterpolatingSecondZone )
            {
                var diffRatio = ( hdAspectRatio - aspectRatio ) / ( hdAspectRatio - extremeRatio );
                size = Mathf.Lerp( hdCameraSize, extremeRatioCameraSize, diffRatio );
                AspectRatioClass = AspectClass.High;
                cameraOffset = Vector3.Lerp( hdCameraOffset, extremeCameraOffset, diffRatio );
            }

            if (Mathf.Abs(extremeRatioCameraSize - size) < 0.1f)
            {
                AspectRatioClass = AspectClass.Extreme;
            }
            
            uCamera.orthographicSize = size;
            if( cameraTracker != null )
            {
                cameraTracker.SetOffset( cameraOffset );
            }
        }

        public float Size
        {
            get
            {
                return size;
            }
        }

        public float Width
        {
            get
            {
                return size * 2 * AspectRatio;
            }
        }

        public float Height
        {
            get
            {
                return size * 2;                
            }

        }

        public float AspectRatio
        {
            get
            {
                return ( float ) Screen.width / Screen.height;
            }
        }

        public AspectClass AspectRatioClass { get; private set; }

        public enum AspectClass
        {
            Low,
            HD,
            High,
            Extreme
        }
    }
}
