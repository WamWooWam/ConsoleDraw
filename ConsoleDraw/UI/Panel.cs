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

        internal void ExtensionDraw(FrameBufferGraphics graphics) => Draw(graphics);

        protected override void Draw(FrameBufferGraphics graph)
        {
            if (Controls.Any(c => c.NeedsUpdate))
            {
                if (NeedsUpdate)
                {
                    graph.Clear(BackgroundColour);
                    foreach (Control control in Controls)
                    {
                        control.InternalDraw(graph);
                    }
                }
                else
                {
                    foreach (Control control in Controls.Where(c => c.NeedsUpdate))
                    {
                        control.InternalDraw(graph);
                    }
                }

                _needsUpdate = false;
            }
        }
    }
}
