using DevExpress.Utils.Serializing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramPoints {
    public class DiagramRelation {
        [XtraSerializableProperty(XtraSerializationVisibility.Content)]
        public DiagramItem Item1 { get; set; }
        [XtraSerializableProperty(XtraSerializationVisibility.Content)]
        public DiagramItem Item2 { get; set; }

        public PointF[] GetPointsByOffset(Size offset) {
                return new PointF[] { PointF.Add(Item1.Location,offset), PointF.Add(Item2.Location,offset)};
        }
        public PointF GetCenter() {
            return GetCenter(Item1.Location, Item2.Location);
        }
        public static PointF GetCenter(PointF p1, PointF p2) {
            float xCenter = p1.X + (p2.X - p1.X) / 2;
            float YCenter = p1.Y + (p2.Y - p1.Y) / 2;
            return new PointF(xCenter, YCenter);
        }
    }
}
