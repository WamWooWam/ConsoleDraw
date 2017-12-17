using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public abstract class Control
    {
        public Control()
        {
            _active = false;
        }

        protected bool _active;
        public event EventHandler Activated;
        public event EventHandler Selected;
        public event EventHandler Deselected;

        public virtual void Activate()
        {
            Activated?.Invoke(this, new EventArgs());
        }

        public virtual void Select()
        {
            _active = true;
            Selected?.Invoke(this, new EventArgs());
        }

        public virtual void Deselect()
        {
            _active = false;
            Deselected?.Invoke(this, new EventArgs());
        }

        public ConsoleColor ForegroundColour { get; set; }
        public ConsoleColor BackgroundColour { get; set; }
        public ConsoleColor BorderColor { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;

        public Thickness BorderThickness { get; set; }
        public Thickness Padding { get; set; }

        public virtual bool IsSelectable => true;

        public abstract void Draw(FrameBufferGraphics graph);

        public static implicit operator Rectangle(Control c) => new Rectangle(c.X, c.Y, c.Width, c.Height);
    }
}
