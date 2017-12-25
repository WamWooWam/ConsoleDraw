using ConsoleDraw.Interfaces;
using ConsoleDraw.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
#if !NET35
#endif

#if NET35 || NET40 || NET452 || NET461
using System.Drawing;
#endif

#if NET452 || NET461 || NETSTANDARD2_0
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        bool _initialDraw = true;
        ConsoleColor foreground = Console.ForegroundColor;
        ConsoleColor background = Console.BackgroundColor;

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

                        if (pix.BackgroundColour != background)
                        {
                            Console.BackgroundColor = pix.BackgroundColour;
                            background = pix.BackgroundColour;
                        }

                        if (pix.ForegroundColour != foreground)
                        {
                            Console.ForegroundColor = pix.ForegroundColour;
                            foreground = pix.ForegroundColour;
                        }

                        Console.Write(pix.Character);

                        _previousFrameBuffer[x, y] = pix;
                    }
                }
            }

            if (UseFrameLimiter)
            {
                if (_frameLimitMS - _watch.ElapsedMilliseconds > 0)
                    Thread.Sleep(Math.Max(0, _frameLimitMS - (int)_watch.ElapsedMilliseconds));
            }

            if ((int)_watch.ElapsedMilliseconds > 0 && !_initialDraw)
            {
                DrawnFrames += 1;
                DrawTime += (int)_watch.ElapsedMilliseconds;
                DrawFPS = 1000 / (DrawTime / (int)DrawnFrames);
            }

            FrameDrawn?.Invoke(this, null);

            _initialDraw = false;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InternalPixelToFramebuffer<TPixel>(TPixel[] pixel, Point point, FrameBufferPixel[,] frameBuffer, int width, int height, bool pseudoGraphics) where TPixel : struct, IPixel<TPixel>
        {
            Rgb24 col = new Rgb24(0, 0, 0);
            if (!pseudoGraphics)
            {
                for(int x = 0; x < width; x++)
                {
                    IPixel pix;
                    for (int y = 0; height > y; y++)
                    {
                        pix = pixel[x + (y * width)];
                        pix.ToRgb24(ref col);
                        NonPseudoSetColor(point, frameBuffer, col, x, y);
                    }
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    IPixel pix;
                    for (int y = 0; height > y; y++)
                    {
                        pix = pixel[x + (y * width)];
                        pix.ToRgb24(ref col);
                        PseudoSetColor(point, frameBuffer, col, x, y);
                    }
                }
            }
        }

        internal static void InternalImageToFramebuffer<TPixel>(Image<TPixel> image, Point point, FrameBufferPixel[,] frameBuffer, bool pseudoGraphics = true) where TPixel : struct, IPixel<TPixel>
        {
            TPixel[] data = new TPixel[image.Width * image.Height];
            image.SavePixelData(data);
            Rgb24 col = new Rgb24(0, 0, 0);
            int width = image.Width;
            int height = image.Height;
            if (!pseudoGraphics)
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; height > y; y++)
                    {
                        IPixel pix = data[x + (y * width)];
                        pix.ToRgb24(ref col);
                        NonPseudoSetColor(point, frameBuffer, col, x, y);
                    }
                });
            }
            else
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; height > y; y++)
                    {
                        IPixel pix = data[x + (y * width)];
                        pix.ToRgb24(ref col);
                        PseudoSetColor(point, frameBuffer, col, x, y);
                    }
                });
            }
        }

        internal void InternalImageFrameToFramebuffer<TPixel>(ImageFrame<TPixel> image, Point point, FrameBufferPixel[,] frameBuffer, bool pseudoGraphics) where TPixel : struct, IPixel<TPixel>
        {
            TPixel[] data = new TPixel[image.Width * image.Height];
            image.SavePixelData(data);
            Rgb24 col = new Rgb24(0, 0, 0);
            int width = image.Width;
            int height = image.Height;
            if (!pseudoGraphics)
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; height > y; y++)
                    {
                        IPixel pix = data[x + (y * width)];
                        pix.ToRgb24(ref col);
                        NonPseudoSetColor(point, frameBuffer, col, x, y);
                    }
                });
            }
            else
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; height > y; y++)
                    {
                        IPixel pix = data[x + (y * width)];
                        pix.ToRgb24(ref col);
                        PseudoSetColor(point, frameBuffer, col, x, y);
                    }
                });
            }
        }

        /// <summary>
        /// Loads a <see cref="FrameBuffer"/> from a file.
        /// Should not be used to display images on an existing <see cref="FrameBuffer"/>. See <see cref="FrameBufferGraphics.DrawImage(Image{TPixel}, Point)"/>
        /// </summary>
        /// <param name="path">Path to the image to draw</param>
        /// <param name="pseudoGraphics">Turns colour approximation on or off</param>
        /// <returns>A <see cref="FrameBuffer"/> created from the <paramref name="path"/>.</returns>
        public static FrameBuffer FromImage(string path, bool pseudoGraphics = false)
        {
            using (FileStream str = File.OpenRead(path))
            {
                Image<Rgba32> bmp = SixLabors.ImageSharp.Image.Load(str);
                FrameBuffer fb = new FrameBuffer(bmp.Width, bmp.Height);
                InternalImageToFramebuffer(bmp, new Point(0, 0), fb._rawFrameBuffer, pseudoGraphics);
                return fb;
            }
        }

#if NET452 || NET461 
        /// <summary>
        /// Copies a bitmap to a framebuffer
        /// </summary>
        /// <param name="bmp">Bitmap to copy</param>
        /// <param name="point">Point to xopy to (x,y)</param>
        /// <param name="frameBuffer">The raw frame buffer to copy to</param>
        /// <param name="pseudoGraphics">Enable or disable pseudo graphics</param>
        internal static void InternalBitmapToFramebuffer(Bitmap bmp, Point point, FrameBufferPixel[,] frameBuffer, bool pseudoGraphics = true)
        {
            //for (int x = 0; bmp.Width > x; x++)
            //{
            //    for (int y = 0; bmp.Height > y; y++)
            //    {
            //        try
            //        {
            //            System.Drawing.Color col = bmp.GetPixel(x, y);
            //            SetPixelColor(point, frameBuffer, pseudoGraphics, col, x, y);
            //        }
            //        catch { }
            //    }
            //}
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
            Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(path);
            FrameBuffer fb = new FrameBuffer(bmp.Width, bmp.Height);

            InternalBitmapToFramebuffer(bmp, new Point(0, 0), fb._rawFrameBuffer, pseudoGraphics);

            return fb;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetPixelColor(Point point, FrameBufferPixel[,] frameBuffer, bool pseudoGraphics, Rgb24 col, int x, int y)
        {
            if (!pseudoGraphics)
            {
                NonPseudoSetColor(point, frameBuffer, col, x, y);
            }
            else
            {
                PseudoSetColor(point, frameBuffer, col, x, y);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PseudoSetColor(Point point, FrameBufferPixel[,] frameBuffer, Rgb24 col, int x, int y)
        {
            frameBuffer[x + point.X, y + point.Y] = ColourTools.RGBDosColors[ColourTools.RGBDosKeys[ColourTools.NearestColorIndex(col, ColourTools.RGBDosKeys)]];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void NonPseudoSetColor(Point point, FrameBufferPixel[,] frameBuffer, Rgb24 col, int x, int y)
        {
            frameBuffer[x + point.X, y + point.Y] = new FrameBufferPixel()
            {
                ForegroundColour = (ConsoleColor)ColourTools.NearestColorIndex(col, ColourTools.RGBDosKeys),
                BackgroundColour = (ConsoleColor)ColourTools.NearestColorIndex(col, ColourTools.RGBDosKeys),
                Character = ' '
            };
        }
    }
}
