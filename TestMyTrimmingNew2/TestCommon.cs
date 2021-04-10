using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMyTrimmingNew2
{
    [TestClass]
    public class TestCommon
    {
        [TestMethod]
        public void TestCalcRotatePoint()
        {
            // テスト作成当時の計算結果を正とする
            System.Windows.Point input = new System.Windows.Point(100, 200);
            double centerX = 120;
            double centerY = 150;
            double rad = MyTrimmingNew2.Common.ToRadian(120);
            System.Windows.Point p = MyTrimmingNew2.Common.CalcRotatePoint(input, centerX, centerY, rad);
            Common.AreEqualRound(86.69873, p.X, 5);    // 86.6987298107781
            Common.AreEqualRound(107.67949, actual: p.Y, 5);    // 107.679491924311
        }
    }
}
