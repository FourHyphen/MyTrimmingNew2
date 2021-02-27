using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;
using System.Windows.Documents;
using Codeer.Friendly.Dynamic;
using System;

namespace TestMyTrimmingNew2
{
    class MainWindowDriver
    {
        // 個人的メモ：本来はCodeer.Friendly APIに依存しないようインタフェースでラップすべきだが、UIテスト可能なAPIを他に知らないのでこのままにする
        private dynamic MainWindow { get; }
        private IWPFDependencyObjectCollection<System.Windows.DependencyObject> Tree { get; set; }
        private LabelAdapter OriginalImageWidth { get; }
        private LabelAdapter OriginalImageHeight { get; }
        private LabelAdapter ImageAreaWidth { get; }
        private LabelAdapter ShowingImageWidth { get; }
        private LabelAdapter ShowingImageHeight { get; }

        public MainWindowDriver(dynamic mainWindow)
        {
            MainWindow = mainWindow;
            Tree = new WindowControl(mainWindow).LogicalTree();
            OriginalImageWidth = new LabelAdapter("OriginalImageWidth");
            OriginalImageHeight = new LabelAdapter("OriginalImageHeight");
            ImageAreaWidth = new LabelAdapter("ImageAreaWidth");
            ShowingImageWidth = new LabelAdapter("ShowingImageWidth");
            ShowingImageHeight = new LabelAdapter("ShowingImageHeight");
        }

        internal void EmurateOpenImage(string imagePath)
        {
            MainWindow.OpenImage(imagePath);
        }

        internal int GetOriginalImageWidth()
        {
            UpdateNowMainWindowStatus();
            return OriginalImageWidth.ContentNum(Tree);
        }

        internal int GetOriginalImageHeight()
        {
            UpdateNowMainWindowStatus();
            return OriginalImageHeight.ContentNum(Tree);
        }

        internal int GetImageAreaWidth()
        {
            UpdateNowMainWindowStatus();
            return ImageAreaWidth.ContentNum(Tree);
        }

        internal int GetShowingImageWidth()
        {
            UpdateNowMainWindowStatus();
            return ShowingImageWidth.ContentNum(Tree);
        }

        internal int GetShowingImageHeight()
        {
            UpdateNowMainWindowStatus();
            return ShowingImageHeight.ContentNum(Tree);
        }

        private void UpdateNowMainWindowStatus()
        {
            Tree = new WindowControl(MainWindow).LogicalTree();    // 現在の画面状況を取得
        }
    }
}
