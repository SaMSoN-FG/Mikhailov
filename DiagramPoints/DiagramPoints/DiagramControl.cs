using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace DiagramPoints {
    public partial class DiagramControl : XtraUserControl {
        internal DiagramHelper helper = new DiagramHelper();
        Size offset = new Size(-4, -7);
        internal Size globalOffset = Size.Empty;
        DiagramItem dragItem;
        Point startPoint = Point.Empty;
        Timer paintTimer = new Timer();
        public DiagramControl() {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            AutoScroll = true;
            paintTimer.Interval = 100;
            paintTimer.Start();
            paintTimer.Tick += paintTimer_Tick;
        }
        void paintTimer_Tick(object sender, EventArgs e) {
            Invalidate();
            Update();
        }
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            helper.BoundingBox = new Rectangle(0,0, Width-1, Height-1);
        }
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            int diagramID = 0;
            for (int i = globalOffset.Width % helper.CellSize.Width; i < Width; i += helper.CellSize.Width)
                e.Graphics.DrawLine(Pens.LightGray, new Point(i, 0), new Point(i, Height));

            for (int i = globalOffset.Height % helper.CellSize.Height; i < Height; i += helper.CellSize.Height)
                e.Graphics.DrawLine(Pens.LightGray, new Point(0, i), new Point(Width, i));

            foreach (var item in helper.DiagramItems) {
                e.Graphics.DrawString("o", Font, Brushes.Blue, PointF.Add(item.Location, Size.Add(offset, globalOffset)));
                e.Graphics.DrawString(diagramID.ToString(), new Font(Font.FontFamily, 6.5f), Brushes.Black, PointF.Add(item.Location, Size.Add(new Size(2, -8), globalOffset)));
                diagramID++;
            }
            foreach (var relation1 in helper.DiagramRelations) {
                e.Graphics.DrawLines(new Pen(Color.Red, 1.5f), relation1.GetPointsByOffset(globalOffset));
                e.Graphics.DrawString("x", Font, Brushes.Orange, PointF.Add(relation1.GetCenter(), Size.Add(offset, globalOffset)));
            }
            e.Graphics.DrawRectangle(Pens.Green, helper.BoundingBox);
        }
        Point lastPoint = Point.Empty;
        protected override void OnInvalidated(InvalidateEventArgs e) {
            base.OnInvalidated(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            dragItem = helper.CalcHitInfo(Point.Add(e.Location,new Size(-globalOffset.Width, -globalOffset.Height)));
            startPoint = e.Location;
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (dragItem != null) {
                PointF pointToSet = new PointF(dragItem.Location.X + (e.Location.X - startPoint.X), dragItem.Location.Y + (e.Location.Y - startPoint.Y));
                dragItem.Location = pointToSet;
                startPoint.X = e.Location.X;
                startPoint.Y = e.Location.Y;
                Refresh();
            } else {
                if (e.Button != System.Windows.Forms.MouseButtons.Right) return; 
                Point pointToSet = new Point((e.Location.X - startPoint.X),(e.Location.Y - startPoint.Y));
                startPoint = e.Location;
                globalOffset.Width += pointToSet.X;
                globalOffset.Height += pointToSet.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            dragItem = null;
            startPoint = Point.Empty;
        }
    }
}
