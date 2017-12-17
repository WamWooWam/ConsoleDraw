using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw
{
    public class Thickness
    {
        public Thickness(int uniform)
        {
            Top = uniform;
            Left = uniform;
            Bottom = uniform;
            Right = uniform;
        }

        public Thickness(int topBottom, int leftRight)
        {
            Top = topBottom;
            Bottom = topBottom;
            Left = leftRight;
            Right = leftRight;
        }

        public Thickness(int top, int left, int bottom, int right)
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }

        public int Top { get; set; }
        public int Left { get; set; }
        public int Bottom { get; set; }
        public int Right { get; set; }

        public int TopBottom => Top + Bottom;
        public int LeftRight => Left + Right;

        internal int TopBottomDiff => Top - Bottom;
        internal int LeftRightDiff => Left - Right;
    }
}
