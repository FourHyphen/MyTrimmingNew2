using System;
using System.Collections.Generic;
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
            string dirPath = Common.GetFilePathOfDependentEnvironment("/Resource");
            string resize01Path = System.IO.Path.Combine(dirPath, resize01Name);
            string imagePath = System.IO.Path.Combine(dirPath, "test001.jpg");
            OriginalImage originalImage = new OriginalImage(imagePath);

            List<string> willCreatingFilePaths = new List<string>();
            willCreatingFilePaths.Add(resize01Path);
            for (int i = 2; i <= 99; i++)
            {
                string replace = "resize_" + i.ToString().PadLeft(2, '0');
                string filePath = System.IO.Path.Combine(dirPath, resize01Path.Replace("resize_01", replace));
                willCreatingFilePaths.Add(filePath);
            }

            // 準備：前回の中間ファイルが残っているなら削除する
            foreach(string f in willCreatingFilePaths)
            {
                DeleteFile(f);
            }

            string example = originalImage.GetSaveImageNameExample(dirPath);
            Assert.IsTrue(example == resize01Name);

            // ファイルがすでにあるなら連番を進めるテスト
            System.IO.FileStream fs = System.IO.File.Create(resize01Path);
            fs.Close();
            List<string> createdFilePaths = new List<string>();
            createdFilePaths.Add(resize01Path);

            example = originalImage.GetSaveImageNameExample(dirPath);
            string resize02Name = "test001_resize_02.jpg";
            Assert.IsTrue(example == resize02Name);

            // 連番が99まで埋まっているなら別の文字列を返すテスト
            for(int i = 1; i < willCreatingFilePaths.Count; i++)
            {
                fs = System.IO.File.Create(willCreatingFilePaths[i]);
                fs.Close();
                createdFilePaths.Add(willCreatingFilePaths[i]);
            }

            example = originalImage.GetSaveImageNameExample(dirPath);
            Assert.IsTrue(example == "test001_resize.jpg");

            // 後始末
            foreach (string f in createdFilePaths)
            {
                DeleteFile(f);
            }
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
