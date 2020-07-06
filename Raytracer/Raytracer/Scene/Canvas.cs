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

        public Matrix4 View { get { return view; } }

        public Matrix4 Projection { get; private set; }

        public void Rotate(float vertical, float horizontal)
        {
            vertical = MathHelper.Clamp(vertical, -3, 3);

            Vector3 forward = new Vector3((float)(Math.Sin(horizontal) * Math.Sin(vertical)),
                                          (float) Math.Cos(vertical),
                                          (float)(Math.Cos(horizontal) * Math.Sin(vertical)));
            forward.Normalize();

            Vector3 right = Vector3.Cross(Vector3.UnitY, forward);

            right.Normalize();
            
            Vector3 up = Vector3.Cross(forward,right);

            view.M13 = right.X;
            view.M23 = right.Y;
            view.M33 = right.Z;

            view.M12 = up.X;
            view.M22 = up.Y;
            view.M32 = up.Z;

            view.M13 = forward.X;
            view.M23 = forward.Y;
            view.M33 = forward.Z;
        }

        public void Move(Vector3 pos)
        {
            view.M14 += pos.X;

            view.M24 += pos.Y;

            view.M34 += pos.Z;
        }

        public Ray GetViewRay(float x, float y)//x and y belongs [-1,1];
        {
            Vector4 dir = View * Projection * new Vector4(0, 0, 1, 0);
 
            Vector4 orig = new Vector4(x, y, 0, 1);

            orig = View * orig;

            orig.X += view.M14;

            orig.Y += view.M24;

            orig.Z += view.M34;

            return new Ray(orig.Xyz, dir.Xyz);
        }

        public Camera(float fov_, float aspect,Vector3 orig)
        {
            fov = 1 / (float)Math.Tan(fov_ / 2); ;

            Aspect = aspect;

            view = new Matrix4(1, 0, 0, orig.X,
                               0, 1, 0, orig.Y,
                               0, 0, 1, orig.Z,
                               0, 0, 0, 1);

            Projection =  Matrix4.CreatePerspectiveFieldOfView(fov_, aspect, 0.1f, 100000);
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
            ///ClearCanvas();

            Vector2 RD;

            Vector2 LU;

            model.OnCamSpace(cam, out LU,out RD);

            int coloms0 = (int)(LU.X * (CanvasTexture.Coloms - 1));

            int coloms1 = (int)(RD.X * (CanvasTexture.Coloms - 1));

            int rows0   = (int)(LU.Y * (CanvasTexture.Coloms - 1));

            int rows1   = (int)(RD.Y * (CanvasTexture.Coloms - 1));

            Parallel.For(rows0, rows1, (row) => 
            {
                float y = -1.0f + row / (CanvasTexture.Rows - 1.0f);

                float x;

                float distance = float.MaxValue;

                PixelColor color = new PixelColor(0, 0, 0);

                for (int cols = coloms0; cols < coloms1; cols++)
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
