using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTrimmingNew2;

namespace TestMyTrimmingNew2
{
    [TestClass]
    public class TestShowingImage
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
        public void TestSuccessOfCreateShowingImage()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            int imageAreaWidth = 800;
            int imageAreaHeight = 600;
            ShowingImage showingImage = new ShowingImage(imagePath, imageAreaWidth, imageAreaHeight);
            Assert.AreEqual(expected: imageAreaWidth, actual: showingImage.Width);
            Assert.AreEqual(expected: 533, actual: showingImage.Height);
        }
    }
}
