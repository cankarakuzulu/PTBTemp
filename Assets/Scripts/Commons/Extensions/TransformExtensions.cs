using nopact.Commons.Utility.Math;
using UnityEngine;

namespace nopact.Commons.Extensions
{
    public static class TransformExtensions
    {
        public static void Oscillate(this Transform transform, Vector3 origin, Vector3 destination, float t)
        {
            //copy origin because it may be transform.position
            Vector3 start = new Vector3(origin.x, origin.y, origin.z);

            //normalize so that coordinates reach their apex at the same time
            Vector3 normalizedDiff = destination - start;
            normalizedDiff.Normalize();

            transform.position = new Vector3(
                MathUtility.PingPong(start.x, destination.x, t, normalizedDiff.x),
                MathUtility.PingPong(start.y, destination.y, t, normalizedDiff.y),
                MathUtility.PingPong(start.z, destination.z, t, normalizedDiff.z)
            );
        }
    }
}