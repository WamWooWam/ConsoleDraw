using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleDraw;
using ConsoleDraw.UI;

namespace ConsoleDraw.Example
{
    class Program
    {
        static UIExtension _ui;

        static void Main(string[] args)
        {
            FrameBuffer buffer = new FrameBuffer(Console.WindowWidth, Console.WindowHeight);
            FrameBufferGraphics graphics = new FrameBufferGraphics(buffer);
            _ui = new UIExtension(buffer, graphics);
            buffer.AddDrawExtension(_ui);

            TextArea area = new TextArea
            {
                Text = "This is a really long piece of text that should flow and wrap onto multiple lines\nIt also includes linebreaks of its own n shit.",
                X = 1,
                Y = 1,
                Width = Console.WindowWidth / 2,
                Height = Console.WindowHeight,
                BackgroundColour = ConsoleColor.Blue,
                ForegroundColour = ConsoleColor.White
            };

            _ui.BasePanel.Controls.Add(area);

            buffer.Run();

            int i = 0;
            while (true)
            {
                Thread.Sleep(100);
                area.Text += $"Thing {i}!\n";
                i++;
            }

            //_ui.BeginEventLoop();
        }

        private static void Button_Activated(object sender, EventArgs e)
        {
            (sender as Button).Text = "You clicked me!";
        }
    }
}
