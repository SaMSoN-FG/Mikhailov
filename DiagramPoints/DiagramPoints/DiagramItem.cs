using DevExpress.Utils.Serializing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DiagramPoints {
    public class DiagramItem {
        public DiagramItem(DiagramHelper owner) {
            this.Owner = owner;
            Size = new Size(20, 20);
        }
        public int EdgesCount { get { return Owner.DiagramRelations.Count(e => e.Item1 == this || e.Item2 == this); } }
        public RectangleF AreaRectangle {
            get {
                return new RectangleF(Location.X - 5, Location.Y - 5, 10, 10);
            }
        }
        public int Id { get { return Owner.DiagramItems.IndexOf(this); } }
        PointF locationCore;
        [XtraSerializableProperty()]
        public PointF Location { get { return locationCore; } set { locationCore = value; } }
        internal Size Size { get; set; }
        public Rectangle Bounds {
            get { return new Rectangle((int)locationCore.X - Size.Width / 2, (int)locationCore.Y - Size.Height / 2, Size.Width, Size.Height); }
            set {
                Size = value.Size;
                locationCore = new PointF(value.Right / 2, value.Bottom / 2);
            }
        }
        public DiagramItemOffset OffsetTo = new DiagramItemOffset();
        //internal SizeF OffsetTo = SizeF.Empty;
        internal void DoOffset() {
            //locationCore.X += float.IsInfinity(OffsetTo.Width) || Math.Abs(OffsetTo.Width) < DiagramConstant.Epsilon ? 0 : OffsetTo.Width;
            //locationCore.Y += float.IsInfinity(OffsetTo.Height) || Math.Abs(OffsetTo.Height) < DiagramConstant.Epsilon ? 0 : OffsetTo.Height;


            locationCore = OffsetTo.GetSumOffset(locationCore);
            //OffsetTo = SizeF.Empty;
            OffsetTo.Clear();
        }

        public DiagramHelper Owner { get; set; }
        public string Name { get; set; }
    }
    public class DiagramItemOffset {
        List<SizeF> offsetCore = new List<SizeF>();
        public void AddOffset(float dx, float dy) { offsetCore.Add(new SizeF(dx, dy)); }
        public PointF GetSumOffset(PointF location) {
            PointF result = location;
            foreach(SizeF size in offsetCore) result = new PointF(result.X + size.Width, result.Y + size.Height);
            if(result.X < 0) result.X = 0;
            if(result.Y < 0) result.Y = 0;
            return result;
        }
        public void Clear() {
            offsetCore.Clear();
        }
    }
}
