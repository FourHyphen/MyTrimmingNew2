using System;
using System.Windows;
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
            double maxTop = si.Height - cl.Height;

            Assert.AreEqual(expected: 0, actual: cl.Top);
            Move(cl, System.Windows.Input.Key.Up, 1);
            Assert.AreEqual(expected: 0, actual: cl.Top);
            Move(cl, System.Windows.Input.Key.Down, 1);
            Assert.AreEqual(expected: 1, actual: cl.Top);
            Move(cl, System.Windows.Input.Key.Down, 50);
            Assert.AreEqual(expected: 51, actual: cl.Top);
            Move(cl, System.Windows.Input.Key.Down, 1000);
            Assert.AreEqual(expected: maxTop, actual: cl.Top);
            Move(cl, System.Windows.Input.Key.Up, 1);
            Assert.AreEqual(expected: maxTop - 1, actual: cl.Top);
        }

        [TestMethod]
        public void TestChangeSizeBaseRightBottom()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            // 縮小: 横幅の変化量が大きい場合
            double changeSizeX = -50;
            double changeSizeY = -10;
            double afterWidth = cl.Width + changeSizeX;
            double afterHeight = cl.Height - (50.0 * 9.0 / 16.0);
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 縮小: 縦幅の変化量が大きい場合
            changeSizeX = -10;
            changeSizeY = -50;
            afterWidth = cl.Width - (50.0 * 16.0 / 9.0);
            afterHeight = cl.Height + changeSizeY;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 拡大: 横幅の変化量が大きい場合
            changeSizeX = 30;
            changeSizeY = -10;
            afterWidth = cl.Width + changeSizeX;
            afterHeight = cl.Height + (30.0 * 9.0 / 16.0);
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 拡大: 縦幅の変化量が大きい場合
            changeSizeX = -10;
            changeSizeY = 20;
            afterWidth = cl.Width + (20.0 * 16.0 / 9.0);
            afterHeight = cl.Height + changeSizeY;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);
        }

        [TestMethod]
        public void TestIsPointNearRightBottom()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            System.Windows.Point near = new System.Windows.Point(cl.Right - 10, cl.Bottom - 10);
            System.Windows.Point far = new System.Windows.Point(cl.Right - 21, cl.Bottom - 21);
            Assert.IsTrue(cl.IsPointNearRightBottom(near));
            Assert.IsFalse(cl.IsPointNearRightBottom(far));

            near = new System.Windows.Point(cl.Right + 10, cl.Bottom + 10);
            far = new System.Windows.Point(cl.Right + 21, cl.Bottom + 21);
            Assert.IsTrue(cl.IsPointNearRightBottom(near));
            Assert.IsFalse(cl.IsPointNearRightBottom(far));
        }

        [TestMethod]
        public void TestChangeSizeDoNotStickOutOfImageBaseRightBottom()
        {
            // 拡大幅が大きすぎて画像をはみ出るような場合は画像いっぱいまでに制限する
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);
            double afterWidth = cl.Width;
            double afterHeight = cl.Height;

            // 横幅の変化量が大きい場合
            ChangeSizeBaseRightBottom(cl, -50, -50);    // まず適当に小さくする
            double changeSizeX = 1000;
            double changeSizeY = 0;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);

            // 縦幅の変化量が大きい場合
            ChangeSizeBaseRightBottom(cl, -50, -50);    // まず適当に小さくする
            changeSizeX = 0;
            changeSizeY = 1000;
            ChangeSizeBaseRightBottomWithCheck(cl, changeSizeX, changeSizeY, afterWidth, afterHeight);
        }

        [TestMethod]
        public void TestMoveLeftAndRightCutLine()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);
            ChangeSizeBaseRightBottom(cl, -50, 0);    // まず適当に小さくする
            double maxLeft = si.Width - cl.Width;

            Assert.AreEqual(expected: 0, actual: cl.Left);
            Move(cl, System.Windows.Input.Key.Left, 1);
            Assert.AreEqual(expected: 0, actual: cl.Left);
            Move(cl, System.Windows.Input.Key.Right, 1);
            Assert.AreEqual(expected: 1, actual: cl.Left);
            Move(cl, System.Windows.Input.Key.Right, 1000);
            Assert.AreEqual(expected: maxLeft, actual: cl.Left);
            Move(cl, System.Windows.Input.Key.Left, 1);
            Assert.AreEqual(expected: maxLeft - 1, actual: cl.Left);
        }

        [TestMethod]
        public void TestDoNotChangeSizeIfDragPointIsNotCorner()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            // 上下左右に動かせるスペースを作る(スペースを作れればスペースの広さは適当でOK)
            ChangeSizeBaseRightBottom(cl, -100, 0);
            Move(cl, System.Windows.Input.Key.Right, 50);
            Move(cl, System.Windows.Input.Key.Down, 30);

            // 正解は切り抜き線の縦横幅が変化しないこと
            double ansWidth = cl.Width;
            double ansHeight = cl.Height;

            double enoughLeave = 50;

            // 右下点付近だがコーナーではない
            System.Windows.Point dragStart = new System.Windows.Point(cl.Right - enoughLeave, cl.Bottom - enoughLeave);
            System.Windows.Point drop = new System.Windows.Point(cl.Right, cl.Bottom);
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);

            // 左下点付近だがコーナーではない
            dragStart = new System.Windows.Point(cl.Left + enoughLeave, cl.Bottom - enoughLeave);
            drop = new System.Windows.Point(cl.Left, cl.Bottom);
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);

            // 左上点付近だがコーナーではない
            dragStart = new System.Windows.Point(cl.Left + enoughLeave, cl.Top + enoughLeave);
            drop = new System.Windows.Point(cl.Left, cl.Top);
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);

            // 右上点付近だがコーナーではない
            dragStart = new System.Windows.Point(cl.Right - enoughLeave, cl.Top + enoughLeave);
            drop = new System.Windows.Point(cl.Right, cl.Top);
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);
        }

        private ShowingImage CreateShowingImage(string imagePathBase, int imageAreaWidth, int imageAreaHeight)
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment(imagePathBase);
            OriginalImage oi = new OriginalImage(imagePath);
            return new ShowingImage(oi, imageAreaWidth, imageAreaHeight);
        }

        private void Move(CutLine cl, System.Windows.Input.Key key, int num)
        {
            cl.ExecuteCommand(key, num);
        }

        private void ChangeSizeBaseRightBottomWithCheck(CutLine cutLine, double changeSizeX, double changeSizeY, double expectedWidth, double expectedHeight)
        {
            ChangeSizeBaseRightBottom(cutLine, changeSizeX, changeSizeY);
            Assert.AreEqual(expected: expectedWidth, actual: cutLine.Width);
            Assert.AreEqual(expected: expectedHeight, actual: cutLine.Height);
        }

        private void ChangeSizeBaseRightBottom(CutLine cutLine, double changeSizeX, double changeSizeY)
        {
            System.Windows.Point dragStart = new System.Windows.Point(cutLine.Right, cutLine.Bottom);
            System.Windows.Point drop = new System.Windows.Point(cutLine.Right + changeSizeX, cutLine.Bottom + changeSizeY);
            cutLine.ExecuteCommand(dragStart, drop);
        }

        private void ChangeSizeAndCheckWidthAndHeight(CutLine cl, Point dragStart, Point drop, double ansWidth, double ansHeight)
        {
            cl.ExecuteCommand(dragStart, drop);
            Assert.AreEqual(expected: ansWidth, actual: cl.Width);
            Assert.AreEqual(expected: ansHeight, actual: cl.Height);
        }
    }
}
