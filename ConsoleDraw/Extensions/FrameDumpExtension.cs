using ConsoleDraw.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace ConsoleDraw.Extensions
{
    /// <summary>
    /// Dumps frames to JSON files.
    /// </summary>
    public class FrameDumpExtension : IDrawExtension
    {
        public FrameDumpExtension(FrameBuffer buffer)
        {
            _buffer = buffer;
        }

        public override void RunExtension()
        {
            File.WriteAllText($"{DateTime.Now.Ticks}.json", JsonConvert.SerializeObject(_buffer.RawFrameBuffer));
        }
    }
}
