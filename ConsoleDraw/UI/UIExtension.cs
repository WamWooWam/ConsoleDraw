using ConsoleDraw.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleDraw.UI
{
    /// <summary>
    /// The main extension that adds WinForms esque UI support to ConsoleDraw
    /// </summary>
    public class UIExtension : IDrawExtension, IDisposable
    {
        private FrameBuffer _uiBuffer;
        private FrameBufferGraphics _uiGraphics;
        private Thread _drawThread;
        private bool _running;

        ManualResetEventSlim sync = new ManualResetEventSlim(false);

        public UIExtension(FrameBuffer buffer)
        {
            _buffer = buffer;

            _uiBuffer = new FrameBuffer(_buffer.Width, _buffer.Height);
            _buffer.GetFramebufferCopy(_uiBuffer.RawFrameBuffer);

            buffer.FrameDrawn += Buffer_FrameDrawn;

            _uiGraphics = new FrameBufferGraphics(_uiBuffer);

            BasePanel = new Panel();
        }

        private void Buffer_FrameDrawn(object sender, EventArgs e)
        {
        }

        public ObservableCollection<Control> Controls => BasePanel.Controls;

        public Panel BasePanel { get; private set; }

        private IEnumerable<Control> _selectableControls => BasePanel.AllControls.Where(c => c.Selectable);

        private int _selectedControlIndex = 0;
        private Stopwatch _watch = new Stopwatch();
        private long _frameLimitMS = 33;

        public void ForceRedraw()
        {
            BasePanel.NeedsUpdate = true;
        }

        public override void RunExtension()
        {
            if (!_running || _drawThread == null)
                BeginDrawLoop();

            sync.Set();
            new FrameBufferGraphics(_buffer).DrawBuffer(_uiBuffer, new Point(0,0));
            sync.Reset();
        }

        public void BeginDrawLoop()
        {
            if (_drawThread != null)
                return;

            _running = true;
            _drawThread = new Thread(() =>
            {
                while (_running)
                {
                    Draw();
                }
            });
            _drawThread.Start();
        }

        private void Draw()
        {
            sync.Wait();

            _watch.Start();

            BasePanel.ExtensionDraw(_uiGraphics);

            if (_frameLimitMS - _watch.ElapsedMilliseconds > 0)
                Thread.Sleep((int)Math.Max(0, _frameLimitMS - _watch.ElapsedMilliseconds));

            Debug.WriteLine(_watch.ElapsedMilliseconds);

            _watch.Reset();
        }

        public void BeginEventLoop()
        {
            _running = true;
            while (_running)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case (ConsoleKey.LeftArrow):
                        if (_selectableControls.Any())
                        {
                            _selectableControls.ElementAtOrDefault(_selectedControlIndex)?.Deselect();
                            _selectedControlIndex -= 1;
                        }
                        break;
                    case (ConsoleKey.RightArrow):
                        if (_selectableControls.Any())
                        {
                            _selectableControls.ElementAtOrDefault(_selectedControlIndex)?.Deselect();
                            _selectedControlIndex += 1;
                        }
                        break;
                    case ConsoleKey.Enter:
                        _selectableControls.ElementAtOrDefault(_selectedControlIndex)?.Activate();
                        break;
                    default:
                        _selectableControls.ElementAtOrDefault(_selectedControlIndex)?.KeyPress(keyInfo);
                        break;
                }


                if (_selectedControlIndex < 0)
                    _selectedControlIndex = _selectableControls.Count() - 1;
                if (_selectedControlIndex > _selectableControls.Count() - 1)
                    _selectedControlIndex = 0;

                _selectableControls.ElementAtOrDefault(_selectedControlIndex)?.Select();
            }
        }

        public void Dispose()
        {

        }
    }
}
