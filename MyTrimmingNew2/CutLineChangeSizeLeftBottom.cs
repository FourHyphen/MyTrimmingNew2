using System.Windows;

namespace MyTrimmingNew2
{
    public class CutLineChangeSizeLeftBottom : CutLineChangeSize
    {
        public CutLineChangeSizeLeftBottom(CutLine cutLine,
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
            return DropPoint.Y - DragStart.Y;
        }

        protected override void AdjustWidthAndHeightIfOverShowingImage(ref double newWidth, ref double newHeight)
        {
            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            AdjustWidthAndHeightIfOverShowingImageWidthLeftSide(ref newWidth, ref newHeight);
            AdjustWidthAndHeightIfOverShowingImageHeightBottomSide(ref newWidth, ref newHeight);
        }

        protected override System.Windows.Point GetNewLeftTop(double newWidth, double newHeight)
        {
            return new Point(Before.RightBottom.X - newWidth, Before.LeftTop.Y);
        }

        protected override System.Windows.Point GetNewRightTop(double newWidth, double newHeight)
        {
            return Before.RightTop;
        }

        protected override System.Windows.Point GetNewLeftBottom(double newWidth, double newHeight)
        {
            return new Point(Before.RightTop.X - newWidth, Before.RightTop.Y + newHeight);
        }

        protected override System.Windows.Point GetNewRightBottom(double newWidth, double newHeight)
        {
            return new Point(Before.RightBottom.X, Before.RightTop.Y + newHeight);
        }

        protected override CutLineParameter CreateNewParameterRotate(double newWidth, double newHeight)
        {
            System.Windows.Point newRightBottom = GetNewRightBottomRotate(newHeight);
            System.Windows.Point newLeftTop = GetNewLeftTopRotate(newWidth);
            System.Windows.Point newLeftBottom = GetNewLeftBottomRotate(newLeftTop, newRightBottom);

            if (DoStickOutImage(newLeftTop, Before.RightTop, newRightBottom, newLeftBottom))
            {
                return Before;
            }
            return new CutLineParameter(newLeftTop, Before.RightTop, newRightBottom, newLeftBottom, Before.Degree);
        }

        private System.Windows.Point GetNewRightBottomRotate(double newHeight)
        {
            return CalcRotatePointBaseOfHeight(Before.RightTop, Before.RightBottom, Before.Height, newHeight);
        }

        private System.Windows.Point GetNewLeftTopRotate(double newWidth)
        {
            return CalcRotatePointBaseOfWidth(Before.RightTop, Before.LeftTop, Before.Width, newWidth);
        }

        private System.Windows.Point GetNewLeftBottomRotate(System.Windows.Point newLeftTop, System.Windows.Point newRightBottom)
        {
            double xDist = Before.RightTop.X - newLeftTop.X;
            double yDist = newRightBottom.Y - Before.RightTop.Y;
            return new Point(newRightBottom.X - xDist, newLeftTop.Y + yDist);
        }

        protected override CutLineParameter CreateNewParameterWhenExchangeOrigin(double newWidth, double newHeight)
        {
            return Before;
        }
    }
}