using ConsoleDraw.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class TextArea : Control
    {
        public TextArea()
        {
            BorderThickness = new Thickness(1);
            Text = "";
        }

        private string _text;
        private int _scrollOffset = 0;
        private bool _autoScroll = true;
        private List<string> _lines = new List<string>();
        private bool _textChanged = true;
        private FrameBuffer _textBuffer;
        private FrameBufferGraphics _textGraphics;

        public string Text
        {
            get => _text;

            set
            {
                _text = value;
                _needsUpdate = true;
                _textChanged = true;
            }
        }

        public bool ReadOnly { get; set; }

        public bool AcceptsReturn { get; set; }

        public override void Activate()
        {
            if (!ReadOnly && AcceptsReturn)
                Text += "\n";
            else
                base.Activate();
        }

        public override void KeyPress(ConsoleKeyInfo k)
        {
            base.KeyPress(k);

            switch (k.Key)
            {
                case ConsoleKey.Backspace:
                    if (!ReadOnly)
                    {
                        if (Text.Length > 1)
                            Text = Text.Substring(0, Text.Length - 1);
                        else
                            Text = "";
                    }
                    break;

                case ConsoleKey.UpArrow:
                    if (Math.Max(0, _lines.Count - DrawHeight) > 0)
                    {
                        _autoScroll = false;
                        _scrollOffset = (_scrollOffset - 1).Clamp(0, _lines.Count - DrawHeight);
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (Math.Max(0, _lines.Count - DrawHeight) > 0)
                    {
                        _autoScroll = false;
                        _scrollOffset = (_scrollOffset + 1).Clamp(0, _lines.Count - DrawHeight);

                        if (_scrollOffset == (_lines.Count - DrawHeight))
                            _autoScroll = true;
                    }
                    break;

                case ConsoleKey.PageUp:
                    if (Math.Max(0, _lines.Count - DrawHeight) > 0)
                    {
                        _autoScroll = false;
                        _scrollOffset = (_scrollOffset - (DrawHeight / 2)).Clamp(0, _lines.Count - DrawHeight);
                    }
                    break;

                case ConsoleKey.PageDown:
                    if (Math.Max(0, _lines.Count - DrawHeight) > 0)
                    {
                        _autoScroll = false;
                        _scrollOffset = (_scrollOffset + (DrawHeight / 2)).Clamp(0, _lines.Count - DrawHeight);
                    }
                    break;

                default:
                    if (!ReadOnly)
                        Text += k.KeyChar;
                    break;
            }

        }

        protected override void Draw(FrameBufferGraphics graph)
        {
            if (_textChanged)
            {
                _lines.Clear();
                IEnumerable<string> linesToDraw = Text.Trim().Replace("\r\n", "\n").Split('\n');
                for (int i = 0; i < linesToDraw.Count(); i++)
                {
                    string line = linesToDraw.ElementAt(i);
                    if (line.Length > DrawWidth)
                    {
                        string cur = line;
                        while (cur.Length > DrawWidth)
                        {
                            _lines.Add(cur.Substring(0, DrawWidth));
                            cur = cur.Substring(DrawWidth);
                        }

                        _lines.Add(cur);
                    }
                    else
                        _lines.Add(line);
                }

                if(_textBuffer == null)
                {
                    _textBuffer = new FrameBuffer(DrawWidth, _lines.Count);
                    _textGraphics = new FrameBufferGraphics(_textBuffer);
                }
                else
                {
                    _textBuffer.Resize(DrawWidth, _lines.Count);
                }

                _textGraphics.Clear(BackgroundColour);

                for (int i = 0; i < _lines.Count; i++)
                {
                    _textGraphics.DrawString(_lines[i], new Point(0, i));
                }
            }

            _scrollOffset = _autoScroll ? Math.Max(0, _lines.Count - DrawHeight) : _scrollOffset;

            graph.Copy(
                _textBuffer,
                new Rectangle(0, _scrollOffset, DrawWidth, Math.Min(_textBuffer.Height, DrawHeight)),
                graph.FrameBuffer,
                new Rectangle(BorderThickness.Left + Padding.Left, BorderThickness.Top + Padding.Top, DrawWidth, DrawHeight));
        }

        public void WriteLine(string text)
        {
            Text += $"{text}\n";
            _needsUpdate = true;
            _textChanged = true;
        }
    }
}
