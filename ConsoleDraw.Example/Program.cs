using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ConsoleDraw;
using ConsoleDraw.Extensions;
using ConsoleDraw.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ConsoleDraw.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            FrameBuffer buffer = new FrameBuffer(Console.WindowWidth, Console.WindowHeight)
            {
                FrameLimit = 60
            };

            FrameBufferGraphics graphics = new FrameBufferGraphics(buffer);
            buffer.Run();

            UIExtension ext = new UIExtension(buffer);
            ImageBox imageBox = new ImageBox()
            {
                Image = Image.Load<Rgb24>(@"C:\Users\wamwo\Downloads\congaparrot.gif"),
                Width = 32,
                Height = 32
            };
            ext.Controls.Add(imageBox);
            ImageBox imageBox2 = new ImageBox()
            {
                Image = Image.Load<Rgb24>(@"C:\Users\wamwo\Downloads\congaparrot.gif"),
                Width = 32,
                Height = 32,
                X = 32
            };
            ext.Controls.Add(imageBox2);
            ImageBox imageBox3 = new ImageBox()
            {
                Image = Image.Load<Rgb24>(@"C:\Users\wamwo\Downloads\congaparrot.gif"),
                Width = 32,
                Height = 32,
                X = 64
            };
            ext.Controls.Add(imageBox3);
            ImageBox imageBox4 = new ImageBox()
            {
                Image = Image.Load<Rgb24>(@"C:\Users\wamwo\Downloads\congaparrot.gif"),
                Width = 32,
                Height = 32,
                X = 96
            };
            ext.Controls.Add(imageBox4);
            buffer.AddDrawExtension(ext);
        }
    }
}
