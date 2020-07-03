using OpenTK;
using Raytracer.Textures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Raytracer.Model.ObjMeshReader;
using Raytracer.Scene;

namespace Raytracer.Model.SpecificModels
{
     public class Mesh : ARTModel
    {
        private Dictionary<int,Vertex> Vertices;

        private List<Face> Faces;
 
        public void AppendFace(Vertex v1, Vertex v2, Vertex v3)
        {
            lock (SyncObj)
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

        public override void LoadModel(string src)
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
          ///  Console.WriteLine(ToString());
        }

        private void BuildModelTree()
        {

        }
        
        private void AppendFace(Vertex v1, Vertex v2, Vertex v3, int matID)
        {
            lock (SyncObj)
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
        
        public bool Intersection(out Face face, out Vector3 barycentric, Ray ray)
        {
            face = new Face(-1,-1,-1);

            barycentric = Vector3.Zero;

            float distance = float.MaxValue;

            for (int i = 0 ; i < Faces.Count ; i++)
            {
                if (ray.TriangleIntersection(out barycentric,Vertices[Faces[i].P1].Position.Xyz,
                                                             Vertices[Faces[i].P2].Position.Xyz,
                                                             Vertices[Faces[i].P3].Position.Xyz))
                {
                    if (distance > barycentric.Z)
                    {
                        face = Faces[i];

                        distance = barycentric.Z;
                    }
                }
            }

            return face.P1 != -1;
        }

        public override PixelColor IntersectionColor(ref Ray ray)
        {
            Vector3 barycentric;

            Face face;

            if (Intersection(out face, out barycentric, ray))
            {
                Vector3 n = Vector3.BaryCentric(Vertices[face.P1].Normal.Xyz, 
                                                Vertices[face.P2].Normal.Xyz,
                                                Vertices[face.P3].Normal.Xyz,
                                                barycentric.X, barycentric.Y);
                ray.Length = barycentric.Z;
          
                byte color = (byte)(255*(ray.Direction.X * n.X +
                                         ray.Direction.Y * n.Y +
                                         ray.Direction.Z * n.Z));

                return new PixelColor(color, color, color);
            }

            return new PixelColor(0,0,0);
        }

        public override void OnCamSpace(Camera cam, out Vector2 LU, out Vector2 RD)
        {
            throw new NotImplementedException();
        }

        public Mesh() : base()
        {
            Vertices = new Dictionary<int, Vertex>();

            Faces = new List<Face>();
         }

        public Mesh(string src) : base()
        {
            Vertices = new Dictionary<int, Vertex>();

            Faces= new List<Face>();
            
           LoadModel(src);

           BuildModelTree();
        }
    }
}
