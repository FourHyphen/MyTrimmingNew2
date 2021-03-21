using System.Windows;

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
            double newLeft = Before.RightEnd - newWidth;
            if (newLeft < 0)
            {
                newWidth = -Before.LeftEnd;
                newHeight = Before.CalcHeightBaseWidth(newWidth);
            }

            double newTop = Before.BottomEnd - newHeight;
            if (newTop < 0)
            {
                newHeight = -Before.TopEnd;
                newWidth = Before.CalcWidthBaseHeight(Before.Height);
            }
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
            return Before;
        }

        protected override CutLineParameter CreateNewParameterWhenExchangeOrigin(double newWidth, double newHeight)
        {
            return Before;
        }
    }
}