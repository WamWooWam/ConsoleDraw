using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class Button : Control
    {
        private string _text;

        public Button()
        {
            BorderColor = ConsoleColor.DarkGray;
            BackgroundColour = ConsoleColor.Gray;
            ForegroundColour = ConsoleColor.Black;

            BorderThickness = new Thickness(1);
            Padding = new Thickness(1);
        }

        public bool AutoSize { get; set; }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _needsUpdate = true;
                if (AutoSize)
                {
                    Width = Text.Length;
                    Height = 1;
                }
            }
        }

        protected override void Draw(FrameBufferGraphics graph)
        {
            // look right
            // i never said what i write is readable

            int offsetX = (InnerWidth - (Text.Length > InnerWidth ? InnerWidth : Text.Length)) / 2 + BorderThickness.Left;
            int offsetY = Height == 1 ? 0 : (Height) / 2;

            graph.DrawString(Text.Length > InnerWidth ? Text.Substring(0, InnerWidth) : Text, new Point(offsetX, offsetY), ForegroundColour);
        }
    }
}
