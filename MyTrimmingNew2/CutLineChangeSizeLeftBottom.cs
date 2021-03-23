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
            return Before;
        }

        protected override CutLineParameter CreateNewParameterWhenExchangeOrigin(double newWidth, double newHeight)
        {
            return Before;
        }
    }
}