using UnityEngine;
using System.Collections;
using System;
using nopact.Commons.Utility.Debug;

namespace nopact.Commons.Utility.Math
{
    public static class Lerp 
    {     
        public static float Classic ( float start, float end, float percent )
        {
            return ( start + percent * ( end - start ) );
        }

        public static float Extended ( float start, float end, float percent, Func<float, float> easing )
        {
            if ( easing == null )
            {
                DebugWrapper.Log( "easing function cannot ve null" );
                return start;
            }
            return Classic( start, end, easing( percent ) );
        }
     
        public static float SinOut ( float percent )
        {
            return Mathf.Sin( percent * Mathf.PI * 0.5f );
        }

        public static float SinIn( float percent )
        {
            return 1f - Mathf.Cos( percent * Mathf.PI * 0.5f );
        }

        public static float QuadraticIn ( float percent )
        {
            return percent * percent;
        }

        public static float SmoothStep (  float percent )
        {
            return percent * percent * ( 3f - 2f * percent );
        }

        public static float SmootherStep ( float percent )
        {
            return percent * percent * percent * ( percent * ( 6f * percent - 15f ) + 10f );
        }
    }
}
