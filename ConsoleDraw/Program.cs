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
using ConsoleDraw.Properties;
using ConsoleDraw.Tools;

namespace ConsoleDraw
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && File.Exists(args[0]))
            {
                try
                {
                    FrameBuffer buffer = new FrameBuffer();
                    buffer.Init(Console.WindowWidth, Console.WindowHeight);

                    FrameBufferGraphics graph = new FrameBufferGraphics();
                    graph.Init(buffer);

                    //buffer.AddDrawExtension(new DebugExtension(buffer, graph));

                    buffer.Run();

                    graph.PseudoGraphics = false;
                    Image img = Image.FromFile(args[0]);

                    if (img.Width > buffer.Width || img.Height > buffer.Height)
                        img = img.GetThumbnailImage(
                            (int)(((double)img.Width / (double)img.Height) * buffer.Width)
                                .Clamp(1, buffer.Width),
                            (int)(((double)img.Height / (double)img.Width) * buffer.Height)
                                .Clamp(1, buffer.Height),
                            null,
                            IntPtr.Zero);

                    graph.DrawImage(img, new Point(0, 0));

                    Console.ReadKey(true);
                    buffer.Dispose();
                }
                catch
                {
                    Console.WriteLine("That image didn't work! Sorry!");
                }
            }
            else
            {
                Console.WriteLine("Choose an image/framedump dumbass!");
            }
        }
    }
}
