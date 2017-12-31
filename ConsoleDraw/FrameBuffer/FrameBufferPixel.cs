using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleDraw
{
    /// <summary>
    /// Storage for data required for each individual pixels.
    /// </summary>
    public class FrameBufferPixel
    {
        /// <summary>
        /// The character to draw with
        /// </summary>
        public char Character;

        /// <summary>
        /// The foreground colour of the pixel
        /// </summary>
        public ConsoleColor ForegroundColour;

        /// <summary>
        /// The background colour of the pixel
        /// </summary>
        public ConsoleColor BackgroundColour;

        /// <summary>
        /// Creates a <see cref="FrameBufferPixel"/> from a <see cref="byte"/>
        /// </summary>
        /// <param name="b">The byte to create from</param>
        /// <returns>A <see cref="FrameBufferPixel"/></returns>
        public static FrameBufferPixel FromByte(byte b) => new FrameBufferPixel() { BackgroundColour = (ConsoleColor)b, ForegroundColour = (ConsoleColor)b };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool NotEqual(FrameBufferPixel pix1, FrameBufferPixel pix2)
        {
            return pix1 != null ? pix1.Character != pix2.Character || pix1.BackgroundColour != pix2.BackgroundColour || pix1.ForegroundColour != pix2.ForegroundColour : true;
        }
    }
}
