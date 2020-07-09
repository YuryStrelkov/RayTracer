using Raytracer.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Raytracer.Events
{
    /// <summary>
    /// 
    /// </summary>
    public enum MouseActionEvents
    {
        Click = 0,

        Press = 1,

        Release = 2,

        Move = 3,

        WheellRotation = 4
    }

    public interface IMouseAction 
    {
        void Action(IMouseEventsSource sender, MouseEventArgs event_);
    }

    public interface IMouseEventsSource 
    {
        Point MouseXY { get;}

        Point MouseDXDY { get;}

        bool Pressed { get;}

        Control ContorlledItem { get;}

        void AddEvent(MouseActionEvents eventType, IMouseAction actionEvent);
    }

    public sealed class MouseEvents : IMouseEventsSource
    {
        private class DummyEvent : IMouseAction
        {
            public  void Action(IMouseEventsSource sender, MouseEventArgs event_)
            {

            }
        }

        public Control ContorlledItem { get; private set; }

        private Dictionary<MouseActionEvents, IMouseAction> MouseActions;

        public Point MouseXY { get; private set; }

        public Point MouseDXDY { get; private set; }

        public bool Pressed { get; private set; }

        public void AddEvent(MouseActionEvents eventType, IMouseAction actionEvent)
        {
            MouseActions.Add(eventType, actionEvent);
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (Pressed)
            {
                MouseDXDY = new Point (e.X - MouseXY.X, e.Y - MouseXY.Y);
                Console.WriteLine(MouseDXDY.ToString());
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

        public MouseEvents(Control parent)
        {
            ContorlledItem = parent;

            MouseActions = new Dictionary<MouseActionEvents, IMouseAction>();

            DummyEvent dummy = new DummyEvent();

            AddEvent(MouseActionEvents.Move, dummy);
            AddEvent(MouseActionEvents.Click, dummy);
            AddEvent(MouseActionEvents.Press, dummy);
            AddEvent(MouseActionEvents.Release, dummy);
            AddEvent(MouseActionEvents.WheellRotation, dummy);

            ContorlledItem.MouseMove += MouseMove;
            ContorlledItem.MouseDown += MouseDown;
            ContorlledItem.MouseClick += MouseClick;
            ContorlledItem.MouseWheel += MouseWhell;
            ContorlledItem.MouseUp += MouseRelease;
        }

    }
}
