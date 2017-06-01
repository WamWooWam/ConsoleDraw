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
        private FrameBufferPixel[] _previousFrameBuffer;
        private Thread _drawThread;
        private Stopwatch _watch = new Stopwatch();
        private List<IDrawExtension> _drawExtensions = new List<IDrawExtension>();

        public int Width => _fbWidth;
        public int Height => _fbHeight;
        public int X => Console.CursorLeft;
        public int Y => Console.CursorTop;
        public int DrawTime { get; private set; }
        public int DrawFPS { get; private set; }
        public int DrawnFrames { get; private set; }
        public FrameBufferPixel[] _frameBuffer;

        public void Init(int width, int height)
        {
            _fbWidth = width; _fbHeight = height;
            _frameBuffer = new FrameBufferPixel[_fbWidth * _fbHeight];
            for (int i = 0; i < _frameBuffer.Length; i++)
            {
                _frameBuffer[i] = new FrameBufferPixel() { BackgroundColour = ConsoleColor.Black, Character = ' ' };
            }
            _previousFrameBuffer = new FrameBufferPixel[_fbWidth * _fbHeight];
        }

        public void Run()
        {
            Console.CursorVisible = false;
            _drawThread = new Thread(() => { while (true) { Draw(); } });
            _drawThread.Start();
        }

        public void Draw()
        {
            _watch.Start();

            foreach (IDrawExtension draw in _drawExtensions)
                draw.RunExtension();

            if (_previousFrameBuffer != _frameBuffer)
            {
                lock (_frameBuffer)
                {
                    for (int i = 0; i < _fbHeight; i++)
                    {
                        for (int j = 0; j < _fbWidth; j++)
                        {
                            int index = (j + (i * _fbWidth)).Clamp(0, _frameBuffer.Length - 1);
                            if (_frameBuffer[index] != _previousFrameBuffer[index])
                            {
                                try
                                {
                                    Console.CursorLeft = j;
                                    Console.CursorTop = i;                                    
                                    Console.BackgroundColor = _frameBuffer[index].BackgroundColour;
                                    Console.ForegroundColor = _frameBuffer[index].ForegroundColour;
                                    Console.Write(_frameBuffer[index].Character);
                                }
                                catch { }
                            }
                        }
                    }
                }

                Console.CursorLeft = 0;
                Console.CursorTop = 0;

                _previousFrameBuffer = _frameBuffer;

                if (15 - _watch.ElapsedMilliseconds > 0)
                    Thread.Sleep(15 - (int)_watch.ElapsedMilliseconds);

                try
                {
                    DrawTime = (int)_watch.ElapsedMilliseconds;
                    DrawFPS += 1000 / (int)_watch.ElapsedMilliseconds;
                    DrawFPS /= 2;
                    DrawnFrames += 1;
                }
                catch { }
            }
            else
            {
                Thread.Sleep(50);
            }

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
            Console.CursorVisible = true;
        }
    }
}
