using ConsoleDraw.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDraw.Interfaces
{
    /// <summary>
    /// Provides a class that allows drawing from directly within
    /// the main draw thread.
    /// </summary>
    public abstract class IDrawExtension
    {
        protected FrameBuffer _buffer;

        public abstract void RunExtension();
    }
}
