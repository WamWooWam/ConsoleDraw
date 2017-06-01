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
                buffer._frameBuffer = JsonConvert.DeserializeObject<FrameBufferPixel[]>(File.ReadAllText(args[0]));
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

                FrameBufferGraphics graph = new FrameBufferGraphics();
                graph.Init(buffer);

                buffer.AddDrawExtension(new DebugExtension(buffer, graph));
                //buffer.AddDrawExtension(new FrameDumpExtension(buffer));

                buffer.Run();

                Thread.Sleep(3000);

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

                Button button2 = new Button()
                {
                    BackgroundColour = ConsoleColor.White,
                    ForegroundColour = ConsoleColor.Gray,
                    Text = "Button 2"
                };
                button2.Create(new Rectangle(buffer.Width - 4 - 18, 4, 18, 3));

                //while (true)
                //{
                //    //graph.Clear();
                //    graph.DrawRect(new Rectangle(1, 1, 25, 25), ConsoleColor.Blue);
                //    graph.DrawRect(new Rectangle(2, 2, 25, 25), ConsoleColor.DarkBlue);
                //    graph.DrawRect(new Rectangle(3, 3, 25, 25), ConsoleColor.Red);
                //    graph.DrawRect(new Rectangle(4, 4, 25, 25), ConsoleColor.DarkRed);
                //    graph.DrawRect(new Rectangle(5, 5, 25, 25), ConsoleColor.Yellow);
                //    graph.DrawRect(new Rectangle(6, 6, 25, 25), ConsoleColor.DarkYellow);
                //    graph.DrawRect(new Rectangle(7, 7, 25, 25), ConsoleColor.Green);
                //    graph.DrawRect(new Rectangle(8, 8, 25, 25), ConsoleColor.DarkGreen);
                //    //button.Draw(graph);
                //    //button2.Draw(graph);
                //    Thread.Sleep(1000);
                //}

                int totalLines =
                    graph.GetLines("This is a very long string that should be wrapped onto multiple lines using hyphens where appropriate.", 50, true).Count +
                    graph.GetLines("Drawing text blocks like these is easy and requires only one function call to do.", 50, true).Count +
                    graph.GetLines("You can also change the foreground colour of the text, if that's your kind of thing", 50, false).Count + 2;

                graph.DrawRect(new Rectangle(2, 2, 52, totalLines + 2), ConsoleColor.Blue);
                graph.DrawRect(new Rectangle(3, 3, 50, totalLines), ConsoleColor.DarkBlue);

                int lines = graph.DrawMultiLineString(
                    "This is a very long string that should be wrapped onto multiple lines using hyphens where appropriate.",
                    new Rectangle(3, 3, 50, 6), hyphenate: true);
                int lines2 = graph.DrawMultiLineString(
                    "Drawing text blocks like these is easy and requires only one function call to do.",
                    new Rectangle(3, 4 + lines, 50, 6), hyphenate: true);
                graph.DrawMultiLineString(
                    "You can also change the foreground colour of the text, if that's your kind of thing",
                    new Rectangle(3, 5 + lines + lines2, 50, 6), ConsoleColor.Red);

                Random rand = new Random();
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Tick += (object sender, EventArgs e) =>
                {
                    graph.DrawRect(new Rectangle(rand.Next(buffer.Width - 10), rand.Next(buffer.Height - 10), 10, 10), (ConsoleColor)rand.Next(1, 16));
                };
                timer.Interval = 1000;
                timer.Start();

                System.Windows.Forms.Application.Run();

                Console.ReadKey(true);
                buffer.Dispose();
            }
        }
    }
}
