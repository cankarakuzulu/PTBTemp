using UnityEditor;
using UnityEngine;

namespace nopact.Commons.Utility.Geometric
{
	public struct LineSegment
	{
		public Vertex p1;
		public Vertex p2;

		public LineSegment(Vertex a, Vertex b)
		{
			this.p1 = a;
			this.p2 = b;
		}

		public bool RayIntersection( Ray ray, out float distance )
		{
            
			float greatestDist = Mathf.Max(p1.position.x - ray.origin.x, p2.position.x - ray.origin.x) * 2f;

			LineSegment raySegment = new LineSegment(new Vertex(ray.origin, 0), new Vertex(ray.origin + (ray.direction * greatestDist), 0));
            
            Vector2? intersection;
            PolyTools.FasterLineSegmentIntersectionWithIntersection ( this, raySegment, out intersection );

            distance = 0.0f;

			if (intersection != null )
            {

                distance = Vector2.Distance( ray.origin, intersection.Value );

                return true;
            
            }
			
			return false;

		}
	
	}

}
