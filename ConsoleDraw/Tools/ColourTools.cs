using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleDraw.Tools
{
    internal static class ColourTools
    {
        static ColourTools()
        {
            CCC = new Dictionary<ConsoleColor, Rgb24>
            {
                { 0, new Rgb24(0, 0, 0) },
                { (ConsoleColor)1, new Rgb24(0, 0, 0xa8) },
                { (ConsoleColor)2, new Rgb24(0, 0xa8, 0) },
                { (ConsoleColor)3, new Rgb24(0, 0xa8, 0xa8) },
                { (ConsoleColor)4, new Rgb24(0xa8, 0, 0) },
                { (ConsoleColor)5, new Rgb24(0xa8, 0, 0xa8) },
                { (ConsoleColor)6, new Rgb24(0xa8, 0xa8, 0) },
                { (ConsoleColor)7, new Rgb24(0xa8, 0xa8, 0xa8) },
                { (ConsoleColor)8, new Rgb24(0x54, 0x54, 0x54) },
                { (ConsoleColor)9, new Rgb24(0x54, 0x54, 0xff) },
                { (ConsoleColor)10, new Rgb24(0x54, 0xff, 0x54) },
                { (ConsoleColor)11, new Rgb24(0x54, 0xff, 0xff) },
                { (ConsoleColor)12, new Rgb24(0xff, 0x54, 0x54) },
                { (ConsoleColor)13, new Rgb24(0xff, 0x54, 0xff) },
                { (ConsoleColor)14, new Rgb24(0xff, 0xff, 0x54) },
                { (ConsoleColor)15, new Rgb24(255, 255, 255) }
            };

            for (int g = 0; 16 > g; g++)
            {
                for (int r = 0; 16 > r; r++)
                {
                    try
                    {
                        RGBDosColors.Add(
                            Blend(CCC[(ConsoleColor)r], CCC[(ConsoleColor)g], 0.45),
                            new FrameBufferPixel { ForegroundColour = (ConsoleColor)r, BackgroundColour = (ConsoleColor)g, Character = (char)0x2592 }
                            );
                    }
                    catch { }
                }
            }

            RGBDosKeys = RGBDosColors.Keys.ToArray();
            RGBDosKeysCount = RGBDosKeys.Length;
        }

        public static Dictionary<Rgb24, FrameBufferPixel> RGBDosColors = new Dictionary<Rgb24, FrameBufferPixel>();
        public static Dictionary<ConsoleColor, Rgb24> CCC = new Dictionary<ConsoleColor, Rgb24>();
        public static Rgb24[] RGBDosKeys;
        private static int RGBDosKeysCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgb24 Blend(Rgb24 color, Rgb24 backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return new Rgb24(r,g,b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ColorDistance(Rgb24 a, Rgb24 b) => Math.Sqrt(((a.R - b.R) * (a.R - b.R)) + ((a.G - b.G) * (a.G - b.G)) + ((a.B - b.B) * (a.B - b.B)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NearestColorIndex(Rgb24 a)
        {
            int nearest = 0;
            for (int i = 0; i < RGBDosKeysCount; i++)
            {
                if (ColorDistance(a, RGBDosKeys[i]) < ColorDistance(a, RGBDosKeys[nearest]))
                    nearest = i;
            }
            return nearest;
        }
    }
}
