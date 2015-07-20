using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramPoints {
    public class DiagramHelper {
        public DiagramHelper() {
            DiagramItems = new List<DiagramItem>();
            DiagramRelations = new List<DiagramRelation>();
            BoundingBox = new Rectangle(200, 200, 400, 400);
        }
        [XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 1)]
        public List<DiagramItem> DiagramItems { get; set; }
        public Rectangle BoundingBox { get; set; }
        internal object XtraCreateDiagramItemsItem(XtraItemEventArgs e) {
            DiagramItem item = new DiagramItem(this);
            DiagramItems.Add(item);
            return item;
        }
        [XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 2)]
        public List<DiagramRelation> DiagramRelations { get; set; }
        internal object XtraCreateDiagramRelationsItem(XtraItemEventArgs e) {
            DiagramRelation item = new DiagramRelation();
            XtraPropertyInfo infoType = e.Item.ChildProperties["Item1"];
            XtraPropertyInfo locationInfo = infoType.ChildProperties["Location"];
            PointF point1 = (PointF)locationInfo.ValueToObject(typeof(PointF));
            item.Item1 = DiagramItems.First(di => di.Location == point1);

            infoType = e.Item.ChildProperties["Item2"];
            locationInfo = infoType.ChildProperties["Location"];
            PointF point2 = (PointF)locationInfo.ValueToObject(typeof(PointF));
            item.Item2 = DiagramItems.First(di => di.Location == point2);

            DiagramRelations.Add(item);
            return item;
        }

        Size cellSizeCore = new Size(150, 100);
        public Size CellSize { get { return cellSizeCore; } set { cellSizeCore = value; } }


        public void SerializeToXMLFile(string fileName) {
            XmlXtraSerializer serializer = new XmlXtraSerializer();
            serializer.SerializeObject(this, fileName, this.GetType().Name);
        }
        public void DeserializeFromXMLFile(string fileName) {
            XmlXtraSerializer serializer = new XmlXtraSerializer();
            serializer.DeserializeObject(this, fileName, this.GetType().Name);
        }
        internal static double GetDistanceBetweenPoints(PointF firstPoint, PointF secondPoint) {
            return Math.Sqrt(Math.Pow(firstPoint.X - secondPoint.X, 2) + Math.Pow(firstPoint.Y - secondPoint.Y, 2)) + 1;
        }
        internal DiagramItem CalcHitInfo(Point location) {
            foreach(var item in DiagramItems) {
                if(item.AreaRectangle.Contains(location)) return item;
            }
            return null;
        }

        internal void DoArrange() {
            foreach(var item in DiagramItems) {
                Point nearestPoint = GetNearestPointByCellSize(item.Location);
                //bool nearestPointIsNotFree = false;
                //foreach (var i in DiagramItems) {
                //    if (i.Location == nearestPoint && i != item) nearestPointIsNotFree = true; break;
                //}
                //if (nearestPointIsNotFree)
                //    nearestPoint = GetNearestPointByCellSize(new PointF((nearestPoint.X <= item.Location.X ? nearestPoint.X + CellSize.Width : nearestPoint.X - CellSize.Width), nearestPoint.Y));

                //item.OffsetTo.Width = nearestPoint.X - item.Location.X;
                //item.OffsetTo.Height = nearestPoint.Y - item.Location.Y;

                item.OffsetTo.AddOffset(nearestPoint.X - item.Location.X, nearestPoint.Y - item.Location.Y);
            }
            DoOffset();
        }
        internal Size GetGlobalOffset() {
            if(DiagramItems.Count == 0) return Size.Empty;
            float maxX = DiagramItems.Select(di => di.Location.X).Max();
            float maxY = DiagramItems.Select(di => di.Location.Y).Max();
            return new Size((int)maxX, (int)maxY);
        }
        internal void DoBestFit() {
            int watchDog = 5;
         //  while(CalcPowerByCenterOfPoints() && watchDog-- > 0) { DoOffset(); }
            CalcPowerBetweenItems();
            CalcPowerSpringPower();
            CalcPowerByCenterOfNonIntersectedPoints();
            CalcPowerRestrictArea();
            DoOffset();
        }
        //int rightLimit = 400
        private void CalcPowerRestrictArea() {
           // foreach(var item in DiagramItems) {
           //     double distanceToX = rightLimit - item.Location.X;
           //     double distanceToY = rightLimit - item.Location.Y;
           //     if(distanceToX <0) {
           //         double forceX = 50 / distanceToX;
           //         item.OffsetTo.Height -= (float)forceX;
           //     }
           //     if(distanceToY <0) {
           //         double forceY = 50 / distanceToY;
           //         item.OffsetTo.Width -= (float)forceY;
           //     }  
           //}
            PointF p1 = new PointF(BoundingBox.X, BoundingBox.Y);
            PointF p2 = new PointF(BoundingBox.X, BoundingBox.Bottom);
            PointF p3 = new PointF(BoundingBox.Right, BoundingBox.Y);
            PointF p4 = new PointF(BoundingBox.Right, BoundingBox.Bottom);
            double forceField = 685;
            CalcPowerForceField(forceField, p1, p3, false, true);
            CalcPowerForceField(forceField, p2, p1, true, true);
            CalcPowerForceField(forceField, p2, p4, true, false);
            CalcPowerForceField(forceField, p3, p4, true, true);
        }

        private Point GetNearestPointByCellSize(PointF pointF) {
            double resultX = CellSize.Width * (int)(pointF.X / CellSize.Width);
            if(pointF.X > resultX + cellSizeCore.Width / 2) resultX += CellSize.Width;
            double resultY = CellSize.Height * (int)(pointF.Y / CellSize.Height);
            if(pointF.Y > resultY + CellSize.Height / 2) resultY += CellSize.Height;
            return new Point((int)resultX, (int)resultY);
        }

        internal void DoOffset() {
            if(DiagramItems.Count == 0) return;
            foreach(var item in DiagramItems) {
                item.DoOffset();
            }
            //float minX = DiagramItems.Select(e => e.Location.X).Min();
            //float minY = DiagramItems.Select(e => e.Location.Y).Min();
            //foreach(var item in DiagramItems) {
            //   // if(minX < 0) item.OffsetTo.Width = -minX;
            //    if(minX < 0) item.OffsetTo.AddOffset(-(float) minX , 0);
            //   // if(minY < 0) item.OffsetTo.Height = -minY;
            //    if(minY < 0) item.OffsetTo.AddOffset(0, -(float)minY);
            //    item.DoOffset();
            //}
        }
        private bool CalcPowerByCenterOfPoints() {
            bool result = false;
            foreach(var relation1 in DiagramRelations) {
                foreach(var relation2 in DiagramRelations) {
                    double resultX = 0;
                    double resultY = 0;
                    if(relation1 == relation2) break;
                    PointF intersect = IntersectLines(relation1.Item1.Location, relation1.Item2.Location, relation2.Item1.Location, relation2.Item2.Location);
                    if(intersect == PointF.Empty) continue;
                    PointF relationCountPoint = GetPointByCountOfRelation(relation1);
                    resultX = DiagramConstant.ForceOfCenterRealtion * (-relationCountPoint.X + intersect.X) / (GetDistanceBetweenPoints(intersect, relationCountPoint));
                    resultY = DiagramConstant.ForceOfCenterRealtion * (-relationCountPoint.Y + intersect.Y) / (GetDistanceBetweenPoints(intersect, relationCountPoint));
                   // relation2.Item1.OffsetTo.Width += (float)resultX;
                   // relation2.Item2.OffsetTo.Width += (float)resultX;
                   // relation2.Item1.OffsetTo.Height += (float)resultY;
                   // relation2.Item2.OffsetTo.Height += (float)resultY;
                    relation2.Item1.OffsetTo.AddOffset((float)resultX, (float)resultY);
                    relation2.Item2.OffsetTo.AddOffset((float)resultX, (float)resultY);
                    result = true;
                    //relation 2
                    relationCountPoint = GetPointByCountOfRelation(relation2);
                    resultX = DiagramConstant.ForceOfCenterRealtion * (-relationCountPoint.X + intersect.X) / (GetDistanceBetweenPoints(intersect, relationCountPoint));
                    resultY = DiagramConstant.ForceOfCenterRealtion * (-relationCountPoint.Y + intersect.Y) / (GetDistanceBetweenPoints(intersect, relationCountPoint));
                    //relation1.Item1.OffsetTo.Width -= (float)resultX;
                    //relation1.Item2.OffsetTo.Width -= (float)resultX;
                    //relation1.Item1.OffsetTo.Height -= (float)resultY;
                    //relation1.Item2.OffsetTo.Height -= (float)resultY;
                    relation1.Item1.OffsetTo.AddOffset((float)-resultX, (float)-resultY);
                    relation1.Item2.OffsetTo.AddOffset((float)-resultX, (float)-resultY);
                }

            }
            return result;
        }

        private PointF GetPointByCountOfRelation(DiagramRelation relation1) {
            int item1RelationCount = DiagramRelations.Where(e => (e.Item1 == relation1.Item1 || e.Item2 == relation1.Item1)).Count();
            int item2RelationCount = DiagramRelations.Where(e => (e.Item1 == relation1.Item2 || e.Item2 == relation1.Item2)).Count();
            return item1RelationCount > item2RelationCount ? relation1.Item1.Location : relation1.Item2.Location;
        }

        private void CalcPowerSpringPower() {
            foreach(var relation in DiagramRelations) {
                var item1 = relation.Item1;
                var item2 = relation.Item2;
                double distance = GetDistanceBetweenPoints(item1.Location, item2.Location);
                double PowerOfRepel = DiagramConstant.BestLengthOfRepelRelation - distance;
                if(distance <= 1) distance = 1;
                double resultX = PowerOfRepel * ((item1.Location.X - item2.Location.X) / distance) * DiagramConstant.ForceOfRepelRelation;
                double resultY = PowerOfRepel * ((item1.Location.Y - item2.Location.Y) / distance) * DiagramConstant.ForceOfRepelRelation;
               // item1.OffsetTo.Width  += (float)resultX;
               // item1.OffsetTo.Height += (float)resultY;
               // item2.OffsetTo.Width  -= (float)resultX;
               // item2.OffsetTo.Height -= (float)resultY;
                item1.OffsetTo.AddOffset((float)resultX, (float)resultY);
                item2.OffsetTo.AddOffset((float)-resultX, (float)-resultY);
            }

            
        }
        private void CalcPowerByCenterOfNonIntersectedPoints() {
            foreach(var relation1 in DiagramRelations) {
                CalcPowerForceField(50, relation1.Item1.Location, relation1.Item2.Location, true, true);
            }
        }

        private void CalcPowerForceField(double forceF, PointF p1, PointF p2, bool flagUp, bool flagDown) {
             // double k = -(relation1.Item1.Location.Y - relation1.Item2.Location.Y) / (relation1.Item2.Location.X - relation1.Item1.Location.X);
            double k = -(p1.Y - p2.Y) / (p2.X - p1.X);
                double angle = Math.Atan(k) + Math.PI / 2;
           //  double hullRadius = GetDistanceBetweenPoints(relation1.Item1.Location, relation1.Item2.Location) / 2;
            double hullRadius = GetDistanceBetweenPoints(p1, p2) / 2;
            PointF center = DiagramRelation.GetCenter(p1,p2);

                foreach(var item in DiagramItems) {
                if(item.Location == p1 || item.Location == p2) continue;
                    double resultX = 0;
                    double resultY = 0;
                    double distance = GetDistanceBetweenPoints(item.Location, center);
                    if(distance < hullRadius) {
                        //DiagramConstant.ForceOfCenterRealtion
                        double positionY = -((p1.Y - p2.Y) * item.Location.X) / (p2.X - p1.X) - (p1.X * p2.Y - p1.Y * p2.X) / (p2.X - p1.X);
                        double forceM = forceF / distance;
                        resultX = Math.Cos(angle) * forceM;
                        resultY = Math.Sin(angle) * forceM;
                    if(item.Location.Y < positionY && flagUp||!flagDown) {
                            //item.OffsetTo.Height -= (float)resultY;
                            //item.OffsetTo.Width -= (float)resultX;
                            item.OffsetTo.AddOffset(-(float)resultX, -(float)resultY);
                           
                        } else {
                            //item.OffsetTo.Height += (float)resultY;
                            //item.OffsetTo.Width  += (float)resultX;
                            item.OffsetTo.AddOffset((float)resultX, (float)resultY);
                        }
                       
                    }
                }
            }

        private void CalcPowerBetweenItems() {
            foreach(var item1 in DiagramItems) {
                double resultX = 0;
                double resultY = 0;
                foreach(var item2 in DiagramItems) {
                    if(item1 == item2 || item1.Location == item2.Location) continue;
                    double distance = GetDistanceBetweenPoints(item1.Location, item2.Location);
                    if(distance > DiagramConstant.MaxDistanceBetweenItemsForSetPower && distance != 0) continue;
                    resultX += ((DiagramConstant.MaxDistanceBetweenItemsForSetPower - distance) * (item1.Location.X - item2.Location.X) / distance) * DiagramConstant.ForceOfRepelBetweenItems;
                    resultY += ((DiagramConstant.MaxDistanceBetweenItemsForSetPower - distance) * (item1.Location.Y - item2.Location.Y) / distance) * DiagramConstant.ForceOfRepelBetweenItems;
                }
               //item1.OffsetTo.Width  += (float)resultX;
               // item1.OffsetTo.Height += (float)resultY;
                item1.OffsetTo.AddOffset((float)resultX, (float)resultY);
            }
        }

        internal static PointF IntersectLines(PointF line1Begin, PointF line1End, PointF line2Begin, PointF line2End) {
            float tolerance = 1f;
            if(line1Begin == line2Begin || line1Begin == line2End || line1End == line2End || line1End == line2Begin) return PointF.Empty;
            float a = Det2(line1Begin.X - line1End.X, line1Begin.Y - line1End.Y, line2Begin.X - line2End.X, line2Begin.Y - line2End.Y);
            if(Math.Abs(a) < float.Epsilon) return PointF.Empty; // Lines are parallel

            float d1 = Det2(line1Begin.X, line1Begin.Y, line1End.X, line1End.Y);
            float d2 = Det2(line2Begin.X, line2Begin.Y, line2End.X, line2End.Y);
            float x = Det2(d1, line1Begin.X - line1End.X, d2, line2Begin.X - line2End.X) / a;
            float y = Det2(d1, line1Begin.Y - line1End.Y, d2, line2Begin.Y - line2End.Y) / a;

            if(x < Math.Min(line1Begin.X, line1End.X) - tolerance || x > Math.Max(line1Begin.X, line1End.X) + tolerance) return PointF.Empty;
            if(y < Math.Min(line1Begin.Y, line1End.Y) - tolerance || y > Math.Max(line1Begin.Y, line1End.Y) + tolerance) return PointF.Empty;
            if(x < Math.Min(line2Begin.X, line2End.X) - tolerance || x > Math.Max(line2Begin.X, line2End.X) + tolerance) return PointF.Empty;
            if(y < Math.Min(line2Begin.Y, line2End.Y) - tolerance || y > Math.Max(line2Begin.Y, line2End.Y) + tolerance) return PointF.Empty;
            PointF result = new PointF(x, y);
            if(result == line1Begin || result == line1End || result == line2Begin || result == line2End) return PointF.Empty;
            return new PointF(x, y);
        }
        static float Det2(float x1, float x2, float y1, float y2) {
            return (x1 * y2 - y1 * x2);
        }

        internal void PrepareForBestFit(Size Size) {
            GraphProcessor processor = new GraphProcessor(DiagramItems, DiagramRelations);
            List<Graph> listGraph = processor.SearchConnectedComponents();
            int globalY = Size.Height / DiagramItems.Count;
            int beginX = 0;
            int beginY = 0;
            foreach(var graph in listGraph.OrderBy(e=>e.Vertices.Count)) {
                int globalX = Size.Width / graph.Vertices.Count;
                beginX = 0;
                int counter = 0;
                foreach(var vertices in graph.Vertices.OrderBy(e => e.EdgesCount)) {
                    vertices.Location = new PointF(counter % 2 == 0 ? beginX : Size.Width - beginX, DiagramConstant.Random.Next(beginY, beginY + graph.Vertices.Count * globalY));
                    beginX += globalX;
                    counter++;
                }
                beginY += graph.Vertices.Count * globalY;
            }
        }

        internal void AddGraphItem(GraphItem graphItem) {
            DiagramItems.Add(graphItem);
            foreach(var item in graphItem.ChildItems) {
                DiagramRelations.Add(new DiagramRelation() { Item1 = graphItem, Item2 = item });
                AddGraphItem(item);
            }
        }

        public void Load(string textProgram) {
            Parse.ParseDot parseDot = new Parse.ParseDot(this, textProgram);
            DiagramItems = parseDot.GetDiagramItems();
            DiagramRelations = parseDot.GetDiagramRelations();
        }
    }
}

