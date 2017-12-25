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

        public string Text
        {
            get => _text;

            set
            {
                _text = value;
                _needsUpdate = true;
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
            if (!ReadOnly)
            {
                if (k.Key == ConsoleKey.Backspace)
                {
                    if (Text.Length > 1)
                        Text = Text.Substring(0, Text.Length - 1);
                    else
                        Text = "";
                }
                else
                {
                    Text += k.KeyChar;
                }
            }
        }

        protected override void Draw(FrameBufferGraphics graph)
        {
            List<string> lines = new List<string>();

            foreach (string line in Text.Trim().Replace("\r\n", "\n").Split('\n').Select(t => t.Trim()))
            {
                if (line.Length > DrawWidth)
                {
                    string cur = line;
                    while (cur.Length > DrawWidth)
                    {
                        lines.Add(cur.Substring(0, DrawWidth));
                        cur = cur.Substring(DrawWidth);
                    }
                    lines.Add(cur);
                }
                else
                    lines.Add(line);
            }

            IEnumerable<string> preparedLines = lines.Skip(Math.Max(0, lines.Count - DrawHeight)).Select(t => t.Trim());
            for (int i = 0; i < preparedLines.Count(); i++)
            {
                graph.DrawString(preparedLines.ElementAt(i), new Point(BorderThickness.Left, i + BorderThickness.Top), ForegroundColour);
            }

        }

        public void WriteLine(string text)
        {
            Text += $"{text}\n";
            _needsUpdate = true;
        }
    }
}
