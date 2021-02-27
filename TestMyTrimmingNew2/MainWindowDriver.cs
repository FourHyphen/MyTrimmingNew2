using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;
using System.Windows.Documents;
using Codeer.Friendly.Dynamic;
using System;
using System.Windows.Input;

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
        private LabelAdapter ImageAreaHeight { get; }
        private LabelAdapter ShowingImageWidth { get; }
        private LabelAdapter ShowingImageHeight { get; }
        private LabelAdapter CutLineWidth { get; }
        private LabelAdapter CutLineHeight { get; }
        private LabelAdapter CutLineLeftTopX { get; }
        private LabelAdapter CutLineLeftTopY { get; }
        private LabelAdapter CutLineLeftBottomY { get; }
        private LabelAdapter CutLineRightBottomY { get; }

        public MainWindowDriver(dynamic mainWindow)
        {
            MainWindow = mainWindow;
            Tree = new WindowControl(mainWindow).LogicalTree();
            OriginalImageWidth = new LabelAdapter("OriginalImageWidth");
            OriginalImageHeight = new LabelAdapter("OriginalImageHeight");
            ImageAreaWidth = new LabelAdapter("ImageAreaWidth");
            ShowingImageWidth = new LabelAdapter("ShowingImageWidth");
            ShowingImageHeight = new LabelAdapter("ShowingImageHeight");
            CutLineWidth = new LabelAdapter("CutLineWidth");
            CutLineHeight = new LabelAdapter("CutLineHeight");
            CutLineLeftTopX = new LabelAdapter("CutLineLeftTopX");
            CutLineLeftTopY = new LabelAdapter("CutLineLeftTopY");
            CutLineLeftBottomY = new LabelAdapter("CutLineLeftBottomY");
            CutLineRightBottomY = new LabelAdapter("CutLineRightBottomY");
        }

        internal void EmurateOpenImage(string imagePath)
        {
            MainWindow.DisplayImage(imagePath);
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

        internal object GetImageAreaHeight()
        {
            UpdateNowMainWindowStatus();
            return ImageAreaHeight.ContentNum(Tree);
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

        internal int GetCutLineWidth()
        {
            UpdateNowMainWindowStatus();
            return CutLineWidth.ContentNum(Tree);
        }

        internal int GetCutLineHeight()
        {
            UpdateNowMainWindowStatus();
            return CutLineHeight.ContentNum(Tree);
        }

        internal int GetCutLineLeftTopX()
        {
            UpdateNowMainWindowStatus();
            return CutLineLeftTopX.ContentNum(Tree);
        }

        internal int GetCutLineLeftTopY()
        {
            UpdateNowMainWindowStatus();
            return CutLineLeftTopY.ContentNum(Tree);
        }

        internal int GetCutLineLeftBottomY()
        {
            UpdateNowMainWindowStatus();
            return CutLineLeftBottomY.ContentNum(Tree);
        }

        internal int GetCutLineRightBottomY()
        {
            UpdateNowMainWindowStatus();
            return CutLineRightBottomY.ContentNum(Tree);
        }

        private void UpdateNowMainWindowStatus()
        {
            Tree = new WindowControl(MainWindow).LogicalTree();    // 現在の画面状況を取得
        }

        internal void EmurateInputKey(Key key, int num)
        {
            for (int i = 0; i < num; i++)
            {
                MainWindow.InputKey(key);
            }
        }
    }
}
