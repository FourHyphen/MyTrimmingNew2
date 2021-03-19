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

        public double Width { get { return Parameter.Width; } }

        public double Height { get { return Parameter.Height; } }

        public double Degree { get { return Parameter.Degree; } }

        public System.Windows.Point LeftTop { get { return Parameter.LeftTop; } }

        public System.Windows.Point RightTop { get { return Parameter.RightTop; } }

        public System.Windows.Point RightBottom { get { return Parameter.RightBottom; } }

        public System.Windows.Point LeftBottom { get { return Parameter.LeftBottom; } }

        public CutLine(ShowingImage showingImage)
        {
            _ShowingImage = showingImage;
            Init();
        }

        private void Init()
        {
            Parameter = new CutLineParameter(_ShowingImage);
        }

        public CutLineParameter CloneParameter()
        {
            return new CutLineParameter(Parameter.LeftTop, Parameter.RightTop, Parameter.RightBottom, Parameter.LeftBottom, Parameter.Degree);
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
            return ((x - range) <= Parameter.RightBottom.X) && (Parameter.RightBottom.X <= (x + range));
        }

        private bool IsPointNearRightBottomY(double y, double range)
        {
            return ((y - range) <= Parameter.RightBottom.Y) && (Parameter.RightBottom.Y <= (y + range));
        }

        public bool IsPointInside(Point p)
        {
            if (p.X < LeftTop.X || p.X > RightTop.X)
            {
                return false;
            }
            if (p.Y < LeftTop.Y || p.Y > RightBottom.Y)
            {
                return false;
            }

            return true;
        }
    }
}
