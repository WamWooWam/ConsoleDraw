using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsoleDraw.Tools
{
    public static class ColourTools
    {
        private static Color[] RGBDosColors =
        {
            Color.FromArgb(0,0,0),
            Color.FromArgb(0,0,0xa8),
            Color.FromArgb(0,0xa8,0),
            Color.FromArgb(0,0xa8,0xa8),
            Color.FromArgb(0xa8,0,0),
            Color.FromArgb(0xa8,0,0xa8),
            Color.FromArgb(0xa8,0xa8,0),
            Color.FromArgb(0xa8,0xa8,0xa8),
            Color.FromArgb(0x54,0x54,0x54),
            Color.FromArgb(0x54,0x54,0xff),
            Color.FromArgb(0x54,0xff,0x54),
            Color.FromArgb(0x54,0xff,0xff),
            Color.FromArgb(0xff,0x54,0x54),
            Color.FromArgb(0xff,0x54,0xff),
            Color.FromArgb(0xff,0xff,0x54),
            Color.FromArgb(255,255,255)
        };

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

        public static byte[] ImageTo4Bit(Bitmap bmp)
        {
            int j = 0;
            byte[] buffer = new byte[bmp.Width * bmp.Height];
            for (int y = 0; bmp.Height > y; y++)
                for (int x = 0; bmp.Width > x; x++, j++)
                    buffer[j] = (byte)NearestColorIndex(bmp.GetPixel(x, y), RGBDosColors);
            return buffer;
        }
    }
}
