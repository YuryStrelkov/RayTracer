using Raytracer.Textures;
using OpenTK;

namespace Raytracer.Model.SpecificModels
{
    public class Sphere : ARTModel
    {
        public float Radius { get; set; }

        public Vector4 Origin { get; set; }
        
        public override PixelColor IntersectionColor(ref Ray ray)
        {
            float tmax, tmin;

            if (ray.SphereIntersection(out tmin, out tmax, Radius, Origin.Xyz))
            {
                ray.Length = tmin;

                Vector3 n = ray.Origin + ray.Direction * tmin - Origin.Xyz;

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
            
        }
        public Sphere(float R, Vector3 orig):base()
        {
            Radius = R;
            Origin = new Vector4(orig.X, orig.Y, orig.Z, 1);
        }
    }
}
