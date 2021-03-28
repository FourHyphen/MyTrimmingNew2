using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTrimmingNew2;

namespace TestMyTrimmingNew2
{
    [TestClass]
    public class TestOriginalImage
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
            OriginalImage originalImage = new OriginalImage(imagePath);
            Assert.AreEqual(expected: 3840, actual: originalImage.Width);
            Assert.AreEqual(expected: 2560, actual: originalImage.Height);

            string imagePath2 = Common.GetFilePathOfDependentEnvironment("/Resource/test002.jpg");
            OriginalImage originalImage2 = new OriginalImage(imagePath2);
            Assert.AreEqual(expected: 1920, actual: originalImage2.Width);
            Assert.AreEqual(expected: 2880, actual: originalImage2.Height);
        }

        [TestMethod]
        public void TestGetSaveNameExample()
        {
            string resize01Name = "test001_resize_01.jpg";
            string resize01Path = Common.GetFilePathOfDependentEnvironment("/Resource/" + resize01Name);
            DeleteFile(resize01Path);

            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            OriginalImage originalImage = new OriginalImage(imagePath);

            string example = originalImage.GetSaveImageNameExample();
            Assert.IsTrue(example == resize01Name);

            // ファイルがすでにあるなら連番を進める
            System.IO.File.Create(resize01Path);
            example = originalImage.GetSaveImageNameExample();
            string resize02Name = "test001_resize_02.jpg";
            Assert.IsTrue(example == resize02Name);

            DeleteFile(resize01Path);
        }

        private void DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
