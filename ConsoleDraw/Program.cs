using ConsoleDraw.Data;
using ConsoleDraw.Extensions;
using ConsoleDraw.UI;
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

            //buffer.AddDrawExtension(new DebugExtension(buffer, graph));

            buffer.Run();

            Thread.Sleep(3000);

            graph.DrawRect(new Rectangle(0, 0, buffer.Width, buffer.Height), ConsoleColor.Red);
            graph.DrawRect(new Rectangle(1, 1, buffer.Width - 2, buffer.Height - 2), ConsoleColor.Blue);
            graph.DrawRect(new Rectangle(2, 2, buffer.Width - 4, buffer.Height - 4), ConsoleColor.Yellow);
            graph.DrawRect(new Rectangle(3, 3, buffer.Width - 6, buffer.Height - 6), ConsoleColor.Black);

            int StrLength = "Draw Example".Length;
            int Offset = (buffer.Width - StrLength) / 2;
            graph.DrawString("Draw Example", new Point(Offset, 1), ConsoleColor.White);

            Button button = new Button()
            {
                BackgroundColour = ConsoleColor.Gray,
                ForegroundColour = ConsoleColor.White,
                Text = "Button 1"
            };
            button.Create(new Rectangle(4, 4, 18, 3));

            button.Draw(graph);

            Button button2 = new Button()
            {
                BackgroundColour = ConsoleColor.White,
                ForegroundColour = ConsoleColor.Gray,
                Text = "Button 2"
            };
            button2.Create(new Rectangle(buffer.Width - 4 - 18, 4, 18, 3));

            button2.Draw(graph);

            Console.ReadKey(true);
            buffer.Dispose();
        }        
    }
}
