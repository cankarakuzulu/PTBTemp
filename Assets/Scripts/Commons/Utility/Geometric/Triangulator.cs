using System.Collections.Generic;
using UnityEngine;

namespace nopact.Commons.Utility.Geometric
{
     public class Triangulator
    {

        private List<Vector2> points = new List<Vector2>();

        public Triangulator( Vector2[ ] points )
        {

            this.points = new List<Vector2>( points );

        }

        public int[ ] Process()
        {

            List<int> indices = new List<int>();
            int numberOfVertices = points.Count;

            if ( numberOfVertices < 3 )
            {
                return indices.ToArray();
            }               

            int[ ] indiceRecord = new int[ numberOfVertices ];

            if ( CalculateArea() > 0 )
            {
                for ( int vertexIndex = 0; vertexIndex < numberOfVertices; vertexIndex++ )
                {
                    indiceRecord[ vertexIndex ] = vertexIndex;
                }
            }
            else
            {
                for ( int vertexIndex = 0; vertexIndex < numberOfVertices; vertexIndex++ )
                {
                    indiceRecord[ vertexIndex ] = ( numberOfVertices - 1 ) - vertexIndex;
                }
            }

            int verticeCount = numberOfVertices;
            int count = 2 * verticeCount;

            for ( int iterationIndex = 0, vertexIndex = verticeCount - 1; verticeCount > 2; )
            {
                if ( ( count-- ) <= 0 )
                {
                    return indices.ToArray();
                }                    

                int p1Index = vertexIndex;
                if ( verticeCount <= p1Index )
                {
                    p1Index = 0;
                }
                    
                vertexIndex = p1Index + 1;
                if ( verticeCount <= vertexIndex )
                {
                    vertexIndex = 0;
                }
                    
                int p2Index = vertexIndex + 1;
                if ( verticeCount <= p2Index )
                {
                    p2Index = 0;
                }
                    

                if ( Snip( p1Index, vertexIndex, p2Index, verticeCount, indiceRecord ) )
                {
                    int x1, x2, x3, s, t;
                    x1 = indiceRecord[ p1Index ];
                    x2 = indiceRecord[ vertexIndex ];
                    x3 = indiceRecord[ p2Index ];

                    indices.Add( x1 );
                    indices.Add( x2 );
                    indices.Add( x3 );

                    iterationIndex++;

                    for ( s = vertexIndex, t = vertexIndex + 1; t < verticeCount; s++, t++ )
                    {
                        indiceRecord[ s ] = indiceRecord[ t ];
                    }
                    verticeCount--;
                    count = 2 * verticeCount;
                }
            }
            indices.Reverse();
            return indices.ToArray();
        }

        private bool Snip( int index1, int index2, int index3, int nIndices, int[ ] indiceRecord )
        {

            int p;

            Vector2 p1 = points[ indiceRecord[ index1 ] ];
            Vector2 p2 = points[ indiceRecord[ index2 ] ];
            Vector2 p3 = points[ indiceRecord[ index3 ] ];

            if ( Mathf.Epsilon > ( ( ( p2.x - p1.x ) * ( p3.y - p1.y ) ) - ( ( p2.y - p1.y ) * ( p3.x - p1.x ) ) ) )
            {

                return false;

            }
                
            for ( p = 0; p < nIndices; p++ )
            {
                if ( ( p == index1 ) || ( p == index2 ) || ( p == index3 ) )
                {

                    continue;

                }
                    
                Vector2 point = points[ indiceRecord[ p ] ];
                Triangle t = new Triangle( new Vertex (p1,0), new Vertex(p2,1), new Vertex(p3,2) );

                if ( t.ContainsPoint( new Vertex(point,0) ))                    
                {

                    return false;

                }

            }

            return true;
        }

        private float CalculateArea()
        {

            int nPoints = points.Count;
            float totalArea = 0.0f;

            for ( int pointIndex = nPoints - 1, lowerIndex = 0; lowerIndex < nPoints; pointIndex = lowerIndex++ )
            {

                Vector2 upperVal = points[ pointIndex ];
                Vector2 lowerVal = points[ lowerIndex ];

                totalArea += upperVal.x * lowerVal.y - lowerVal.x * upperVal.y;

            }

            return ( totalArea * 0.5f );

        }

    }
}