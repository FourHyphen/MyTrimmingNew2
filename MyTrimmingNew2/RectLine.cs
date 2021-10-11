using System.Windows;

namespace MyTrimmingNew2
{
    public class RectLine
    {
        // y = a*x + b
        private double LineLeftA { get; set; }

        private double LineLeftB { get; set; }

        private double LineRightA { get; set; }

        private double LineRightB { get; set; }

        private double LineTopA { get; set; }

        private double LineTopB { get; set; }

        private double LineBottomA { get; set; }

        private double LineBottomB { get; set; }

        public RectLine(Point leftTop,
                        Point rightTop,
                        Point rightBottom,
                        Point leftBottom)
        {
            LineLeftA = CalcLineA(leftTop, leftBottom);
            LineLeftB = CalcLineB(leftTop.X, leftTop.Y, LineLeftA);
            LineRightA = CalcLineA(rightTop, rightBottom);
            LineRightB = CalcLineB(rightTop.X, rightTop.Y, LineRightA);
            LineTopA = CalcLineA(rightTop, leftTop);
            LineTopB = CalcLineB(rightTop.X, rightTop.Y, LineTopA);
            LineBottomA = CalcLineA(rightBottom, leftBottom);
            LineBottomB = CalcLineB(rightBottom.X, rightBottom.Y, LineBottomA);
        }

        public bool IsInside((double X, double Y) p)
        {
            if (!IsYInsideRectLineLeft(p))
            {
                return false;
            }

            if (!IsYInsideRectLineRight(p))
            {
                return false;
            }

            if (!IsYInsideRectLineTop(p))
            {
                return false;
            }

            if (!IsYInsideRectLineBottom(p))
            {
                return false;
            }

            return true;
        }

        private bool IsYInsideRectLineLeft((double X, double Y) p)
        {
            double lineY = LineLeftA * p.X + LineLeftB;
            if (LineLeftA > 0.0)
            {
                return (lineY > p.Y);
            }
            else
            {
                return (lineY < p.Y);
            }
        }

        private bool IsYInsideRectLineRight((double X, double Y) p)
        {
            double lineY = LineRightA * p.X + LineRightB;
            if (LineRightA > 0.0)
            {
                return (lineY < p.Y);
            }
            else
            {
                return (lineY > p.Y);
            }
        }

        private bool IsYInsideRectLineTop((double X, double Y) p)
        {
            double lineY = LineTopA * p.X + LineTopB;
            return (lineY < p.Y);
        }

        private bool IsYInsideRectLineBottom((double X, double Y) p)
        {
            double lineY = LineBottomA * p.X + LineBottomB;
            return (lineY > p.Y);
        }

        private double CalcLineA(Point p1, Point p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;

            if (x == 0.0)
            {
                // 直線が x = 定数 となる場合、aを十分大きな値にした直線で近似する
                return (y / 1e-6);
            }
            else
            {
                return (y / x);
            }
        }

        private double CalcLineB(double x, double y, double a)
        {
            return y - a * x;
        }
    }
}
