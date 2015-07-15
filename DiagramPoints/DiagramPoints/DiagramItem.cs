using DevExpress.Utils.Serializing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramPoints {
    public class DiagramItem {
        public RectangleF AreaRectangle {
            get {
                return new RectangleF(Location.X - 5, Location.Y - 5, 10, 10);
            }
        }
        PointF locationCore;
        [XtraSerializableProperty()]
        public PointF Location { get { return locationCore; } set { locationCore = value; } }
        internal SizeF OffsetTo = SizeF.Empty;
        internal void DoOffset() {
            locationCore.X += float.IsInfinity(OffsetTo.Width) || Math.Abs(OffsetTo.Width) < DiagramConstant.Epsilon ? 0 : OffsetTo.Width;
            locationCore.Y += float.IsInfinity(OffsetTo.Height) || Math.Abs(OffsetTo.Height) < DiagramConstant.Epsilon ? 0 : OffsetTo.Height;
            OffsetTo = SizeF.Empty;
        }
    }
}
