namespace nopact.Commons.Utility.Geometric
{
    public  struct Triangle
    {
        public readonly Vertex p1;
        public readonly Vertex p2;
        public readonly Vertex p3;

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            this.p1 = a;
            this.p2 = b;
            this.p3 = c;
        }

        public bool ContainsPoint(Vertex point)
        {
			
            if (point.Equals(p1) || point.Equals(p2) || point.Equals( p3 ) )
            {

                return true;

            }
				

            bool oddNodes = false;

            if (checkPointToSegment(p3, p1, point ) )
            {

                oddNodes = !oddNodes;

            }
				
            if (checkPointToSegment(p1, p2, point ) )
            {

                oddNodes = !oddNodes;

            }
				
            if (checkPointToSegment(p2, p3, point ) )
            {

                oddNodes = !oddNodes;

            }				

            return oddNodes;

        }

        public static bool ContainsPoint(Vertex a, Vertex b, Vertex c, Vertex point)
        {
            return new Triangle(a, b, c).ContainsPoint(point);
        }

        static bool checkPointToSegment(Vertex sA, Vertex sB, Vertex point)
        {
            if ((sA.position.y < point.position.y && sB.position.y >= point.position.y) ||
                (sB.position.y < point.position.y && sA.position.y >= point.position.y))
            {
                float x = 
                    sA.position.x + 
                    (point.position.y - sA.position.y) / 
                    (sB.position.y - sA.position.y) * 
                    (sB.position.x - sA.position.x);
				
                if (x < point.position.x)
                    return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {

            if (obj.GetType() != typeof (Triangle))
            {

                return false;

            }
				
            return Equals((Triangle) obj);

        }

        public bool Equals(Triangle obj)
        {

            return obj.p1.Equals(p1) && obj.p2.Equals(p2) && obj.p3.Equals(p3);

        }

        public override int GetHashCode()
        {
            unchecked
            {

                int result = p1.GetHashCode();
                result = (result * 100) ^ p2.GetHashCode();
                result = (result * 100) ^ p3.GetHashCode();

                return result;

            }
        }
    }
}