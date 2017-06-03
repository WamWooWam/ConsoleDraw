using ConsoleDraw.Data;
using ConsoleDraw.Extensions;
using ConsoleDraw.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace ConsoleDraw
{
    static class Program
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        static void Main(string[] args)
        {
            if (args.Length == 1 && File.Exists(args[0]))
            {
                FrameBuffer buffer = new FrameBuffer();
                buffer.Init(Console.WindowWidth, Console.WindowHeight);
                buffer.RawFrameBuffer = JsonConvert.DeserializeObject<FrameBufferPixel[]>(File.ReadAllText(args[0]));
                buffer.Draw();

                try
                {
                    DateTime dt = new DateTime(long.Parse(Path.GetFileNameWithoutExtension(args[0])));
                    Console.Title = $"Framedump from {dt.ToLongDateString()} - {dt.ToLongTimeString()}";
                }
                catch { }

                Console.CursorVisible = false;
                Console.ReadKey(true);
                buffer.Dispose();
            }
            else
            {
                FrameBuffer buffer = new FrameBuffer();
                buffer.Init(Console.WindowWidth, Console.WindowHeight);

                //buffer.UseFrameLimiter = false;

                FrameBufferGraphics graph = new FrameBufferGraphics();
                graph.Init(buffer);

                buffer.AddDrawExtension(new DebugExtension(buffer, graph));
                buffer.AddDrawExtension(new FrameDumpExtension(buffer));

                buffer.Run();

                Thread.Sleep(1000);

                graph.DrawRect(new Rectangle(1, 3, 25, 25), ConsoleColor.Blue);
                Thread.Sleep(100);
                graph.DrawRect(new Rectangle(2, 4, 25, 25), ConsoleColor.DarkBlue);
                Thread.Sleep(100);
                graph.DrawRect(new Rectangle(3, 5, 25, 25), ConsoleColor.Red);
                Thread.Sleep(100);
                graph.DrawRect(new Rectangle(4, 6, 25, 25), ConsoleColor.DarkRed);
                Thread.Sleep(100);
                graph.DrawRect(new Rectangle(5, 7, 25, 25), ConsoleColor.Yellow);
                Thread.Sleep(100);
                graph.DrawRect(new Rectangle(6, 8, 25, 25), ConsoleColor.DarkYellow);
                Thread.Sleep(100);
                graph.DrawRect(new Rectangle(7, 9, 25, 25), ConsoleColor.Green);
                Thread.Sleep(100);
                graph.DrawRect(new Rectangle(8, 10, 25, 25), ConsoleColor.DarkGreen);

                Thread.Sleep(1000);

                int totalLines =
                    graph.GetLines("This is a very long string that should be wrapped onto multiple lines using hyphens where appropriate.", 50, true).Count +
                    graph.GetLines("Drawing text blocks like these is easy and requires only one function call to do.", 50, true).Count +
                    graph.GetLines("You can also change the foreground colour of the text, if that's your kind of thing", 50, false).Count + 2;

                graph.DrawRect(new Rectangle(buffer.Width - 54, 3, 52, totalLines + 2), ConsoleColor.Blue);
                graph.DrawRect(new Rectangle(buffer.Width - 53, 4, 50, totalLines), ConsoleColor.DarkBlue);

                int lines = graph.DrawMultiLineString(
                    "This is a very long string that should be wrapped onto multiple lines using hyphens where appropriate.",
                    new Rectangle(buffer.Width - 53, 4, 50, 6), hyphenate: true);
                int lines2 = graph.DrawMultiLineString(
                    "Drawing text blocks like these is easy and requires only one function call to do.",
                    new Rectangle(buffer.Width - 53, 5 + lines, 50, 6), hyphenate: true);
                graph.DrawMultiLineString(
                    "You can also change the foreground colour of the text, if that's your kind of thing",
                    new Rectangle(buffer.Width - 53, 6 + lines + lines2, 50, 6), ConsoleColor.Red);

                int welcomeLines = graph.GetLines(
                    "Welcome to ConsoleDraw, a library designed to make CLI a whole shit ton easier. Designed by WamWooWam, Bitmaps by Nikitpad.",
                    50,
                    true).Count;

                Thread.Sleep(1000);

                graph.DrawBorder(new Rectangle(2, buffer.Height - welcomeLines - 4, 53, welcomeLines + 2), 1, ConsoleColor.Red);
                graph.DrawRect(new Rectangle(3, buffer.Height - welcomeLines - 3, 51, welcomeLines), ConsoleColor.DarkRed);

                graph.DrawMultiLineString(
                    "Welcome to ConsoleDraw, a library designed to make CLI a whole shit ton easier. Designed by WamWooWam, Bitmaps by Nikitpad. Circles/Ellipses by 0x3F",
                    new Rectangle(3, buffer.Height - 6, 51, 6), ConsoleColor.White, true);

                Random rand = new Random();
                Rectangle rect = new Rectangle(buffer.Width - 54, 3, 52, totalLines + 2);
                graph.FillEllipse(new Point((rect.Width / 2) + rect.X, (rect.Height / 2) + rect.Y + 20), 26, 10, ConsoleColor.Blue);

                //while (true)
                //{
                //    graph.DrawRect(new Rectangle(rand.Next(buffer.Width), rand.Next(buffer.Height), 10, 10), (ConsoleColor)rand.Next(1, 16));
                //    Thread.Sleep(buffer.DrawTime);
                //}

                Console.ReadKey(true);
                buffer.Dispose();
            }
        }
    }
}
