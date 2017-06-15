using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleDraw.UI
{
    class Button : Control
    {
        public string Text { get; set; }

        public override void Draw(FrameBufferGraphics graph)
        {
            graph.DrawRect(_rect, ConsoleColor.DarkGray);
            graph.DrawRect(new Rectangle(_rect.X + 1, _rect.Y + 1, _rect.Width - 2, _rect.Height - 2), BackgroundColour);
            int offsetX = (_rect.Width - Text.Length) / 2;
            int offsetY = (_rect.Height) / 2;
            graph.DrawString(Text, new Point(_rect.X + offsetX, _rect.Y + offsetY), ForegroundColour);
        }
    }
}
