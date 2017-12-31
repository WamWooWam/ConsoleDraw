using ConsoleDraw.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public abstract class Control
    {
        private FrameBuffer _frameBuffer;
        private FrameBufferGraphics _graphics;

        public Control()
        {
            _active = false;
            Enabled = true;
            Padding = new Thickness(0);
            BorderThickness = new Thickness(1);
        }

        protected bool _active;
        protected bool _needsUpdate;

        private ConsoleColor _foregroundColour;
        private ConsoleColor _backgroundColour;
        private ConsoleColor _borderColor;
        private int _x;
        private int _y;
        private int _width;
        private int _height;
        private Thickness _borderThickness;
        private Thickness _padding;
        private bool _enabled;
        private DockStyle _dock;

        public event EventHandler Activated;
        public event EventHandler Selected;
        public event EventHandler Deselected;
        public event EventHandler<ConsoleKeyPressEventArgs> KeyPressed;

        internal virtual bool NeedsUpdate { get => _needsUpdate || UpdateAlways; set => _needsUpdate = value; }

        public virtual void Activate()
        {
            Activated?.Invoke(this, new EventArgs());
            _needsUpdate = true;
        }

        public virtual void Select()
        {
            _active = true;
            Selected?.Invoke(this, new EventArgs());
            _needsUpdate = true;
        }

        public virtual void Deselect()
        {
            _active = false;
            Deselected?.Invoke(this, new EventArgs());
            _needsUpdate = true;
        }

        public virtual void KeyPress(ConsoleKeyInfo k)
        {
            KeyPressed?.Invoke(this, new ConsoleKeyPressEventArgs(k));
            _needsUpdate = true;
        }

        /// <summary>
        /// The foreground colour of the control
        /// </summary>
        public ConsoleColor ForegroundColour
        {
            get => _foregroundColour;
            set
            {
                _foregroundColour = value;
                _needsUpdate = true;
            }
        }

        /// <summary>
        /// The background colour of the control
        /// </summary>
        public ConsoleColor BackgroundColour
        {
            get => _backgroundColour;
            set
            {
                _backgroundColour = value;
                _needsUpdate = true;
            }
        }

        /// <summary>
        /// The control's border colour if applicable
        /// </summary>
        public ConsoleColor BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                _needsUpdate = true;
            }
        }

        /// <summary>
        /// The control's X position
        /// </summary>
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                _needsUpdate = true;
            }
        }

        /// <summary>
        /// The control's Y position
        /// </summary>
        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                _needsUpdate = true;
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                _needsUpdate = true;
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                _needsUpdate = true;
            }
        }

        public DockStyle Dock
        {
            get => _dock;
            set
            {
                _dock = value;
                _needsUpdate = true;
            }
        }

        public Thickness BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                _needsUpdate = true;
            }
        }

        public Thickness Padding
        {
            get => _padding;
            set
            {
                _padding = value;
                _needsUpdate = true;
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                _needsUpdate = true;
            }
        }

        public Panel Parent { get; internal set; }

        protected virtual bool UpdateAlways => false;

        #region Helpers

        protected int DrawWidth => Width - BorderThickness.LeftRight - Padding.LeftRight;
        protected int DrawHeight => Height - BorderThickness.TopBottom - Padding.TopBottom;

        protected int InnerWidth => Width - BorderThickness.LeftRight;
        protected int InnerHeight => Height - BorderThickness.TopBottom;

        internal bool Selectable => Enabled && IsSelectable;

        protected virtual bool InvertOnSelect => false;

        public virtual bool IsSelectable => true;


        public static implicit operator Rectangle(Control c) => new Rectangle(c.X, c.Y, c.Width, c.Height);

        #endregion

        internal void InternalLayout(FrameBuffer buffer, ref LayoutInfo info) => Layout(buffer, ref info);

        internal void InternalDraw(FrameBufferGraphics graph)
        {
            if (NeedsUpdate || _frameBuffer == null)
            {
                if (_frameBuffer == null || _frameBuffer.Width != Width || _frameBuffer.Height != Height)
                {
                    _frameBuffer = new FrameBuffer(Width, Height);
                    _graphics = new FrameBufferGraphics(_frameBuffer);
                }

                if (!UpdateAlways)
                {
                    _graphics.Clear(_active && InvertOnSelect ? BackgroundColour : BorderColor);
                    _graphics.DrawRect(new Rectangle(BorderThickness.Left, BorderThickness.Right, DrawWidth, DrawHeight),
                        _active && InvertOnSelect ? BorderColor : BackgroundColour);
                }

                Draw(_graphics);
            }

            graph.DrawBuffer(_frameBuffer, new Point(X, Y));

            _needsUpdate = false;
        }

        protected virtual void Layout(FrameBuffer buffer, ref LayoutInfo info)
        {
            switch (Dock)
            {
                case DockStyle.Top:
                    Width = info.Width;
                    Y = info.CurrentY;
                    info.CurrentY += Height;

                    info.AvailableHeight -= Height;
                    break;
                case DockStyle.Bottom:
                    Width = info.Width;
                    Y = (buffer.Height - Height);
                    X = 0;
                    info.AvailableHeight -= Height;
                    break;
                case DockStyle.Left:
                    X = 0;
                    Y = info.CurrentY;
                    if (Width == 0)
                        Width = info.Width / 2;
                    Height = info.AvailableHeight;
                    break;
                case DockStyle.Right:
                    X = info.AvailableWidth / 2;
                    Y = info.CurrentY;
                    if (Width == 0)
                        Width = info.Width / 2;
                    Height = info.AvailableHeight;
                    break;
                case DockStyle.Fill:
                    break;

                case DockStyle.None:
                default:
                    break;
            }
        }

        protected abstract void Draw(FrameBufferGraphics graph);

    }
}
