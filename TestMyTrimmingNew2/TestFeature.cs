using System;
using System.Dynamic;
using System.Diagnostics;
using System.Threading.Tasks;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMyTrimmingNew2
{
    [TestClass]
    public class TestFeature
    {
        // 必要なパッケージ
        //  -> Codeer.Friendly
        //  -> Codeer.Friendly.Windows         -> WindowsAppFriend()
        //  -> Codeer.Friendly.Windows.Grasp   -> WindowControl()
        //  -> RM.Friendly.WPFStandardControls -> 各種WPFコントロールを取得するために必要
        // 必要な作業
        //  -> MyTextEditorプロジェクトを参照に追加

        private string AttachExeName = "MyTrimmingNew2.exe";
        private WindowsAppFriend TestApp;
        private Process TestProcess;
        private dynamic MainWindow;
        private MainWindowDriver Driver;

        private string BeforeEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            // MainWindowプロセスにattach
            string exePath = System.IO.Path.GetFullPath(AttachExeName);
            TestApp = new WindowsAppFriend(Process.Start(exePath));
            TestProcess = Process.GetProcessById(TestApp.ProcessId);
            MainWindow = TestApp.Type("System.Windows.Application").Current.MainWindow;
            Driver = new MainWindowDriver(MainWindow);

            BeforeEnvironment = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Common.GetEnvironmentDirPath();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestApp.Dispose();
            TestProcess.CloseMainWindow();

            Environment.CurrentDirectory = BeforeEnvironment;
        }

        [TestMethod]
        public void TestSuccessImageOpen()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            Assert.AreEqual(expected: 3840, actual: Driver.GetOriginalImageWidth());
            Assert.AreEqual(expected: 2560, actual: Driver.GetOriginalImageHeight());

            double imageAreaWidth = Driver.GetImageAreaWidth();
            Assert.AreEqual(expected: imageAreaWidth, actual: Driver.GetShowingImageWidth());
            // 436[pixel] = 横654[pixel]に縦横比率を維持したまま縮小した結果
            Assert.AreEqual(expected: 436, actual: Driver.GetShowingImageHeight());
        }

        [TestMethod]
        public void TestDisplayCutLineOfImageWidthLongerThanHeight()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);

            double cutLineWidth = Driver.GetCutLineWidth();
            double cutLineHeight = Driver.GetCutLineHeight();
            Common.AreEqualRound(expected: Driver.GetShowingImageWidth(), actual: cutLineWidth);
            // 367.875[pixel] = 横654[pixel]に対して 縦:横 = 16:9 を適用した結果
            Common.AreEqualRound(expected: 367.875, actual: cutLineHeight);
        }

        [TestMethod]
        public void TestDisplayCutLineOfImageHeightLongerThanWidth()
        {
            string imagePath2 = Common.GetFilePathOfDependentEnvironment("/Resource/test002.jpg");
            Driver.EmurateOpenImage(imagePath2);

            double cutLineWidth = Driver.GetCutLineWidth();
            double cutLineHeight = Driver.GetCutLineHeight();
            // (362, 203.62) = 縦長画像を 横654 x 縦544 で表示したところ 横362 x 縦544 で表示された
            //              これに対して 縦:横 = 16:9 を適用した結果
            Common.AreEqualRound(expected: 362, actual: cutLineWidth);
            Common.AreEqualRound(expected: 203.62, actual: cutLineHeight);
        }

        [TestMethod]
        public void TestMoveCutLineWhenInputCursolKeyUpAndDown()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            double maxBottom = Driver.GetShowingImageHeight();
            double maxTop = maxBottom - Driver.GetCutLineHeight();

            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineLeftTopX());
            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Up, 1);
            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 1);
            Common.AreEqualRound(expected: 1, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 300);
            Common.AreEqualRound(expected: maxTop, actual: Driver.GetCutLineLeftTopY());
            Common.AreEqualRound(expected: maxTop, actual: Driver.GetCutLineRightTopY());
            Common.AreEqualRound(expected: maxBottom, actual: Driver.GetCutLineLeftBottomY());
            Common.AreEqualRound(expected: maxBottom, actual: Driver.GetCutLineRightBottomY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Up, 1);
            Common.AreEqualRound(expected: maxTop - 1, actual: Driver.GetCutLineLeftTopY());
            Common.AreEqualRound(expected: maxBottom - 1, actual: Driver.GetCutLineLeftBottomY());
        }

        [TestMethod]
        public void TestChangeOfCutLineSizeWhenMouseDragAndDropRightBottom()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            double beforeCutLineWidth = Driver.GetCutLineWidth();
            double beforeCutLineHeight = Driver.GetCutLineHeight();

            double moveX = 50;    // 根拠なし、適当
            double dragStartX = Driver.GetCutLineRightBottomX();
            double dropX = dragStartX - moveX;
            System.Windows.Point drag = new System.Windows.Point(dragStartX, Driver.GetCutLineRightBottomY());
            System.Windows.Point drop = new System.Windows.Point(dropX, Driver.GetCutLineRightBottomY());

            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            double afterCutLineWidth = beforeCutLineWidth - moveX;
            double afterCutLineHeight = beforeCutLineHeight - (moveX * 9.0 / 16.0);

            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineLeftTopX());
            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineLeftTopY());
            Common.AreEqualRound(expected: afterCutLineWidth, actual: Driver.GetCutLineWidth());
            Common.AreEqualRound(expected: afterCutLineHeight, actual: Driver.GetCutLineHeight(), 1);    // 339.755 と 339.75の比較、この差は無視する
            Common.AreEqualRound(expected: afterCutLineWidth, actual: Driver.GetCutLineRightTopX());
            Common.AreEqualRound(expected: afterCutLineWidth, actual: Driver.GetCutLineRightBottomX());
            Common.AreEqualRound(expected: afterCutLineHeight, actual: Driver.GetCutLineRightBottomY(), 1);    // 339.755 と 339.75の比較、この差は無視する
        }

        [TestMethod]
        public void TestMoveCutLineWhenInputCursolKeyLeftAnRight()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            double moveX = 50;
            double minLeft = 0;
            double maxLeft = moveX;
            double maxRight = Driver.GetShowingImageWidth();

            // まず切り抜き線を適当に小さくし、左右に動けるスペースを作る
            System.Windows.Point drag = new System.Windows.Point(Driver.GetCutLineRightBottomX(), Driver.GetCutLineRightBottomY());
            System.Windows.Point drop = new System.Windows.Point(Driver.GetCutLineRightBottomX() - moveX, Driver.GetCutLineRightBottomY());
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);

            Common.AreEqualRound(expected: minLeft, actual: Driver.GetCutLineLeftTopX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Left, 1);
            Common.AreEqualRound(expected: minLeft, actual: Driver.GetCutLineLeftTopX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Right, 2);
            Common.AreEqualRound(expected: 2, actual: Driver.GetCutLineLeftTopX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Right, (int)moveX * 2);    // 2倍もあれば画像をはみ出ようとするのに十分
            Common.AreEqualRound(expected: maxLeft, actual: Driver.GetCutLineLeftTopX());
            Common.AreEqualRound(expected: maxRight, actual: Driver.GetCutLineRightBottomX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Left, 1);
            Common.AreEqualRound(expected: maxLeft - 1, actual: Driver.GetCutLineLeftTopX());
            Common.AreEqualRound(expected: maxRight - 1, actual: Driver.GetCutLineRightBottomX());
        }

        [TestMethod]
        public void TestMoveCutLineByMouseDragAndDrop()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            double moveX = 50;
            double minLeft = 0;
            double maxRight = Driver.GetShowingImageWidth();
            double minTop = 0;
            double maxBottom = Driver.GetShowingImageHeight();
            double centerX = Driver.GetCutLineWidth() / 2.0;
            double centerY = Driver.GetCutLineHeight() / 2.0;

            // まず切り抜き線を適当に小さくし、左右に動けるスペースを作る
            System.Windows.Point drag = new System.Windows.Point(Driver.GetCutLineRightBottomX(), Driver.GetCutLineRightBottomY());
            System.Windows.Point drop = new System.Windows.Point(Driver.GetCutLineRightBottomX() - moveX, Driver.GetCutLineRightBottomY());
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);

            // 移動：右下方向
            drag = new System.Windows.Point(centerX, centerY);
            drop = new System.Windows.Point(maxRight, maxBottom);
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            Common.AreEqualRound(expected: maxRight, actual: Driver.GetCutLineRightBottomX());
            Common.AreEqualRound(expected: maxBottom, actual: Driver.GetCutLineRightBottomY());

            // 移動：左下方向
            drag = new System.Windows.Point(centerX, centerY);
            drop = new System.Windows.Point(minLeft, maxBottom);
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            Common.AreEqualRound(expected: minLeft, actual: Driver.GetCutLineLeftBottomX());
            Common.AreEqualRound(expected: maxBottom, actual: Driver.GetCutLineLeftBottomY());

            // 移動：左上方向
            drag = new System.Windows.Point(centerX, centerY);
            drop = new System.Windows.Point(minLeft, minTop);
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            Common.AreEqualRound(expected: minLeft, actual: Driver.GetCutLineLeftTopX());
            Common.AreEqualRound(expected: minTop, actual: Driver.GetCutLineLeftTopY());

            // 移動：右上方向
            drag = new System.Windows.Point(centerX, centerY);
            drop = new System.Windows.Point(maxRight, minTop);
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            Common.AreEqualRound(expected: maxRight, actual: Driver.GetCutLineRightTopX());
            Common.AreEqualRound(expected: minTop, actual: Driver.GetCutLineRightTopY());
        }

        [TestMethod]
        public void TestRotateCutLine()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);

            // まず切り抜き線を適当に小さくして中央に寄せ、回転できるスペースを作る
            System.Windows.Point drag = new System.Windows.Point(Driver.GetCutLineRightBottomX(), Driver.GetCutLineRightBottomY());
            System.Windows.Point drop = new System.Windows.Point(Driver.GetCutLineRightBottomX() - 200, Driver.GetCutLineRightBottomY());
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            Driver.EmurateInputKey(System.Windows.Input.Key.Right, 100);
            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 100);

            double beforeWidth = Driver.GetCutLineWidth();
            double beforeHeight = Driver.GetCutLineHeight();
            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineRotateDegree());

            Driver.EmurateInputKey(System.Windows.Input.Key.OemMinus, 10);

            Common.AreEqualRound(expected: -10, actual: Driver.GetCutLineRotateDegree());
            Common.AreEqualRound(expected: beforeWidth, actual: Driver.GetCutLineWidth());
            Common.AreEqualRound(expected: beforeHeight, actual: Driver.GetCutLineHeight());
            Common.AreEqualRound(expected: 81.28, actual: Driver.GetCutLineLeftTopX());
            Common.AreEqualRound(expected: 141.36, actual: Driver.GetCutLineLeftTopY());
            Common.AreEqualRound(expected: 528.38, actual: Driver.GetCutLineRightTopX());
            Common.AreEqualRound(expected: 62.52, actual: Driver.GetCutLineRightTopY());
            Common.AreEqualRound(expected: 572.72, actual: Driver.GetCutLineRightBottomX());
            Common.AreEqualRound(expected: 314.02, actual: Driver.GetCutLineRightBottomY());
            Common.AreEqualRound(expected: 125.62, actual: Driver.GetCutLineLeftBottomX());
            Common.AreEqualRound(expected: 392.85, actual: Driver.GetCutLineLeftBottomY());
        }

        [TestMethod]
        public void TestCutLineIsInsideImageWhenMoveAfterRotate()
        {
            // 回転後の矩形の移動で画像からはみ出ないことを確認するテスト
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);

            // 準備
            System.Windows.Point drag = new System.Windows.Point(Driver.GetCutLineRightBottomX(), Driver.GetCutLineRightBottomY());
            System.Windows.Point drop = new System.Windows.Point(Driver.GetCutLineRightBottomX() - 200, Driver.GetCutLineRightBottomY());
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);           // 小さくする
            Driver.EmurateInputKey(System.Windows.Input.Key.Right, 100);      // 中央に寄せる
            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 100);       // 中央に寄せる
            Driver.EmurateInputKey(System.Windows.Input.Key.OemMinus, 10);    // 回転

            // テスト: 左方向    (500: 各方向に突き抜けるには十分な移動距離)
            int tooLong = 500;
            Driver.EmurateInputKey(System.Windows.Input.Key.Left, tooLong);
            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineLeftTopX());

            // テスト: 上方向
            Driver.EmurateInputKey(System.Windows.Input.Key.Up, tooLong);
            Common.AreEqualRound(expected: 0, actual: Driver.GetCutLineRightTopY());

            // テスト: 右方向
            Driver.EmurateInputKey(System.Windows.Input.Key.Right, tooLong);
            Common.AreEqualRound(expected: Driver.GetShowingImageWidth(), actual: Driver.GetCutLineRightBottomX());

            // テスト: 下方向
            Driver.EmurateInputKey(System.Windows.Input.Key.Down, tooLong);
            Common.AreEqualRound(expected: Driver.GetShowingImageHeight(), actual: Driver.GetCutLineLeftBottomY());
        }

        [TestMethod]
        public void TestChangeOfCutLineSizeWhenMouseDragAndDropLeftTop()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            double beforeCutLineWidth = Driver.GetCutLineWidth();
            double beforeCutLineHeight = Driver.GetCutLineHeight();

            double moveX = 50;    // 根拠なし、適当
            double heightDiff = moveX * 9.0 / 16.0;
            double dragStartX = Driver.GetCutLineLeftTopX();
            double dropX = dragStartX + moveX;
            System.Windows.Point drag = new System.Windows.Point(dragStartX, Driver.GetCutLineLeftTopY());
            System.Windows.Point drop = new System.Windows.Point(dropX, Driver.GetCutLineLeftTopY());

            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            double afterCutLineWidth = beforeCutLineWidth - moveX;
            double afterCutLineHeight = beforeCutLineHeight - heightDiff;

            Common.AreEqualRound(expected: 50, actual: Driver.GetCutLineLeftTopX());
            Common.AreEqualRound(expected: heightDiff, actual: Driver.GetCutLineLeftTopY());
            Common.AreEqualRound(expected: afterCutLineWidth, actual: Driver.GetCutLineWidth());
            Common.AreEqualRound(expected: afterCutLineHeight, actual: Driver.GetCutLineHeight());
        }
    }
}
