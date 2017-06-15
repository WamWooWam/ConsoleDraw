using ConsoleDraw.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

        public DebugExtension(FrameBuffer buffer, FrameBufferGraphics graphics)
        {
            _buffer = buffer;
            _graphics = graphics;
        }

        public override void RunExtension()
        {
            _toDraw = $"Frames Drawn :{_buffer.DrawnFrames}, Draw time: {_buffer.DrawTime.ToString("000")}ms, FPS: {_buffer.DrawFPS.ToString("000")}";

            _graphics.DrawRect(new Rectangle(1, 1, _toDraw.Length, 1), ConsoleColor.White);
            _graphics.DrawString(_toDraw, new Point(1, 1), ConsoleColor.Black);

            _toDraw = $"Allocated RAM: {_process.WorkingSet64 / 1024L}KB";            

            _graphics.DrawRect(new Rectangle(1, 2, _toDraw.Length, 1), ConsoleColor.White);
            _graphics.DrawString(_toDraw, new Point(1, 2), ConsoleColor.Black);
        }
    }
}
