using OpenTK;
using Raytracer.Cameras;
using Raytracer.Model;
using Raytracer.Model.Nodes;
using Raytracer.Model.SpecificModels;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Raytracer.Scene
{
    public class SceneRT : NodeList<ARTModel>
    {
        public static int FPS60 = 17;//ms

        public static int FPS30 = 34;//ms

        public static int FPS120 = 120;//8ms

        public int ActiveCameraID { get; private set; }

        private Dictionary<int,Camera> Cameras;
        
        private Canvas canvas;

        private Timer RenderTimer;

        private void Update()
        {
        }
        
        private void ResolveToBitmap(Bitmap bmp)
        {
            canvas.ResolveToBitmap( bmp);
        }

        private  void Draw(object o)
        {
             Update();
             //canvas.Render(models, cam);
        }


        public  void Draw(Bitmap bmp)
        {
            ResolveToBitmap(bmp);
        }

        public void LoadScn (string src)
        {

        }

        public void AddModel(string src)
        {
            AddNode(new Mesh(src));
        }

        public void AddModel(ARTModel m)
        {
            AddNode(m);
        }

        public void AddCamera(Vector3 origin, Vector3 forward, float fov, float aspect)
        {
            Camera c = new Camera(fov, aspect, origin, forward);

            ActiveCameraID = c.GetHashCode();

            Cameras.Add(ActiveCameraID, c);
        }

        public SceneRT(int w, int h)
        {
            RenderTimer = new Timer(new TimerCallback(Draw), null, 0, FPS30);

            Cameras = new Dictionary<int, Camera>();

            AddCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 1), 90, h * 1.0f / w);

            canvas = new Canvas(w,h);
        }
    }
}
