using Raytracer.Model;
using System.Collections.Generic;
using System.Drawing;

namespace Raytracer.Scene
{
    public class SceneRT
    {
        private List<ARTModel> models;

        private Camera cam;

        private Canvas canvas;

        private void Update()
        {
        }
        
        private void ResolveToBitmap(ref Bitmap bmp)
        {
        }

        public void Draw(ref Bitmap bmp)
        {
            Update();
            canvas.Render(models, cam);
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
            models = new List<ARTModel>();
            cam = new Camera(90, 1.5f, new OpenTK.Vector3(0, 0, 0));
            canvas = new Canvas(w,h);
        }
    }
}
