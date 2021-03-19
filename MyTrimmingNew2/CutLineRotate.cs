using System;
using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLineRotate : CutLineCommand
    {
        private double MaxRight { get; }

        private double MaxBottom { get; }

        private double RotateDegree { get; set; }

        public CutLineRotate(CutLine cutLine, ShowingImage image, Key key, int keyInputNum) : base(cutLine)
        {
            MaxRight = image.Width;
            MaxBottom = image.Height;
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
            //double r = CalcR();
            //System.Windows.Point newLeftTop = CalcLeftTop(Before.ThetaDegree, RotateDegree);
            //System.Windows.Point newRightTop = RightTop(Before.ThetaDegree, RotateDegree);
            //System.Windows.Point newRightBottom = RightBottom(Before.ThetaDegree, RotateDegree);
            //System.Windows.Point newLeftBottom = LeftBottom(Before.ThetaDegree, RotateDegree);
            //return new CutLineParameter(newLeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree + RotateDegree);

            double centerX = CalcCenterX(Before.LeftTop, Before.RightBottom);
            double centerY = CalcCenterY(Before.LeftBottom, Before.RightTop);

            // メッセージ：計算結果が何かズレてる、少なくともWidthは回転前からズレる
            System.Windows.Point newLeftTop = CalcRotatePoint(Before.LeftTop, centerX, centerY, RotateDegree);
            System.Windows.Point newRightTop = CalcRotatePoint(Before.RightTop, centerX, centerY, RotateDegree);
            System.Windows.Point newRightBottom = CalcRotatePoint(Before.RightBottom, centerX, centerY, RotateDegree);
            System.Windows.Point newLeftBottom = CalcRotatePoint(Before.LeftBottom, centerX, centerY, RotateDegree);
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

        private System.Windows.Point CalcRotatePoint(System.Windows.Point p, double centerX, double centerY, double degree)
        {
            double rad = ToRadian(degree);
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

        //public double CalcR()
        //{
        //    return Math.Sqrt(Math.Pow(Before.LeftTop.X - CenterX, 2) + Math.Pow(Top - CenterY, 2));
        //}

        //private System.Windows.Point CalcLeftTop(double thetaDegree, double rotateDegree)
        //{
        //    double thetaMinus = thetaDegree - rotateDegree;

        //    // LeftTop -> -180 <= Degree < -90
        //    double newX = Before.R * Math.Cos(ToRadian(thetaMinus - 180)) + Before.CenterX;
        //    double newY = Before.R * Math.Sin(ToRadian(thetaMinus - 180)) + Before.CenterY;
        //    return new System.Windows.Point(newX, newY);
        //}

        //private System.Windows.Point RightTop()
        //{
        //    // RightTop -> -90 <= Degree < 0
        //    double theta = ThetaBase - Degree - 90;
        //    double newX = R * Math.Cos(ToRadian(theta)) + CenterX;
        //    double newY = R * Math.Sin(ToRadian(theta)) + CenterY;
        //    return new System.Windows.Point(newX, newY);
        //}

        //private System.Windows.Point RightBottom()
        //{
        //    // RightBottom -> 0 <= Degree < 90
        //    double theta = ThetaBase + Degree;
        //    double newX = R * Math.Cos(ToRadian(theta)) + CenterX;
        //    double newY = R * Math.Sin(ToRadian(theta)) + CenterY;
        //    return new System.Windows.Point(newX, newY);
        //}

        //private System.Windows.Point LeftBottom()
        //{
        //    // LeftBottom -> 90 <= Degree < 180
        //    double theta = 180 - (ThetaBase - Degree);
        //    double newX = R * Math.Cos(ToRadian(theta)) + CenterX;
        //    double newY = R * Math.Sin(ToRadian(theta)) + CenterY;
        //    return new System.Windows.Point(newX, newY);
        //}

        //private void SetTheta()
        //{
        //    double rad = Math.Atan((Top - CenterY) / (Left - CenterX));
        //    ThetaBase = (180.0 * rad) / Math.PI;
        //}

        //private void SetCenter()
        //{
        //    CenterX = (RightBottom.X + LeftTop.X) / 2.0;
        //    CenterY = (LeftBottom.Y + RightTop.Y) / 2.0;
        //}

        //private double ThetaBase { get; set; }

        //public double ThetaDegree
        //{
        //    get
        //    {
        //        return ThetaBase + Degree;
        //    }
        //}
    }
}