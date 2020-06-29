using OpenTK;
using Raytracer.Textures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Raytracer.Model.ObjMeshReader;

namespace Raytracer.Model
{
     public class Mesh/// : IRTModel
    {
        private object mutex;

        Dictionary<int,Vertex> Vertices;

        List<Face> Faces;
        
        public void AppendFace(Vertex v1, Vertex v2, Vertex v3)
        {
            lock (mutex)
            {
                Faces.Add(new Face(AppendVertex(v1), AppendVertex(v2), AppendVertex(v3)));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Faces.Count; i++)
            {
                sb.Append(new string('_', 10) + "\n");
                sb.Append(Faces[i].ToString());
                sb.Append(new string('_', 10) + "\n");
                sb.Append(Vertices[Faces[i].P1].ToString());
                sb.Append(new string('_', 10) + "\n");
                sb.Append(Vertices[Faces[i].P2].ToString());
                sb.Append(new string('_', 10) + "\n");
                sb.Append(Vertices[Faces[i].P3].ToString());
            }
            return sb.ToString();
        }

        public void LoadModel(string src)
        {
            RawMesh mesh = ReadObj(src);

            Console.WriteLine(mesh.ToString());

            foreach (int geoID in mesh.Faces.Keys)
            {
               Parallel.ForEach(mesh.Faces[geoID], (face) => 
              {
                    Vertex v_0 = new Vertex(mesh.Positons[geoID][face.V1.PositionID], mesh.Normals[geoID][face.V1.NormalID], Vector4.Zero, mesh.Uvs[geoID][face.V1.UVID]);
                    Vertex v_1 = new Vertex(mesh.Positons[geoID][face.V2.PositionID], mesh.Normals[geoID][face.V2.NormalID], Vector4.Zero, mesh.Uvs[geoID][face.V2.UVID]);
                    Vertex v_2 = new Vertex(mesh.Positons[geoID][face.V3.PositionID], mesh.Normals[geoID][face.V3.NormalID], Vector4.Zero, mesh.Uvs[geoID][face.V3.UVID]);

                    Vector3 deltaPos1 = v_1.Position.Xyz - v_0.Position.Xyz;
                    Vector3 deltaPos2 = v_2.Position.Xyz - v_0.Position.Xyz;

                    Vector2 deltaUV1 = v_1.UV - v_0.UV;
                    Vector2 deltaUV2 = v_2.UV - v_0.UV;

                    float dividend = (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);

                    dividend = dividend == 0 ? 0.0f : 1.0f / dividend;

                    Vector3 tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * dividend;

                    v_0.Tangent = new Vector4(tangent.X, tangent.Y, tangent.Z, 0);
                    v_1.Tangent = v_0.Tangent;
                    v_2.Tangent = v_0.Tangent;

                    AppendFace(v_0, v_1, v_2, face.MatID);
              }
              );
            }
            Console.WriteLine(ToString());
        }

        private void BuildModelTree()
        {

        }
        
        private void AppendFace(Vertex v1, Vertex v2, Vertex v3, int matID)
        {
            lock (mutex)
            {
                Faces.Add(new Face(AppendVertex(v1), AppendVertex(v2), AppendVertex(v3), matID));
            }
        }

        private int AppendVertex(Vertex v)
        {
            int vertexHashCode = v.GetHashCode();

            if (!Vertices.ContainsKey(vertexHashCode))
            {
                Vertices.Add(vertexHashCode, v);
            }
            return vertexHashCode;
        }

        public Matrix4 Transform()
        {
            return Matrix4.Identity;
        }

        public bool Intersection(out Face face, out Vector3 barycentric, Ray ray)
        {
            face = new Face(-1,-1,-1);

            barycentric = Vector3.Zero;
            
            for (int i = 0 ; i < Faces.Count ; i++)
            {
                if (ray.TriangleIntersection(out barycentric,Vertices[Faces[i].P1].Position.Xyz,
                                                             Vertices[Faces[i].P2].Position.Xyz,
                                                             Vertices[Faces[i].P3].Position.Xyz))
                {
                    face = Faces[i];
                    return true;
                }
            }
            return false;
        }

        public PixelColor IntersectionColor(Ray ray)
        {
            throw new NotImplementedException();
        }

        public Mesh()
        {
            Vertices = new Dictionary<int, Vertex>();

            Faces = new List<Face>();

            mutex = new object();
        }

        public Mesh(string src)
        {
            Vertices = new Dictionary<int, Vertex>();

            Faces= new List<Face>();

            mutex = new object();

            LoadModel(src);

            BuildModelTree();
        }
    }
}
