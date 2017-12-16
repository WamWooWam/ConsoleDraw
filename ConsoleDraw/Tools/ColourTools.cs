using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.Tools
{
    public static class ColourTools
    {
        static ColourTools()
        {
            CCC = new Dictionary<ConsoleColor, Color>
            {
                { 0, Color.FromArgb(0, 0, 0) },
                { (ConsoleColor)1, Color.FromArgb(0, 0, 0xa8) },
                { (ConsoleColor)2, Color.FromArgb(0, 0xa8, 0) },
                { (ConsoleColor)3, Color.FromArgb(0, 0xa8, 0xa8) },
                { (ConsoleColor)4, Color.FromArgb(0xa8, 0, 0) },
                { (ConsoleColor)5, Color.FromArgb(0xa8, 0, 0xa8) },
                { (ConsoleColor)6, Color.FromArgb(0xa8, 0xa8, 0) },
                { (ConsoleColor)7, Color.FromArgb(0xa8, 0xa8, 0xa8) },
                { (ConsoleColor)8, Color.FromArgb(0x54, 0x54, 0x54) },
                { (ConsoleColor)9, Color.FromArgb(0x54, 0x54, 0xff) },
                { (ConsoleColor)10, Color.FromArgb(0x54, 0xff, 0x54) },
                { (ConsoleColor)11, Color.FromArgb(0x54, 0xff, 0xff) },
                { (ConsoleColor)12, Color.FromArgb(0xff, 0x54, 0x54) },
                { (ConsoleColor)13, Color.FromArgb(0xff, 0x54, 0xff) },
                { (ConsoleColor)14, Color.FromArgb(0xff, 0xff, 0x54) },
                { (ConsoleColor)15, Color.FromArgb(255, 255, 255) }
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

        public static double ColorDistance(Color a, Color b) => Math.Sqrt(((a.R - b.R) * (a.R - b.R)) + ((a.G - b.G) * (a.G - b.G)) + ((a.B - b.B) * (a.B - b.B)));

        public static int NearestColorIndex(Color a, Color[] b)
        {
            int nearest = 0;
            for (int i = 0; i < b.Length; i++)
            {
                if (ColorDistance(a, b[i]) < ColorDistance(a, b[nearest]))
                    nearest = i;
            }
            return nearest;
        }
    }
}
