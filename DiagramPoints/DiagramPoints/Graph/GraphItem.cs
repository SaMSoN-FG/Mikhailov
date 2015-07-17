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

        internal Point DoBestFit(Point startLocation) {
            if(ChildItems.Count > 0) {
                Point coreLocation = startLocation;
                coreLocation.Y += heightNode;
                foreach(var item in ChildItems) {
                    int offsetX = item.DoBestFit(coreLocation).X;
                    coreLocation.X += offsetX;
                }
                Location = coreLocation;
                return coreLocation;
            } else {
                Location = startLocation;
                return startLocation;
            }
        }
        int heightNode = 25;
    }
}
