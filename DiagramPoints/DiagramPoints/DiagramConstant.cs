﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramPoints{
    public static class DiagramConstant {
        public const double Epsilon = 0.001;

        public const double ForceOfRepelBetweenItems = 0.7;
        public const double ForceOfCenterRealtion = 36.5;
        public const double ForceOfRepelRelation = 0.7;
        public const double PowerByCenterOfNonIntersectedPoints = 50;

        public const double MaxDistanceBetweenItemsForSetPower = 100;
        public const double BestLengthOfRepelRelation = 50;

        public const int GraphWidth = 10;
        public const int GraphHeight = 100;
        public static Random Random = new Random();
    }
  public enum PointLocation {
      LeftCenter = 0, 
      TopCenter = 1, 
      RightCenter = 2, 
      BottomCenter = 3,
      None = 4
  }
}
