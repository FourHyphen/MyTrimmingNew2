using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineChangeSizeRightBottom : CutLineChangeSize
    {
        public CutLineChangeSizeRightBottom(CutLine cutLine,
                                            ShowingImage image,
                                            System.Windows.Point dragStart,
                                            System.Windows.Point dropPoint) : base (cutLine, image, dragStart, dropPoint)
        {
        }

        protected override double GetDistanceX(System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            return DropPoint.X - DragStart.X;
        }

        protected override double GetDistanceY(System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            return DropPoint.Y - DragStart.Y;
        }

        protected override void AdjustWidthAndHeightIfOverShowingImage(ref double newWidth, ref double newHeight)
        {
            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            AdjustWidthAndHeightIfOverShowingImageWidthRightSide(ref newWidth, ref newHeight);
            AdjustWidthAndHeightIfOverShowingImageHeightBottomSide(ref newWidth, ref newHeight);
        }

        protected override System.Windows.Point GetNewLeftTop(double newWidth, double newHeight)
        {
            return Before.LeftTop;
        }

        protected override System.Windows.Point GetNewRightTop(double newWidth, double newHeight)
        {
            return new System.Windows.Point(Before.LeftTop.X + newWidth, Before.LeftTop.Y);
        }

        protected override System.Windows.Point GetNewLeftBottom(double newWidth, double newHeight)
        {
            return new System.Windows.Point(Before.LeftTop.X, Before.LeftTop.Y + newHeight);
        }

        protected override System.Windows.Point GetNewRightBottom(double newWidth, double newHeight)
        {
            return new System.Windows.Point(Before.LeftTop.X + newWidth, Before.RightTop.Y + newHeight);
        }

        protected override CutLineParameter CreateNewParameterRotate(double newWidth, double newHeight)
        {
            System.Windows.Point newRightTop = GetNewRightTopRotate(newWidth);
            System.Windows.Point newLeftBottom = GetNewLeftBottomRotate(newHeight);
            System.Windows.Point newRightBottom = GetNewRightBottomRotate(newRightTop, newLeftBottom);

            if (DoStickOutImage(Before.LeftTop, newRightTop, newRightBottom, newLeftBottom))
            {
                return Before;
            }
            return new CutLineParameter(Before.LeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree);
        }

        private System.Windows.Point GetNewRightTopRotate(double newWidth)
        {
            double tmp1 = newWidth * (Before.RightTop.X - Before.LeftTop.X);
            double tmp2 = Before.Width * Before.LeftTop.X;
            double x = (tmp1 + tmp2) / Before.Width;

            double tmp3 = newWidth * (Before.RightTop.Y - Before.LeftTop.Y);
            double tmp4 = Before.Width * Before.LeftTop.Y;
            double y = (tmp3 + tmp4) / Before.Width;

            return new System.Windows.Point(x, y);
        }

        private System.Windows.Point GetNewLeftBottomRotate(double newHeight)
        {
            double tmp1 = newHeight * (Before.LeftTop.X - Before.LeftBottom.X);
            double tmp2 = -(Before.Height * Before.LeftTop.X);
            double x = (tmp1 + tmp2) / (-Before.Height);

            double tmp3 = newHeight * (Before.LeftBottom.Y - Before.LeftTop.Y);
            double tmp4 = Before.Height * Before.LeftTop.Y;
            double y = (tmp3 + tmp4) / Before.Height;
            return new System.Windows.Point(x, y);
        }

        private System.Windows.Point GetNewRightBottomRotate(System.Windows.Point newRightTop, System.Windows.Point newLeftBottom)
        {
            double xDist = newRightTop.X - Before.LeftTop.X;
            double yDist = newRightTop.Y - Before.LeftTop.Y;
            return new System.Windows.Point(newLeftBottom.X + xDist, newLeftBottom.Y + yDist);
        }

        protected override CutLineParameter CreateNewParameterWhenExchangeOrigin(double newWidth, double newHeight)
        {
            // 回転後に原点を変更するような大きなサイズ変更はしない
            if (Before.Degree == 0)
            {
                AdjustWidthAndHeightWhenExchangeOrigin(ref newWidth, ref newHeight);
                return CreateNewParameterWhenExchangeOriginCore(newWidth, newHeight);
            }
            else
            {
                return Before;
            }
        }

        private void AdjustWidthAndHeightWhenExchangeOrigin(ref double newWidth, ref double newHeight)
        {
            // 左上の座標が変わる場合
            // (1) 旧左上点を新右下点とし、移動後の旧右下点を新左上点(仮)とする
            // (2) 新左上点が画像をはみ出ている場合は調整する
            newWidth = -newWidth;
            newHeight = -newHeight;    // Before.Top - newBottom = Before.Top - (Before.Top + baseHeight)
            if ((Before.LeftEnd - newWidth) < 0)
            {
                newWidth = Before.LeftEnd;
                newHeight = Before.CalcHeightBaseWidth(newWidth);
            }

            if ((Before.TopEnd - newHeight) < 0)
            {
                newHeight = Before.TopEnd;
                newWidth = Before.CalcWidthBaseHeight(newHeight);
            }
        }

        private CutLineParameter CreateNewParameterWhenExchangeOriginCore(double newWidth, double newHeight)
        {
            System.Windows.Point newLeftTop = GetNewLeftTopExchangeOrigin(newWidth, newHeight);
            System.Windows.Point newRightTop = GetNewRightTopExchangeOrigin(newLeftTop, newWidth);
            System.Windows.Point newRightBottom = GetNewRightBottomExchangeOrigin();
            System.Windows.Point newLeftBottom = GetNewLeftBottomExchangeOrigin(newLeftTop, newHeight);
            return new CutLineParameter(newLeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree);
        }

        private System.Windows.Point GetNewLeftTopExchangeOrigin(double newWidth, double newHeight)
        {
            return new System.Windows.Point(Before.LeftEnd - newWidth, Before.TopEnd - newHeight);
        }

        private System.Windows.Point GetNewRightTopExchangeOrigin(System.Windows.Point newLeftTop, double newWidth)
        {
            return new System.Windows.Point(newLeftTop.X + newWidth, newLeftTop.Y);
        }

        private System.Windows.Point GetNewRightBottomExchangeOrigin()
        {
            return new System.Windows.Point(Before.LeftTop.X, Before.LeftTop.Y);
        }

        private System.Windows.Point GetNewLeftBottomExchangeOrigin(System.Windows.Point newLeftTop, double newHeight)
        {
            return new System.Windows.Point(newLeftTop.X, newLeftTop.Y + newHeight);
        }
    }
}
