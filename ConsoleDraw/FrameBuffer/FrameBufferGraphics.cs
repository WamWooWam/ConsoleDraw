using ConsoleDraw.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw
{
    public class FrameBufferGraphics
    {
        private FrameBuffer _frameBuffer;
        private FrameBufferPixel[,] _fbToModify;

        /// <summary>
        /// Enables or disables colour approximations
        /// </summary>
        public bool PseudoGraphics { get; set; }

        public FrameBuffer FrameBuffer => _frameBuffer;

        /// <summary>
        /// Initialises a <see cref="FrameBufferGraphics"/> object based on a <see cref="FrameBuffer"/>
        /// </summary>
        /// <param name="frameBuffer">The <see cref="FrameBuffer"/>.</param>
        public FrameBufferGraphics(FrameBuffer frameBuffer)
        {
            _frameBuffer = frameBuffer;
            _fbToModify = new FrameBufferPixel[_frameBuffer.Width, _frameBuffer.Height];
        }

        /// <summary>
        /// Clears the <see cref="FrameBuffer"/>, filling it with a specific colour
        /// </summary>
        /// <param name="colour"></param>
        public void Clear(ConsoleColor colour = ConsoleColor.Black)
        {
            DrawRect(new Rectangle(0, 0, _frameBuffer.Width, _frameBuffer.Height), colour, colour);
        }

        /// <summary>
        /// Draws a rectangle
        /// </summary>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="bgColour">The background colour</param>
        /// <param name="fgColour">The foreground colour</param>
        /// <param name="character">The character to draw with</param>
        public void DrawRect(Rectangle rect, ConsoleColor bgColour, ConsoleColor fgColour = ConsoleColor.Gray, char character = ' ')
        {
            _frameBuffer.GetFramebufferCopy(_fbToModify);

            //if (rect.X + rect.Width < _frameBuffer.Width && rect.Y + rect.Height < _frameBuffer.Height)
            //{
            for (int x = 0; x < rect.Width; x++)
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    _fbToModify[(x + rect.X).Clamp(0, _frameBuffer.Width - 1), (y + rect.Y).Clamp(0, _frameBuffer.Height - 1)] = new FrameBufferPixel() { BackgroundColour = bgColour, ForegroundColour = fgColour, Character = character };
                }
            }
            //}


            _frameBuffer.RawFrameBuffer = _fbToModify;
        }

        /// <summary>
        /// Draws an external framebuffer to this one.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="point"></param>
        public void DrawBuffer(FrameBuffer buffer, Point point)
        {
            if (!buffer.Running)
            {
                _frameBuffer.GetFramebufferCopy(_fbToModify);
                for (int x = 0; x < buffer.Width; x++)
                {
                    for (int y = 0; y < buffer.Height; y++)
                    {
                        _fbToModify[(x + point.X).Clamp(0, _frameBuffer.Width - 1), (y + point.Y).Clamp(0, _frameBuffer.Height - 1)] = buffer.RawFrameBuffer[x, y];
                    }
                }
                _frameBuffer.RawFrameBuffer = _fbToModify;
            }
            else
                throw new InvalidOperationException("Buffer cannot be running.");
        }

#if NET35 || NET40 || NET452
        /// <summary>
        /// Draws an image to the current <see cref="FrameBuffer"/>. Thanks nikitpad!
        /// </summary>
        /// <param name="image">The image to draw</param>
        /// <param name="point">The point to draw the image at</param>
        public void DrawImage(System.Drawing.Image image, Point point)
        {
            _frameBuffer.GetFramebufferCopy(_fbToModify);

            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)image;

            if (point.X <= _frameBuffer.Width && point.Y <= _frameBuffer.Height && point.X + bmp.Width <= _frameBuffer.Width && point.Y + bmp.Height <= _frameBuffer.Height)
            {
                FrameBuffer.InternalBitmapToFramebuffer(bmp, point, _fbToModify, PseudoGraphics);
            }
            else
                throw new InvalidOperationException("Image is too large for buffer");

            _frameBuffer.RawFrameBuffer = _fbToModify;
        }
#endif

        /// <summary>
        /// Draws the outile of an ellipse. Thanks to 0x3F!
        /// </summary>
        /// <param name="center">The center point of the ellipse</param>
        /// <param name="xRadius">The radius of the ellipse on the X axis</param>
        /// <param name="yRadius">The radius of the ellipse on the Y axis</param>
        /// <param name="bgColour">The background colour</param>
        /// <param name="fgColour">The foreground colour</param>
        /// <param name="character">The character to draw with</param>
        public void DrawEllipse(Point center, int xRadius, int yRadius, ConsoleColor bgColour, ConsoleColor fgColour = ConsoleColor.Gray, char character = ' ')
        {
            _frameBuffer.GetFramebufferCopy(_fbToModify);

            double angle = 0.0;
            double anglestepsize = 0.008;

            while (angle < 2 * Math.PI)
            {
                int x1 = (int)(center.X + xRadius * Math.Cos(angle));
                int y1 = (int)(center.Y + yRadius * Math.Sin(angle));

                angle += anglestepsize;

                _fbToModify[x1, y1] = new FrameBufferPixel() { BackgroundColour = bgColour, ForegroundColour = fgColour, Character = character };
            }

            _frameBuffer.RawFrameBuffer = _fbToModify;
        }

        /// <summary>
        /// Draws an ellipse. Thanks to 0x3F & nikitpad!
        /// </summary>
        /// <param name="center">The center point of the ellipse</param>
        /// <param name="xRadius">The radius of the ellipse on the X axis</param>
        /// <param name="yRadius">The radius of the ellipse on the Y axis</param>
        /// <param name="bgColour">The background colour</param>
        /// <param name="fgColour">The foreground colour</param>
        /// <param name="character">The character to draw with</param>
        public void FillEllipse(Point center, int xRadius, int yRadius, ConsoleColor bgColour, ConsoleColor fgColour = ConsoleColor.Gray, char character = ' ')
        {
            for (int i = xRadius; 0 < i; i--)
                DrawEllipse(center, i, yRadius, bgColour, fgColour, character);
        }

        /// <summary>
        /// Draws a border line
        /// </summary>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="bgColour">The background colour</param>
        /// <param name="width">The background colour</param>
        /// <param name="fgColour">The foreground colour</param>
        /// <param name="character">The character to draw with</param>
        public void DrawBorder(Rectangle rect, int width, ConsoleColor bgColour, ConsoleColor fgColour = ConsoleColor.Gray, char character = ' ')
        {
            DrawRect(rect, bgColour, fgColour, character);
            DrawRect(new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2), ConsoleColor.Black, ConsoleColor.Black, character);
        }

        /// <summary>
        /// Draws a string that can span multiple lines and will wrap automatically
        /// </summary>
        /// <param name="text">The string to draw</param>
        /// <param name="rect">The rectangle in which to draw the string</param>
        /// <param name="fgColour">The colour the string should be</param>
        /// <param name="hyphenate">Use hyphens to wrap large words?</param>
        /// <returns>The number of lines used to wrap the test.</returns>
        public int DrawMultiLineString(string text, Rectangle rect, ConsoleColor fgColour = ConsoleColor.Gray, bool hyphenate = false)
        {
            List<string> lines = GetLines(text, rect.Width, hyphenate);

            foreach (string lineToDraw in lines)
            {
                if (lines.IndexOf(lineToDraw) < rect.Height)
                    DrawString(lineToDraw, new Point(rect.X, rect.Y + lines.IndexOf(lineToDraw)), fgColour);
            }

            return lines.Count;
        }


        /// <summary>
        /// Splits a string into lines of a specified length
        /// TODO: Optimise
        /// </summary>
        /// <param name="text">The text to split</param>
        /// <param name="width">The maximum length of any one line</param>
        /// <param name="hyphenate">Split words and hyphenate or not.</param>
        /// <returns>A list containing the split strings.</returns>
        public List<string> GetLines(string text, int width, bool hyphenate = false)
        {
            List<string> lines = new List<string>();
            List<string> words = new List<string>();
            string[] rawWords = text.Split(' ', '-');

            foreach (string word in rawWords)
            {
                if (word.Length > width)
                {
                    words.Add(word.Substring(0, width - 1) + "-");
                    words.Add(word.Substring(width - 1));
                }
                else
                {
                    words.Add(word);
                }
            }

            string line = "";
            foreach (string word in words)
            {
                line += word;

                if (line.Length == width)
                {
                    lines.Add(line);
                    line = "";
                }
                else if (line.Trim(' ').Length > width)
                {
                    if (hyphenate)
                    {
                        string remainder = line.Substring(width - 1).TrimStart(' ');
                        if (remainder != word)
                        {
                            line = line.Substring(0, width - 1) + "-";
                            lines.Add(line);
                            line = remainder + " ";
                        }
                        else
                        {
                            line = line.Substring(0, line.Length - word.Length);
                            lines.Add(line);
                            line = word + " ";
                        }
                    }
                    else
                    {
                        line = line.Substring(0, line.Length - word.Length);
                        lines.Add(line);
                        line = word + " ";
                    }
                }
                else
                    line += " ";
            }
            lines.Add(line);

            return lines;
        }

        /// <summary>
        /// Draws a string to the screen
        /// </summary>
        /// <param name="text">The string to draw</param>
        /// <param name="point">The point to begin drawing at</param>
        /// <param name="fgColour">The colour to draw with</param>
        public void DrawString(string text, Point point, ConsoleColor fgColour = ConsoleColor.Gray)
        {
            for (int j = 0; j < text.Length; j++)
            {
                FrameBufferPixel currentPoint = _frameBuffer.RawFrameBuffer[point.X + j, point.Y];
                _frameBuffer.RawFrameBuffer[point.X + j, point.Y] = new FrameBufferPixel() { BackgroundColour = currentPoint.BackgroundColour, ForegroundColour = fgColour, Character = text[j] };
            }
        }
    }
}
