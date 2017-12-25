using ConsoleDraw.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class ImageBox : Control
    {
        private Image<Rgb24> _image;
        private IResampler _resampler = new NearestNeighborResampler();
        private int _framePos;

        private List<Rgb24[]> _frames = new List<Rgb24[]>();

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
                if(_needsUpdate)
                {
                    _frames.Clear();

                    foreach(ImageFrame<Rgb24> f in _image.Frames)
                    {
                        Rgb24[] image = new Rgb24[f.Width * f.Height];
                        f.SavePixelData(image);
                        _frames.Add(image);
                    }
                }
                
                graph.DrawImage(_frames[_framePos], new Point(BorderThickness.Left + Padding.Left, BorderThickness.Top + Padding.Top), width, height);
                _framePos += 1;

                if (_framePos >= _image.Frames.Count)
                    _framePos = 0;
            }
            else
            {
                graph.DrawImage(_image, new Point(BorderThickness.Left + Padding.Left, BorderThickness.Top + Padding.Top));
            }
        }
    }
}