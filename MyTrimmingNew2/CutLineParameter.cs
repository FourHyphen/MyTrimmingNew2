using System;

namespace MyTrimmingNew2
{
    public class CutLineParameter
    {
        private static double RatioWidth = 16.0;

        private static double RatioHeight = 9.0;

        public double Width
        {
            get
            {
                return Math.Sqrt(Math.Pow(RightTop.X - LeftTop.X, 2.0) + Math.Pow(RightTop.Y - LeftTop.Y, 2.0));
            }
        }

        public double Height
        {
            get
            {
                return Math.Sqrt(Math.Pow(LeftBottom.X - LeftTop.X, 2.0) + Math.Pow(LeftBottom.Y - LeftTop.Y, 2.0));
            }
        }

        public double Left { get { return LeftTop.X; } }

        public double Top { get { return LeftTop.Y; } }

        public double Degree { get; private set; }

        public double RightEnd
        {
            get
            {
                return Math.Max(RightTop.X, RightBottom.X);
            }
        }

        public double BottomEnd
        {
            get
            {
                return Math.Max(RightBottom.Y, LeftBottom.Y);
            }
        }

        public System.Windows.Point LeftTop { get; private set; }

        public System.Windows.Point RightTop { get; private set; }

        public System.Windows.Point RightBottom { get; private set; }

        public System.Windows.Point LeftBottom { get; private set; }

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

        public double CalcHeightBaseWidth(double width)
        {
            return width * RatioHeight / RatioWidth;
        }

        public double CalcWidthBaseHeight(double height)
        {
            return height * RatioWidth / RatioHeight;
        }
    }
}