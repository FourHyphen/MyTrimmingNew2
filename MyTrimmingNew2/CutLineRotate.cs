using System;
using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLineRotate : CutLineCommand
    {
        private double RotateDegree { get; set; }

        public CutLineRotate(CutLine cutLine, ShowingImage image, Key key, int keyInputNum) : base(cutLine, image)
        {
            CalcRotateDegree(key, keyInputNum);
        }

        private void CalcRotateDegree(System.Windows.Input.Key key, int keyInputNum)
        {
            if (key == Key.OemPlus)
            {
                RotateDegree = keyInputNum;
            }
            else if (key == Key.OemMinus)
            {
                RotateDegree = -keyInputNum;
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
            double centerX = CalcCenterX(Before.LeftTop, Before.RightBottom);
            double centerY = CalcCenterY(Before.LeftBottom, Before.RightTop);
            double rotateRad = ToRadian(RotateDegree);

            System.Windows.Point newLeftTop = CalcRotatePoint(Before.LeftTop, centerX, centerY, rotateRad);
            System.Windows.Point newRightTop = CalcRotatePoint(Before.RightTop, centerX, centerY, rotateRad);
            System.Windows.Point newRightBottom = CalcRotatePoint(Before.RightBottom, centerX, centerY, rotateRad);
            System.Windows.Point newLeftBottom = CalcRotatePoint(Before.LeftBottom, centerX, centerY, rotateRad);

            if (DoStickOutImage(newLeftTop, newRightTop, newRightBottom, newLeftBottom))
            {
                return Before;
            }

            return new CutLineParameter(newLeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree + RotateDegree);
        }

        private double CalcCenterX(System.Windows.Point p1, System.Windows.Point p2)
        {
            return (p1.X + p2.X) / 2.0;
        }

        private double CalcCenterY(System.Windows.Point p1, System.Windows.Point p2)
        {
            return (p1.Y + p2.Y) / 2.0;
        }

        private System.Windows.Point CalcRotatePoint(System.Windows.Point p, double centerX, double centerY, double rad)
        {
            double x = p.X - centerX;
            double y = p.Y - centerY;
            double rotateX = x * Math.Cos(rad) - y * Math.Sin(rad);
            double rotateY = y * Math.Cos(rad) + x * Math.Sin(rad);
            return new System.Windows.Point(rotateX + centerX, rotateY + centerY);
        }

        private double ToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}