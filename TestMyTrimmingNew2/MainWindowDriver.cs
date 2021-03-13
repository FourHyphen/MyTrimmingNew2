using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;
using System.Windows.Documents;
using Codeer.Friendly.Dynamic;
using System;
using System.Windows.Input;
using System.Windows;

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
        private LabelAdapter CutLineLeftBottomX { get; }
        private LabelAdapter CutLineLeftBottomY { get; }
        private LabelAdapter CutLineRightTopX { get; }
        private LabelAdapter CutLineRightTopY { get; }
        private LabelAdapter CutLineRightBottomX { get; }
        private LabelAdapter CutLineRightBottomY { get; }
        private LabelAdapter CutLineRotateDegree { get; }

        public MainWindowDriver(dynamic mainWindow)
        {
            MainWindow = mainWindow;
            Tree = new WindowControl(mainWindow).LogicalTree();
            OriginalImageWidth = new LabelAdapter("OriginalImageWidth");
            OriginalImageHeight = new LabelAdapter("OriginalImageHeight");
            ImageAreaWidth = new LabelAdapter("ImageAreaWidth");
            ImageAreaHeight = new LabelAdapter("ImageAreaHeight");
            ShowingImageWidth = new LabelAdapter("ShowingImageWidth");
            ShowingImageHeight = new LabelAdapter("ShowingImageHeight");
            CutLineWidth = new LabelAdapter("CutLineWidth");
            CutLineHeight = new LabelAdapter("CutLineHeight");
            CutLineLeftTopX = new LabelAdapter("CutLineLeftTopX");
            CutLineLeftTopY = new LabelAdapter("CutLineLeftTopY");
            CutLineLeftBottomX = new LabelAdapter("CutLineLeftBottomX");
            CutLineLeftBottomY = new LabelAdapter("CutLineLeftBottomY");
            CutLineRightTopX = new LabelAdapter("CutLineRightTopX");
            CutLineRightTopY = new LabelAdapter("CutLineRightTopY");
            CutLineRightBottomX = new LabelAdapter("CutLineRightBottomX");
            CutLineRightBottomY = new LabelAdapter("CutLineRightBottomY");
            CutLineRotateDegree = new LabelAdapter("CutLineRotateDegree");
        }

        internal void EmurateOpenImage(string imagePath)
        {
            MainWindow.DisplayImage(imagePath);
        }

        internal double GetOriginalImageWidth()
        {
            UpdateNowMainWindowStatus();
            return OriginalImageWidth.ContentNum(Tree);
        }

        internal double GetOriginalImageHeight()
        {
            UpdateNowMainWindowStatus();
            return OriginalImageHeight.ContentNum(Tree);
        }

        internal double GetImageAreaWidth()
        {
            UpdateNowMainWindowStatus();
            return ImageAreaWidth.ContentNum(Tree);
        }

        internal object GetImageAreaHeight()
        {
            UpdateNowMainWindowStatus();
            return ImageAreaHeight.ContentNum(Tree);
        }

        internal double GetShowingImageWidth()
        {
            UpdateNowMainWindowStatus();
            return ShowingImageWidth.ContentNum(Tree);
        }

        internal double GetShowingImageHeight()
        {
            UpdateNowMainWindowStatus();
            return ShowingImageHeight.ContentNum(Tree);
        }

        internal double GetCutLineWidth()
        {
            UpdateNowMainWindowStatus();
            return CutLineWidth.ContentNum(Tree);
        }

        internal double GetCutLineHeight()
        {
            UpdateNowMainWindowStatus();
            return CutLineHeight.ContentNum(Tree);
        }

        internal double GetCutLineLeftTopX()
        {
            UpdateNowMainWindowStatus();
            return CutLineLeftTopX.ContentNum(Tree);
        }

        internal double GetCutLineLeftTopY()
        {
            UpdateNowMainWindowStatus();
            return CutLineLeftTopY.ContentNum(Tree);
        }

        internal double GetCutLineLeftBottomX()
        {
            UpdateNowMainWindowStatus();
            return CutLineLeftBottomX.ContentNum(Tree);
        }

        internal double GetCutLineLeftBottomY()
        {
            UpdateNowMainWindowStatus();
            return CutLineLeftBottomY.ContentNum(Tree);
        }

        internal double GetCutLineRightTopX()
        {
            UpdateNowMainWindowStatus();
            return CutLineRightTopX.ContentNum(Tree);
        }

        internal double GetCutLineRightTopY()
        {
            UpdateNowMainWindowStatus();
            return CutLineRightTopY.ContentNum(Tree);
        }

        internal double GetCutLineRightBottomX()
        {
            UpdateNowMainWindowStatus();
            return CutLineRightBottomX.ContentNum(Tree);
        }

        internal double GetCutLineRightBottomY()
        {
            UpdateNowMainWindowStatus();
            return CutLineRightBottomY.ContentNum(Tree);
        }

        internal double GetCutLineRotateDegree()
        {
            UpdateNowMainWindowStatus();
            return CutLineRotateDegree.ContentNum(Tree);
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

        internal void EmurateShowingImageMouseDragAndDrop(Point drag, Point drop)
        {
            MainWindow.ShowingImageMouseUp(drag, drop);
        }
    }
}
