using System;

namespace ConsoleDraw
{
    public struct Color
    {

        public byte R;
        public byte G;
        public byte B;

        public static Color FromRgb(uint color)
        {
            return new Color()
            {
                R = (byte)((color >> 16) & 0xFF),
                G = (byte)((color >> 8) & 0xFF),
                B = (byte)(color & 0xFF)
            };
        }

        public static Color FromArgb(byte r, byte g, byte b)
        {
            return new Color()
            {
                R = r,
                G = g,
                B = b
            };
        }
        
        public static implicit operator System.Drawing.Color(Color c) => System.Drawing.Color.FromArgb(c.R, c.G, c.B);
        public static implicit operator Color(System.Drawing.Color c) => FromArgb(c.R, c.G, c.B);

    }
}