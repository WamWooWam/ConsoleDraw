using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class Button : Control
    {
        public Button()
        {
            BorderColor = ConsoleColor.DarkGray;
            BackgroundColour = ConsoleColor.Gray;
            ForegroundColour = ConsoleColor.Black;

            BorderThickness = new Thickness(1);
            Padding = new Thickness(1);
        }

        public string Text { get; set; }

        public override void Draw(FrameBufferGraphics graph)
        {
            // look right
            // i never said what i write is readable

            int drawWidth = Width != -1 ? Width : Text.Length + BorderThickness.LeftRight + Padding.LeftRight;
            int drawHeight = Height != -1 ? Height : 1 + BorderThickness.TopBottom + Padding.TopBottom;

            graph.DrawRect(
                new Rectangle(X , Y, drawWidth, drawHeight),
                _active? BackgroundColour: BorderColor);

            graph.DrawRect(
                new Rectangle(X + BorderThickness.Left, Y + BorderThickness.Top, drawWidth - BorderThickness.LeftRight, drawHeight - BorderThickness.TopBottom),
                _active ? BorderColor : BackgroundColour);

            int offsetX = (drawWidth - Text.Length) / 2;
            int offsetY = (drawHeight) / 2;

            graph.DrawString(Text, new Point(X + offsetX, Y + offsetY), ForegroundColour);
        }
    }
}
