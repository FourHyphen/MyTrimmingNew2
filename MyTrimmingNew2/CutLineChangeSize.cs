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
            if ((Before.Left - newWidth) < 0)
            {
                newWidth = Before.Left;
                newHeight = Before.CalcHeightBaseWidth(newWidth);
            }

            if ((Before.Top - newHeight) < 0)
            {
                newHeight = Before.Top;
                newWidth = Before.CalcWidthBaseHeight(newHeight);
            }

            double newLeft = Before.Left - newWidth;
            double newTop = Before.Top - newHeight;
            return new CutLineParameter(newLeft, newTop, newWidth, newHeight, Before.Degree);
        }

        private CutLineParameter CreateNewParameterIfOverShowingImage(double willWidth, double willHeight)
        {
            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            double newWidth = willWidth;
            double newHeight = willHeight;

            double newRight = Before.Left + newWidth;
            if (newRight > MaxRight)
            {
                newWidth = MaxRight - Before.Left;
                newHeight = Before.CalcHeightBaseWidth(newWidth);
            }

            double newBottom = Before.Top + newHeight;
            if (newBottom > MaxBottom)
            {
                newHeight = MaxBottom - Before.Top;
                newWidth = Before.CalcWidthBaseHeight(Before.Height);
            }

            return new CutLineParameter(Before.Left, Before.Top, newWidth, newHeight, Before.Degree);
        }
    }
}
