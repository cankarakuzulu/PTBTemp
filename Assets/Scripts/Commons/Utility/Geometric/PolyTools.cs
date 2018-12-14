using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Utility.Geometric
{
  public static class PolyTools
    {
        
        public static bool CheckContainment( Vector3[] pointList, Vector2 pointToCheck )
        { 

            Vector2 aPointFarAway = new Vector2( 100000, pointToCheck.y);
            
            int intersectionCount = 0;            

            for ( int index = 0; index < pointList.Length; index++ )
            {
                
                int next = (index + 1) % pointList.Length;

                Vector2 sideP1 = pointList[ index ];
                Vector2 sideP2 = pointList[ next ];                

                if ( FasterLineSegmentIntersection( pointToCheck, aPointFarAway, sideP1, sideP2 ))
                {

                    intersectionCount++;

                } 

            }
            
            bool isInside = ( intersectionCount % 2 == 1 );
            
            return isInside ;
            
        }

        public static bool IsCollinear( Vector2 p1, Vector2 p2, Vector2 p3 )
        {

            return Mathf.Abs( ( p1.x * ( p2.y - p3.y ) + p2.x * ( p3.y - p1.y ) + p3.x * ( p1.y - p2.y ) ) ) < Mathf.Epsilon;

        }

        public static bool FasterLineSegmentIntersectionWithIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
            out Vector2? intersection)
        {
            float x1 = p1.x;
            float y1 = p1.y;
            float x2 = p2.x;
            float y2 = p2.y;
            float x3 = p3.x;
            float y3 = p3.y;
            float x4 = p4.x;
            float y4 = p4.y;
            
            float denom = ( y4 - y3 ) * ( x2 - x1 ) - ( x4 - x3 ) * ( y2 - y1 );

            float alphaNumerator = ( x4 - x3 ) * ( y1 - y3 ) - ( y4 - y3 ) * ( x1 - x3 );
            float betaNumerator = ( x2 - x1 ) * ( y1 - y3 ) - ( y2 - y1 ) * ( x1 - x3 );

            float ua = alphaNumerator / denom;
            float ub = betaNumerator / denom;

            if ( Mathf.Clamp( ua, 0f, 1f ) != ua || Mathf.Clamp( ub, 0f, 1f ) != ub )
            {

                intersection = null;
                return false;

            }

            intersection = p1 + ( p2 - p1 ) * ua;
            return true;
        }
        
        public static bool FasterLineSegmentIntersectionWithIntersection ( LineSegment s1, LineSegment s2, out Vector2? intersection  )
        {

            float x1 = s1.p1.position.x;
            float y1 = s1.p1.position.y;
            float x2 = s1.p1.position.x;
            float y2 = s1.p1.position.y;
            float x3 = s2.p1.position.x;
            float y3 = s2.p1.position.y;
            float x4 = s2.p2.position.x;
            float y4 = s2.p2.position.y;

            float denom = ( y4 - y3 ) * ( x2 - x1 ) - ( x4 - x3 ) * ( y2 - y1 );

            float alphaNumerator = ( x4 - x3 ) * ( y1 - y3 ) - ( y4 - y3 ) * ( x1 - x3 );
            float betaNumerator = ( x2 - x1 ) * ( y1 - y3 ) - ( y2 - y1 ) * ( x1 - x3 );

            float ua = alphaNumerator / denom;
            float ub = betaNumerator / denom;

            if ( Mathf.Clamp( ua, 0f, 1f ) != ua || Mathf.Clamp( ub, 0f, 1f ) != ub )
            {

                intersection = null;
                return false;

            }

            intersection = s1.p1.position + ( s1.p2.position - s1.p1.position ) * ua;
            return true;

        }

        public static Vector2 CalculateCenteroid ( Vector2[] polyPoints )
        {
            Vector2 sum = Vector2.zero;
            for ( int pointIndex = 0; pointIndex < polyPoints.Length; pointIndex ++ )
            {
                sum += polyPoints[ pointIndex ];
            }

            return sum / ( float ) polyPoints.Length;
        }

        public static void UniformScale ( Vector2[] polyPoints, float scaleFactor )
        {

            NonUniformScale( polyPoints, scaleFactor, scaleFactor );

        }

        public static void NonUniformScale ( Vector2[] polyPoints, float xScaleFactor, float yScaleFactor )
        {
            Vector2 centeroid = CalculateCenteroid( polyPoints );

            for ( int pointIndex = 0; pointIndex < polyPoints.Length; pointIndex++ )
            {
                var translatedPoint = ( polyPoints[ pointIndex ] - centeroid );
                polyPoints[ pointIndex ] = new Vector2( translatedPoint.x * xScaleFactor, translatedPoint.y * yScaleFactor ) + centeroid;
            }

        }

        public static bool FasterLineSegmentIntersection( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4 )
        {

            Vector2 a = p2 - p1;
            Vector2 b = p3 - p4;
            Vector2 c = p1 - p3;

            float alphaNumerator = b.y * c.x - b.x * c.y;
            float alphaDenominator = a.y * b.x - a.x * b.y;
            float betaNumerator = a.x * c.y - a.y * c.x;
            float betaDenominator = a.y * b.x - a.x * b.y;

            bool doIntersect = true;

            if ( alphaDenominator==0 || betaDenominator==0 )
            {

                doIntersect = false;                

            }
            else
            {

                if ( alphaDenominator > 0 )
                {
                    if ( alphaNumerator < 0 || alphaNumerator > alphaDenominator )
                    {
                        doIntersect = false;

                    }
                }
                else if ( alphaNumerator > 0 || alphaNumerator < alphaDenominator )
                {
                    doIntersect = false;
                }

                if ( doIntersect && betaDenominator > 0 )
                {
                    if ( betaNumerator < 0 || betaNumerator > betaDenominator )
                    {
                        doIntersect = false;
                    }
                }
                else if ( betaNumerator > 0 || betaNumerator < betaDenominator )
                {
                    doIntersect = false;
                }
            }

            return doIntersect;
        }

        public static Vector2[] ComputeConvexHullS( Vector3[ ] points, bool isOrthogonal )
        {
            return ComputeConvexHull( points, isOrthogonal ).ToArray();
        }


        public static List<Vector2> ComputeConvexHull( Vector3[ ] points, bool isOrthogonal )
        {
            var list = new List<Vector2>(  );
            for ( int pointIndex = 0; pointIndex < points.Length; pointIndex++ )
            {
                list.Add( points[ pointIndex ]);
            }
            return ComputeConvexHull( list, isOrthogonal );
        }

        public static List<Vector2> ComputeConvexHull( Vector2[ ] points, bool isOrthogonal )
        {
            var list = new List<Vector2>( points );
            return ComputeConvexHull( list, isOrthogonal);
        }

        public static float Cross ( Vector2 a, Vector2 b )
        {
            return ( a.x * b.y - a.y * b.x );
        }
        public static List<Vector2> ComputeConvexHull( List<Vector2> points, bool isOrthogonal)
        {
            
            points.Sort( ( a, b ) =>
            {
                return a.x == b.x ? a.y.CompareTo( b.y ) : ( a.x > b.x ? 1 : -1 );
            } );

            List<Vector2> hull = new List<Vector2>();
            int lower = 0, upper = 0; // size of lower and upper hulls

            for ( int i = points.Count - 1; i >= 0; i-- )
            {
                Vector2 p = points[ i ];

                while ( lower >= 2 && Cross (  ( hull[hull.Count-1]-hull[ hull.Count - 2 ] ), ( p - hull[ hull.Count - 1 ] ) ) >= 0 )
                {
                    hull.RemoveAt( hull.Count - 1 );
                    lower--;
                }

                hull.Insert( hull.Count, p );
                lower++;

                while ( upper >= 2 && Cross(( hull[0] - hull[ 1 ] ),( p - hull[0]) ) <= 0 )
                {
                    hull.RemoveAt( 0 );
                    upper--;
                }
                if ( upper != 0 )
                {
                    hull.Insert( 0, p );
                }
                    
                upper++;
            }

            hull.RemoveAt( hull.Count - 1 );
            if ( isOrthogonal )
            {
                MakeOrthogonal( hull );
            }
            return hull;

        }

        public static void MakeOrthogonal( List<Vector2> hull )
        {
            Vector2 centeroid = CalculateCenteroid( hull.ToArray() );

            for ( int i = hull.Count-2; i>= 0; i-- )
            {

                if ( hull[i].x != hull[i+1].x && hull[i].y != hull[i+1].y)
                {
                    var p1 = new Vector2( hull[ i ].x, hull[ i + 1 ].y );
                    var p2 = new Vector2( hull[ i + 1 ].x, hull[ i ].y );
                    var pointToAdd = ( ( p1 - centeroid ).sqrMagnitude <= ( p2 - centeroid ).sqrMagnitude ) ? p1 : p2;

                    hull.Insert( i + 1, pointToAdd );
                }
            }
        }
    }
}