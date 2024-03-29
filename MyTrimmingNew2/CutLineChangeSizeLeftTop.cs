﻿using System.Windows;

namespace MyTrimmingNew2
{
    public class CutLineChangeSizeLeftTop : CutLineChangeSize
    {
        public CutLineChangeSizeLeftTop(CutLine cutLine,
                                        ShowingImage image,
                                        Point dragStart,
                                        Point dropPoint) : base(cutLine, image, dragStart, dropPoint)
        {
        }

        protected override double GetDistanceX(System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            return DragStart.X - DropPoint.X;
        }

        protected override double GetDistanceY(System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            return DragStart.Y - DropPoint.Y;
        }

        protected override void AdjustWidthAndHeightIfOverShowingImage(ref double newWidth, ref double newHeight)
        {
            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            AdjustWidthAndHeightIfOverShowingImageWidthLeftSide(ref newWidth, ref newHeight);
            AdjustWidthAndHeightIfOverShowingImageHeightTopSide(ref newWidth, ref newHeight);
        }

        protected override System.Windows.Point GetNewLeftTop(double newWidth, double newHeight)
        {
            return new Point(Before.RightBottom.X - newWidth, Before.RightBottom.Y - newHeight);
        }

        protected override System.Windows.Point GetNewRightTop(double newWidth, double newHeight)
        {
            return new Point(Before.RightTop.X, Before.RightBottom.Y - newHeight);
        }

        protected override System.Windows.Point GetNewLeftBottom(double newWidth, double newHeight)
        {
            return new Point(Before.RightBottom.X - newWidth, Before.LeftBottom.Y);
        }

        protected override System.Windows.Point GetNewRightBottom(double newWidth, double newHeight)
        {
            return Before.RightBottom;
        }

        protected override CutLineParameter CreateNewParameterRotate(double newWidth, double newHeight)
        {
            System.Windows.Point newRightTop = GetNewRightTopRotate(newHeight);
            System.Windows.Point newLeftBottom = GetNewLeftBottomRotate(newWidth);
            System.Windows.Point newLeftTop = GetNewLeftTopRotate(newRightTop, newLeftBottom);

            if (DoStickOutImage(newLeftTop, newRightTop, Before.RightBottom, newLeftBottom))
            {
                return Before;
            }
            return new CutLineParameter(newLeftTop, newRightTop, Before.RightBottom, newLeftBottom, Before.Degree);
        }

        private System.Windows.Point GetNewRightTopRotate(double newHeight)
        {
            return CalcRotatePointBaseOfHeight(Before.RightBottom, Before.RightTop, Before.Height, newHeight);
        }

        private System.Windows.Point GetNewLeftBottomRotate(double newWidth)
        {
            return CalcRotatePointBaseOfWidth(Before.RightBottom, Before.LeftBottom, Before.Width, newWidth);
        }

        private System.Windows.Point GetNewLeftTopRotate(System.Windows.Point newRightTop, System.Windows.Point newLeftBottom)
        {
            double xDist = Before.RightBottom.X - newLeftBottom.X;
            double yDist = newLeftBottom.Y - Before.RightBottom.Y;
            return new System.Windows.Point(newRightTop.X - xDist, newRightTop.Y + yDist);
        }

        protected override CutLineParameter CreateNewParameterWhenExchangeOrigin(double newWidth, double newHeight)
        {
            return Before;
        }
    }
}