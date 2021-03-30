using System;
using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLineRotate : CutLineCommand
    {
        private double RotateDegree { get; set; }

        public CutLineRotate(CutLine cutLine,
                             ShowingImage image,
                             Key key,
                             ModifierKeys modifierKey,
                             int keyInputNum) : base(cutLine, image)
        {
            CalcRotateDegree(key, modifierKey, keyInputNum);
        }

        private void CalcRotateDegree(Key key, ModifierKeys modifierKey, int keyInputNum)
        {
            int rate = 1;
            if (modifierKey == ModifierKeys.Shift)
            {
                rate = 10;
            }

            if (key == Key.OemPlus)
            {
                RotateDegree = keyInputNum * rate;
            }
            else if (key == Key.OemMinus)
            {
                RotateDegree = -keyInputNum * rate;
            }
            else
            {
                throw new System.Exception("key must be OemPlus or OemMinus.");
            }
        }

        protected override CutLineParameter CalcNewParameterCore()
        {
            return CalcRotate();
        }

        private CutLineParameter CalcRotate()
        {
            double centerX = Common.CalcCenterX(Before.LeftTop, Before.RightBottom);
            double centerY = Common.CalcCenterY(Before.LeftBottom, Before.RightTop);
            double rotateRad = Common.ToRadian(RotateDegree);

            System.Windows.Point newLeftTop = Common.CalcRotatePoint(Before.LeftTop, centerX, centerY, rotateRad);
            System.Windows.Point newRightTop = Common.CalcRotatePoint(Before.RightTop, centerX, centerY, rotateRad);
            System.Windows.Point newRightBottom = Common.CalcRotatePoint(Before.RightBottom, centerX, centerY, rotateRad);
            System.Windows.Point newLeftBottom = Common.CalcRotatePoint(Before.LeftBottom, centerX, centerY, rotateRad);

            if (DoStickOutImage(newLeftTop, newRightTop, newRightBottom, newLeftBottom))
            {
                return Before;
            }

            return new CutLineParameter(newLeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree + RotateDegree);
        }
    }
}