using Raytracer.Events;
using System.Drawing;
using System.Windows.Forms;

namespace Raytracer.Scene
{
    public class SceneFrame:Form
    {

        public PictureBox ViewPort { get; private set;}

        private MouseEvents pictureBoxEvents;

        private SceneRT scn;

        public void UpdateFrame(float dt)
        {

        }

        public void DrawFrame()
        {
            while (true)
            {
                scn.Draw((Bitmap)ViewPort.Image);
            }
        }

        public void ClearFrame()
        {
             
        }


        public SceneFrame():base()
        {
            ViewPort = new PictureBox();

            ViewPort.SizeMode = PictureBoxSizeMode.Zoom;

            ViewPort.Dock = DockStyle.Fill;

            ViewPort.Image = new Bitmap(800, 600);

            pictureBoxEvents = new MouseEvents(ViewPort);
            
            this.Controls.Add(ViewPort);

            this.Size = new Size(800, 600);

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SceneFrame
            // 
            this.Size = new Size(800, 600);

            this.Name = "SceneFrame";

            this.Load += new System.EventHandler(this.SceneFrame_Load);

            this.ResumeLayout(false);
        }

        private void SceneFrame_Load(object sender, System.EventArgs e)
        {

        }
    }
}
