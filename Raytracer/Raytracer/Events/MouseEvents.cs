using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Raytracer.Events
{
    public enum MouseActionEvents
    {
        Clique = 0,

        Press = 1,

        Release = 2,

        Move = 3,

        Drag = 4,

        WheellRotation = 5
    }

    public abstract class MouseAction
    {
        public abstract void Action(object sender, MouseEventArgs event_);

        public Point MouseCoordinates(MouseEventArgs event_)
        {
            return event_.Location;
        }
    }

    public sealed class MouseEvents<T> where T : ContainerControl
    {
        private class DummyEvent : MouseAction
        {
            public override void Action(object sender, MouseEventArgs event_)
            {
            }
        }

        private T container;

        private Dictionary<MouseActionEvents, MouseAction> MouseActions;

        public Point MouseXY { get; private set; }

        public Point MouseDXDY { get; private set; }

        public bool Pressed { get; private set; }

        public void AddEventProcessor(MouseActionEvents eventType, MouseAction actionEvent)
        {
            MouseActions.Add(eventType, actionEvent);
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            MouseActions[MouseActionEvents.Move].Action(sender,e);
            MouseXY = MouseActions[MouseActionEvents.Move].MouseCoordinates(e);
        }

        public void MouseDrag(object sender, MouseEventArgs e)
        {
            MouseActions[MouseActionEvents.Drag].Action(sender, e);
            MouseXY = MouseActions[MouseActionEvents.Move].MouseCoordinates(e);
        }

        public void MouseClique(object sender, MouseEventArgs e)
        {
               Pressed = true;
               MouseActions[MouseActionEvents.Clique].Action(sender, e);
        }

        public void MousePressed(object sender, MouseEventArgs e)
        {
            if (!Pressed)
            {
                return;
            }
            MouseActions[MouseActionEvents.Press].Action(sender, e);
        }

        public void MouseRelease(object sender, MouseEventArgs e)
        {
            Pressed = false;
            MouseActions[MouseActionEvents.Release].Action(sender, e);
        }

        public void MouseWhell(object sender, MouseEventArgs e)
        {
           MouseActions[MouseActionEvents.WheellRotation].Action(sender, e);
        }

        public MouseEvents(T parent)
        {
            MouseActions = new Dictionary<MouseActionEvents, MouseAction>();
            DummyEvent dummy = new DummyEvent();
            MouseActions.Add(MouseActionEvents.Move, dummy);
            MouseActions.Add(MouseActionEvents.Drag, dummy);
            MouseActions.Add(MouseActionEvents.Clique, dummy);
            MouseActions.Add(MouseActionEvents.Press, dummy);
            MouseActions.Add(MouseActionEvents.Release, dummy);
            MouseActions.Add(MouseActionEvents.WheellRotation, dummy);

            container.MouseMove += MouseMove;
            container.MouseDown += MouseMove;
            container.MouseClick += MouseClique;
            container.MouseWheel += MouseWhell;
            container.MouseUp += MouseRelease;
        }

    }
}
