using ConsoleDraw.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleDraw.Data
{
    public class FrameBuffer : IDisposable
    {
        private int _fbWidth = 0;
        private int _fbHeight = 0;
        private FrameBufferPixel[] _frameBuffer;
        private Thread _drawThread;
        private Stopwatch _watch = new Stopwatch();
        private List<IDrawExtension> _drawExtensions = new List<IDrawExtension>();

        public int Width => _fbWidth;
        public int Height => _fbHeight;
        public FrameBufferPixel[] RawPixels => _frameBuffer;
        public int DrawTime { get; private set; }
        public int DrawFPS { get; private set; }


        public void Init(int width, int height)
        {
            _fbWidth = width; _fbHeight = height;
            _frameBuffer = new FrameBufferPixel[_fbWidth * _fbHeight];
            for (int i = 0; i < _frameBuffer.Length; i++)
            {
                _frameBuffer[i] = new FrameBufferPixel() { BackgroundColour = ConsoleColor.Black, Character = ' ' };
            }
        }

        public void Run()
        {
            _drawThread = new Thread(() => { while (true) { Draw(); } });
            _drawThread.Start();
        }

        public void Draw()
        {
            _watch.Start();

            foreach (IDrawExtension draw in _drawExtensions)
                draw.RunExtension();

            for (int i = 0; i < _frameBuffer.Length; i++)
            {
                Console.BackgroundColor = _frameBuffer[i].BackgroundColour;
                Console.ForegroundColor = _frameBuffer[i].ForegroundColour;
                Console.Write(_frameBuffer[i].Character);
            }

            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            
            try
            {
                DrawTime = (int)_watch.ElapsedMilliseconds;
                DrawFPS = 1000 / (int)_watch.ElapsedMilliseconds;
            }
            catch { }


            _watch.Reset();
        }

        public void AddDrawExtension(IDrawExtension extension)
        {
            _drawExtensions.Add(extension);
        }

        public void Dispose()
        {
            if ((_drawThread?.IsAlive).GetValueOrDefault())
                _drawThread.Abort();
        }
    }
}
