using ConsoleDraw.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleDraw.UI
{
    /// <summary>
    /// The main extension that adds WinForms esque UI support to ConsoleDraw
    /// </summary>
    public class UIExtension : IDrawExtension
    {
        private FrameBufferGraphics _graphics;
        private Thread _eventLoop;
        private bool _running;

        public UIExtension(FrameBuffer buffer, FrameBufferGraphics graphics)
        {
            _buffer = buffer;
            _graphics = graphics;
            BasePanel = new Panel();            
        }

        public Panel BasePanel { get; private set; }

        private IEnumerable<Control> _selectableControls => BasePanel.Controls.Where(c => c.Selectable);

        private int _selectedControlIndex = 0;

        public void ForceRedraw()
        {
            BasePanel.NeedsUpdate = true;
        }

        public override void RunExtension()
        {
            BasePanel.ExtensionDraw(_graphics);
        }

        public void BeginEventLoop()
        {
            _running = true;
            while (_running)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case (ConsoleKey.UpArrow):
                        if (_selectableControls.Any())
                        {
                            _selectableControls.ElementAtOrDefault(_selectedControlIndex)?.Deselect();
                            _selectedControlIndex -= 1;
                        }
                        break;
                    case (ConsoleKey.DownArrow):
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
    }
}
