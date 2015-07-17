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
        }
        public List<GraphItem> ChildItems { get; set; }
        //todo removerandom
        internal void CreateRandom(int maxNestedCount,int maxChildCount) {
            int maxNestedCountCore = maxNestedCount;
            Random rand = new Random();
            for(int i = 0; i < rand.Next(1,maxChildCount); i++) {
                GraphItem item = new GraphItem(Owner);
                if(maxNestedCountCore > 0) {
                    maxNestedCountCore--;
                    item.CreateRandom(maxNestedCountCore, maxChildCount);
                }
                ChildItems.Add(item);
            }
        }
        int grapWidth { get { return ChildItems.Count > 0 ? ChildItems.Sum(e => e.grapWidth) : DiagramConstant.GraphWidth; } }
        internal void DoBestFit(ref int X, int Y) {
            Y += DiagramConstant.GraphHeight;
            if(ChildItems.Count != 0) {
                foreach(var item in ChildItems) {
                    item.DoBestFit(ref X,Y);
                    X += (item.grapWidth > DiagramConstant.GraphWidth) ? item.grapWidth / 2 : item.grapWidth;
                }
                int maxXFromChild = (int)ChildItems.Max(e => e.Location.X);
                int minXFromChild = (int)ChildItems.Min(e => e.Location.X);
                X = (maxXFromChild == minXFromChild ? maxXFromChild : (maxXFromChild - minXFromChild) / 2 + minXFromChild);
                Location = new PointF(X, Y);
            } else {
                Location = new PointF(X, Y);
            }
        }
    }
}
