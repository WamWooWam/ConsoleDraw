using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDraw.UI.Data
{
    public struct LayoutInfo
    {
        public int CurrentX;
        public int CurrentY;

        public int Width;
        public int Height;
        internal int AvailableWidth;
        internal int AvailableHeight;
    }
}
