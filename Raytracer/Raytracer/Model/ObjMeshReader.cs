using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;

namespace Raytracer.Model
{
    public static class ObjMeshReader
    {
        public struct VertexComponentsIndeses
        {
            public int positionID;

            public int normalID;

            public int UVID;

            public VertexComponentsIndeses(int p, int n, int uv)
            {
                positionID = p;
                normalID = n;
                UVID = uv;
            }
        }

        public struct ObjFace
        {
            public VertexComponentsIndeses V1;

            public VertexComponentsIndeses V2;

            public VertexComponentsIndeses V3;

            public int MatID;

            public ObjFace(VertexComponentsIndeses v1, VertexComponentsIndeses v2, VertexComponentsIndeses v3,int _MatID)
            {
                V1 = v1;
                V2 = v2;
                V3 = v3;
                MatID = _MatID;
            }
        }

        public class RawMesh
        {
            public List<Vector4> Positons;/// = new List<Vector4>();
            public List<Vector4> Normals;/// = new List<Vector4>();
            public List<Vector4> Tangents;// = new List<Vector4>();
            public List<Vector2> Uvs;/// = new List<Vector2>();
            public Dictionary<int, List<ObjFace>> Faces { get; private set; }/// = new List<ObjFace>();
            public Dictionary<int, MeshMaterial> Materials { get; private set; }/// = new List<ObjFace>();

            public void AddMaterial(int matID,MeshMaterial mat)
            {
                if (!Materials.ContainsKey(matID))
                {
                    Materials.Add(matID, mat);
                }
            }

            public MeshMaterial GetMaterial(int matID)
            {
                return Materials[matID];
            }

            public void AddFace(int geomety, ObjFace face)
            {
                face.V1.positionID += Positons.Count;
                face.V1.normalID += Normals.Count;
                face.V1.UVID += Uvs.Count;

                face.V2.positionID += Positons.Count;
                face.V2.normalID += Normals.Count;
                face.V2.UVID += Uvs.Count;

                face.V3.positionID += Positons.Count;
                face.V3.normalID += Normals.Count;
                face.V3.UVID += Uvs.Count;

                if (Faces.ContainsKey(geomety))
                {
                    Faces[geomety].Add(face);
                    return;
                }

                Faces.Add(geomety, new List<ObjFace>());

                Faces[geomety].Add(face);
            }

            public RawMesh()
            {
                Positons = new List<Vector4>();
                Normals = new List<Vector4>();
                Tangents = new List<Vector4>();
                Uvs = new List<Vector2>();
                Faces = new Dictionary<int, List<ObjFace>>();
                Materials = new Dictionary<int, MeshMaterial>();
            }
        }

        public class MeshMaterial
        {
            public string MatName;
            public float Ns;
            public float D;
            public float Tr;
            public float Illum;
            public Vector3 Tf;
            public Vector3 Ka;
            public Vector3 Kd;
            public Vector3 Ks;
       }

        public static RawMesh ReadObj(string path)
        {
            RawMesh meshes = new RawMesh();

            int matID = -1;

            string[] Lines;

            try
            {
                Lines = File.ReadAllLines(path);

                string geometryName = "";

                for (int i = 0; i < Lines.Length; i++)
                {
      

                    if (Lines[i].StartsWith("#"))
                    {
                        continue;
                    }

                    if (Lines[i].StartsWith("mtllib"))
                    {
                        ReadMaterials(Lines[i].Split(' ')[1], ref meshes);
                        continue;
                    }

                    if (Lines[i].StartsWith("g"))
                    {
                        geometryName = Lines[i].Split(' ')[1];
                        continue;
                    }

                    if (Lines[i].StartsWith("usemtl"))
                    {
                        matID = ReadObjMatID(ref Lines[i]);
                        continue;
                    }
                    if (Lines[i].StartsWith("vn"))
                    {
                        meshes.Normals.Add(ReadObjVector4(ref Lines[i]));
                        continue;
                    }

                    if (Lines[i].StartsWith("vt"))
                    {
                        meshes.Uvs.Add(ReadObjVector2(ref Lines[i]));
                        continue;
                    }

                    if (Lines[i].StartsWith("v"))
                    {
                        meshes.Positons.Add(ReadObjVector4(ref Lines[i]));
                        continue;
                    }

                    if (Lines[i].StartsWith("f"))
                    {
                        meshes.AddFace(geometryName.GetHashCode(), ReadObjFace(ref Lines[i], matID));
                        continue;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("File not found...");
                Console.WriteLine(e.StackTrace);
            }
            return meshes;
        }

        private static void ReadMaterials(string path, ref RawMesh mesh)
        {
            try
            {
                string[] Lines = File.ReadAllLines(path);

                int matName = -1;

                MeshMaterial mat = new MeshMaterial();

                string line = "";

                for (int i = 0; i < Lines.Length; i++)
                {
                    line = Lines[i].Trim();

                    if (line.StartsWith("newmtl"))
                    {
                        matName = line.Split(' ')[1].GetHashCode();
                        mesh.AddMaterial(matName, new MeshMaterial());
                        continue;
                    }

                    if (line.StartsWith("Ns"))
                    {
                        mesh.GetMaterial(matName).Ns = ReadObjFloat(ref line);
                        continue;
                    }

                    if (line.StartsWith("d"))
                    {
                        mesh.GetMaterial(matName).D = ReadObjFloat(ref line);
                        continue;
                    }

                    if (line.StartsWith("Tr"))
                    {
                        mesh.GetMaterial(matName).Tr = ReadObjFloat(ref line);
                        continue;
                    }

                    if (line.StartsWith("Tf"))
                    {
                        mesh.GetMaterial(matName).Tf = ReadObjVector3(ref line);
                        continue;
                    }

                    if (line.StartsWith("illum"))
                    {
                        mesh.GetMaterial(matName).Illum = ReadObjFloat(ref line);
                        continue;
                    }

                    if (line.StartsWith("Ka"))
                    {
                        mesh.GetMaterial(matName).Ka = ReadObjVector3(ref line);
                        continue;
                    }

                    if (line.StartsWith("Kd"))
                    {
                        mesh.GetMaterial(matName).Kd = ReadObjVector3(ref line);
                        continue;
                    }

                    if (line.StartsWith("Ks"))
                    {
                        mesh.GetMaterial(matName).Ks = ReadObjVector3(ref line);
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("no material found for " + path.Split('.')[0]);
            }
        }

        private static Vector4 ReadObjVector4(ref string line)
        {
            string[] lineSplit = line.Split(' ');
            return new Vector4(float.Parse(lineSplit[lineSplit.Length - 3].Replace('.', ',')),
                               float.Parse(lineSplit[lineSplit.Length - 2].Replace('.', ',')), 
                               float.Parse(lineSplit[lineSplit.Length - 1].Replace('.', ',')),1);
        }
        
        private static Vector3 ReadObjVector3(ref string line)
        {
            string[] lineSplit = line.Split(' ');
            return new Vector3(float.Parse(lineSplit[lineSplit.Length - 3].Replace('.', ',')),
                               float.Parse(lineSplit[lineSplit.Length - 2].Replace('.', ',')), 
                               float.Parse(lineSplit[lineSplit.Length - 1].Replace('.', ',')));
        }

        private static Vector2 ReadObjVector2(ref string line)
        {
            string[] lineSplit = line.Split(' ');
            return new Vector2(float.Parse(lineSplit[lineSplit.Length - 2].Replace('.', ',')),
                               float.Parse(lineSplit[lineSplit.Length - 1].Replace('.', ',')));
        }

        private static float ReadObjFloat(ref string line)
        {
            string[] lineSplit = line.Split(' ');
            return float.Parse(lineSplit[lineSplit.Length - 1].Replace('.', ','));
        }

        private static int ReadObjMatID(ref string line)
        {
            string[] lineSplit = line.Split(' ');
            return lineSplit[1].GetHashCode();
        }

        private static ObjFace ReadObjFace(ref string line, int matID)
        {
            string[] lineSplit = line.Split(' ');

            string[] tmp = lineSplit[1].Split('/');

            VertexComponentsIndeses V1 = new VertexComponentsIndeses(Math.Abs(int.Parse(tmp[0])),
                                                                     Math.Abs(int.Parse(tmp[1])),
                                                                     Math.Abs(int.Parse(tmp[2])));

            tmp = lineSplit[2].Split('/');

            VertexComponentsIndeses V2 = new VertexComponentsIndeses(Math.Abs(int.Parse(tmp[0])),
                                                                     Math.Abs(int.Parse(tmp[1])),
                                                                     Math.Abs(int.Parse(tmp[2])));

            tmp = lineSplit[3].Split('/');

            VertexComponentsIndeses V3 = new VertexComponentsIndeses(Math.Abs(int.Parse(tmp[0])),
                                                                     Math.Abs(int.Parse(tmp[1])),
                                                                     Math.Abs(int.Parse(tmp[2])));

            return new ObjFace(V1,V2,V3,matID);
        }

    }
}
