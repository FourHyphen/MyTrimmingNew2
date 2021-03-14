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
            AreEqualCutLineParameter(expected: Driver.GetShowingImageWidth(), actual: cutLineWidth);
            // 367[pixel] = 横654[pixel]に対して 縦:横 = 16:9 を適用した結果
            AreEqualCutLineParameter(expected: 367, actual: cutLineHeight);
        }

        [TestMethod]
        public void TestDisplayCutLineOfImageHeightLongerThanWidth()
        {
            string imagePath2 = Common.GetFilePathOfDependentEnvironment("/Resource/test002.jpg");
            Driver.EmurateOpenImage(imagePath2);

            double cutLineWidth = Driver.GetCutLineWidth();
            double cutLineHeight = Driver.GetCutLineHeight();
            // (362, 203) = 縦長画像を 横654 x 縦544 で表示したところ 横362 x 縦544 で表示された
            //              これに対して 縦:横 = 16:9 を適用した結果
            AreEqualCutLineParameter(expected: 362, actual: cutLineWidth);
            AreEqualCutLineParameter(expected: 203, actual: cutLineHeight);
        }

        [TestMethod]
        public void TestMoveCutLineWhenInputCursolKeyUpAndDown()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            double maxBottom = Driver.GetShowingImageHeight();
            double maxTop = maxBottom - Driver.GetCutLineHeight();

            AreEqualCutLineParameter(expected: 0, actual: Driver.GetCutLineLeftTopX());
            AreEqualCutLineParameter(expected: 0, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Up, 1);
            AreEqualCutLineParameter(expected: 0, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 1);
            AreEqualCutLineParameter(expected: 1, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 300);
            AreEqualCutLineParameter(expected: maxTop, actual: Driver.GetCutLineLeftTopY());
            AreEqualCutLineParameter(expected: maxTop, actual: Driver.GetCutLineRightTopY());
            AreEqualCutLineParameter(expected: maxBottom, actual: Driver.GetCutLineLeftBottomY());
            AreEqualCutLineParameter(expected: maxBottom, actual: Driver.GetCutLineRightBottomY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Up, 1);
            AreEqualCutLineParameter(expected: maxTop - 1, actual: Driver.GetCutLineLeftTopY());
            AreEqualCutLineParameter(expected: maxBottom - 1, actual: Driver.GetCutLineLeftBottomY());
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
            double afterCutLineHeight = Math.Round(beforeCutLineHeight - (moveX * 9.0 / 16.0), 2);

            AreEqualCutLineParameter(expected: 0, actual: Driver.GetCutLineLeftTopX());
            AreEqualCutLineParameter(expected: 0, actual: Driver.GetCutLineLeftTopY());
            AreEqualCutLineParameter(expected: afterCutLineWidth, actual: Driver.GetCutLineWidth());
            AreEqualCutLineParameter(expected: afterCutLineHeight, actual: Driver.GetCutLineHeight());
            AreEqualCutLineParameter(expected: afterCutLineWidth, actual: Driver.GetCutLineRightTopX());
            AreEqualCutLineParameter(expected: afterCutLineWidth, actual: Driver.GetCutLineRightBottomX());
            AreEqualCutLineParameter(expected: afterCutLineHeight, actual: Driver.GetCutLineRightBottomY());
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

            AreEqualCutLineParameter(expected: minLeft, actual: Driver.GetCutLineLeftTopX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Left, 1);
            AreEqualCutLineParameter(expected: minLeft, actual: Driver.GetCutLineLeftTopX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Right, 2);
            AreEqualCutLineParameter(expected: 2, actual: Driver.GetCutLineLeftTopX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Right, (int)moveX * 2);    // 2倍もあれば画像をはみ出ようとするのに十分
            AreEqualCutLineParameter(expected: maxLeft, actual: Driver.GetCutLineLeftTopX());
            AreEqualCutLineParameter(expected: maxRight, actual: Driver.GetCutLineRightBottomX());

            Driver.EmurateInputKey(System.Windows.Input.Key.Left, 1);
            AreEqualCutLineParameter(expected: maxLeft - 1, actual: Driver.GetCutLineLeftTopX());
            AreEqualCutLineParameter(expected: maxRight - 1, actual: Driver.GetCutLineRightBottomX());
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
            AreEqualCutLineParameter(expected: maxRight, actual: Driver.GetCutLineRightBottomX());
            AreEqualCutLineParameter(expected: maxBottom, actual: Driver.GetCutLineRightBottomY());

            // 移動：左下方向
            drag = new System.Windows.Point(centerX, centerY);
            drop = new System.Windows.Point(minLeft, maxBottom);
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            AreEqualCutLineParameter(expected: minLeft, actual: Driver.GetCutLineLeftBottomX());
            AreEqualCutLineParameter(expected: maxBottom, actual: Driver.GetCutLineLeftBottomY());

            // 移動：左上方向
            drag = new System.Windows.Point(centerX, centerY);
            drop = new System.Windows.Point(minLeft, minTop);
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            AreEqualCutLineParameter(expected: minLeft, actual: Driver.GetCutLineLeftTopX());
            AreEqualCutLineParameter(expected: minTop, actual: Driver.GetCutLineLeftTopY());

            // 移動：右上方向
            drag = new System.Windows.Point(centerX, centerY);
            drop = new System.Windows.Point(maxRight, minTop);
            Driver.EmurateShowingImageMouseDragAndDrop(drag, drop);
            AreEqualCutLineParameter(expected: maxRight, actual: Driver.GetCutLineRightTopX());
            AreEqualCutLineParameter(expected: minTop, actual: Driver.GetCutLineRightTopY());
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
            AreEqualCutLineParameter(expected: 0, actual: Driver.GetCutLineRotateDegree());

            Driver.EmurateInputKey(System.Windows.Input.Key.OemMinus, 10);

            AreEqualCutLineParameter(expected: -10, actual: Driver.GetCutLineRotateDegree());
            AreEqualCutLineParameter(expected: beforeWidth, actual: Driver.GetCutLineWidth());
            AreEqualCutLineParameter(expected: beforeHeight, actual: Driver.GetCutLineHeight());
            AreEqualCutLineParameter(expected: 81.351909, actual: Driver.GetCutLineLeftTopX());
            AreEqualCutLineParameter(expected: 141.351350, actual: Driver.GetCutLineLeftTopY());
            AreEqualCutLineParameter(expected: 528.454629, actual: Driver.GetCutLineRightTopX());
            AreEqualCutLineParameter(expected: 62.515077, actual: Driver.GetCutLineRightTopY());
            AreEqualCutLineParameter(expected: 572.648091, actual: Driver.GetCutLineRightBottomX());
            AreEqualCutLineParameter(expected: 313.148650, actual: Driver.GetCutLineRightBottomY());
            AreEqualCutLineParameter(expected: 125.545371, actual: Driver.GetCutLineLeftBottomX());
            AreEqualCutLineParameter(expected: 391.984923, actual: Driver.GetCutLineLeftBottomY());
        }

        private void AreEqualCutLineParameter(double expected, double actual)
        {
            double toCompare = Math.Round(expected, 2);
            Assert.AreEqual(toCompare, actual);
        }
    }
}
