using ConsoleDraw.Interfaces;
using ConsoleDraw.Tools;
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
    public class FrameBuffer : IDisposable
    {
        private int _fbWidth = 0; // The internal frame buffer width
        private int _fbHeight = 0; // The internal frame buffer height
        private FrameBufferPixel[] _previousFrameBuffer; // Storage for the previously drawn framebuffer, aids performance
        private FrameBufferPixel[] _drawingFramebuffer; // The framebuffer being drawn.
        private Thread _drawThread; // The thread we're using to draw
        private Stopwatch _watch = new Stopwatch(); // A stopwatch. For draw time.
        private List<IDrawExtension> _drawExtensions = new List<IDrawExtension>(); // Any loaded draw extensions

        /// <summary>
        /// The framebuffer width.
        /// </summary>
        public int Width => _fbWidth;
        /// <summary>
        /// The framebuffer height.
        /// </summary>
        public int Height => _fbHeight;
        /// <summary>
        /// The current cursor X position.
        /// </summary>
        public int X => Console.CursorLeft;
        /// <summary>
        /// The current cursor Y position.
        /// </summary>
        public int Y => Console.CursorTop;

        /// <summary>
        /// The draw time of the previous frame in milliseconds
        /// </summary>
        public int DrawTime { get; private set; }
        /// <summary>
        /// The current draw framerate
        /// </summary>
        public int DrawFPS { get; private set; }
        /// <summary>
        /// A count of frames drawn
        /// </summary>
        public long DrawnFrames { get; private set; }
        /// <summary>
        /// Turns the framelimiter on or off
        /// </summary>
        public bool UseFrameLimiter { get; set; } = true;
        /// <summary>
        /// The raw <see cref="FrameBufferPixel"/>s. You shouldn't directly draw to this.
        /// Use <see cref="FrameBufferGraphics"/> instead.
        /// </summary>
        public FrameBufferPixel[] RawFrameBuffer { get; set; }

        private int _frameLimitMS = 16; // Internal frame limit in milliseconds

        /// <summary>
        /// The frame limit in FPS
        /// </summary>
        public int FrameLimit
        {
            get
            {
                return (_frameLimitMS + 1) * 1000;
            }
            set
            {
                _frameLimitMS = 1000 / (value - 1);
            }
        }

        /// <summary>
        /// Loads a <see cref="FrameBuffer"/> from a file.
        /// Should not be used to display images on an existing <see cref="FrameBuffer"/>. See <see cref="FrameBufferGraphics.DrawImage(Image, Point)"/>
        /// </summary>
        /// <param name="path">Path to the image to draw</param>
        /// <param name="enablePseudoGraphics">Turns colour approximation on or off</param>
        /// <returns>A <see cref="FrameBuffer"/> created from the <paramref name="path"/>.</returns>
        public static FrameBuffer FromFile(string path, bool enablePseudoGraphics = false)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            FrameBuffer fb = new FrameBuffer();
            fb.Init(bmp.Width, bmp.Height);
            for (int x = 0; bmp.Width > x; x++)
            {
                for (int y = 0; bmp.Height > y; y++)
                {
                    try
                    {
                        fb.RawFrameBuffer[x + (y * bmp.Width)] = new FrameBufferPixel()
                        {
                            ForegroundColour = (ConsoleColor)ColourTools.NearestColorIndex(bmp.GetPixel(x, y + (enablePseudoGraphics ? 1 : 0)), ColourTools.RGBDosColors),
                            BackgroundColour = (ConsoleColor)ColourTools.NearestColorIndex(bmp.GetPixel(x, y), ColourTools.RGBDosColors),
                            Character = enablePseudoGraphics ? (char)0x2592 : ' '
                        };
                    }
                    catch { }
                }
            }
            return fb;
        }

        /// <summary>
        /// Sets up a new <see cref="FrameBuffer"/>
        /// </summary>
        /// <param name="width">Width of the new <see cref="FrameBuffer"/></param>
        /// <param name="height">Height of the new <see cref="FrameBuffer"/></param>
        public void Init(int width, int height)
        {
            _fbWidth = width; _fbHeight = height;
            RawFrameBuffer = new FrameBufferPixel[_fbWidth * _fbHeight];
            for (int i = 0; i < RawFrameBuffer.Length; i++)
            {
                RawFrameBuffer[i] = new FrameBufferPixel() { BackgroundColour = ConsoleColor.Black, Character = ' ' };
            }
            _previousFrameBuffer = new FrameBufferPixel[_fbWidth * _fbHeight];
            _drawingFramebuffer = new FrameBufferPixel[_fbWidth * _fbHeight];
        }

        /// <summary>
        /// Starts draw loop
        /// </summary>
        public void Run()
        {
            Console.CursorVisible = false;
            _drawThread = new Thread(() =>
            {
                while (true)
                {
                    Draw();
                }
            });
            _drawThread.Start();
        }

        /// <summary>
        /// Draws the <see cref="FrameBuffer"/> to the console
        /// </summary>
        public void Draw()
        {
            _watch.Start();

            foreach (IDrawExtension draw in _drawExtensions)
                draw.RunExtension();

            lock (RawFrameBuffer)
            {
                RawFrameBuffer.CopyTo(_drawingFramebuffer, 0);
                for (int i = 0; i < _fbHeight; i++)
                {
                    for (int j = 0; j < _fbWidth; j++)
                    {
                        int index = (j + (i * _fbWidth)).Clamp(0, _drawingFramebuffer.Length - 1);
                        if (_drawingFramebuffer[index] != _previousFrameBuffer[index])
                        {
                            try
                            {
                                Console.CursorLeft = j;
                                Console.CursorTop = i;
                                Console.BackgroundColor = _drawingFramebuffer[index].BackgroundColour;
                                Console.ForegroundColor = _drawingFramebuffer[index].ForegroundColour;
                                Console.Write(_drawingFramebuffer[index].Character);
                                _previousFrameBuffer[index] = _drawingFramebuffer[index];
                            }
                            catch { }
                        }
                    }

                }
            }
            Console.CursorLeft = 0;
            Console.CursorTop = 0;


            if (UseFrameLimiter)
            {
                if (_frameLimitMS - _watch.ElapsedMilliseconds > 0)
                    Thread.Sleep(_frameLimitMS - (int)_watch.ElapsedMilliseconds);
            }

            try
            {
                DrawTime = (int)_watch.ElapsedMilliseconds;
                DrawFPS += 1000 / (int)_watch.ElapsedMilliseconds;
                DrawFPS /= 2;
                DrawnFrames += 1;
            }
            catch { }


            _watch.Reset();
        }

        /// <summary>
        /// Adds an <see cref="IDrawExtension"/> to the <see cref="FrameBuffer"/>'s draw pipeline
        /// </summary>
        /// <param name="extension">The <see cref="IDrawExtension"/> to add</param>
        public void AddDrawExtension(IDrawExtension extension)
        {
            _drawExtensions.Add(extension);
        }

        /// <summary>
        /// Stops drawing the <see cref="FrameBuffer"/> and resets the console.
        /// </summary>
        public void Dispose()
        {
            if ((_drawThread?.IsAlive).GetValueOrDefault())
                _drawThread.Abort();
            Console.CursorVisible = true;
        }
    }
}
