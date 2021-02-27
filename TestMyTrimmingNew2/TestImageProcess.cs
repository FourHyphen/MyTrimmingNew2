using System;
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
    }
}
