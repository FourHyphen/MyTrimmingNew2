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
            double newLeft = Parameter.Left;
            double newTop = Parameter.Top;
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

            Parameter = new CutLineMove(this, _ShowingImage).CalcNewParameter(xDirection, yDirection);
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
            double newWidth = Parameter.Width;
            double newHeight = Parameter.Height;
            double distanceX = dropPoint.X - dragStart.X;
            double distanceY = dropPoint.Y - dragStart.Y;
            double changeSizeY = Parameter.CalcHeightBaseWidth(distanceX);
            if (Math.Abs(changeSizeY) > Math.Abs(distanceY))
            {
                newWidth += distanceX;
                newHeight += changeSizeY;
            }
            else
            {
                newWidth += Parameter.CalcWidthBaseHeight(distanceY);
                newHeight += distanceY;
            }

            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            double newRight = Parameter.Left + newWidth;
            if (newRight > _ShowingImage.Width)
            {
                newWidth = _ShowingImage.Width - Parameter.Left;
                newHeight = Parameter.CalcHeightBaseWidth(newWidth);
            }

            double newBottom = Parameter.Top + newHeight;
            if (newBottom > _ShowingImage.Height)
            {
                newHeight = _ShowingImage.Height - Parameter.Top;
                newWidth = Parameter.CalcWidthBaseHeight(Parameter.Height);
            }

            Parameter = new CutLineParameter(Parameter.Left, Parameter.Top, newWidth, newHeight);
        }
    }
}
