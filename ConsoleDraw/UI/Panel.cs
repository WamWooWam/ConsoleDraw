using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class Panel : Control
    {
        public Panel()
        {
            Controls = new List<Control>();
        }

        public List<Control> Controls { get; private set; }

        public override bool IsSelectable => false;

        public override void Draw(FrameBufferGraphics graph)
        {
            graph.Clear(BackgroundColour);
            foreach (Control control in Controls)
            {
                control.Draw(graph);
            }
        }
    }
}
