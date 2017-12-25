using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.Tools
{
    internal static class Misc
    {
        public static void ScaleProportions(ref int width, ref int height, int maxWidth, int maxHeight)
        {
            if (width <= maxWidth && height <= maxHeight)
                return;
            else
            {
                double ratioX = (double)maxWidth / width;
                double ratioY = (double)maxHeight / height;
                double ratio = Math.Min(ratioX, ratioY);

                width = (int)(width * ratio);
                height = (int)(height * ratio);
            }
        }
    }
}
