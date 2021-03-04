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

        private double XDirection { get; set; }

        private double YDirection { get; set; }

        public void SetCommand(Key key, int num = 1)
        {
            XDirection = 0.0;
            YDirection = 0.0;
            if (key == Key.Left)
            {
                XDirection = -1 * num;
            }
            else if (key == Key.Right)
            {
                XDirection = 1 * num;
            }
            else if (key == Key.Up)
            {
                YDirection = -1 * num;
            }
            else if (key == Key.Down)
            {
                YDirection = 1 * num;
            }
        }

        public void ExecuteCommand()
        {
            Parameter = CutLineCommandFactory.Create(this, _ShowingImage, XDirection, YDirection).CalcNewParameter();
        }

        private System.Windows.Point DragStart { get; set; }

        private bool NowDraging { get; set; } = false;

        public void SetCommand(Point p)
        {
            if (IsPointNearRightBottom(p))
            {
                DragStart = p;
                NowDraging = true;
            }
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

        public void ExecuteCommand(System.Windows.Point p)
        {
            if (NowDraging)
            {
                ChangeSizeBaseRightBottom(DragStart, p);
                NowDraging = false;
            }
        }

        private void ChangeSizeBaseRightBottom(Point dragStart, Point dropPoint)
        {
            Parameter = CutLineCommandFactory.Create(this, _ShowingImage, dragStart, dropPoint).CalcNewParameter();
        }
    }
}
