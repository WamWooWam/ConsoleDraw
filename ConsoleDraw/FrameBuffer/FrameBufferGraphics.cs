using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDraw.Data
{
    public class FrameBufferGraphics
    {
        private FrameBuffer _frameBuffer;

        /// <summary>
        /// Initialises a Graphics object based on a framebuffer
        /// </summary>
        /// <param name="frameBuffer">The frame buffer.</param>
        public void Init(FrameBuffer frameBuffer)
        {
            _frameBuffer = frameBuffer;
        }

        public void DrawRect(Rectangle rect, ConsoleColor bgColour, ConsoleColor fgColour = ConsoleColor.Gray, char character = ' ')
        {
            if (rect.X < 0 || rect.X > _frameBuffer.Width)
                throw new ArgumentException("X cannot be less than 0 or greater than the framebuffer width", "rext.X");
            if (rect.Y < 0 || rect.Y > _frameBuffer.Width)
                throw new ArgumentException("Y cannot be less than 0 or greater than the framebuffer height", "rext.Y");

            int intialPoint = (rect.X) + (rect.Y * _frameBuffer.Width);
            for (int i = 0; i < rect.Height; i++)
            {
                for (int j = 0; j < rect.Width; j++)
                {
                    int newPoint = j + (i * _frameBuffer.Width) + intialPoint;
                    _frameBuffer.RawPixels[newPoint] = new FrameBufferPixel() { BackgroundColour = bgColour, ForegroundColour = fgColour, Character = character };
                }
            }
        }

        public void DrawString(string Text, Point point, ConsoleColor fgColour = ConsoleColor.Gray)
        {
            int intialPoint = (point.X) + (point.Y * _frameBuffer.Width);
            for (int j = 0; j < Text.Length; j++)
            {
                int newPoint = j + intialPoint;
                FrameBufferPixel currentPoint = _frameBuffer.RawPixels[newPoint];
                _frameBuffer.RawPixels[newPoint] = new FrameBufferPixel() { BackgroundColour = currentPoint.BackgroundColour, ForegroundColour = fgColour, Character = Text[j] };
            }
        }
    }
}
