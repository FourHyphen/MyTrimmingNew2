using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;
using System.Windows.Input;

namespace TestMyTrimmingNew2
{
    class PreviewDriver
    {
        private dynamic MainWindow { get; set; }
        private WindowControl _WindowControl { get; set; }
        private IWPFDependencyObjectCollection<System.Windows.DependencyObject> Tree { get; set; }
        private LabelAdapter PreviewTrimImageWidth { get; }
        private LabelAdapter PreviewTrimImageHeight { get; }

        public PreviewDriver(dynamic mainWindow, WindowsAppFriend app)
        {
            // 本来はPreview画面を取得すべきだが知識不足でできなかったためMainWindowで代用
            // (Preview画面を取得できればPreview.InputKey()に直接繋げられるためMainWindowのIsPurposeClosePreview()周りが不要になる)
            MainWindow = mainWindow;
            _WindowControl = WindowControl.FromZTop(app);
            Tree = TreeUtilityExtensions.LogicalTree(_WindowControl);

            PreviewTrimImageWidth = new LabelAdapter("TrimImageWidth");
            PreviewTrimImageHeight = new LabelAdapter("TrimImageHeight");
        }

        internal double GetPreviewTrimImageWidth()
        {
            UpdateNowMainWindowStatus();
            return PreviewTrimImageWidth.ContentNum(Tree);
        }

        internal double GetPreviewTrimImageHeight()
        {
            UpdateNowMainWindowStatus();
            return PreviewTrimImageHeight.ContentNum(Tree);
        }

        private void UpdateNowMainWindowStatus()
        {
            Tree = TreeUtilityExtensions.LogicalTree(_WindowControl);    // 現在の画面状況を取得
        }

        internal void EmurateInputKey(Key key, int num, ModifierKeys modifierKeys = ModifierKeys.None)
        {
            for (int i = 0; i < num; i++)
            {
                MainWindow.InputKey(key, modifierKeys);
            }
        }
    }
}
