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

            double newRight = Before.Left + newWidth;
            double newBottom = Before.Top + newHeight;

            // 縮小し過ぎると左上点を超えて、左上の座標が変わる
            if (newRight < Before.Left || newBottom < Before.Top)
            {
                // 左上の座標が変わる場合は新右下点＝旧左上点とし、左上点を算出する
                distanceX = Before.Left - newRight;
                distanceY = Before.Top - newBottom;
                changeSizeY = Before.CalcHeightBaseWidth(distanceX);
                if (Math.Abs(changeSizeY) > Math.Abs(distanceY))
                {
                    newWidth = distanceX;
                    newHeight = changeSizeY;
                }
                else
                {
                    newWidth = Before.CalcWidthBaseHeight(distanceY);
                    newHeight = distanceY;
                }

                double newLeft = Before.Left - newWidth;
                if (newLeft < 0)
                {
                    newWidth = Before.Left;
                    newHeight = Before.CalcHeightBaseWidth(newWidth);
                }

                double newTop = Before.Top - newHeight;
                if (newTop < 0)
                {
                    newHeight = Before.Top;
                    newWidth = Before.CalcWidthBaseHeight(newHeight);
                }

                newLeft = Before.Left - newWidth;
                newTop = Before.Top - newHeight;
                return new CutLineParameter(newLeft, newTop, newWidth, newHeight);
            }
            else
            {
                // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
                if (newRight > MaxRight)
                {
                    newWidth = MaxRight - Before.Left;
                    newHeight = Before.CalcHeightBaseWidth(newWidth);
                }

                newBottom = Before.Top + newHeight;
                if (newBottom > MaxBottom)
                {
                    newHeight = MaxBottom - Before.Top;
                    newWidth = Before.CalcWidthBaseHeight(Before.Height);
                }

                return new CutLineParameter(Before.Left, Before.Top, newWidth, newHeight);
            }
        }
    }
}
