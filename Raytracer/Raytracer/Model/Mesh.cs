using OpenTK;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Raytracer.Model.ObjMeshReader;

namespace Raytracer.Model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector4 Position;

        public Vector4 Normal;

        public Vector4 Tangent;

        public Vector2 UV;

        public Vertex(Vector4 _Position, Vector4 _Normal, Vector4 _Tangent, Vector2 _UV)
        {
            Position = _Position;
            Normal = _Normal;
            Tangent = _Tangent;
            UV = _UV;
        }

        public new int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Face
    {
        public  int P1 { get; private set; }

        public  int P2 { get; private set; }

        public  int P3 { get; private set; }

        public  int MatID { get; set; }

        public void InvertOrder()
        {
            int p = P1;

            P1 = P3;

            P3 = p;  
        }

        public Face(int p1, int p2, int p3)
       {
            P1 = p1;

            P2 = p2;

            P3 = p3;

            MatID = -1;
       }

        public Face(int p1, int p2, int p3,int mat)
        {
            P1 = p1;

            P2 = p2;

            P3 = p3;

            MatID = mat;
        }
    }

    public class Mesh
    {
        private object mutex;

        Dictionary<int,Vertex> Vertices;

        List<Face> Faces;

        private void LoadModel(string src)
        {
            RawMesh mesh = ReadObj(src);
            Console.WriteLine(mesh.ToString());
            Parallel.ForEach(mesh.Faces, (KVpair) => 
            {
                foreach (ObjFace face in mesh.Faces[KVpair.Key])
                {
                    Vertex v_0 = new Vertex(mesh.Positons[face.V1.positionID], mesh.Normals[face.V1.normalID], Vector4.Zero, mesh.Uvs[face.V1.UVID]);
                    Vertex v_1 = new Vertex(mesh.Positons[face.V2.positionID], mesh.Normals[face.V2.normalID], Vector4.Zero, mesh.Uvs[face.V2.UVID]);
                    Vertex v_2 = new Vertex(mesh.Positons[face.V3.positionID], mesh.Normals[face.V3.normalID], Vector4.Zero, mesh.Uvs[face.V3.UVID]);

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
                };
            }
            );
 
        }

        private void BuildModelTree()
        {

        }

        public void AppendFace(Vertex v1, Vertex v2, Vertex v3)
        {
            lock (mutex)
            {
                Faces.Add(new Face(AppendVertex(v1), AppendVertex(v2), AppendVertex(v3)));
            }
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
