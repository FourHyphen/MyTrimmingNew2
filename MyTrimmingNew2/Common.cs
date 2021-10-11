using System;

namespace MyTrimmingNew2
{
    public class Common
    {
        private Common() { }

        public static double CalcCenterX(System.Windows.Point p1, System.Windows.Point p2)
        {
            return (p1.X + p2.X) / 2.0;
        }

        public static double CalcCenterY(System.Windows.Point p1, System.Windows.Point p2)
        {
            return (p1.Y + p2.Y) / 2.0;
        }

        public static double CalcDistance(System.Windows.Point p1, System.Windows.Point p2)
        {
            double xDiff = p1.X - p2.X;
            double yDiff = p1.Y - p2.Y;
            return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public static double CalcDistance((double X, double Y) p1, (double X, double Y) p2)
        {
            double xDiff = p1.X - p2.X;
            double yDiff = p1.Y - p2.Y;
            return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public static double ToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }

        public static System.Windows.Point CalcRotatePoint(System.Windows.Point p, double centerX, double centerY, double rad)
        {
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);
            return CalcRotatePoint(p, centerX, centerY, cos, sin);
        }

        public static System.Windows.Point CalcRotatePoint(System.Windows.Point p, double centerX, double centerY, double cos, double sin)
        {
            double x = p.X - centerX;
            double y = p.Y - centerY;
            double rotateX = x * cos - y * sin;
            double rotateY = y * cos + x * sin;
            return new System.Windows.Point(rotateX + centerX, rotateY + centerY);
        }

        public static (double X, double Y) CalcRotatePoint(double x, double y, double centerX, double centerY, double cos, double sin)
        {
            double tmpX = x - centerX;
            double tmpY = y - centerY;
            double rotateX = tmpX * cos - tmpY * sin;
            double rotateY = tmpY * cos + tmpX * sin;
            return (rotateX + centerX, rotateY + centerY);
        }
    }
}
