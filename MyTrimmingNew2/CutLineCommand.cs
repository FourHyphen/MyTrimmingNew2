using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public abstract class CutLineCommand
    {
        public CutLineParameter Before { get; private set; }

        public CutLineParameter After { get; private set; }

        protected double MaxRight { get; }

        protected double MaxBottom { get; }

        public CutLineCommand(CutLine cutLine, ShowingImage image)
        {
            Before = cutLine.CloneParameter();
            MaxRight = image.Width;
            MaxBottom = image.Height;
        }

        public void CalcNewParameter()
        {
            After = CalcNewParameterCore();
        }

        protected abstract CutLineParameter CalcNewParameterCore();

        protected bool DoStickOutImage(System.Windows.Point leftTop,
                                       System.Windows.Point rightTop,
                                       System.Windows.Point rightBottom,
                                       System.Windows.Point leftBottom)
        {
            return (DoStickOutImageWidth(leftTop, rightTop, rightBottom, leftBottom)) ||
                   (DoStickOutImageHeight(leftTop, rightTop, rightBottom, leftBottom));
        }

        private bool DoStickOutImageWidth(System.Windows.Point leftTop,
                                          System.Windows.Point rightTop,
                                          System.Windows.Point rightBottom,
                                          System.Windows.Point leftBottom)
        {
            double leftEnd = Math.Min(leftTop.X, leftBottom.X);
            if (leftEnd < 0)
            {
                return true;
            }

            double rightEnd = Math.Max(rightTop.X, rightBottom.X);
            if (rightEnd > MaxRight)
            {
                return true;
            }

            return false;
        }

        private bool DoStickOutImageHeight(System.Windows.Point leftTop,
                                           System.Windows.Point rightTop,
                                           System.Windows.Point rightBottom,
                                           System.Windows.Point leftBottom)
        {
            double topEnd = Math.Min(leftTop.Y, rightTop.Y);
            if (topEnd < 0)
            {
                return true;
            }

            double bottomEnd = Math.Max(leftBottom.Y, rightBottom.Y);
            if (bottomEnd > MaxBottom)
            {
                return true;
            }

            return false;
        }
    }
}
