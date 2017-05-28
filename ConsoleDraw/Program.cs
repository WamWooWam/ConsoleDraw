using ConsoleDraw.Data;
using ConsoleDraw.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleDraw
{
    class Program
    {        
        static void Main(string[] args)
        {
            FrameBuffer buffer = new FrameBuffer();
            buffer.Init(Console.WindowWidth, Console.WindowHeight);

            FrameBufferGraphics graph = new FrameBufferGraphics();
            graph.Init(buffer);

            buffer.AddDrawExtension(new DebugExtension(buffer, graph));

            Random rand = new Random();
            for (int i = 0; i < buffer.RawPixels.Length; i++)
            {
                buffer.RawPixels[i] = new FrameBufferPixel() { BackgroundColour = ConsoleColor.Black};
            }

            buffer.Run();

            Thread.Sleep(3000);

            graph.DrawRect(new Rectangle(2, 2, 20, 20), ConsoleColor.Red);
            graph.DrawRect(new Rectangle(3, 3, 20, 20), ConsoleColor.Blue);
            graph.DrawRect(new Rectangle(4, 4, 20, 20), ConsoleColor.Cyan);
            graph.DrawRect(new Rectangle(5, 5, 20, 20), ConsoleColor.Green);            
            graph.DrawRect(new Rectangle(6, 6, 20, 20), ConsoleColor.Magenta);
            graph.DrawRect(new Rectangle(7, 7, 20, 20), ConsoleColor.Yellow);

            Console.ReadKey(true);
            buffer.Dispose();
        }        
    }
}
