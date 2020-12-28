using OpenTK;
using Raytracer.Events;
using Raytracer.Model;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Raytracer.Cameras
{

    public abstract class CameraCallBack : IMouseAction
    {
        public Camera CameraUnderControl { get; set; }

        public abstract void Action(IMouseEventsSource sender, MouseEventArgs event_);
        
        public CameraCallBack(Camera cam)
        {
            this.CameraUnderControl = cam;
        }
    }

    public class CameraMouseWheelCB : CameraCallBack
    {
        public CameraMouseWheelCB(Camera cam) : base(cam)
        {

        }

        public override void Action(IMouseEventsSource sender, MouseEventArgs event_)
        {
            CameraUnderControl.MoveAlondForward(0.05f * event_.Delta);
        }
    }

    public class CameraMouseButtonCB : CameraCallBack
    {
        public CameraMouseButtonCB(Camera cam) : base(cam)
        {

        }

        public override void Action(IMouseEventsSource sender, MouseEventArgs event_)
        {
            if (event_.Button.Equals(MouseButtons.Left))
            {
                CameraUnderControl.Rotate((float)(sender.MouseDXDY.X * 1.0f / sender.ContorlledItem.Width), (float)(sender.MouseDXDY.Y * 1.0f / sender.ContorlledItem.Height));
            }

            //if (event_.Button.Equals(MouseButtons.Right))
            //{

            //}

            if (event_.Button.Equals(MouseButtons.Middle))
            {
                CameraUnderControl.MoveAlondUp(0.05f * sender.MouseDXDY.X);
                CameraUnderControl.MoveAlondRight(0.05f * sender.MouseDXDY.Y);
            }

        }
    }


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
                                          (float)Math.Cos(vertical),
                                          (float)(Math.Cos(horizontal) * Math.Sin(vertical)));
            forward.Normalize();

            Vector3 right = Vector3.Cross(Vector3.UnitY, forward);

            right.Normalize();

            Vector3 up = Vector3.Cross(forward, right);

            view.M11 = right.X;
            view.M21 = right.Y;
            view.M31 = right.Z;

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

        public void Move(float x, float y, float z)
        {
            view.M14 += x;

            view.M24 += y;

            view.M34 += z;
        }

        public void MoveAlondForward(float step)
        {
            view.M14 += view.M13 * step;

            view.M24 += view.M23 * step;

            view.M34 += view.M33 * step;
        }

        public void MoveAlondUp(float step)
        {
            view.M14 += view.M12 * step;

            view.M24 += view.M22 * step;

            view.M34 += view.M32 * step;
        }

        public void MoveAlondRight(float step)
        {
            view.M14 += view.M11 * step;

            view.M24 += view.M21 * step;

            view.M34 += view.M31 * step;
            
        }

        public override int GetHashCode()
        {
            return view.GetHashCode() << Projection.GetHashCode();
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

        public Camera(float fov_, float aspect, Vector3 orig)
        {
            fov = 1 / (float)Math.Tan(fov_ / 2); ;

            Aspect = aspect;

            view = new Matrix4(1, 0, 0, orig.X,
                               0, 1, 0, orig.Y,
                               0, 0, 1, orig.Z,
                               0, 0, 0, 1);

            Projection = Matrix4.CreatePerspectiveFieldOfView(fov_, aspect, 0.1f, 100000);
        }
        
        public Camera(float fov_, float aspect, Vector3 orig, Vector3 forward)
        {
            fov = 1 / (float)Math.Tan(fov_ / 2); ;

            Aspect = aspect;

            view = new Matrix4(1, 0, 0, orig.X,
                               0, 1, 0, orig.Y,
                               0, 0, 1, orig.Z,
                               0, 0, 0, 1);

            forward.Normalize();

            Vector3 right = Vector3.Cross(Vector3.UnitY, forward);

            right.Normalize();

            Vector3 up = Vector3.Cross(forward, right);

            view.M11 = right.X;
            view.M21 = right.Y;
            view.M31 = right.Z;

            view.M12 = up.X;
            view.M22 = up.Y;
            view.M32 = up.Z;

            view.M13 = forward.X;
            view.M23 = forward.Y;
            view.M33 = forward.Z;

            Projection = Matrix4.CreatePerspectiveFieldOfView(fov_, aspect, 0.1f, 100000);
        }

    }
}
