using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyTrimmingNew2
{
    class CutLineDisplay
    {
        internal static void Update(MainWindow main, CutLine cutLine)
        {
            System.Windows.Point leftTop = new System.Windows.Point(cutLine.Left, cutLine.Top);
            System.Windows.Point rightTop = new System.Windows.Point(cutLine.Right, cutLine.Top);
            System.Windows.Point rightBottom = new System.Windows.Point(cutLine.Right, cutLine.Bottom);
            System.Windows.Point leftBottom = new System.Windows.Point(cutLine.Left, cutLine.Bottom);

            main.CutLine.Points[0] = leftTop;
            main.CutLine.Points[1] = rightTop;
            main.CutLine.Points[2] = rightBottom;
            main.CutLine.Points[3] = leftBottom;
            main.CutLine.Points[4] = leftTop;  // 長方形として閉じる

            SetLabelCoordinate(leftTop, main.CutLineLeftTopX, main.CutLineLeftTopY);
            SetLabelCoordinate(rightTop, main.CutLineRightTopX, main.CutLineRightTopY);
            SetLabelCoordinate(rightBottom, main.CutLineRightBottomX, main.CutLineRightBottomY);
            SetLabelCoordinate(leftBottom, main.CutLineLeftBottomX, main.CutLineLeftBottomY);

            main.CutLineWidth.Content = ToDisplayString(cutLine.Width);
            main.CutLineHeight.Content = ToDisplayString(cutLine.Height);
        }

        private static void SetLabelCoordinate(System.Windows.Point p, Label labelX, Label labelY)
        {
            labelX.Content = ToDisplayString(p.X);
            labelY.Content = ToDisplayString(p.Y);
        }

        private static string ToDisplayString(double d)
        {
            return Math.Round(d, 2).ToString();
        }
    }
}
