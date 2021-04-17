using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMyTrimmingNew2
{
    [TestClass]
    public class TestImageTrim
    {
        private string BeforeEnvironment { get; set; }

        [TestInitialize]
        public void TestInit()
        {
            BeforeEnvironment = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Common.GetEnvironmentDirPath();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.CurrentDirectory = BeforeEnvironment;
        }

        [TestMethod]
        public void TestCreate()
        {
            // 現在の実装を正とする
            // 処理
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            MyTrimmingNew2.ImageTrim it = new MyTrimmingNew2.ImageTrim(imagePath,
                                                                       new System.Windows.Point(2120.27675513433, 1435.71887930538),
                                                                       new System.Windows.Point(2273.53956790268, 1491.50198117297),
                                                                       new System.Windows.Point(2242.16157310216, 1577.71231335517),
                                                                       new System.Windows.Point(2088.89876033381, 1521.92921148758),
                                                                       20);
            System.Drawing.Bitmap result = it.Create(MyTrimmingNew2.ImageProcess.Interpolate.PixelMixing, 0.5);

            // 正解の読み込み
            string answerPath = Common.GetFilePathOfDependentEnvironment("/Resource/test001_TestCreate_Answer.jpg");
            System.Drawing.Bitmap answer = new System.Drawing.Bitmap(answerPath);

            // 比較
            Assert.IsTrue(AreEqualBitmap(result, answer));
        }

        private bool AreEqualBitmap(System.Drawing.Bitmap result, System.Drawing.Bitmap answer)
        {
            // 計算誤差吸収、全Pixelの完全一致でなくても良しとする
            int diffPixelSum = 0;

            Assert.AreEqual(expected: result.Width, actual: answer.Width);
            Assert.AreEqual(expected: result.Height, actual: answer.Height);
            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    if (!(result.GetPixel(x, y) == answer.GetPixel(x, y)))
                    {
                        diffPixelSum++;
                    }
                }
            }

            // 予備実験の結果 3.35% (= 494 / 14742) の誤差が実際に出た
            return (diffPixelSum <= 494);
        }
    }
}
