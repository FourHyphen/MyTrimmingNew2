using System;

namespace MyTrimmingNew2
{
    public class CutLineParameter
    {
        private static double RatioWidth = 16.0;

        private static double RatioHeight = 9.0;

        public double Width { get; private set; }

        public double Height { get; private set; }

        public double Left { get; private set; }

        public double Top { get; private set; }

        public double Degree { get; private set; }

        public double Right
        {
            get
            {
                return Left + Width;
            }
        }

        public double Bottom
        {
            get
            {
                return Top + Height;
            }
        }

        public CutLineParameter(ShowingImage image)
        {
            Init(image);
        }

        private void Init(ShowingImage image)
        {
            InitOrigin();
            InitSize(image);
        }

        private void InitOrigin()
        {
            Left = 0;
            Top = 0;
            Degree = 0;
        }

        private void InitSize(ShowingImage image)
        {
            double width = image.Width;
            double height = CalcHeightBaseWidth(width);
            if (height > image.Height)
            {
                height = image.Height;
                width = CalcWidthBaseHeight(height);
            }

            Width = (int)width;
            Height = (int)height;
        }

        public CutLineParameter(double left, double top, double width, double height, double degree)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
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