using ConsoleDraw.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDraw.Data;
using System.Drawing;

namespace ConsoleDraw.Extensions
{
    public class DebugExtension : IDrawExtension
    {
        private FrameBufferGraphics _graphics;

        public DebugExtension(FrameBuffer buffer, FrameBufferGraphics graphics)
        {
            _buffer = buffer;
            _graphics = graphics;
        }

        public override void RunExtension()
        {
            string toDraw = $"Draw time: {_buffer.DrawTime}ms, FPS: {_buffer.DrawFPS}";

            _graphics.DrawRect(new Rectangle(1, 1, toDraw.Length, 1), ConsoleColor.White);
            _graphics.DrawString(toDraw, new Point(1, 1), ConsoleColor.Black);
        }
    }
}
