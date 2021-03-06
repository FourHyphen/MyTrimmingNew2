using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLineMove : CutLineCommand
    {
        private double MaxRight { get; }

        private double MaxBottom { get; }

        private double XDirection { get; set; }

        private double YDirection { get; set; }

        public CutLineMove(CutLine cutLine, ShowingImage image, Key key, int keyInputNum) : base (cutLine)
        {
            MaxRight = image.Width;
            MaxBottom = image.Height;
            CalcMoveDistance(key, keyInputNum);
        }

        private void CalcMoveDistance(Key key, int num)
        {
            XDirection = 0.0;
            YDirection = 0.0;
            if (key == Key.Left)
            {
                XDirection = -1 * num;
            }
            else if (key == Key.Right)
            {
                XDirection = 1 * num;
            }
            else if (key == Key.Up)
            {
                YDirection = -1 * num;
            }
            else if (key == Key.Down)
            {
                YDirection = 1 * num;
            }
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
