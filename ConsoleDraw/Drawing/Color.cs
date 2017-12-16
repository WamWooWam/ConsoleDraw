using System;

namespace ConsoleDraw
{
    public struct Color
    {
        byte _r;
        byte _g;
        byte _b;

        public byte R => _r;
        public byte G => _g;
        public byte B => _b;

        public static Color FromRgb(uint color)
        {
            return new Color()
            {
                _r = (byte)((color >> 16) & 0xFF),
                _g = (byte)((color >> 8) & 0xFF),
                _b = (byte)(color & 0xFF)
            };
        }

        public static Color FromArgb(byte r, byte g, byte b)
        {
            return new Color()
            {
                _r = r,
                _g = g,
                _b = b
            };
        }

#if NET35 || NET40 || NET452 || NETSTANDARD2_0
        public static implicit operator System.Drawing.Color(Color c) => System.Drawing.Color.FromArgb(c.R, c.G, c.B);

        public static implicit operator Color(System.Drawing.Color c) => FromArgb(c.R, c.G, c.B);
#endif

    }
}