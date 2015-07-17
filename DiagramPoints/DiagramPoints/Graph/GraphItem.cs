using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramPoints {
    public  class GraphItem : DiagramItem {
        public GraphItem(DiagramHelper owner) : base(owner) {
            ChildItems = new List<GraphItem>();
            Size = new Size(DiagramConstant.Random.Next(10, 100), DiagramConstant.Random.Next(10, 100));
        }
        public List<GraphItem> ChildItems { get; set; }
        //todo removerandom
        internal void CreateRandom(int maxNestedCount,int maxChildCount) {
            int maxNestedCountCore = maxNestedCount;
            for(int i = 0; i <DiagramConstant.Random.Next(1,maxChildCount); i++) {
                GraphItem item = new GraphItem(Owner);
                if(maxNestedCountCore > 0) {
                    maxNestedCountCore--;
                    item.CreateRandom(maxNestedCountCore, maxChildCount);
                }
                ChildItems.Add(item);
            }
        }
        int grapWidth {
            get {
                if(ChildItems.Count == 0) return Size.Width / 2 + DiagramConstant.GraphWidth / 2;
                return ChildItems.Sum(e => e.grapWidth);
            }
        }
        internal void DoBestFit(ref int X, int Y) {
            if(ChildItems.Count != 0) {
                foreach(var item in ChildItems) {
                    X += item.Size.Width / 2 + DiagramConstant.GraphWidth;
                    int yForChild = Y + (ChildItems.Max(e => e.Size.Height) + Size.Height) / 2 + DiagramConstant.GraphHeight;
                    item.DoBestFit(ref X, yForChild);
                    X += item.grapWidth;
                }
                int maxXFromChild = (int)ChildItems.Max(e => e.Bounds.Right);
                int minXFromChild = (int)ChildItems.Min(e => e.Bounds.Left);
                int dif = (maxXFromChild - minXFromChild) / 2;
                dif = (dif > Size.Width / 2 ? dif : Size.Width / 2);
                X = minXFromChild + dif;
                Location = new PointF(X, Y);
                X += DiagramConstant.GraphWidth;
            } else {
                Location = new PointF(X, Y);
            }
        }
    }
}
