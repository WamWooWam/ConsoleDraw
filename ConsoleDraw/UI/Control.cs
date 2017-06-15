using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDraw.UI
{
    public abstract class Control
    {
        internal Rectangle _rect;

        public ConsoleColor ForegroundColour { get; set; }
        public ConsoleColor BackgroundColour { get; set; }

        public virtual void Create(Rectangle rect)
        {
            _rect = rect;
        }

        public abstract void Draw(FrameBufferGraphics graph);
    }
}
