using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class TextArea : Control
    {
        private string _text;
        private bool _needsUpdate = true;
        private FrameBuffer _frameBuffer; // speeeeed
        private FrameBufferGraphics _graphics;

        public string Text
        {
            get => _text;

            set
            {
                _text = value;
                _needsUpdate = true;
            }
        }

        public override void Draw(FrameBufferGraphics graph)
        {
            if (_needsUpdate || _frameBuffer == null || _frameBuffer.Width != Width || _frameBuffer.Height != Height)
            {
                if(_frameBuffer == null || _frameBuffer.Width != Width || _frameBuffer.Height != Height)
                {
                    _frameBuffer = new FrameBuffer(Width, Height);
                    _graphics = new FrameBufferGraphics(_frameBuffer);
                }

                int drawWidth = Width - BorderThickness.LeftRight;
                int drawHeight = Height - BorderThickness.TopBottom;

                _graphics.Clear(BorderColor);
                _graphics.DrawRect(new Rectangle(BorderThickness.Left, BorderThickness.Top, drawWidth, drawHeight), BackgroundColour);
                List<string> lines = new List<string>();

                foreach (string line in Text.Split('\n'))
                {
                    if (line.Length > drawWidth)
                    {
                        string cur = line;
                        while (cur.Length > drawWidth)
                        {
                            lines.Add(cur.Substring(0, drawWidth));
                            cur = cur.Substring(drawWidth);
                        }
                        lines.Add(cur);
                    }
                    else
                        lines.Add(line);
                }

                IEnumerable<string> preparedLines = lines.Skip(Math.Max(0, lines.Count - drawHeight));
                for (int i = 0; i < preparedLines.Count(); i++)
                {
                    _graphics.DrawString(preparedLines.ElementAt(i), new Point(BorderThickness.Left, i + BorderThickness.Top), ForegroundColour);
                }

                _needsUpdate = false;
                graph.DrawBuffer(_frameBuffer, new Point(X, Y));
            }
            else
            {
                graph.DrawBuffer(_frameBuffer, new Point(X, Y));
            }
        }

        public void WriteLine(string text)
        {
            Text += $"{text}\n";
            _needsUpdate = true;
        }
    }
}
