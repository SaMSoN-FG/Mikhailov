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
        int idCore = -1;
        public int Id {
            get {
                if(idCore == -1) idCore = Owner.DiagramItems.IndexOf(this);
                return idCore;
            }
        }
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
        internal PointF[] GetAreaPoints() {
            PointF[] points = new PointF[] { new PointF(locationCore.X - Size.Width / 2, locationCore.Y), new PointF(locationCore.X, locationCore.Y - Size.Height / 2), new PointF(locationCore.X + Size.Width / 2, locationCore.Y), new PointF(locationCore.X, locationCore.Y + Size.Height / 2) };
            return points;
        }
    }
    public class DiagramItemOffset {
        List<SizeF> offsetCore = new List<SizeF>();
        public void AddOffset(float dx, float dy) { offsetCore.Add(new SizeF(dx, dy)); }
        public PointF GetSumOffset(PointF location) {
            PointF result = PointF.Empty;
            foreach(SizeF size in offsetCore) result = result + size;
            float sx, sy;
            sx = Math.Abs(result.X);
            sy = Math.Abs(result.Y);
            if(sx <= 1) result.X = 0;
            if(sy <= 1) result.Y= 0;

            result = result + new SizeF(location);
            if(result.X < 0) result.X = 0;
            if(result.Y < 0) result.Y = 0;
            return result;
        }
        public void Clear() {
            offsetCore.Clear();
        }
    }
}
