using ConsoleDraw.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ConsoleDraw.Extensions
{
    /// <summary>
    /// An extension that shows framerate and draw time.
    /// </summary>
    public class DebugExtension : IDrawExtension
    {
        private FrameBufferGraphics _graphics;
        private Process _process = Process.GetCurrentProcess();
        string _toDraw;
        int prevDrawTime = 0;
        public DebugExtension(FrameBuffer buffer, FrameBufferGraphics graphics)
        {
            _buffer = buffer;
            _graphics = graphics;
        }

        public override void RunExtension()
        {
            _toDraw = (_buffer.DrawTime - prevDrawTime).ToString("000");
            _graphics.DrawString(_toDraw, new Point(1, 1), ConsoleColor.White);
           
            prevDrawTime = _buffer.DrawTime;
        }
    }
}
