using ConsoleDraw.UI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ConsoleDraw.UI
{
    public class Panel : Control
    {
        public Panel()
        {
            Controls = new ObservableCollection<Control>();
            Padding = new Thickness(0);
            BorderThickness = new Thickness(0);
            Controls.CollectionChanged += Controls_CollectionChanged;
        }

        public ObservableCollection<Control> Controls { get; private set; }

        internal IEnumerable<Control> AllControls => Controls.Union(Controls.Where(c => c is Panel p).SelectMany(p => (p as Panel).Controls));

        public override bool IsSelectable => false;

        internal void ExtensionDraw(FrameBufferGraphics graphics) => Draw(graphics);

        internal override bool NeedsUpdate { get => base.NeedsUpdate || (AllControls.Any(c => c.NeedsUpdate) && Parent != null); set => base.NeedsUpdate = value; }

        protected override void Layout(FrameBuffer buffer, ref LayoutInfo info)
        {
            base.Layout(buffer, ref info);
            if (!(Parent is Panel))
                foreach (Control control in Controls)
                {
                    control.InternalLayout(buffer, ref info);
                }
        }

        private void Controls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Control ctrl in Controls)
                ctrl.Parent = this;
            _needsUpdate = true;
        }

        protected override void Draw(FrameBufferGraphics graph)
        {
            if (Controls.Any(c => c.NeedsUpdate))
            {
                if (NeedsUpdate)
                {
                    graph.Clear(BackgroundColour);

                    RunLayout(graph);

                    foreach (Control control in Controls)
                    {
                        control.InternalDraw(graph);
                    }
                }
                else
                {
                    RunLayout(graph);
                    foreach (Control control in Controls.Where(c => c.NeedsUpdate))
                    {
                        control.InternalDraw(graph);
                    }
                }

                _needsUpdate = false;
            }

        }

        private void RunLayout(FrameBufferGraphics graph)
        {
            LayoutInfo info = new LayoutInfo()
            {
                CurrentX = 0,
                CurrentY = 0,
                Width = graph.FrameBuffer.Width,
                Height = graph.FrameBuffer.Height,
                AvailableWidth = graph.FrameBuffer.Width,
                AvailableHeight = graph.FrameBuffer.Height
            };

            Layout(graph.FrameBuffer, ref info);
        }
    }
}
