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

            int imageAreaWidth = Driver.GetImageAreaWidth();
            Assert.AreEqual(expected: imageAreaWidth, actual: Driver.GetShowingImageWidth());
            // 436[pixel] = 横654[pixel]に縦横比率を維持したまま縮小した結果
            Assert.AreEqual(expected: 436, actual: Driver.GetShowingImageHeight());
        }

        [TestMethod]
        public void TestDisplayCutLineOfImageWidthLongerThanHeight()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);

            int cutLineWidth = Driver.GetCutLineWidth();
            int cutLineHeight = Driver.GetCutLineHeight();
            Assert.AreEqual(expected: Driver.GetShowingImageWidth(), actual: cutLineWidth);
            // 367[pixel] = 横654[pixel]に対して 縦:横 = 16:9 を適用した結果
            Assert.AreEqual(expected: 367, actual: cutLineHeight);
        }

        [TestMethod]
        public void TestDisplayCutLineOfImageHeightLongerThanWidth()
        {
            string imagePath2 = Common.GetFilePathOfDependentEnvironment("/Resource/test002.jpg");
            Driver.EmurateOpenImage(imagePath2);

            int cutLineWidth = Driver.GetCutLineWidth();
            int cutLineHeight = Driver.GetCutLineHeight();
            // (362, 203) = 縦長画像を 横654 x 縦544 で表示したところ 横362 x 縦544 で表示された
            //              これに対して 縦:横 = 16:9 を適用した結果
            Assert.AreEqual(expected: 362, actual: cutLineWidth);
            Assert.AreEqual(expected: 203, actual: cutLineHeight);
        }

        [TestMethod]
        public void TestMoveCutLineWhenInputCursolKeyUpAndDown()
        {
            string imagePath = Common.GetFilePathOfDependentEnvironment("/Resource/test001.jpg");
            Driver.EmurateOpenImage(imagePath);
            Assert.AreEqual(expected: 0, actual: Driver.GetCutLineLeftTopX());
            Assert.AreEqual(expected: 0, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Up, 1);
            Assert.AreEqual(expected: 0, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 1);
            Assert.AreEqual(expected: 1, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 1);
            Assert.AreEqual(expected: 2, actual: Driver.GetCutLineLeftTopY());

            Driver.EmurateInputKey(System.Windows.Input.Key.Down, 1000);
            Assert.AreEqual(expected: Driver.GetImageAreaHeight(), actual: Driver.GetCutLineLeftBottomY());
            Assert.AreEqual(expected: Driver.GetImageAreaHeight(), actual: Driver.GetCutLineRightBottomY());
        }
    }
}
