using System;

namespace MyTrimmingNew2
{
    public class CutLineParameter
    {
        private static double RatioWidth = 16.0;

        private static double RatioHeight = 9.0;


        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Left { get; private set; }

        public int Top { get; private set; }

        public int Right
        {
            get
            {
                return Left + Width;
            }
        }

        public int Bottom
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

        public CutLineParameter(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
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