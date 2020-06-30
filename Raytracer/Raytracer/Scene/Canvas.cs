using OpenTK;
using Raytracer.Model;
using Raytracer.Textures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Raytracer.Scene
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Camera
    {
        private float fov;

        public float FOV { get { return (float)Math.Atan(2 * fov); } set { fov = 1 / (float)Math.Tan(value / 2); } }

        public float Aspect { get; set; }

        public Transform CameraTransform { get; private set; }

        public void VerticalRotation(float angle)
        {

        }

        public void HorizontalRotation(float angle)
        {

        }

        public void Move(Vector3 pos)
        {
            CameraTransform.Translation(pos.X, pos.Y, pos.Z);
        }

        public Ray GetViewRay(float x, float y)//x and y belongs [-1,1];
        {
            Vector4 dir = new Vector4(x, y * Aspect, FOV, 0);

            float l = MathHelper.InverseSqrtFast(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z);

            dir.X *= l;
            dir.Y *= l;
            dir.Z *= l;

            dir = CameraTransform.TransformM * dir;

            Vector4 orig = new Vector4(x, y, 0, 1);

            orig = CameraTransform.TransformM * orig;

            return new Ray(orig.Xyz, dir.Xyz);

        }

        public Camera(float fov_, float aspect,Vector3 orig)
        {
            fov = fov_;
            Aspect = aspect;
            CameraTransform = new Transform(orig.X, orig.Y, orig.Z);
        }

    }

    public class Canvas
    {
        public Texture CanvasTexture { get; private set;}

        public void ClearCanvas()
        {
            CanvasTexture.ClearTexture();
        }

        public void ResolveToBitmap(ref Bitmap frame)
        {
            CanvasTexture.ToBitmap(ref frame);
        }

        public void Render(List<ARTModel> models, Camera cam)
        {
            ClearCanvas();

            Parallel.For(0, CanvasTexture.Rows, (row) => 
            {
                float y = -1.0f + row / (CanvasTexture.Rows - 1.0f);

                float x;

                float distance = float.MaxValue;

                PixelColor color = new PixelColor(0, 0, 0);

                for (int cols = 0 ; cols < CanvasTexture.Coloms ; cols++)
                {
                     x = -1.0f + cols / (CanvasTexture.Coloms - 1.0f);

                    Ray r = cam.GetViewRay(x, y);

                    foreach (ARTModel m in models)
                    {
                        color = m.IntersectionColor(ref r);
                        if (r.Length < distance)
                        {
                            distance = r.Length;
                            CanvasTexture.SetPixel( (x + 1) / 2, (y + 1) / 2, color);
                        }
                    }
                }
            }
            );
        }

        public Canvas(int w, int h)
        {
             CanvasTexture = new Texture( w, h );
        }

    }
}
