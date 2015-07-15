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

        public PointF[] Points {
            get {
                return new PointF[] { Item1.Location, Item2.Location };
            }
        }
        public PointF GetCenter() {
            float xCenter = Item1.Location.X + (Item2.Location.X - Item1.Location.X) / 2;
            float YCenter = Item1.Location.Y + (Item2.Location.Y - Item1.Location.Y) / 2;
            return new PointF(xCenter, YCenter);
        }
    }
}
