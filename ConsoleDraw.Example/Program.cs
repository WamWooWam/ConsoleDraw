using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ConsoleDraw;
using ConsoleDraw.Extensions;
using ConsoleDraw.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ConsoleDraw.Example
{
    class Program
    {
        static UIExtension _ui;

        static void Main(string[] args)
        {
            FrameBuffer buffer = new FrameBuffer(Console.WindowWidth, Console.WindowHeight)
            {
                FrameLimit = 60
            };

            FrameBufferGraphics graphics = new FrameBufferGraphics(buffer);
            _ui = new UIExtension(buffer, graphics);
            buffer.AddDrawExtension(_ui);
            buffer.AddDrawExtension(new DebugExtension(buffer, graphics));

            ImageBox imageBox = new ImageBox()
            {
                Width = 32,
                Height = 32,
                X = 3,
                Y = 3,
                Image = Image.Load<Rgb24>(@"C:\Users\wamwo\Downloads\394504330683744257.gif")
            };

            _ui.BasePanel.Controls.Add(imageBox);

            buffer.Run();
           // _ui.BeginEventLoop();
        }

        private static void Button_Activated(object sender, EventArgs e)
        {
            (sender as Button).Text = "You clicked me!";
        }
    }
}
