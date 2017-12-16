using System;
using System.Threading.Tasks;
using ConsoleDraw;

namespace ConsoleDraw.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FrameBuffer buffer = new FrameBuffer(Console.WindowWidth, Console.WindowHeight);
            FrameBufferGraphics graphics = new FrameBufferGraphics(buffer);

            graphics.DrawRect(new Rectangle(0, 0, 20, 20), ConsoleColor.Cyan);

            buffer.Draw();

            await Task.Delay(-1);
        }
    }
}
