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

        public double Degree { get { return Parameter.Degree; } }

        public CutLine(ShowingImage showingImage)
        {
            _ShowingImage = showingImage;
            Init();
        }

        private void Init()
        {
            Parameter = new CutLineParameter(_ShowingImage);
        }

        public void ExecuteCommand(Key key, int keyInputNum = 1)
        {
            CutLineCommand command = CutLineCommandFactory.Create(this, _ShowingImage, key, keyInputNum);
            ExecuteCommandCore(command);
        }

        public void ExecuteCommand(Point dragStart, Point dropPoint)
        {
            CutLineCommand command = CutLineCommandFactory.Create(this, _ShowingImage, dragStart, dropPoint);
            ExecuteCommandCore(command);
        }

        private void ExecuteCommandCore(CutLineCommand command)
        {
            if (command != null)
            {
                Parameter = command.CalcNewParameter();
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

        public bool IsPointInside(Point p)
        {
            if (p.X < Left || p.X > Right)
            {
                return false;
            }
            if (p.Y < Top || p.Y > Bottom)
            {
                return false;
            }

            return true;
        }
    }
}
