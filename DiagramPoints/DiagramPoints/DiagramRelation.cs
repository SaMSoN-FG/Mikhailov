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
            //PointF[] AreaPointFromItem1 = Item1.GetAreaPoints();
            //PointF[] AreaPointFromItem2 = Item2.GetAreaPoints();
            //PointF candidate1 = PointF.Empty;
            //PointF candidate2 = PointF.Empty;
            //double res = double.MaxValue;
            //foreach(var point1 in AreaPointFromItem1) {
            //    foreach(var point2 in AreaPointFromItem2) {
            //        double tryDistance =DiagramHelper.GetDistanceBetweenPoints(point1, point2);
            //        if(tryDistance < res) {
            //            res = tryDistance;
            //            candidate1 = point1;
            //            candidate2 = point2;
            //        }
            //    }
            //}
            //PointF candidate3 = new PointF(candidate1.X,candidate2.Y);
            //return new PointF[] { PointF.Add(candidate1, offset), PointF.Add(candidate3, offset), PointF.Add(candidate2, offset) };
            return new PointF[] { PointF.Add(Item1.Location, offset), PointF.Add(Item2.Location, offset) };
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
