using UnityEngine;

namespace nopact.Commons.Utility.Geometric
{
    public struct Vertex
    {
        public readonly Vector2 position;
        public readonly int index;

        public Vertex(Vector2 position, int index)
        {
            this.position = position;
            this.index = index;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Vertex)) 
                return false;
            return Equals((Vertex)obj);
        }

        public bool Equals(Vertex obj)
        {
            return obj.position.Equals(position) && obj.index == index;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (position.GetHashCode() * 397) ^ index;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", position, index);
        }
    }
}