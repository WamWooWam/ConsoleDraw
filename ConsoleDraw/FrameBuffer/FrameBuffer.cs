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
        private int _fbWidth = 0;
        private int _fbHeight = 0;
        private FrameBufferPixel[] _previousFrameBuffer;
        private FrameBufferPixel[] _drawingFramebuffer;
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
        public bool UseFrameLimiter { get; set; } = true;
        public FrameBufferPixel[] RawFrameBuffer { get; set; }

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

        private int _frameLimitMS = 16;
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
                            if(!enablePseudoGraphics) fb.RawFrameBuffer[x + (y * bmp.Width)] = new FrameBufferPixel()
                            {
                                ForegroundColour = (ConsoleColor)NearestColorIndex(bmp.GetPixel(x, y), RGBDosColors.Keys.ToArray()),
                                BackgroundColour = (ConsoleColor)NearestColorIndex(bmp.GetPixel(x, y), RGBDosColors.Keys.ToArray()),
                                Character = enablePseudoGraphics ? (char)0x2592 : ' '
                            };
                            else fb.RawFrameBuffer[x + (y * bmp.Width)] = RGBDosColors[RGBDosColors.Keys.ToArray()[NearestColorIndex(bmp.GetPixel(x, y), RGBDosColors.Keys.ToArray())]];
                    }
                    catch { }
                }
            }
            return fb;
        }
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
        public static Dictionary<Color, FrameBufferPixel> RGBDosColors = new Dictionary<Color, FrameBufferPixel>();
        public static Dictionary<ConsoleColor, Color> CCC = new Dictionary<ConsoleColor, Color>();
        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        private static double ColorDistance(Color a, Color b) => Math.Sqrt(Math.Pow(a.R - b.R, 2) + Math.Pow(a.G - b.G, 2) + Math.Pow(a.B - b.B, 2));
        private static int NearestColorIndex(Color a, Color[] b)
        {
            int nearest = 0;
            for (int i = 0; i < b.Length; i++)
            {
                if (ColorDistance(a, b[i]) < ColorDistance(a, b[nearest]))
                    nearest = i;
            }
            return nearest;
        }
        static FrameBuffer()
        {
            CCC.Add(0, Color.FromArgb(0, 0, 0));
            CCC.Add((ConsoleColor)1, Color.FromArgb(0, 0, 0xa8));
            CCC.Add((ConsoleColor)2, Color.FromArgb(0, 0xa8, 0));
            CCC.Add((ConsoleColor)3, Color.FromArgb(0, 0xa8, 0xa8));
            CCC.Add((ConsoleColor)4, Color.FromArgb(0xa8, 0, 0));
            CCC.Add((ConsoleColor)5, Color.FromArgb(0xa8, 0, 0xa8));
            CCC.Add((ConsoleColor)6, Color.FromArgb(0xa8, 0xa8, 0));
            CCC.Add((ConsoleColor)7, Color.FromArgb(0xa8, 0xa8, 0xa8));
            CCC.Add((ConsoleColor)8, Color.FromArgb(0x54, 0x54, 0x54));
            CCC.Add((ConsoleColor)9, Color.FromArgb(0x54, 0x54, 0xff));
            CCC.Add((ConsoleColor)10, Color.FromArgb(0x54, 0xff, 0x54));
            CCC.Add((ConsoleColor)11, Color.FromArgb(0x54, 0xff, 0xff));
            CCC.Add((ConsoleColor)12, Color.FromArgb(0xff, 0x54, 0x54));
            CCC.Add((ConsoleColor)13, Color.FromArgb(0xff, 0x54, 0xff));
            CCC.Add((ConsoleColor)14, Color.FromArgb(0xff, 0xff, 0x54));
            CCC.Add((ConsoleColor)15, Color.FromArgb(255, 255, 255));
            for (int g = 0; 16 > g; g++)
                for (int r = 0; 16 > r; r++)
                    try { RGBDosColors.Add(Blend(CCC[(ConsoleColor)r], CCC[(ConsoleColor)g], 0.45), new FrameBufferPixel { ForegroundColour = (ConsoleColor)r, BackgroundColour = (ConsoleColor)g, Character = (char)0x2592 }); } catch { }
        }
    }
}
