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
        internal static void Update(MainWindow main, ShowingImage showingImage, CutLine cutLine)
        {
            main.CutLine.Points[0] = cutLine.LeftTop;
            main.CutLine.Points[1] = cutLine.RightTop;
            main.CutLine.Points[2] = cutLine.RightBottom;
            main.CutLine.Points[3] = cutLine.LeftBottom;
            main.CutLine.Points[4] = cutLine.LeftTop;  // 長方形として閉じる

            SetLabelCoordinate(cutLine.LeftTop, main.CutLineLeftTopX, main.CutLineLeftTopY);
            SetLabelCoordinate(cutLine.RightTop, main.CutLineRightTopX, main.CutLineRightTopY);
            SetLabelCoordinate(cutLine.RightBottom, main.CutLineRightBottomX, main.CutLineRightBottomY);
            SetLabelCoordinate(cutLine.LeftBottom, main.CutLineLeftBottomX, main.CutLineLeftBottomY);

            main.CutLineWidth.Content = ToDisplayString(cutLine.Width);
            main.CutLineHeight.Content = ToDisplayString(cutLine.Height);
            main.CutLineRotateDegree.Content = ToDisplayString(cutLine.Degree);

            main.CutSizeWidth.Content = ToDisplayString(cutLine.Width / showingImage.Ratio);
            main.CutSizeHeight.Content = ToDisplayString(cutLine.Height / showingImage.Ratio);
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
