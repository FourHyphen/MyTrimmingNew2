using System;

namespace MyTrimmingNew2
{
    public class CutLineParameter
    {
        private static double RatioWidth = 16.0;

        private static double RatioHeight = 9.0;

        public System.Windows.Point LeftTop { get; private set; }

        public System.Windows.Point RightTop { get; private set; }

        public System.Windows.Point RightBottom { get; private set; }

        public System.Windows.Point LeftBottom { get; private set; }

        public double Degree { get; private set; }

        public double Width { get { return CalcDistance(RightTop, LeftTop); } }

        public double Height { get { return CalcDistance(LeftBottom, LeftTop); } }

        public double Ratio { get { return RatioHeight / RatioWidth; } }

        public double LeftEnd { get { return Math.Min(LeftTop.X, LeftBottom.X); } }

        public double TopEnd { get { return Math.Min(LeftTop.Y, RightTop.Y); } }

        public double RightEnd { get { return Math.Max(RightTop.X, RightBottom.X); } }

        public double BottomEnd { get { return Math.Max(RightBottom.Y, LeftBottom.Y); } }

        public CutLineParameter(ShowingImage image)
        {
            Init(image);
        }

        private void Init(ShowingImage image)
        {
            double width = image.Width;
            double height = CalcHeightBaseWidth(width);
            if (height > image.Height)
            {
                height = image.Height;
                width = CalcWidthBaseHeight(height);
            }

            InitPoint(width, height);
        }

        private void InitPoint(double width, double height)
        {
            LeftTop = new System.Windows.Point(0, 0);
            RightTop = new System.Windows.Point(width, 0);
            RightBottom = new System.Windows.Point(width, height);
            LeftBottom = new System.Windows.Point(0, height);
            Degree = 0;
        }

        public CutLineParameter(System.Windows.Point leftTop,
                                System.Windows.Point rightTop,
                                System.Windows.Point rightBottom,
                                System.Windows.Point leftBottom,
                                double degree)
        {
            LeftTop = leftTop;
            RightTop = rightTop;
            RightBottom = rightBottom;
            LeftBottom = leftBottom;
            Degree = degree;
        }

        private double CalcDistance(System.Windows.Point p1, System.Windows.Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2.0) + Math.Pow(p1.Y - p2.Y, 2.0));
        }

        public double CalcHeightBaseWidth(double width)
        {
            return width * RatioHeight / RatioWidth;
        }

        public double CalcWidthBaseHeight(double height)
        {
            return height * RatioWidth / RatioHeight;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            try
            {
                return Equals((CutLineParameter)obj);
            }
            catch
            {
                return false;
            }
        }

        private bool Equals(CutLineParameter clp)
        {
            if (!LeftTop.Equals(clp.LeftTop))
            {
                return false;
            }

            if (!RightTop.Equals(clp.RightTop))
            {
                return false;
            }

            if (!RightBottom.Equals(clp.RightBottom))
            {
                return false;
            }

            if (!LeftBottom.Equals(clp.LeftBottom))
            {
                return false;
            }

            if (Degree != clp.Degree)
            {
                return false;
            }

            return true;
        }
    }
}