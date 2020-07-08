using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Raytracer.Events
{
    public enum MouseActionEvents
    {
        Click = 0,

        Press = 1,

        Release = 2,

        Move = 3,
        
        WheellRotation = 4
    }

    public interface IMouseAction<ItemUnderControl>
    {
        void Action(IMouseEvents<ItemUnderControl> sender, MouseEventArgs event_);
    }

    public interface IMouseEvents<ItemUnderControl> 
    {
        Point MouseXY { get;}

        Point MouseDXDY { get;}

        bool Pressed { get;}

        ItemUnderControl ContorlledItem { get;}

        void AddEventProcessor(MouseActionEvents eventType, IMouseAction<ItemUnderControl> actionEvent);
    }

    public sealed class MouseEvents<T> : IMouseEvents<T> where T:Control
    {
        private class DummyEvent : IMouseAction<T>
        {
            public  void Action(IMouseEvents<T> sender, MouseEventArgs event_)
            {

            }
        }

        public T ContorlledItem { get; private set; }

        private Dictionary<MouseActionEvents, IMouseAction<T>> MouseActions;

        public Point MouseXY { get; private set; }

        public Point MouseDXDY { get; private set; }

        public bool Pressed { get; private set; }

        public void AddEventProcessor(MouseActionEvents eventType, IMouseAction<T> actionEvent)
        {
            MouseActions.Add(eventType, actionEvent);
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (Pressed)
            {
                MouseDXDY = new Point (e.X - MouseXY.X, e.Y - MouseXY.Y);
            }

            MouseActions[MouseActionEvents.Move]. Action(this, e);
        }

        public void MouseClick(object sender, MouseEventArgs e)
        {
               MouseActions[MouseActionEvents.Click].Action(this, e);
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            Pressed = true;

            MouseXY = e.Location;

            MouseActions[MouseActionEvents.Press].Action(this, e);
        }

        public void MouseRelease(object sender, MouseEventArgs e)
        {
            Pressed = false;

            MouseActions[MouseActionEvents.Release].Action(this, e);
        }

        public void MouseWhell(object sender, MouseEventArgs e)
        {
           MouseActions[MouseActionEvents.WheellRotation].Action(this, e);
        }

        public MouseEvents(T parent)
        {
            MouseActions = new Dictionary<MouseActionEvents, IMouseAction<T>>();
            DummyEvent dummy = new DummyEvent();
            MouseActions.Add(MouseActionEvents.Move, dummy);
            MouseActions.Add(MouseActionEvents.Click, dummy);
            MouseActions.Add(MouseActionEvents.Press, dummy);
            MouseActions.Add(MouseActionEvents.Release, dummy);
            MouseActions.Add(MouseActionEvents.WheellRotation, dummy);

            ContorlledItem.MouseMove += MouseMove;
            ContorlledItem.MouseDown += MouseDown;
            ContorlledItem.MouseClick += MouseClick;
            ContorlledItem.MouseWheel += MouseWhell;
            ContorlledItem.MouseUp += MouseRelease;
        }

    }
}
