using System;
using OpenTK;
using Raytracer.Scene;
using Raytracer.Textures;
using Raytracer.Cameras;

namespace Raytracer.Model.SpcificModels
{
    public class Surface : ARTModel
    {
        public static float MaxClipDistance = 10;

        public float D { get {return GetTransform().GetOrigin().Length; } }

        public Vector4 ABC { get; set; }

        public override PixelColor IntersectionColor(ref Ray ray)
        {
                float t;

                ray.SurfaceIntersection(out t,D, ABC.Xyz);

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
            Matrix4 tmp = cam.Projection * cam.View;

            Vector4 LU3;

            Vector4 RD3;

            if (1 - Math.Abs(ABC.X) < 0.01)
            {
                LU3 = tmp * new Vector4(0, -MaxClipDistance / 2, -MaxClipDistance / 2, 1);

                RD3 = tmp * new Vector4(0,  MaxClipDistance / 2,  MaxClipDistance / 2, 1);

                LU = new Vector2(Math.Min(LU3.X, RD3.X), Math.Min(LU3.Y, RD3.Y));

                RD = new Vector2(Math.Max(LU3.X, RD3.X), Math.Max(LU3.Y, RD3.Y));

                return;
            }

            if (1 - Math.Abs(ABC.Y) < 0.01)
            {
                LU3 = tmp * new Vector4(-MaxClipDistance, 0 , -MaxClipDistance, 1);

                RD3 = tmp * new Vector4( MaxClipDistance, 0, MaxClipDistance, 1);

                LU = new Vector2(Math.Min(LU3.X, RD3.X), Math.Min(LU3.Y, RD3.Y));

                RD = new Vector2(Math.Max(LU3.X, RD3.X), Math.Max(LU3.Y, RD3.Y));

                return;
            }

            if (1 - Math.Abs(ABC.Z) < 0.01)
            {
                LU3 = tmp * new Vector4(-MaxClipDistance, -MaxClipDistance, 0, 1);

                RD3 = tmp * new Vector4(MaxClipDistance, MaxClipDistance, 0, 1);

                LU = new Vector2(Math.Min(LU3.X, RD3.X), Math.Min(LU3.Y, RD3.Y));

                RD = new Vector2(Math.Max(LU3.X, RD3.X), Math.Max(LU3.Y, RD3.Y));

                return;
            }

            Vector3 Right = Vector3.Cross(Vector3.UnitY, ABC.Xyz);

            Right.Normalize();

            Vector3 Up = Vector3.Cross(ABC.Xyz, Right);

            Right.Normalize();

            LU3 = tmp * new Vector4(ABC.X * D - (Right.X + Up.X) * MaxClipDistance,
                                    ABC.Y * D - (Right.Y + Up.Y) * MaxClipDistance,
                                    ABC.Z * D - (Right.Z + Up.Z) * MaxClipDistance, 1);

            RD3 = tmp * new Vector4(ABC.X * D + (Right.X + Up.X) * MaxClipDistance,
                                    ABC.Y * D + (Right.Y + Up.Y) * MaxClipDistance,
                                    ABC.Z * D + (Right.Z + Up.Z) * MaxClipDistance, 1);
            
            LU = new Vector2(Math.Min(LU3.X, RD3.X), Math.Min(LU3.Y, RD3.Y));

            RD = new Vector2(Math.Max(LU3.X, RD3.X), Math.Max(LU3.Y, RD3.Y));
        }


        public override ARTModel Copy()
        {
            return new Surface(D, ABC.Xyz);
        }


        public Surface(float shiftAlongNormal, Vector3 surfNormal):base()
        {
            surfNormal.Normalize();

            GetTransform().Translation(surfNormal.X * shiftAlongNormal, surfNormal.Y * shiftAlongNormal, surfNormal.Z * shiftAlongNormal);

            ABC = new Vector4(surfNormal.X, surfNormal.Y, surfNormal.Z, 0);
        }
    }
}
