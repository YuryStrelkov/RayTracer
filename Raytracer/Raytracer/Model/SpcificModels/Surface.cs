using System;
using OpenTK;
using Raytracer.Scene;
using Raytracer.Textures;

namespace Raytracer.Model.SpcificModels
{
    public class Surface : ARTModel
    {
        public static float MaxClipDistance = 10000;

        public float D { get; set; }

        public Vector4 ABC { get; set; }

        public override PixelColor IntersectionColor(ref Ray ray)
        {
                float t;

                ray.SurfaceIntersection(out t,D,ABC.Xyz);

                ray.Length = t;

                byte color = (byte)(255 * (ray.Direction.X * ABC.X +
                                           ray.Direction.Y * ABC.Y +
                                           ray.Direction.Z * ABC.Z));

                return new PixelColor(color, color, color);
        }

        public override void LoadModel(string src)
        {

        }

        public override void OnCamSpace(Camera cam, out Vector2 LU, out Vector2 RD)
        {
        
        }

        public Surface(float shiftAlongNormal, Vector3 surfNormal):base()
        {
            D = shiftAlongNormal;
            surfNormal.Normalize();
            ABC = new Vector4(surfNormal.X, surfNormal.Y, surfNormal.Z, 0);
        }
    }
}
