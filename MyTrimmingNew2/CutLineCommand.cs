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

        public CutLineParameter CalcNewParameter()
        {
            After = CalcNewParameterCore();
            return After;
        }

        protected abstract CutLineParameter CalcNewParameterCore();

        protected bool DoStickOutImageOfAfterParameter(System.Windows.Point newLeftTop,
                                                       System.Windows.Point newRightTop,
                                                       System.Windows.Point newRightBottom,
                                                       System.Windows.Point newLeftBottom)
        {
            double leftEnd = Math.Min(newLeftTop.X, newLeftBottom.X);
            if (leftEnd < 0)
            {
                return true;
            }

            double rightEnd = Math.Max(newRightTop.X, newRightBottom.X);
            if (rightEnd > MaxRight)
            {
                return true;
            }

            double topEnd = Math.Min(newLeftTop.Y, newRightTop.Y);
            if (topEnd < 0)
            {
                return true;
            }

            double bottomEnd = Math.Max(newLeftBottom.Y, newRightBottom.Y);
            if (bottomEnd > MaxBottom)
            {
                return true;
            }

            return false;
        }
    }
}
