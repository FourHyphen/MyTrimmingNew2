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
            Assert.AreEqual(expected: 0, actual: cl.LeftTop.X);
            Assert.AreEqual(expected: 0, actual: cl.LeftTop.Y);

            si = CreateShowingImage("/Resource/test002.jpg", 800, 600);
            cl = new CutLine(si);

            Assert.AreEqual(expected: si.Width, actual: cl.Width);
            Assert.AreEqual(expected: 225, actual: cl.Height);    // 225 = (int)(450.0 * 9.0 / 16.0)
            Assert.AreEqual(expected: 0, actual: cl.LeftTop.X);
            Assert.AreEqual(expected: 0, actual: cl.LeftTop.Y);
        }

        [TestMethod]
        public void TestUpAndDownCutLine()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);
            double maxTop = si.Height - cl.Height;

            Assert.AreEqual(expected: 0, actual: cl.LeftTop.Y);
            Move(cl, System.Windows.Input.Key.Up, 1);
            Assert.AreEqual(expected: 0, actual: cl.LeftTop.Y);
            Move(cl, System.Windows.Input.Key.Down, 1);
            Assert.AreEqual(expected: 1, actual: cl.LeftTop.Y);
            Move(cl, System.Windows.Input.Key.Down, 50);
            Assert.AreEqual(expected: 51, actual: cl.LeftTop.Y);
            Move(cl, System.Windows.Input.Key.Down, 1000);
            Assert.AreEqual(expected: maxTop, actual: cl.LeftTop.Y);
            Move(cl, System.Windows.Input.Key.Up, 1);
            Assert.AreEqual(expected: maxTop - 1, actual: cl.LeftTop.Y);
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

            System.Windows.Point near = new System.Windows.Point(cl.RightBottom.X - 10, cl.RightBottom.Y - 10);
            System.Windows.Point far = new System.Windows.Point(cl.RightBottom.X - 21, cl.RightBottom.Y - 21);
            Assert.IsTrue(cl.IsPointNearRightBottom(near));
            Assert.IsFalse(cl.IsPointNearRightBottom(far));

            near = new System.Windows.Point(cl.RightBottom.X + 10, cl.RightBottom.Y + 10);
            far = new System.Windows.Point(cl.RightBottom.X + 21, cl.RightBottom.Y + 21);
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

            Assert.AreEqual(expected: 0, actual: cl.LeftTop.X);
            Move(cl, System.Windows.Input.Key.Left, 1);
            Assert.AreEqual(expected: 0, actual: cl.LeftTop.X);
            Move(cl, System.Windows.Input.Key.Right, 1);
            Assert.AreEqual(expected: 1, actual: cl.LeftTop.X);
            Move(cl, System.Windows.Input.Key.Right, 1000);
            Assert.AreEqual(expected: maxLeft, actual: cl.LeftTop.X);
            Move(cl, System.Windows.Input.Key.Left, 1);
            Assert.AreEqual(expected: maxLeft - 1, actual: cl.LeftTop.X);
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
            System.Windows.Point dragStart = new System.Windows.Point(cl.RightBottom.X - enoughLeave, cl.RightBottom.Y - enoughLeave);
            System.Windows.Point drop = cl.RightBottom;
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);

            // 左下点付近だがコーナーではない
            dragStart = new System.Windows.Point(cl.LeftBottom.X + enoughLeave, cl.LeftBottom.Y - enoughLeave);
            drop = cl.LeftBottom;
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);

            // 左上点付近だがコーナーではない
            dragStart = new System.Windows.Point(cl.LeftTop.X + enoughLeave, cl.LeftTop.Y + enoughLeave);
            drop = cl.LeftTop;
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);

            // 右上点付近だがコーナーではない
            dragStart = new System.Windows.Point(cl.RightTop.X - enoughLeave, cl.RightTop.Y + enoughLeave);
            drop = cl.RightTop;
            ChangeSizeAndCheckWidthAndHeight(cl, dragStart, drop, ansWidth, ansHeight);
        }

        [TestMethod]
        public void TestIsPointInside()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            // 切り抜き線の外ならfalse
            Assert.IsFalse(cl.IsPointInside(new Point(cl.LeftTop.X - 1, cl.LeftTop.Y - 1)));
            Assert.IsFalse(cl.IsPointInside(new Point(cl.RightTop.X + 1, cl.RightTop.Y - 1)));
            Assert.IsFalse(cl.IsPointInside(new Point(cl.LeftBottom.X - 1, cl.LeftBottom.Y + 1)));
            Assert.IsFalse(cl.IsPointInside(new Point(cl.RightBottom.X + 1, cl.RightBottom.Y + 1)));

            // 切り抜き線の中ならtrue
            Assert.IsTrue(cl.IsPointInside(new Point(cl.LeftTop.X + 1, cl.LeftTop.Y + 1)));
            Assert.IsTrue(cl.IsPointInside(new Point(cl.RightTop.X - 1, cl.RightTop.Y + 1)));
            Assert.IsTrue(cl.IsPointInside(new Point(cl.LeftBottom.X + 1, cl.LeftBottom.Y - 1)));
            Assert.IsTrue(cl.IsPointInside(new Point(cl.RightBottom.X - 1, cl.RightBottom.Y - 1)));
        }

        [TestMethod]
        public void TestChangeSizeBaseRightBottomIfOriginChanging()
        {
            // 右下点を思いきり左上に引っ張って、原点の位置が変わる場合のテスト
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            // 右下点を思いきり左上に引っ張って、原点の位置を旧右下点にできるだけのスペースを作る
            // 左上に引っ張る際、高さ方向に大きく引っ張ることでサイズ変化後の左上点のx座標が < 0 にならないことを確認する
            int moveX = 10;    // 移動するy距離に比べて十分に小さくする
            ChangeSizeBaseRightBottom(cl, -100, 0);
            Move(cl, System.Windows.Input.Key.Right, moveX);
            Move(cl, System.Windows.Input.Key.Down, 500);

            double beforeLeft = cl.LeftTop.X;
            double beforeTop = cl.LeftTop.Y;

            // 右下点を思いきり左上に引っ張る
            ChangeSizeBaseRightBottom(cl, -1000, -2000);

            Assert.AreEqual(expected: 0, actual: cl.LeftTop.X);
            Assert.AreEqual(expected: beforeLeft, actual: cl.RightBottom.X);
            Assert.AreEqual(expected: beforeTop, actual: cl.RightBottom.Y);
            Assert.AreEqual(expected: moveX, actual: cl.Width);
            Assert.AreEqual(expected: moveX * 9.0 / 16.0, actual: cl.Height);

            si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            cl = new CutLine(si);

            // 左上に引っ張る際、横方向に大きく引っ張ることでサイズ変化後の左上点のy座標が < 0 にならないことを確認する
            int moveY = 10;    // 移動するx距離に比べて十分に小さくする
            ChangeSizeBaseRightBottom(cl, -100, 0);
            Move(cl, System.Windows.Input.Key.Right, 500);
            Move(cl, System.Windows.Input.Key.Down, moveY);

            beforeLeft = cl.LeftTop.X;
            beforeTop = cl.LeftTop.Y;

            // 右下点を思いきり左上に引っ張る
            ChangeSizeBaseRightBottom(cl, -3000, -1000);

            Assert.AreEqual(expected: 0, actual: cl.LeftTop.Y);
            Assert.AreEqual(expected: beforeLeft, actual: cl.RightBottom.X);
            Assert.AreEqual(expected: beforeTop, actual: cl.RightBottom.Y);
            Assert.AreEqual(expected: Math.Round(moveY * 16.0 / 9.0, 3), actual: Math.Round(cl.Width, 3));
            Assert.AreEqual(expected: moveY, actual: cl.Height);
        }

        [TestMethod]
        public void TestDoNotRotateIfStickOutOfImage()
        {
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            Rotate(cl, 1);
            Assert.AreEqual(expected: 0, actual: cl.Degree);

            Rotate(cl, -1);
            Assert.AreEqual(expected: 0, actual: cl.Degree);
        }

        [TestMethod]
        public void TestDoNotChangeSizeBaseRightBottomAfterRotateIfOriginChanging()
        {
            // 右下点を思いきり左上に引っ張った際、原点の位置が変わる場合に回転済みならサイズ変更しないテスト
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            // 準備: 適当にサイズを小さくして中央に寄せて回転
            ChangeSizeBaseRightBottom(cl, -200, 0);
            Move(cl, System.Windows.Input.Key.Right, 100);
            Move(cl, System.Windows.Input.Key.Down, 50);
            Rotate(cl, 10);
            System.Windows.Point beforeLeftTop = cl.LeftTop;

            // 右下点を思いきり左上に引っ張る
            ChangeSizeBaseRightBottom(cl, -2000, -1000);

            Assert.AreEqual(expected: beforeLeftTop, actual: cl.LeftTop);
        }

        [TestMethod]
        public void TestChangeSizeBaseRightBottomAfterRotate()
        {
            // 回転後のサイズ変更テスト
            ShowingImage si = CreateShowingImage("/Resource/test001.jpg", 800, 600);
            CutLine cl = new CutLine(si);

            // 準備: 適当にサイズを小さくして中央に寄せて回転
            ChangeSizeBaseRightBottom(cl, -300, 0);
            Move(cl, System.Windows.Input.Key.Right, 200);
            Move(cl, System.Windows.Input.Key.Down, 50);
            Rotate(cl, 10);

            // 回転後のサイズ変更
            System.Windows.Point beforeLeftTop = cl.LeftTop;
            double beforeLeftXSlope = CalcSlope(cl.LeftTop, cl.RightTop);
            double beforeLeftYSlope = CalcSlope(cl.LeftTop, cl.LeftBottom);
            double beforeRightXSlope = CalcSlope(cl.RightBottom, cl.LeftBottom);
            double beforeRightYSlope = CalcSlope(cl.RightTop, cl.RightBottom);
            double beforeWidth = cl.Width;
            double beforeHeight = cl.Height;
            ChangeSizeBaseRightBottom(cl, -100, -10);

            // サイズは変わっていれば良しとする(正解値の計算困難)
            Assert.IsTrue(beforeWidth != cl.Width);
            Assert.IsTrue(beforeHeight != cl.Height);

            // 矩形の角度が変わってないかをチェック
            double afterLeftXSlope = CalcSlope(cl.LeftTop, cl.RightTop);
            double afterLeftYSlope = CalcSlope(cl.LeftTop, cl.LeftBottom);
            double afterRightXSlope = CalcSlope(cl.RightBottom, cl.LeftBottom);
            double afterRightYSlope = CalcSlope(cl.RightTop, cl.RightBottom);
            Assert.AreEqual(expected: beforeLeftTop, actual: cl.LeftTop);
            AreEqualRound(beforeLeftXSlope, afterLeftXSlope, 10);    // 厳密には17桁目から違う
            AreEqualRound(beforeLeftYSlope, afterLeftYSlope, 10);    // 厳密には15桁目から違う
            AreEqualRound(beforeRightXSlope, afterRightXSlope, 10);    // 厳密には15桁目から違う
            AreEqualRound(beforeRightYSlope, afterRightYSlope, 10);    // 厳密には15桁目から違う
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
            System.Windows.Point dragStart = cutLine.RightBottom;
            System.Windows.Point drop = new System.Windows.Point(cutLine.RightBottom.X + changeSizeX, cutLine.RightBottom.Y + changeSizeY);
            cutLine.ExecuteCommand(dragStart, drop);
        }

        private void ChangeSizeAndCheckWidthAndHeight(CutLine cl, Point dragStart, Point drop, double ansWidth, double ansHeight)
        {
            cl.ExecuteCommand(dragStart, drop);
            Assert.AreEqual(expected: ansWidth, actual: cl.Width);
            Assert.AreEqual(expected: ansHeight, actual: cl.Height);
        }

        private void Rotate(CutLine cutLine, int degree)
        {
            if (degree > 0)
            {
                cutLine.ExecuteCommand(System.Windows.Input.Key.OemPlus, degree);
            }
            else
            {
                cutLine.ExecuteCommand(System.Windows.Input.Key.OemMinus, degree);
            }
        }

        private double CalcSlope(System.Windows.Point p1, System.Windows.Point p2)
        {
            double xDiff = p2.X - p1.X;
            double yDiff = p2.Y - p1.Y;
            if (xDiff == 0.0)
            {
                return 0.0;
            }
            return yDiff / xDiff;
        }

        private void AreEqualRound(double expected, double actual, int round = 2)
        {
            double expectedRound = Math.Round(expected, round);
            double actualRound = Math.Round(actual, round);
            Assert.AreEqual(expectedRound, actualRound);
        }
    }
}
