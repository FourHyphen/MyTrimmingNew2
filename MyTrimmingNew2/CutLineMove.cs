using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineMove : CutLineCommand
    {
        private double MaxRight { get; }

        private double MaxBottom { get; }

        private double XDirection { get; }

        private double YDirection { get; }

        public CutLineMove(CutLine cutLine, ShowingImage image, double xDirection, double yDirection) : base (cutLine)
        {
            MaxRight = image.Width;
            MaxBottom = image.Height;
            XDirection = xDirection;
            YDirection = yDirection;
        }

        protected override CutLineParameter CalcNewParameterCore()
        {
            double newLeft = MoveX(XDirection);
            double newTop = MoveY(YDirection);
            return new CutLineParameter(newLeft, newTop, Before.Width, Before.Height);
        }

        private double MoveX(double xDirection)
        {
            double newLeft = Before.Left + xDirection;
            double newRight = Before.Right + xDirection;
            return AdjustLeft(newLeft, newRight);
        }

        private double AdjustLeft(double newLeft, double newRight)
        {
            if (DoLeftStickOutOfImage(newLeft))
            {
                return 0.0;
            }
            else if (DoRightStickOutOfImage(newRight))
            {
                return MaxRight - Before.Width;
            }

            return newLeft;
        }

        private bool DoLeftStickOutOfImage(double left)
        {
            return (left < 0);
        }

        private bool DoRightStickOutOfImage(double right)
        {
            return (right > MaxRight);
        }

        private double MoveY(double yDirection)
        {
            double newTop = Before.Top + yDirection;
            double newBottom = Before.Top + Before.Height + yDirection;
            return AdjustTop(newTop, newBottom);
        }

        private double AdjustTop(double newTop, double newBottom)
        {
            if (DoTopStickOutOfImage(newTop))
            {
                return 0;
            }
            else if (DoBottomStickOutOfImage(newBottom))
            {
                return MaxBottom - Before.Height;
            }

            return newTop;
        }

        private bool DoTopStickOutOfImage(double top)
        {
            return (top < 0);
        }

        private bool DoBottomStickOutOfImage(double bottom)
        {
            return (bottom > MaxBottom);
        }
    }
}
