using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Utility.Math
{
    public static class Dampening 
    {
        public static float Inertial ( float current, float target, float smoothTime, float deltaTime = 0.1f )
        {
            if ( smoothTime < 0.005f )
            {
                current = target;
                return target;
            }
            float delta = current - target;
            float newDelta = delta + deltaTime * ( -1f / smoothTime * delta );
            return target + newDelta;
        }

        public static float Smooth ( float current, float target, ref float speed, float smoothTime, float deltaTime = 0.1f )
        {
            if ( smoothTime < 0.005f )
            {
                current = target;
                return target;
            }
            // time constrains modified in approximation to reflect unity smoothdamp.smoothTime behaviour.
            float t1 = 0.36f * smoothTime;
            float t2 = 0.64f * smoothTime;

            float delta = current - target;
            float newSpeed = speed + deltaTime * ( -1f / ( t1 * t2 ) * delta - ( t1 + t2 ) / ( t1 * t2 ) * speed );
            float newDelta = delta + deltaTime * speed;
            speed = newSpeed;

            return target + newDelta;
        }

        public static Vector2 Inertial( Vector2 current, Vector2 target, float smoothTime, float deltaTime = 0.1f )
        {
            return new Vector2( Inertial( current.x, target.x, smoothTime, deltaTime ),
                Inertial( current.y, target.y, smoothTime, deltaTime )                
                );
        }

        public static Vector3 Inertial ( Vector3 current, Vector3 target, float smoothTime, float deltaTime = 0.1f )
        {
            return new Vector3( Inertial( current.x, target.x, smoothTime, deltaTime ),
                Inertial( current.y, target.y, smoothTime, deltaTime ),
                Inertial( current.z, target.z, smoothTime, deltaTime )
                );
        }

        public static Vector3 Smooth( Vector3 current, Vector3 target, ref Vector3 speed, float smoothTime, float deltaTime = 0.1f )
        {
            return new Vector3(
                Smooth( current.x, target.x, ref speed.x, smoothTime, deltaTime ),
                Smooth( current.y, target.y, ref speed.y, smoothTime, deltaTime ),
                Smooth( current.z, target.z, ref speed.z, smoothTime, deltaTime )
                );
        }
    }
}
