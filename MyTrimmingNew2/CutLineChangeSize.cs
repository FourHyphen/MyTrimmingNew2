using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineChangeSize
    {
        public CutLineParameter Before { get; private set; }

        public CutLineParameter After { get; private set; }

        private double MaxRight { get; }

        private double MaxBottom { get; }

        public CutLineChangeSize(CutLine cutLine, ShowingImage image)
        {
            Before = new CutLineParameter(cutLine.Left, cutLine.Top, cutLine.Width, cutLine.Height);
            MaxRight = image.Width;
            MaxBottom = image.Height;
        }

        public CutLineParameter CalcNewParameter(System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            double newWidth = Before.Width;
            double newHeight = Before.Height;
            double distanceX = dropPoint.X - dragStart.X;
            double distanceY = dropPoint.Y - dragStart.Y;
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

            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
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

            After = new CutLineParameter(Before.Left, Before.Top, newWidth, newHeight);
            return After;
        }
    }
}
