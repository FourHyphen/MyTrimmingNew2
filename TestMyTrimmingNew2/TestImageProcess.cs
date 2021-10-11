using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMyTrimmingNew2
{
    [TestClass]
    public class TestImageProcess
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
        public void TestSuccessOfCreateShowImage()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");

            // 等倍サイズのチェック
            int imageWidth = 3840;
            int imageHeight = 2560;
            System.Windows.Media.Imaging.BitmapSource bs = MyTrimmingNew2.ImageProcess.GetShowImage(imagePath, imageWidth, imageHeight);
            Assert.AreEqual(expected: imageWidth, actual: bs.PixelWidth);
            Assert.AreEqual(expected: imageHeight, actual: bs.PixelHeight);

            // 縮小サイズのチェック、縮小率は明確な理由なし
            int resizeWidth = 3840 / 2;
            int resizeHeight = 2560 / 4;
            System.Windows.Media.Imaging.BitmapSource bsResize = MyTrimmingNew2.ImageProcess.GetShowImage(imagePath, resizeWidth, resizeHeight);
            Assert.AreEqual(expected: resizeWidth, actual: bsResize.PixelWidth);
            Assert.AreEqual(expected: resizeHeight, actual: bsResize.PixelHeight);
        }

        [TestMethod]
        public void TestFakePixelMixing()
        {
            // テスト作成時点の実際の計算結果を正とする
            byte[] rBuf = new byte[9];
            byte[] gBuf = new byte[9];
            byte[] bBuf = new byte[9];
            CreateTestFilter(ref rBuf, ref gBuf, ref bBuf);

            // (1) 中央 -> テスト画像では 200 万 pixel のうち一度も中央にならなかったため、中央ロジックを削除したためテスト対象外

            // 5x5 サイズの bitmap の (2, 2) = 中心 の 3x3 範囲のフィルタを入力とする
            int x = 2, y = 2, bitmapWidth = 5, bitmapHeight = 5;

            // (2) 左上方向
            (double X, double Y) rotate = (1.8, 1.9);
            System.Drawing.Color c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(rBuf, gBuf, bBuf, x, y, bitmapWidth, bitmapHeight, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(72, 62, 87), actual: c);

            // (3) 右上方向
            rotate = (2.3, 1.7);
            c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(rBuf, gBuf, bBuf, x, y, bitmapWidth, bitmapHeight, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(138, 55, 80), actual: c);

            // (4) 左下方向
            rotate = (1.6, 2.4);
            c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(rBuf, gBuf, bBuf, x, y, bitmapWidth, bitmapHeight, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(55, 97, 122), actual: c);

            // (5) 右下方向
            rotate = (2.2, 2.1);
            c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(rBuf, gBuf, bBuf, x, y, bitmapWidth, bitmapHeight, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(127, 87, 112), actual: c);
        }

        [TestMethod]
        public void TestUnsharpFilter()
        {
            // テスト作成時点の実際の計算結果を正とする
            byte[] rBuf = new byte[9];
            byte[] gBuf = new byte[9];
            byte[] bBuf = new byte[9];
            CreateTestFilter(ref rBuf, ref gBuf, ref bBuf);

            System.Drawing.Color expect = System.Drawing.Color.FromArgb(100, 74, 100);
            System.Drawing.Color result = MyTrimmingNew2.ImageProcess.ApplyUnsharpFilter(rBuf, gBuf, bBuf, 0.5);
            Assert.AreEqual(expected: expect, actual: result);

            expect = System.Drawing.Color.FromArgb(100, 75, 100);
            result = MyTrimmingNew2.ImageProcess.ApplyUnsharpFilter(rBuf, gBuf, bBuf, 0.2);
            Assert.AreEqual(expected: expect, actual: result);
        }

        private void CreateTestFilter(ref byte[] rBuf, ref byte[] gBuf, ref byte[] bBuf)
        {
            System.Drawing.Color c1 = System.Drawing.Color.FromArgb(0, 25, 50);
            System.Drawing.Color c2 = System.Drawing.Color.FromArgb(100, 25, 50);
            System.Drawing.Color c3 = System.Drawing.Color.FromArgb(200, 25, 50);
            System.Drawing.Color c4 = System.Drawing.Color.FromArgb(0, 75, 100);
            System.Drawing.Color c5 = System.Drawing.Color.FromArgb(100, 75, 100);
            System.Drawing.Color c6 = System.Drawing.Color.FromArgb(200, 75, 100);
            System.Drawing.Color c7 = System.Drawing.Color.FromArgb(0, 125, 150);
            System.Drawing.Color c8 = System.Drawing.Color.FromArgb(100, 125, 150);
            System.Drawing.Color c9 = System.Drawing.Color.FromArgb(200, 125, 150);
            rBuf[0] = c1.R; gBuf[0] = c1.G; bBuf[0] = c1.B;
            rBuf[1] = c2.R; gBuf[1] = c2.G; bBuf[1] = c2.B;
            rBuf[2] = c3.R; gBuf[2] = c3.G; bBuf[2] = c3.B;
            rBuf[3] = c4.R; gBuf[3] = c4.G; bBuf[3] = c4.B;
            rBuf[4] = c5.R; gBuf[4] = c5.G; bBuf[4] = c5.B;
            rBuf[5] = c6.R; gBuf[5] = c6.G; bBuf[5] = c6.B;
            rBuf[6] = c7.R; gBuf[6] = c7.G; bBuf[6] = c7.B;
            rBuf[7] = c8.R; gBuf[7] = c8.G; bBuf[7] = c8.B;
            rBuf[8] = c9.R; gBuf[8] = c9.G; bBuf[8] = c9.B;
        }
    }
}
