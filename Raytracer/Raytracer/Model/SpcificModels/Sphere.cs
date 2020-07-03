using Raytracer.Textures;
using OpenTK;
using Raytracer.Scene;
using System;

namespace Raytracer.Model.SpecificModels
{
    public class Sphere : ARTModel
    {
        public float Radius { get; set; }

        public override PixelColor IntersectionColor(ref Ray ray)
        {
            float tmax, tmin;

            if (ray.SphereIntersection(out tmin, out tmax, Radius, GetTransform().GetOrigin()))
            {
                ray.Length = tmin;

                Vector3 n = ray.Origin + ray.Direction * tmin - GetTransform().GetOrigin();

                n.Normalize();

                byte color = (byte)(255 * (ray.Direction.X * n.X +
                                           ray.Direction.Y * n.Y +
                                           ray.Direction.Z * n.Z));

                return new PixelColor(color, color, color);
            }
            return new PixelColor(0, 0, 0);
        }
        
        public override void LoadModel(string src)
        {
            if (src.StartsWith("sphere"))
            {
                string [] srcsplit = src.Split(' ');
            }
        }

        public override void OnCamSpace(Camera cam, out Vector2 LU, out Vector2 RD)
        {
            Matrix4 tmp = cam.Projection * cam.View;

            Vector4 maxB = tmp * new Vector4(GetTransform().GetOrigin().X + Radius, GetTransform().GetOrigin().Y + Radius, GetTransform().GetOrigin().Z + Radius, 1);

            Vector4 minB = tmp * new Vector4(GetTransform().GetOrigin().X - Radius, GetTransform().GetOrigin().Y - Radius, GetTransform().GetOrigin().Z - Radius, 1);

            LU = new Vector2(Math.Min(maxB.X, minB.X), Math.Min(maxB.Y, minB.Y));

            RD = new Vector2(Math.Max(maxB.X, minB.X), Math.Max(maxB.Y, minB.Y));
        }

        public Sphere(float R, Vector3 orig):base()
        {
            Radius = R;

            GetTransform().Translation(orig.X, orig.Y, orig.Z);
        }
    }
}
