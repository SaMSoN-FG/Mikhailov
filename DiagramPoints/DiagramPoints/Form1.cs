using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiagramPoints{
    public partial class Form1 : XtraForm {
        internal DiagramHelper helper = new DiagramHelper();
        Random random = new Random();
        Size offset = new Size(-4, -7);
        DiagramItem dragItem;
        Point startPoint;
        Timer timer = new Timer();
        Timer paintTimer = new Timer();
        public Form1() {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            timer.Interval = 1;
            timer.Tick += timer_Tick;
            paintTimer.Interval = 100;
            paintTimer.Start();
            paintTimer.Tick += paintTimer_Tick;
        }

        void paintTimer_Tick(object sender, EventArgs e) {
            Invalidate();
            Update();
        }

        void timer_Tick(object sender, EventArgs e) {
            helper.DoBestFit();
            
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            int diagramID = 0;
            for (int i = 0; i < Width; i += helper.CellSize.Width)
                e.Graphics.DrawLine(Pens.LightGray, new Point(i, 0), new Point(i, Height));

            for (int i = 0; i < Height; i += helper.CellSize.Height)
                e.Graphics.DrawLine(Pens.LightGray, new Point(0, i), new Point(Width, i));

            foreach (var item in helper.DiagramItems) {
                e.Graphics.DrawString("o", Font, Brushes.Blue, PointF.Add(item.Location, offset));
                e.Graphics.DrawString(diagramID.ToString(), new Font(Font.FontFamily, 6.5f), Brushes.Black, PointF.Add(item.Location, new Size(2, -8)));
                diagramID++;
            }
            foreach (var relation1 in helper.DiagramRelations) {
                e.Graphics.DrawLines(new Pen(Color.Red, 1.5f), relation1.Points);
                e.Graphics.DrawString("x", Font, Brushes.Orange, PointF.Add(relation1.GetCenter(), offset));
            }
        }

        public void AddItem(object sender, EventArgs e) {
            DiagramItem item = new DiagramItem();
            rectangleXTextEdit.Value = random.Next(10, 200);
            rectangleYTextEdit.Value = random.Next(10, 200);
            item.Location = new Point((int)rectangleXTextEdit.Value, (int)rectangleYTextEdit.Value);
            helper.DiagramItems.Add(item);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            dragItem = helper.CalcHitInfo(e);
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
            }   
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            dragItem = null;
            startPoint = Point.Empty;
        }

        static Point GetCenter(Rectangle rect) {
            return new Point(rect.Right / 2, rect.Height / 2);
        }

        private void timerStart(object sender, EventArgs e) {
            timer.Start();
        }

        private void makeRelation(object sender, EventArgs e) {
            try {
                DiagramRelation relation = new DiagramRelation();
                int id1 = Convert.ToInt32(item1IDTextEdit.Value);
                int id2 = Convert.ToInt32(item2IdTextEdit.Value);
                if (id1 < 0 || id2 < 0 || id1 == id2) return;
                relation.Item1 = helper.DiagramItems[id1];
                relation.Item2 = helper.DiagramItems[id2];
                helper.DiagramRelations.Add(relation);
            } catch { }
            
        }

        public void makeRandomRelation(object sender, EventArgs e) {
            if (helper.DiagramItems.Count < 2) return;
           int first = random.Next(0, helper.DiagramItems.Count);
           int second = random.Next(0, helper.DiagramItems.Count);
           while (first == second) {
               second = random.Next(0, helper.DiagramItems.Count);
           }
           DiagramRelation relation = new DiagramRelation();
           relation.Item1 = helper.DiagramItems[first];
           relation.Item2 = helper.DiagramItems[second];
           helper.DiagramRelations.Add(relation);
           
        }

        private void timerStop(object sender, EventArgs e) {
            timer.Stop();
        }

        private void bestFit(object sender, EventArgs e) {
            helper.DoBestFit();
            
        }

        public void clear(object sender, EventArgs e) {
            helper = new DiagramHelper();
            
        }

        private void generateRandom(object sender, EventArgs e) {
            for (int i = 0; i < 15; i++) {
                AddItem(null, null);
            }
            for (int i = 0; i < 10; i++) {
                makeRandomRelation(null, null);
            }
        }

        private void moveX(object sender, EventArgs e) {
            foreach (var item in helper.DiagramItems) {
                item.OffsetTo = Size.Empty;
                item.OffsetTo.Width = (float)moveXValueSpinEdit.Value;
                item.DoOffset();
            }
            
        }

        private void moveY(object sender, EventArgs e) {
            foreach (var item in helper.DiagramItems) {
                item.OffsetTo = Size.Empty;
                item.OffsetTo.Height = (float)moveYValueSpinEdit.Value;
                item.DoOffset();
            }
            
        }

        private void save(object sender, EventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = @"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore";
            dialog.Filter = "XML Files (*.xml)|*.xml";
            dialog.DefaultExt = "*.xml";
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                helper.SerializeToXMLFile(dialog.FileName);
        }

        private void load(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"D:\Project\DiagramPoints\DiagramPoints\Test\XMLStore";
            dialog.DefaultExt = "*.xml";
            if (dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            helper = new DiagramHelper();
            helper.DeserializeFromXMLFile(dialog.FileName);
            
        }

        private void doArrange(object sender, EventArgs e) {
            helper.DoArrange();
        }
    }
}
