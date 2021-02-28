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
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            Assert.AreEqual(expected: si.Width, actual: cl.Width);
            Assert.AreEqual(expected: 450, actual: cl.Height);    // 450 = (int)(800.0 * 9.0 / 16.0)
            Assert.AreEqual(expected: 0, actual: cl.Left);
            Assert.AreEqual(expected: 0, actual: cl.Top);

            si = CreateShowingImage("/Resource/test002.jpg", 800, 600);
            cl = new CutLine(si);

            Assert.AreEqual(expected: si.Width, actual: cl.Width);
            Assert.AreEqual(expected: 225, actual: cl.Height);    // 225 = (int)(450.0 * 9.0 / 16.0)
            Assert.AreEqual(expected: 0, actual: cl.Left);
            Assert.AreEqual(expected: 0, actual: cl.Top);
        }

        [TestMethod]
        public void TestUpAndDownCutLine()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
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

        [TestMethod]
        public void TestChangeSizeBaseRightBottom()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            // 縮小: 横幅の変化量が大きい場合
            int changeSizeX = -50;
            int changeSizeY = -10;
            int afterWidth = cl.Width + changeSizeX;
            int afterHeight = cl.Height - 28;    // 50 * 9 / 16 = 28.125
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 縮小: 縦幅の変化量が大きい場合
            changeSizeX = -10;
            changeSizeY = -50;
            afterWidth = cl.Width - 88;    // 50 * 16 / 9 = 88.888...
            afterHeight = cl.Height + changeSizeY;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 拡大: 横幅の変化量が大きい場合
            changeSizeX = 30;
            changeSizeY = -10;
            afterWidth = cl.Width + changeSizeX;
            afterHeight = cl.Height + 16;    // 30 * 9 / 16 = 16.875
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 拡大: 縦幅の変化量が大きい場合
            changeSizeX = -10;
            changeSizeY = 20;
            afterWidth = cl.Width + 35;    // 20 * 16 / 9 = 35.555...
            afterHeight = cl.Height + changeSizeY;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);
        }

        [TestMethod]
        public void TestChangeSizeDoNotStickOutOfImageBaseRightBottom()
        {
            // 拡大幅が大きすぎて画像をはみ出るような場合は画像いっぱいまでに制限する
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);
            int afterWidth = cl.Width;
            int afterHeight = cl.Height;

            // 横幅の変化量が大きい場合
            ChangeSizeBaseRightBottom(cl, -50, -50);    // まず適当に小さくする
            int changeSizeX = 1000;
            int changeSizeY = 0;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 縦幅の変化量が大きい場合
            ChangeSizeBaseRightBottom(cl, -50, -50);    // まず適当に小さくする
            changeSizeX = 0;
            changeSizeY = 1000;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);
        }

        private ShowingImage CreateShowingImage(string imagePathBase, int imageAreaWidth, int imageAreaHeight)
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment(imagePathBase);
            OriginalImage oi = new OriginalImage(imagePath);
            return new ShowingImage(oi, imageAreaWidth, imageAreaHeight);
        }

        private void ChangeSizeBaseRightBottomWithCheck(CutLine cutLine, int changeSizeX, int changeSizeY, int expectedWidth, int expectedHeight)
        {
            ChangeSizeBaseRightBottom(cutLine, changeSizeX, changeSizeY);
            Assert.AreEqual(expected: expectedWidth, actual: cutLine.Width);
            Assert.AreEqual(expected: expectedHeight, actual: cutLine.Height);
        }

        private void ChangeSizeBaseRightBottom(CutLine cutLine, int changeSizeX, int changeSizeY)
        {
            System.Windows.Point dragStart = new System.Windows.Point(cutLine.Right, cutLine.Bottom);
            System.Windows.Point drop = new System.Windows.Point(cutLine.Right + changeSizeX, cutLine.Bottom + changeSizeY);
            cutLine.ChangeSizeBaseRightBottom(dragStart, drop);
        }
    }
}
