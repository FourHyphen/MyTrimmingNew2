using System.Windows;

namespace MyTrimmingNew2
{
    public class CutLineChangeSizeRightTop : CutLineChangeSize
    {
        public CutLineChangeSizeRightTop(CutLine cutLine,
                                         ShowingImage image,
                                         Point dragStart,
                                         Point dropPoint) : base(cutLine, image, dragStart, dropPoint)
        {
        }

        protected override double GetDistanceX(System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            return DropPoint.X - DragStart.X;
        }

        protected override double GetDistanceY(System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            return DragStart.Y - DropPoint.Y;
        }

        protected override void AdjustWidthAndHeightIfOverShowingImage(ref double newWidth, ref double newHeight)
        {
            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            AdjustWidthAndHeightIfOverShowingImageWidthRightSide(ref newWidth, ref newHeight);

            AdjustWidthAndHeightIfOverShowingImageHeightTopSide(ref newWidth, ref newHeight);
        }

        protected override System.Windows.Point GetNewLeftTop(double newWidth, double newHeight)
        {
            return new Point(Before.LeftBottom.X, Before.LeftBottom.Y - newHeight);
        }

        protected override System.Windows.Point GetNewRightTop(double newWidth, double newHeight)
        {
            return new Point(Before.LeftBottom.X + newWidth, Before.LeftBottom.Y - newHeight);
        }

        protected override System.Windows.Point GetNewLeftBottom(double newWidth, double newHeight)
        {
            return Before.LeftBottom;
        }

        protected override System.Windows.Point GetNewRightBottom(double newWidth, double newHeight)
        {
            return new Point(Before.LeftBottom.X + newWidth, Before.RightBottom.Y);
        }

        protected override CutLineParameter CreateNewParameterRotate(double newWidth, double newHeight)
        {
            System.Windows.Point newRightBottom = GetNewRightBottomRotate(newWidth);
            System.Windows.Point newLeftTop = GetNewLeftTopRotate(newHeight);
            System.Windows.Point newRightTop = GetNewRightTopRotate(newLeftTop, newRightBottom);

            if (DoStickOutImage(newLeftTop, newRightTop, newRightBottom, Before.LeftBottom))
            {
                return Before;
            }
            return new CutLineParameter(newLeftTop, newRightTop, newRightBottom, Before.LeftBottom, Before.Degree);
        }

        private System.Windows.Point GetNewRightBottomRotate(double newWidth)
        {
            return CalcRotatePointBaseOfWidth(Before.LeftBottom, Before.RightBottom, Before.Width, newWidth);
        }

        private System.Windows.Point GetNewLeftTopRotate(double newHeight)
        {
            return CalcRotatePointBaseOfHeight(Before.LeftBottom, Before.LeftTop, Before.Height, newHeight);
        }

        private System.Windows.Point GetNewRightTopRotate(System.Windows.Point newLeftTop, System.Windows.Point newRightBottom)
        {
            double xDist = newRightBottom.X - Before.LeftBottom.X;
            double yDist = Before.LeftBottom.Y - newLeftTop.Y;
            return new Point(newLeftTop.X + xDist, newRightBottom.Y - yDist);
        }

        protected override CutLineParameter CreateNewParameterWhenExchangeOrigin(double newWidth, double newHeight)
        {
            return Before;
        }
    }
}