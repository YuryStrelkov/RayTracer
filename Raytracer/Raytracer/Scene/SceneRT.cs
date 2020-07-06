using Raytracer.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Raytracer.Scene
{
    public class SceneRT
    {
        public static int FPS60 = 17;//ms

        public static int FPS30 = 34;//ms

        public static int FPS120 = 120;//8ms

        private List<ARTModel> models;

        private Camera cam;

        private Canvas canvas;

        private Timer RenderTimer;

        private void Update()
        {
        }
        
        private void ResolveToBitmap(ref Bitmap bmp)
        {
            canvas.ResolveToBitmap(ref bmp);
        }

        private  void Draw(object o)
        {
             Update();
            //canvas.Render(models, cam);
        }


        public  void Draw(ref Bitmap bmp)
        {
            ResolveToBitmap(ref bmp);
            
        }

        public void LoadScn (string src)
        {

        }

        public void AddModel(string src)
        {

        }

        public void AddModel(ARTModel m)
        {

        }

        public SceneRT(int w, int h)
        {

            RenderTimer = new Timer(new TimerCallback(Draw), null, 0, FPS30);

            models = new List<ARTModel>();

            cam = new Camera(90, 1.5f, new OpenTK.Vector3(0, 0, 0));

            canvas = new Canvas(w,h);
        }
    }
}
