using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineChangeSize : CutLineCommand
    {
        private double MaxRight { get; }

        private double MaxBottom { get; }

        private System.Windows.Point DragStart { get; }

        private System.Windows.Point DropPoint { get; }

        public CutLineChangeSize(CutLine cutLine, ShowingImage image, System.Windows.Point dragStart, System.Windows.Point dropPoint) : base (cutLine)
        {
            MaxRight = image.Width;
            MaxBottom = image.Height;
            DragStart = dragStart;
            DropPoint = dropPoint;
        }

        protected override CutLineParameter CalcNewParameterCore()
        {
            double newWidth = Before.Width;
            double newHeight = Before.Height;
            CalcNewParameterWidthAndHeight(ref newWidth, ref newHeight);

            if (newWidth >= 0.0 && newHeight >= 0.0)
            {
                return CreateNewParameterIfOverShowingImage(newWidth, newHeight);
            }
            else
            {
                // 縮小方向に行き過ぎると、右下点が左上点を超えて原点(左上)が変わる
                return CreateNewParameterWhenExchangeOrigin(newWidth, newHeight);
            }
        }

        private void CalcNewParameterWidthAndHeight(ref double newWidth, ref double newHeight)
        {
            double distanceX = DropPoint.X - DragStart.X;
            double distanceY = DropPoint.Y - DragStart.Y;
            double changeSizeY = Before.CalcHeightBaseWidth(distanceX);
            if (Math.Abs(changeSizeY) > Math.Abs(distanceY))
            {
                newWidth += distanceX;
                newHeight += changeSizeY;
            }
            else
            {
                newWidth += Before.CalcWidthBaseHeight(distanceY);
                newHeight += distanceY;
            }
        }

        private CutLineParameter CreateNewParameterWhenExchangeOrigin(double baseWidth, double baseHeight)
        {
            // 左上の座標が変わる場合
            // (1) 旧左上点を新右下点とし、移動後の旧右下点を新左上点(仮)とする
            // (2) 新左上点が画像をはみ出ている場合は調整する
            double newWidth = -baseWidth;
            double newHeight = -baseHeight;    // Before.Top - newBottom = Before.Top - (Before.Top + baseHeight)
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

        private CutLineParameter CreateNewParameterIfOverShowingImage(double willWidth, double willHeight)
        {
            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            double newWidth = willWidth;
            double newHeight = willHeight;

            double newRight = Before.LeftEnd + newWidth;
            if (newRight > MaxRight)
            {
                newWidth = MaxRight - Before.LeftEnd;
                newHeight = Before.CalcHeightBaseWidth(newWidth);
            }

            double newBottom = Before.TopEnd + newHeight;
            if (newBottom > MaxBottom)
            {
                newHeight = MaxBottom - Before.TopEnd;
                newWidth = Before.CalcWidthBaseHeight(Before.Height);
            }

            System.Windows.Point newRightTop = GetNewRightTop(newWidth);
            System.Windows.Point newRightBottom = GetNewRightBottom(newWidth, newHeight);
            System.Windows.Point newLeftBottom = GetNewLeftBottom(newHeight);
            return new CutLineParameter(Before.LeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree);
        }

        private System.Windows.Point GetNewRightTop(double newWidth)
        {
            return new System.Windows.Point(Before.LeftTop.X + newWidth, Before.LeftTop.Y);
        }

        private System.Windows.Point GetNewRightBottom(double newWidth, double newHeight)
        {
            return new System.Windows.Point(Before.LeftTop.X + newWidth, Before.LeftTop.Y + newHeight);
        }

        private System.Windows.Point GetNewLeftBottom(double newHeight)
        {
            return new System.Windows.Point(Before.LeftTop.X, Before.LeftTop.Y + newHeight);
        }
    }
}
