using System.Collections.Generic;
using System.Windows.Forms;

namespace Raytracer.Events
{
    class KeyBoardEvents
    {
        public enum KeyBoardActionEvents
        {
            Click = 0,

            Press = 1,

            Release = 2,
        }

        public interface IKeyboardAction
        {
            void Action(IKeyboardEventsSource sender, KeyEventArgs event_);
        }

        public interface IKeyboardEventsSource
        {
            bool Pressed { get; }

            Control ContorlledItem { get; }

            void AddEvent(KeyBoardActionEvents eventType, IKeyboardAction actionEvent);
        }

        public sealed class MouseEvents : IKeyboardEventsSource
        {
            private class DummyEvent : IKeyboardAction
            {
                public void Action(IKeyboardEventsSource sender, KeyEventArgs event_)
                {

                }
            }

            public Control ContorlledItem { get; private set; }

            private Dictionary<KeyBoardActionEvents, IKeyboardAction> KeysActions;

            public bool Pressed { get; private set; }

            public void AddEvent(KeyBoardActionEvents eventType, IKeyboardAction actionEvent)
            {
                KeysActions.Add(eventType, actionEvent);
            }
 

            public void KeyPress(object sender, KeyEventArgs e)
            {
                KeysActions[KeyBoardActionEvents.Press].Action(this, e);
            }

            public void KeyDown(object sender, KeyEventArgs e)
            {
                Pressed = true;

                KeysActions[KeyBoardActionEvents.Click].Action(this, e);
            }

            public void KeyRelease(object sender, KeyEventArgs e)
            {
                Pressed = false;

                KeysActions[KeyBoardActionEvents.Release].Action(this, e);
            }
 

            public MouseEvents(Control parent)
            {
                ContorlledItem = parent;

                KeysActions = new Dictionary<KeyBoardActionEvents, IKeyboardAction>();

                DummyEvent dummy = new DummyEvent();

                AddEvent(KeyBoardActionEvents.Click, dummy);

                AddEvent(KeyBoardActionEvents.Press, dummy);

                AddEvent(KeyBoardActionEvents.Release, dummy);

                ContorlledItem.KeyDown += KeyDown;
          //      ContorlledItem.KeyPress += KeyDown;

            //    ContorlledItem.MouseMove += MouseMove;
              //  ContorlledItem.MouseDown += MouseDown;
               // ContorlledItem.MouseClick += MouseClick;
                //ContorlledItem.MouseWheel += MouseWhell;
                //ContorlledItem.MouseUp += MouseRelease;
            }

        }
    }
}
