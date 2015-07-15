using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramPoints{
    public static class DiagramConstant {
        public const double Epsilon = 0.001;

        public const double ForceOfRepelBetweenItems = 0.2;
        public const double ForceOfCenterRealtion = 40.5;
        public const double ForceOfRepelRelation = 0.1;

        public const double MaxDistanceBetweenItemsForSetPower = 200;
        public const double BestLengthOfRepelRelation = 25;
    }
  public enum PointLocation {
      LeftCenter = 0, 
      TopCenter = 1, 
      RightCenter = 2, 
      BottomCenter = 3,
      None = 4
  }
}
