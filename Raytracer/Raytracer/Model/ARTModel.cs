using OpenTK;
using System;
using System.Runtime.InteropServices;
using Raytracer.Textures;

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

        public override string ToString()
        {
            return "V : pos [" + Position.X.ToString() + " , " + Position.Y.ToString() + " , " + Position.Z.ToString() + "]\n" +
                   "V : nor [" + Normal.X.ToString() + " , " + Normal.Y.ToString() + " , " + Normal.Z.ToString() + "]\n" +
                   "V : tan [" + Tangent.X.ToString() + " , " + Tangent.Y.ToString() + " , " + Tangent.Z.ToString() + "]\n" +
                   "V :  uv [" + UV.X.ToString() + " , " + UV.Y.ToString() + "]\n";
        }

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
        public int P1 { get; private set; }

        public int P2 { get; private set; }

        public int P3 { get; private set; }

        public int MatID { get; set; }

        public override string ToString()
        {
            return "f " + P1.ToString() + " / " + P2.ToString() + " / " + P3.ToString() + "\n";
        }

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

        public Face(int p1, int p2, int p3, int mat)
        {
            P1 = p1;

            P2 = p2;

            P3 = p3;

            MatID = mat;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Ray
    {
        public Vector3 Origin;

        public Vector3 Direction;

        public float Length;

        public Ray Reflection(Vector3 normal)
        {
            return new Ray(Origin + Direction * Length, Direction - 2 * (Direction.X * normal.X +
                                                                          Direction.Y * normal.Y +
                                                                          Direction.Z * normal.Z) * normal);
        }

        public Ray Refraction(Vector3 normal, float RI1, float RI2)
        {
            float e_dot_n = Direction.X * normal.X + Direction.Y * normal.Y + Direction.Z * normal.Z;

            return new Ray(Origin + Direction * Length, Direction - (float)((Math.Sqrt(((RI2 * RI2 - RI1 * RI1) / e_dot_n / e_dot_n + 1)) - 1) * e_dot_n) * normal);
        }

        public void Refract(Vector3 normal, float RI1, float RI2)
        {
            float e_dot_n = Direction.X * normal.X + Direction.Y * normal.Y + Direction.Z * normal.Z;

            Origin = Origin + Direction * Length;

            Direction = Direction - (float)((Math.Sqrt(((RI2 * RI2 - RI1 * RI1) / e_dot_n / e_dot_n + 1)) - 1) * e_dot_n) * normal;

            Length = 0;
        }

        public void Reflect(Vector3 normal)
        {
            Origin = Origin + Direction * Length;
            Direction = Direction - 2 * (Direction.X * normal.X +
                                         Direction.Y * normal.Y +
                                         Direction.Z * normal.Z) * normal;
            Length = 0;
        }

        public bool TriangleIntersection(out Vector3 barycentric, Vector3 p0, Vector3 p1, Vector3 p2)
        {

            barycentric = new Vector3(0, 0, 0);

            float t = float.MaxValue;

            Vector3 E1 = p1 - p0;

            Vector3 E2 = p2 - p0;

            Vector3 pvec = Vector3.Cross(Origin, E2);

            float det = Vector3.Dot(pvec, E1);
            // if the determinant is negative the triangle is backfacing
            // if the determinant is close to 0, the ray misses the triangle
            // ray and triangle are parallel if det is close to 0
            if (Math.Abs(det) < 1e-8)
            {
                return false;
            }
            Vector3 tvec;
            if (det > 0)
            {
                tvec = Origin - p0;
            }
            else
            {
                tvec = p0 - Origin;
                det *= -1;
            }


            float u = Vector3.Dot(tvec, pvec);

            if (u < 0 || u > det)
            {
                return false;
            }

            Vector3 qvec = Vector3.Cross(tvec, E1);

            float v = Vector3.Dot(Direction, qvec);

            if (v < 0 || u + v > det)
            {
                return false;

            }

            t = Vector3.Dot(E2, qvec);

            t /= det;

            barycentric = new Vector3(u / det, v / det, t);

            return true;

        }

        public bool SphereIntersection(out float tMin, out float tMax, float R, Vector3 sphereOrigin)
        {
            Vector3 rdiff = Origin - sphereOrigin;

            float rdiffDotDir = rdiff.X * Direction.X + rdiff.Y * Direction.Y + rdiff.Z * Direction.Z;

            float D = rdiffDotDir * rdiffDotDir - (rdiff.X * rdiff.X + rdiff.Y * rdiff.Y + rdiff.Z * rdiff.Z) + R * R;

            if (D < 0)
            {
                tMin = 0;
                tMax = 0;
                return false;
            }

            float x1 = rdiffDotDir + (float)Math.Sqrt(D);

            float x2 = rdiffDotDir + (float)Math.Sqrt(D);

            tMin = Math.Min(x1, x2);

            tMax = Math.Max(x1, x2);

            return true;

        }

        public bool BoxIntersection(out float tmin, out float tmax, Vector3 minBound, Vector3 maxBound)
        {
            float lo = (minBound.X - Origin.X) / Direction.X;

            float hi = (maxBound.X - Origin.X) / Direction.X;

            tmin = Math.Min(lo, hi);

            tmax = Math.Max(lo, hi);

            float lo1 = (minBound.Y - Origin.Y) / Direction.Y;

            float hi1 = (maxBound.Y - Origin.Y) / Direction.Y;

            tmin = Math.Max(tmin, Math.Min(lo1, hi1));

            tmax = Math.Min(tmax, Math.Max(lo1, hi1));

            float lo2 = (minBound.Z - Origin.Z) / Direction.Z;

            float hi2 = (maxBound.Z - Origin.Z) / Direction.Z;

            tmin = Math.Max(tmin, Math.Min(lo2, hi2));

            tmax = Math.Min(tmax, Math.Max(lo2, hi2));

            return (tmin <= tmax) && (tmax > 0.0f);
        }

        public bool SurfaceIntersection(out float t, float shiftAlongNormal, Vector3 surfNormal)
        {
            t = (shiftAlongNormal + (surfNormal.X * Origin.X 
                                   + surfNormal.Y * Origin.Y
                                   + surfNormal.Z * Origin.Z)) 
                / (surfNormal.X * Direction.X 
                 + surfNormal.Y * Direction.Y
                 + surfNormal.Z * Direction.Z);

            return true;
        }

        public Ray(Vector3 origin)
        {
            Origin = origin;
            Direction = Vector3.UnitZ;
            Length = 0;
        }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
            Length = 0;
        }
    }
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform
    {
        private Matrix4 translation;

        private Matrix4 rotation;

        private Matrix4 scale;

        private Matrix4 transform;

        private Vector3 rotatinV;//костыль

        public Matrix4 TranslationM { get { return translation; } }

        public Matrix4 RotationM { get { return rotation; } }

        public Matrix4 ScalingM { get { return scale; } }

        public Matrix4 TransformM { get { return transform; } }

        public void Scaling(float x, float y, float z)
        {
            scale.M11 = x;
            scale.M22 = y;
            scale.M33 = z;

            Update();
        }

        public void Translation(float x, float y, float z)
        {
            scale.M14 += x;
            scale.M24 += y;
            scale.M34 += z;

            Update();
        }

        public void Rotation(float x, float y, float z)
        {
            rotatinV.X += x;

            rotatinV.Y += y;

            rotatinV.Z += z;

            rotation = Matrix4.CreateRotationX(rotatinV.X) * Matrix4.CreateRotationX(rotatinV.Y) * Matrix4.CreateRotationX(rotatinV.Z);

            Update();
        }

        public Vector3 GetOrigin()
        {
            return new Vector3(translation.M14, translation.M24, translation.M34);
        }

        public Vector3 GetScaling()
        {
            return new Vector3(scale.M11, scale.M22, scale.M33);
        }

        public Vector3 GetRotation()
        {
            return rotatinV;
        }

        private void Update()
        {
            transform = translation * rotation * scale;
        }

        public Transform(float x,float y, float z)
        {
            translation = new Matrix4(1, 0, 0, x,
                                      0, 1, 0, y,
                                      0, 0, 1, z,
                                      0, 0, 0, 1);

            scale     = new Matrix4(1, 0, 0, 0,
                                    0, 1, 0, 0,
                                    0, 0, 1, 0,
                                    0, 0, 0, 1);

            rotation  = new Matrix4(1, 0, 0, 0,
                                    0, 1, 0, 0,
                                    0, 0, 1, 0,
                                    0, 0, 0, 1);

            transform = new Matrix4(1, 0, 0, x,
                                    0, 1, 0, y,
                                    0, 0, 1, z,
                                    0, 0, 0, 1);

            rotatinV = new Vector3();
        }

        public Transform(Vector3 origin)
        {
            translation = new Matrix4(1, 0, 0, origin.X,
                                      0, 1, 0, origin.Y,
                                      0, 0, 1, origin.Z,
                                      0, 0, 0, 1);

            scale = new Matrix4(1, 0, 0, 0,
                                0, 1, 0, 0,
                                0, 0, 1, 0,
                                0, 0, 0, 1);

            rotation = new Matrix4(1, 0, 0, 0,
                                   0, 1, 0, 0,
                                   0, 0, 1, 0,
                                   0, 0, 0, 1);
            transform = new Matrix4(1, 0, 0, origin.X,
                                    0, 1, 0, origin.Y,
                                    0, 0, 1, origin.Z,
                                    0, 0, 0, 1);
            rotatinV = new Vector3();
        }
    }

    [Serializable]
    public class Material
    {
        private Texture Diffuse;

        private Texture Normals;

        private Texture Specular;

        private Texture Transparent;

        public PixelColor DiffusePixColor(float px, float py)
        {
            return Diffuse.ColorAt(px, py);
        }

        public PixelColor NormalsPixColor(float px, float py)
        {
            return Normals.ColorAt(px, py);
        }

        public PixelColor SpecularPixColor(float px, float py)
        {
            return Specular.ColorAt(px, py);
        }

        public PixelColor TransparentPixColor(float px, float py)
        {
            return Transparent.ColorAt(px, py);
        }

        public void LoadDiffuse(string src)
        {
            Diffuse.LoadTexture(src);
        }

        public void LoadNormals(string src)
        {
            Normals.LoadTexture(src);
        }

        public void LoadSpecular(string src)
        {
            Specular.LoadTexture(src);
        }

        public void LoadTransparent(string src)
        {
            Transparent.LoadTexture(src);
        }

        public Material()
        {
            Diffuse     = new Texture(0, 0, 0);
            Normals     = new Texture(0, 0, 0);
            Specular    = new Texture(0, 0, 0);
            Transparent = new Texture(0, 0, 0);
        }

        public Material(float r, float g, float b)
      {
            Diffuse     = new Texture(r, g, b);
            Normals     = new Texture(0, 0, 0);
            Specular    = new Texture(0, 0, 0);
            Transparent = new Texture(0, 0, 0);
        }

        public Material(string srcDiff,string srcNorm, string srcSpec, string srcTrans)
        {
            Diffuse = new Texture(0, 0, 0);

            Normals = new Texture(0, 0, 0);

            Specular = new Texture(0, 0, 0);

            Transparent = new Texture(0, 0, 0);

            LoadDiffuse(srcDiff);

            LoadNormals(srcNorm);

            LoadSpecular(srcSpec);

            LoadTransparent(srcTrans);
        }
    }

    public abstract class ARTModel
    {
        Transform t;

        protected object SyncObj;

        public int MaterialID { get; set; }

        public Transform GetTransform()
        {
            return t;
        }

        public abstract void LoadModel(string src);

        public abstract PixelColor IntersectionColor(ref Ray ray);

        public ARTModel()
        {
            t = new Transform(0,0,0);

            SyncObj = new object();

            MaterialID = -1;
        }
    }
}
