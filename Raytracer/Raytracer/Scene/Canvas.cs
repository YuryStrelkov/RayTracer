using OpenTK;
using Raytracer.Model;
using Raytracer.Textures;
using System;
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

        private Matrix4 view;

        private Matrix4 translation;

        public Matrix4 Translation { get { return translation; } }

        public Matrix4 View { get { return view; } }

        public Matrix4 Projection { get; private set; }

        public Matrix4 CameraTransform { get { return translation * view; } }
 
        public void Rotate(float vertical, float horizontal)
        {
            vertical = MathHelper.Clamp(vertical, -3, 3);

            Vector3 forward = new Vector3((float)(Math.Sin(horizontal) * Math.Sin(vertical)),
                                          (float) Math.Cos(vertical),
                                          (float)(Math.Cos(horizontal) * Math.Sin(vertical)));

            Vector3 left = Vector3.Cross(Vector3.UnitZ, forward);

            left.Normalize();

            Vector3 up = Vector3.Cross(left, forward);

            view.M11 = forward.X;
            view.M21 = forward.Y;
            view.M31 = forward.Z;

            view.M12 = left.X;
            view.M22 = left.Y;
            view.M32 = left.Z;

            view.M13 = up.X;
            view.M23 = up.Y;
            view.M33 = up.Z;
            
        }

        public void Move(Vector3 pos)
        {
            translation.M14 += pos.X;

            translation.M24 += pos.Y;

            translation.M34 += pos.Z;
        }

        public Ray GetViewRay(float x, float y)//x and y belongs [-1,1];
        {
            Vector4 dir = View * Projection * new Vector4(0, 0, 1, 0);
 
            Vector4 orig = new Vector4(x, y, 0, 1);

            orig = View * orig;

            orig.X += translation.M14;

            orig.Y += translation.M24;

            orig.Z += translation.M34;

            return new Ray(orig.Xyz, dir.Xyz);
        }

        public Camera(float fov_, float aspect,Vector3 orig)
        {
            fov = 1 / (float)Math.Tan(fov_ / 2); ;

            Aspect = aspect;

            view = new Matrix4(1,0,0,0,
                               0,1,0,0,
                               0,0,1,0,
                               0,0,0,1);

            Projection =  Matrix4.CreatePerspectiveFieldOfView(fov_, aspect, 0.1f, 100000);

            translation = new Matrix4();

            translation.M14 = orig.X;

            translation.M24 = orig.Y;

            translation.M34 = orig.Z;
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

        public void Render(ARTModel model, Camera cam)
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
 
                        color = model.IntersectionColor(ref r);

                        if (r.Length < distance)
                        {
                            distance = r.Length;
                            CanvasTexture.SetPixel( (x + 1) / 2, (y + 1) / 2, color);
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
