using OpenTK;
using Raytracer.Model;
using Raytracer.Cameras;
using Raytracer.Textures;
using System.Drawing;
using System.Threading.Tasks;

namespace Raytracer.Scene
{
    
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
