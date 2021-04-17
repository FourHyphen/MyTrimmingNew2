using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMyTrimmingNew2
{
    class CutLineSlope
    {
        public double LeftXSlope { get; private set; }

        public double LeftYSlope { get; private set; }

        public double RightXSlope { get; private set; }

        public double RightYSlope { get; private set; }

        public CutLineSlope(MyTrimmingNew2.CutLine cl)
        {
            LeftXSlope = CalcSlope(cl.LeftTop, cl.RightTop);
            LeftYSlope = CalcSlope(cl.LeftTop, cl.LeftBottom);
            RightXSlope = CalcSlope(cl.RightBottom, cl.LeftBottom);
            RightYSlope = CalcSlope(cl.RightTop, cl.RightBottom);
        }

        private double CalcSlope(System.Windows.Point p1, System.Windows.Point p2)
        {
            double xDiff = p2.X - p1.X;
            double yDiff = p2.Y - p1.Y;
            if (xDiff == 0.0)
            {
                return 0.0;
            }
            return yDiff / xDiff;
        }
    }
}
