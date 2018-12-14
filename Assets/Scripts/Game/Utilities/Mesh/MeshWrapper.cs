using System.Collections.Generic;
using UnityEngine;

namespace nopact.Game.Utilities.Mesh
{
    public class MeshWrapper
    {
        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Vector2> uv;
        private UnityEngine.Mesh mesh;
        
        public void Migrate()
        {
            mesh.GetVertices(vertices);
            mesh.GetTriangles(triangles,0);
            mesh.GetUVs(0, uv);
        }
        
        public void Apply( bool isCalculatingBounds = false)
        {
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles,0);
            mesh.SetUVs(0, uv);
            mesh.RecalculateNormals();
            if (isCalculatingBounds)
            {
                mesh.RecalculateBounds();
            }
        }
        
        public MeshWrapper(UnityEngine.Mesh mesh, uint capacity, uint triangleCapacity = 0)
        {
            this.mesh = mesh;
            vertices = new List<Vector3>( (int)capacity );
            triangleCapacity = triangleCapacity == 0 ? capacity * 3 : triangleCapacity;
            triangles = new List<int>((int)triangleCapacity);
            uv = new List<Vector2>((int) capacity);
        }
        
        public List<Vector3> Vertices => vertices;
        public List<int> Triangles => triangles;
        public List<Vector2> UV => uv;

        public void Clear()
        {
            if (mesh != null)
            {
                mesh.Clear();
                Migrate();
            }
        }
    }
}