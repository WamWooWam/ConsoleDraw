using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public char Character { get; set; }
        /// <summary>
        /// The foreground colour of the pixel
        /// </summary>
        public ConsoleColor ForegroundColour { get; set; }
        /// <summary>
        /// The background colour of the pixel
        /// </summary>
        public ConsoleColor BackgroundColour { get; set; }

        /// <summary>
        /// Creates a <see cref="FrameBufferPixel"/> from a <see cref="byte"/>
        /// </summary>
        /// <param name="b">The byte to create from</param>
        /// <returns>A <see cref="FrameBufferPixel"/></returns>
        public static FrameBufferPixel FromByte(byte b) => new FrameBufferPixel() { BackgroundColour = (ConsoleColor)b, ForegroundColour = (ConsoleColor)b };
    }
}
