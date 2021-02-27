using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTrimmingNew2;

namespace TestMyTrimmingNew2
{
    [TestClass]
    public class TestCutLine
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
        public void TestSuccessOfCreateInstance()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            OriginalImage oi = new OriginalImage(imagePath);
            ShowingImage si = new ShowingImage(oi, 800, 600);
            CutLine cl = new CutLine(si);

            Assert.AreEqual(expected: si.Width, actual: cl.Width);
            Assert.AreEqual(expected: 450, actual: cl.Height);    // 450 = (int)(800.0 * 9.0 / 16.0)
            Assert.AreEqual(expected: 0, actual: cl.Left);
            Assert.AreEqual(expected: 0, actual: cl.Top);

            imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test002.jpg");
            oi = new OriginalImage(imagePath);
            si = new ShowingImage(oi, 800, 600);
            cl = new CutLine(si);

            Assert.AreEqual(expected: si.Width, actual: cl.Width);
            Assert.AreEqual(expected: 225, actual: cl.Height);    // 225 = (int)(450.0 * 9.0 / 16.0)
            Assert.AreEqual(expected: 0, actual: cl.Left);
            Assert.AreEqual(expected: 0, actual: cl.Top);
        }

        [TestMethod]
        public void TestUpAndDownCutLine()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            OriginalImage oi = new OriginalImage(imagePath);
            ShowingImage si = new ShowingImage(oi, 800, 600);
            CutLine cl = new CutLine(si);
            int maxTop = si.Height - cl.Height;

            Assert.AreEqual(expected: 0, actual: cl.Top);
            cl.MoveY(-1);
            Assert.AreEqual(expected: 0, actual: cl.Top);
            cl.MoveY(1);
            Assert.AreEqual(expected: 1, actual: cl.Top);
            cl.MoveY(50);
            Assert.AreEqual(expected: 51, actual: cl.Top);
            cl.MoveY(1000);
            Assert.AreEqual(expected: maxTop, actual: cl.Top);
            cl.MoveY(-1);
            Assert.AreEqual(expected: maxTop - 1, actual: cl.Top);
        }
    }
}
