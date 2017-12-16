using ConsoleDraw.Interfaces;
using ConsoleDraw.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;


#if NET35 || NET40 || NET452
using System.Drawing;
#endif

namespace ConsoleDraw
{
    public class FrameBuffer : IDisposable
    {
        private int _fbWidth = 0; // The internal frame buffer width
        private int _fbHeight = 0; // The internal frame buffer height
        private FrameBufferPixel[,] _previousFrameBuffer; // Storage for the previously drawn framebuffer, aids performance
        private FrameBufferPixel[,] _rawFrameBuffer; // The internal raw buffer
        private Thread _drawThread; // The thread we're using to draw
        private Stopwatch _watch = new Stopwatch(); // A stopwatch. For draw time.
        private List<IDrawExtension> _drawExtensions = new List<IDrawExtension>(); // Any loaded draw extensions

        public bool Running => _running;

        public event EventHandler FrameDrawn;
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
        /// The raw <see cref="FrameBufferPixel"/>s. You can, but shouldn't directly draw to this.
        /// Use <see cref="FrameBufferGraphics"/> instead/
        /// </summary>
        public FrameBufferPixel[,] RawFrameBuffer { get => _rawFrameBuffer; set => _rawFrameBuffer = value; }

        private int _frameLimitMS = 16; // Internal frame limit in milliseconds

        private bool _running = false;

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

        public FrameBuffer(int width, int height)
        {
            _fbWidth = width; _fbHeight = height;
            _rawFrameBuffer = new FrameBufferPixel[_fbWidth, _fbHeight];

            for (int x = 0; x < _fbWidth; x++)
            {
                for (int y = 0; y < _fbHeight; y++)
                {
                    _rawFrameBuffer[x, y] = new FrameBufferPixel() { BackgroundColour = ConsoleColor.Black, Character = ' ' };
                }
            }

            _previousFrameBuffer = new FrameBufferPixel[_fbWidth, _fbHeight];
        }               

        public void Run()
        {
            Console.CursorVisible = false;
            _running = true;
            _drawThread = new Thread(() =>
            {
                while (_running)
                {
                    Draw();
                }
            });
            _drawThread.Start();
        }

        public void Stop()
        {
            Console.CursorVisible = true;
            _running = false;
        }

        public void Draw()
        {
            _watch.Start();

            foreach (IDrawExtension draw in _drawExtensions)
                draw.RunExtension();

            FrameBufferPixel pix;

            for (int y = 0; y < _fbHeight; y++)
            {
                for (int x = 0; x < _fbWidth; x++)
                {
                    if (!FrameBufferPixel.Equal(_previousFrameBuffer[x, y], _rawFrameBuffer[x, y]))
                    {
                        pix = _rawFrameBuffer[x, y];

                        Console.SetCursorPosition(x, y);
                        Console.BackgroundColor = pix.BackgroundColour;
                        Console.ForegroundColor = pix.ForegroundColour;
                        Console.Write(pix.Character);

                        _previousFrameBuffer[x, y] = pix;
                    }
                }
            }

            if (UseFrameLimiter)
            {
                if (_frameLimitMS - _watch.ElapsedMilliseconds > 0)
                    Thread.Sleep(_frameLimitMS - (int)_watch.ElapsedMilliseconds);
            }


            DrawTime = (int)_watch.ElapsedMilliseconds;
            DrawFPS += 1000 / (int)_watch.ElapsedMilliseconds;
            DrawFPS /= 2;
            DrawnFrames += 1;

            FrameDrawn?.Invoke(this, null);

            _watch.Reset();
        }

        public void AddDrawExtension(IDrawExtension extension)
        {
            _drawExtensions.Add(extension);
        }

        public void Dispose()
        {
            _running = false;
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Gets a copy of the current raw framebuffer
        /// </summary>
        /// <param name="dest">Output array</param>
        internal void GetFramebufferCopy(FrameBufferPixel[,] dest)
        {
            for (int x = 0; x < _fbWidth; x++)
            {
                for (int y = 0; y < _fbHeight; y++)
                {
                    dest[x, y] = _rawFrameBuffer[x, y];
                }
            }
        }

#if NET35 || NET40 || NET452
        /// <summary>
        /// Copies a bitmap to a framebuffer
        /// </summary>
        /// <param name="bmp">Bitmap to copy</param>
        /// <param name="point">Point to xopy to (x,y)</param>
        /// <param name="frameBuffer">The raw frame buffer to copy to</param>
        /// <param name="pseudoGraphics">Enable or disable pseudo graphics</param>
        internal static void InternalBitmapToFramebuffer(System.Drawing.Bitmap bmp, Point point, FrameBufferPixel[,] frameBuffer, bool pseudoGraphics = true)
        {
            for (int x = 0; bmp.Width > x; x++)
            {
                for (int y = 0; bmp.Height > y; y++)
                {
                    try
                    {
                        System.Drawing.Color col = bmp.GetPixel(x, y);
                        if (!pseudoGraphics)
                        {
                            frameBuffer[x + point.X, y + point.Y] = new FrameBufferPixel()
                            {
                                ForegroundColour = (ConsoleColor)ColourTools.NearestColorIndex(col, ColourTools.RGBDosColors.Keys.ToArray()),
                                BackgroundColour = (ConsoleColor)ColourTools.NearestColorIndex(col, ColourTools.RGBDosColors.Keys.ToArray()),
                                Character = ' '
                            };
                        }
                        else
                        {
                            frameBuffer[x + point.X, y + point.Y] = ColourTools.RGBDosColors[ColourTools.RGBDosColors.Keys.ToArray()[ColourTools.NearestColorIndex(col, ColourTools.RGBDosColors.Keys.ToArray())]];
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Loads a <see cref="FrameBuffer"/> from a file.
        /// Should not be used to display images on an existing <see cref="FrameBuffer"/>. See <see cref="FrameBufferGraphics.DrawImage(Image, Point)"/>
        /// </summary>
        /// <param name="path">Path to the image to draw</param>
        /// <param name="pseudoGraphics">Turns colour approximation on or off</param>
        /// <returns>A <see cref="FrameBuffer"/> created from the <paramref name="path"/>.</returns>
        public static FrameBuffer FromFile(string path, bool pseudoGraphics = false)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            FrameBuffer fb = new FrameBuffer(bmp.Width, bmp.Height);

            InternalBitmapToFramebuffer(bmp, new Point(0, 0), fb._rawFrameBuffer, pseudoGraphics);

            return fb;
        }
#endif
    }
}
