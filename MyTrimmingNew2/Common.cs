using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2.0) + Math.Pow(p1.Y - p2.Y, 2.0));
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
    }
}
