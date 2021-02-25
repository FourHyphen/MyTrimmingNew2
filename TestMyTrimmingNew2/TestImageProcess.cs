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
            System.Windows.Media.Imaging.BitmapSource bs = MyTrimmingNew2.MyImage.GetShowImage(imagePath);
            Assert.AreEqual(expected: 3840, actual: bs.PixelWidth);
            Assert.AreEqual(expected: 2560, actual: bs.PixelHeight);
        }
    }
}
