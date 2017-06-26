using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsoleDraw.Tools
{
    public static class ColourTools
    {
        public static Dictionary<Color, FrameBufferPixel> RGBDosColors = new Dictionary<Color, FrameBufferPixel>();
        public static Dictionary<ConsoleColor, Color> CCC = new Dictionary<ConsoleColor, Color>();
        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        public static double ColorDistance(Color a, Color b) => Math.Sqrt(Math.Pow(a.R - b.R, 2) + Math.Pow(a.G - b.G, 2) + Math.Pow(a.B - b.B, 2));
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
