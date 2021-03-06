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

        private CutLineCommand NextCommand { get; set; } = null;

        public void SetCommand(Key key, int keyInputNum = 1)
        {
            NextCommand = CutLineCommandFactory.Create(this, _ShowingImage, key, keyInputNum);
        }

        public void ExecuteCommand()
        {
            Parameter = NextCommand.CalcNewParameter();
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
