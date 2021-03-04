using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLine
    {
        private ShowingImage _ShowingImage { get; set; }

        private CutLineParameter Parameter { get; set; }

        private static double NearRange = 20.0;

        public double Left { get { return Parameter.Left; } }

        public double Right { get { return Parameter.Right; } }

        public double Top { get { return Parameter.Top; } }

        public double Bottom { get { return Parameter.Bottom; } }

        public double Width { get { return Parameter.Width; } }

        public double Height { get { return Parameter.Height; } }

        public CutLine(ShowingImage showingImage)
        {
            _ShowingImage = showingImage;
            Init();
        }

        private void Init()
        {
            Parameter = new CutLineParameter(_ShowingImage);
        }

        public void Move(Key key, int num = 1)
        {
            double xDirection = 0.0;
            double yDirection = 0.0;
            if (key == Key.Left)
            {
                xDirection = -1 * num;
            }
            else if (key == Key.Right)
            {
                xDirection = 1 * num;
            }
            else if (key == Key.Up)
            {
                yDirection = -1 * num;
            }
            else if (key == Key.Down)
            {
                yDirection = 1 * num;
            }

            Parameter = CutLineCommandFactory.Create(this, _ShowingImage, xDirection, yDirection).CalcNewParameter();
        }

        public bool IsPointNearRightBottom(Point p)
        {
            return (IsPointNearRightBottomX(p.X, NearRange)) && (IsPointNearRightBottomY(p.Y, NearRange));
        }

        private bool IsPointNearRightBottomX(double x, double range)
        {
            return ((x - range) <= Parameter.Right) && (Parameter.Right <= (x + range));
        }

        private bool IsPointNearRightBottomY(double y, double range)
        {
            return ((y - range) <= Parameter.Bottom) && (Parameter.Bottom <= (y + range));
        }

        public void ChangeSizeBaseRightBottom(Point dragStart, Point dropPoint)
        {
            Parameter = CutLineCommandFactory.Create(this, _ShowingImage, dragStart, dropPoint).CalcNewParameter();
        }
    }
}
