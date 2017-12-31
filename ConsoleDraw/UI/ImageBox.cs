using ConsoleDraw.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class ImageBox : Control
    {
        private Image<Rgb24> _image;
        private IResampler _resampler = new NearestNeighborResampler();
        private int _framePos;

        private List<FrameBuffer> _frames = new List<FrameBuffer>();

        public Image<Rgb24> Image
        {
            get => _image;
            set
            {
                _image = value;
                _needsUpdate = true;
            }
        }

        public IResampler Resampler
        {
            get => _resampler;
            set
            {
                _resampler = value;
                _needsUpdate = true;
            }
        }

        protected override bool UpdateAlways => Image?.Frames.Count > 1 == true;

        private Stopwatch _watch = Stopwatch.StartNew();

        protected override void Draw(FrameBufferGraphics graph)
        {
            int width = Image.Width;
            int height = Image.Height;
            Misc.ScaleProportions(ref width, ref height, DrawWidth, DrawHeight);

            if (_image.Width != width || _image.Height != height)
            {
                _image.Mutate(m => m.Resize(width, height, _resampler));
            }

            if (_image.Frames.Count > 1)
            {
                Debug.WriteLine(_watch.ElapsedMilliseconds);
                if (_needsUpdate)
                {
                    _frames.Clear();

                    foreach(ImageFrame<Rgb24> f in _image.Frames)
                    {
                        Rgb24[] image = new Rgb24[f.Width * f.Height];
                        f.SavePixelData(image);
                        _frames.Add(FrameBuffer.FromImage(image, f.Width, f.Height, true));
                    }
                }
                
                graph.DrawBuffer(_frames[_framePos], new Point(BorderThickness.Left + Padding.Left, BorderThickness.Top + Padding.Top));
                _framePos += 1;

                if (_framePos >= _image.Frames.Count)
                    _framePos = 0;
                _watch.Restart();
            }
            else
            {
                graph.DrawImage(_image, new Point(BorderThickness.Left + Padding.Left, BorderThickness.Top + Padding.Top));
            }
        }
    }
}