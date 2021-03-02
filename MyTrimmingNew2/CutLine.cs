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

        private static int NearRange = 20;

        public int Left { get { return Parameter.Left; } }

        public int Right { get { return Parameter.Right; } }

        public int Top { get { return Parameter.Top; } }

        public int Bottom { get { return Parameter.Bottom; } }

        public int Width { get { return Parameter.Width; } }

        public int Height { get { return Parameter.Height; } }

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
            int newLeft = Parameter.Left;
            int newTop = Parameter.Top;
            if (key == Key.Left)
            {
                newLeft = MoveX(-1 * num);
            }
            else if (key == Key.Right)
            {
                newLeft = MoveX(1 * num);
            }
            else if (key == Key.Up)
            {
                newTop = MoveY(-1 * num);
            }
            else if (key == Key.Down)
            {
                newTop = MoveY(1 * num);
            }

            Parameter = new CutLineParameter(newLeft, newTop, Parameter.Width, Parameter.Height);
        }

        private int MoveX(int xDirection)
        {
            int newLeft = Parameter.Left + xDirection;
            int newRight = Parameter.Right + xDirection;
            bool doStickOutLeft = DoLeftStickOutOfImage(newLeft);
            bool doStickOutRight = DoRightStickOutOfImage(newRight);

            if (!IsCutLineInsideImageHorizontal(doStickOutLeft, doStickOutRight))
            {
                newLeft = AdjustLeft(doStickOutLeft, doStickOutRight);
            }

            return newLeft;
        }

        private bool DoLeftStickOutOfImage(int left)
        {
            return (left < 0);
        }

        private bool DoRightStickOutOfImage(int right)
        {
            return (right > _ShowingImage.Width);
        }

        private bool IsCutLineInsideImageHorizontal(bool doStickOutLeft, bool doStickOutRight)
        {
            return (!doStickOutLeft && !doStickOutRight);
        }

        private int AdjustLeft(bool doStickOutLeft, bool doStickOutRight)
        {
            if (doStickOutLeft)
            {
                return 0;
            }
            else if (doStickOutRight)
            {
                return _ShowingImage.Width - Parameter.Width;
            }

            return Parameter.Left;
        }

        private int MoveY(int yDirection)
        {
            int newTop = Parameter.Top + yDirection;
            int newBottom = Parameter.Top + Parameter.Height + yDirection;
            bool doStickOutTop = DoTopStickOutOfImage(newTop);
            bool doStickOutBottom = DoBottomStickOutOfImage(newBottom);

            if (!IsCutLineInsideImageVertical(doStickOutTop, doStickOutBottom))
            {
                newTop = AdjustTop(doStickOutTop, doStickOutBottom);
            }

            return newTop;
        }

        private bool DoTopStickOutOfImage(int top)
        {
            return (top < 0);
        }

        private bool DoBottomStickOutOfImage(int bottom)
        {
            return (bottom > _ShowingImage.Height);
        }

        private bool IsCutLineInsideImageVertical(bool doStickOutTop, bool doStickOutBottom)
        {
            return (!doStickOutTop && !doStickOutBottom);
        }

        private int AdjustTop(bool doStickOutTop, bool doStickOutBottom)
        {
            if (doStickOutTop)
            {
                return 0;
            }
            else if (doStickOutBottom)
            {
                return _ShowingImage.Height - Parameter.Height;
            }

            return Parameter.Top;
        }

        public bool IsPointNearRightBottom(Point p)
        {
            return (IsPointNearRightBottomX((int)p.X, NearRange)) && (IsPointNearRightBottomY((int)p.Y, NearRange));
        }

        private bool IsPointNearRightBottomX(int x, int range)
        {
            return ((x - range) <= Parameter.Right) && (Parameter.Right <= (x + range));
        }

        private bool IsPointNearRightBottomY(int y, int range)
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
                newWidth += (int)distanceX;
                newHeight += (int)changeSizeY;
            }
            else
            {
                newWidth += (int)Parameter.CalcWidthBaseHeight(distanceY);
                newHeight += (int)distanceY;
            }

            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            int newRight = Parameter.Left + (int)newWidth;
            if (newRight > _ShowingImage.Width)
            {
                newWidth = _ShowingImage.Width - Parameter.Left;
                newHeight = (int)Parameter.CalcHeightBaseWidth((double)newWidth);
            }

            int newBottom = Parameter.Top + (int)newHeight;
            if (newBottom > _ShowingImage.Height)
            {
                newHeight = _ShowingImage.Height - Parameter.Top;
                newWidth = (int)Parameter.CalcWidthBaseHeight((double)Parameter.Height);
            }

            Parameter = new CutLineParameter(Parameter.Left, Parameter.Top, (int)newWidth, (int)newHeight);
        }
    }
}
