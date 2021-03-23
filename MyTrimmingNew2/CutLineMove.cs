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
        private double XDirection { get; set; }

        private double YDirection { get; set; }

        public CutLineMove(CutLine cutLine,
                           ShowingImage image,
                           Key key,
                           System.Windows.Input.ModifierKeys modifierKey,
                           int keyInputNum) : base (cutLine, image)
        {
            CalcMoveDistance(key, modifierKey, keyInputNum);
        }

        public CutLineMove(CutLine cutLine, ShowingImage image, System.Windows.Point dragStart, System.Windows.Point drop) : base(cutLine, image)
        {
            CalcMoveDistance(dragStart, drop);
        }

        private void CalcMoveDistance(Key key, ModifierKeys modifierKey, int num)
        {
            int dist = num;
            if (modifierKey == ModifierKeys.Shift)
            {
                // TODO: 定数の外部管理化
                dist = num * 10;
            }
            CalcMoveDistance(key, dist);
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

        private void CalcMoveDistance(System.Windows.Point dragStart, System.Windows.Point drop)
        {
            XDirection = drop.X - dragStart.X;
            YDirection = drop.Y - dragStart.Y;
        }

        protected override CutLineParameter CalcNewParameterCore()
        {
            double xDistance = CalcXDistance(XDirection);
            double yDistance = CalcYDistance(YDirection);
            System.Windows.Point newLeftTop = GetNewPoint(Before.LeftTop, xDistance, yDistance);
            System.Windows.Point newRightTop = GetNewPoint(Before.RightTop, xDistance, yDistance);
            System.Windows.Point newRightBottom = GetNewPoint(Before.RightBottom, xDistance, yDistance);
            System.Windows.Point newLeftBottom = GetNewPoint(Before.LeftBottom, xDistance, yDistance);
            return new CutLineParameter(newLeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree);
        }

        private double CalcXDistance(double xDirection)
        {
            double newLeft = Before.LeftEnd + xDirection;
            double newRight = Before.RightEnd + xDirection;

            if (DoLeftStickOutOfImage(newLeft))
            {
                return -Before.LeftEnd;
            }
            else if (DoRightStickOutOfImage(newRight))
            {
                return MaxRight - Before.RightEnd;
            }

            return xDirection;
        }

        private bool DoLeftStickOutOfImage(double left)
        {
            return (left < 0);
        }

        private bool DoRightStickOutOfImage(double right)
        {
            return (right > MaxRight);
        }

        private double CalcYDistance(double yDirection)
        {
            double newTop = Before.TopEnd + yDirection;
            double newBottom = Before.BottomEnd + yDirection;

            if (DoTopStickOutOfImage(newTop))
            {
                return -Before.TopEnd;
            }
            else if (DoBottomStickOutOfImage(newBottom))
            {
                return MaxBottom - Before.BottomEnd;
            }

            return yDirection;
        }

        private bool DoTopStickOutOfImage(double top)
        {
            return (top < 0);
        }

        private bool DoBottomStickOutOfImage(double bottom)
        {
            return (bottom > MaxBottom);
        }

        private System.Windows.Point GetNewPoint(System.Windows.Point p, double xDistance, double yDistance)
        {
            return new System.Windows.Point(p.X + xDistance, p.Y + yDistance);
        }
    }
}
