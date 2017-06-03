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
        private FrameBufferPixel[] _fbToModify;

        /// <summary>
        /// Initialises a Graphics object based on a framebuffer
        /// </summary>
        /// <param name="frameBuffer">The frame buffer.</param>
        public void Init(FrameBuffer frameBuffer)
        {
            _frameBuffer = frameBuffer;
            _fbToModify = new FrameBufferPixel[_frameBuffer.Width * _frameBuffer.Height];
        }

        /// <summary>
        /// Clears the framebuffer, filling it with a specific colour
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
            _frameBuffer.RawFrameBuffer.CopyTo(_fbToModify, 0);

            int intialPoint = (rect.X) + (rect.Y * _frameBuffer.Width);
            for (int i = 0; i < rect.Height; i++)
            {
                for (int j = 0; j < rect.Width; j++)
                {
                    int newPoint = j + (i * _frameBuffer.Width) + intialPoint;
                    if (newPoint < _fbToModify.Length)
                    {
                        FrameBufferPixel pixel = new FrameBufferPixel() { BackgroundColour = bgColour, ForegroundColour = fgColour, Character = character };
                        _fbToModify[newPoint] = pixel;
                    }
                }
            }

            _frameBuffer.RawFrameBuffer = _fbToModify;
        }

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
            _frameBuffer.RawFrameBuffer.CopyTo(_fbToModify, 0);

            double angle = 0.0;
            double anglestepsize = 0.008;

            while (angle < 2 * Math.PI)
            {
                int x1 = (int)(center.X + xRadius * Math.Cos(angle));
                int y1 = (int)(center.Y + yRadius * Math.Sin(angle));

                angle += anglestepsize;

                int newPoint = x1 + (y1 * _frameBuffer.Width);
                if (newPoint >= 0 && newPoint < _fbToModify.Length)
                {
                    FrameBufferPixel pixel = new FrameBufferPixel() { BackgroundColour = bgColour, ForegroundColour = fgColour, Character = character };
                    _fbToModify[newPoint] = pixel;
                }
            }

            _frameBuffer.RawFrameBuffer = _fbToModify;
        }

        /// <summary>
        /// Draws an ellipse. Thanks to 0x3F!
        /// </summary>
        /// <param name="center">The center point of the ellipse</param>
        /// <param name="xRadius">The radius of the ellipse on the X axis</param>
        /// <param name="yRadius">The radius of the ellipse on the Y axis</param>
        /// <param name="bgColour">The background colour</param>
        /// <param name="fgColour">The foreground colour</param>
        /// <param name="character">The character to draw with</param>
        public void FillEllipse(Point center, int xRadius, int yRadius, ConsoleColor bgColour, ConsoleColor fgColour = ConsoleColor.Gray, char character = ' ')
        {
            throw new NotImplementedException();
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
            int intialPoint = (point.X) + (point.Y * _frameBuffer.Width);
            for (int j = 0; j < text.Length; j++)
            {
                int newPoint = j + intialPoint;
                FrameBufferPixel currentPoint = _frameBuffer.RawFrameBuffer[newPoint];
                _frameBuffer.RawFrameBuffer[newPoint] = new FrameBufferPixel() { BackgroundColour = currentPoint.BackgroundColour, ForegroundColour = fgColour, Character = text[j] };
            }
        }
    }
}
