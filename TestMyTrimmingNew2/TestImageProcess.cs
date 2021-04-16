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
            System.Drawing.Bitmap bitmap = CreateTestBitmap();

            // (1) 中央
            System.Windows.Point rotate = new System.Windows.Point(1, 1);
            System.Drawing.Color c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(bitmap, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(100, 75, 100), actual: c);

            // (2) 左上方向
            rotate = new System.Windows.Point(0.8, 0.9);
            c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(bitmap, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(72, 62, 87), actual: c);

            // (3) 右上方向
            rotate = new System.Windows.Point(1.3, 0.7);
            c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(bitmap, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(138, 55, 80), actual: c);

            // (4) 左下方向
            rotate = new System.Windows.Point(0.6, 1.4);
            c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(bitmap, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(55, 97, 122), actual: c);

            // (5) 右下方向
            rotate = new System.Windows.Point(1.2, 1.1);
            c = MyTrimmingNew2.ImageProcess.GetPixelColorFakePixelMixing(bitmap, rotate);
            Assert.AreEqual(expected: System.Drawing.Color.FromArgb(127, 87, 112), actual: c);
        }

        [TestMethod]
        public void TestUnsharpFilter()
        {
            // テスト作成時点の実際の計算結果を正とする
            System.Drawing.Bitmap bitmap = CreateTestBitmap();
            System.Drawing.Color c1 = bitmap.GetPixel(0, 0);
            System.Drawing.Color c2 = bitmap.GetPixel(1, 0);
            System.Drawing.Color c3 = bitmap.GetPixel(2, 0);
            System.Drawing.Color c4 = bitmap.GetPixel(0, 1);
            System.Drawing.Color c5 = bitmap.GetPixel(1, 1);
            System.Drawing.Color c6 = bitmap.GetPixel(2, 1);
            System.Drawing.Color c7 = bitmap.GetPixel(0, 2);
            System.Drawing.Color c8 = bitmap.GetPixel(1, 2);
            System.Drawing.Color c9 = bitmap.GetPixel(2, 2);
            List<System.Drawing.Color> cs = new List<System.Drawing.Color>() { c1, c2, c3, c4, c5, c6, c7, c8, c9 };

            System.Drawing.Color expect = System.Drawing.Color.FromArgb(100, 74, 100);
            System.Drawing.Color result = MyTrimmingNew2.ImageProcess.ApplyUnsharpFilter(cs, 0.5);
            Assert.AreEqual(expected: expect, actual: result);

            expect = System.Drawing.Color.FromArgb(100, 75, 100);
            result = MyTrimmingNew2.ImageProcess.ApplyUnsharpFilter(cs, 0.2);
            Assert.AreEqual(expected: expect, actual: result);
        }

        private System.Drawing.Bitmap CreateTestBitmap()
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(3, 3);
            bitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(0, 25, 50));
            bitmap.SetPixel(0, 1, System.Drawing.Color.FromArgb(0, 75, 100));
            bitmap.SetPixel(0, 2, System.Drawing.Color.FromArgb(0, 125, 150));
            bitmap.SetPixel(1, 0, System.Drawing.Color.FromArgb(100, 25, 50));
            bitmap.SetPixel(1, 1, System.Drawing.Color.FromArgb(100, 75, 100));
            bitmap.SetPixel(1, 2, System.Drawing.Color.FromArgb(100, 125, 150));
            bitmap.SetPixel(2, 0, System.Drawing.Color.FromArgb(200, 25, 50));
            bitmap.SetPixel(2, 1, System.Drawing.Color.FromArgb(200, 75, 100));
            bitmap.SetPixel(2, 2, System.Drawing.Color.FromArgb(200, 125, 150));
            return bitmap;
        }
    }
}
