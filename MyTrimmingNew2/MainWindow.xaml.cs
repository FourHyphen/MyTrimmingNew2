using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyTrimmingNew2
{
    public partial class MainWindow : Window
    {
        private OriginalImage _OriginalImage { get; set; } = null;

        private ShowingImage _ShowingImage { get; set; } = null;

        private Preview PreviewWindow { get; set; } = null;

        private CutLine _CutLine { get; set; } = null;

        private System.Windows.Point MouseDownPoint { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            MouseDownPoint = GetMouseDownInitPoint();
        }

        private System.Windows.Point GetMouseDownInitPoint()
        {
            // 負の値なら値は適当でOK
            return new Point(-1, -1);
        }

        private void MenuOpenFileClick(object sender, RoutedEventArgs e)
        {
            string filePath = DialogOpenImageFile.Show();
            if (filePath != "")
            {
                DisplayImageAndCutLine(filePath);
            }
        }

        private void DisplayImageAndCutLine(string imagePath)
        {
            _OriginalImage = new OriginalImage(imagePath);
            DisplayImageAndCutLine(_OriginalImage);
        }

        private void DisplayImageAndCutLine(OriginalImage originalImage)
        {
            _ShowingImage = new ShowingImage(originalImage, (int)ImageArea.ActualWidth, (int)ImageArea.ActualHeight);
            DisplayImage(originalImage, _ShowingImage);
            DisplayCutLine(_ShowingImage);
        }

        private void DisplayImage(OriginalImage originalImage, ShowingImage showingImage)
        {
            OriginalImageWidth.Content = originalImage.Width.ToString();
            OriginalImageHeight.Content = originalImage.Height.ToString();

            ShowingImage.Source = showingImage.Source;
            ShowingImageWidth.Content = showingImage.Width.ToString();
            ShowingImageHeight.Content = showingImage.Height.ToString();
        }

        private void DisplayCutLine(ShowingImage image)
        {
            _CutLine = new CutLine(image);
            CutLineDisplay.Update(this, image, _CutLine);
        }

        private void MainWindowKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.Input.Key key = e.Key;
            System.Windows.Input.ModifierKeys modifierKeys = e.KeyboardDevice.Modifiers;
            InputKey(key, modifierKeys);
        }

        private void InputKey(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKeys)
        {
            if (_CutLine != null)
            {
                if (AppKey.IsPurposeShowPreview(key, modifierKeys))
                {
                    OpenPreviewWindow();
                }
                else if (AppKey.IsPurposeClosePreview(key, modifierKeys))
                {
                    ClosePreviewWindow();
                }
                else
                {
                    _CutLine.ExecuteCommand(key, modifierKeys, 1);
                    CutLineDisplay.Update(this, _ShowingImage, _CutLine);
                }
            }
        }

        private void MainWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO: サイズ変更中は連続的に本イベントが発生する、本当はResizeEndが良い
            // FormのResizeEndに相当するイベントがWPFにはないので別途検討
            double width = e.NewSize.Width;
            double height = e.NewSize.Height;
            ChangeWindowSize((int)width, (int)height);
        }

        private void ChangeWindowSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            ImageAreaWidth.Content = ((int)ImageArea.ActualWidth).ToString();
            ImageAreaHeight.Content = ((int)ImageArea.ActualHeight).ToString();

            if (_OriginalImage != null)
            {
                DisplayImageAndCutLine(_OriginalImage);
            }
        }

        private void OpenPreviewWindow()
        {
            if (_OriginalImage == null)
            {
                return;
            }

            PreviewWindow = new Preview(_OriginalImage, _ShowingImage, _CutLine);
            PreviewWindow.Show();
        }

        private void ClosePreviewWindow()
        {
            if (PreviewWindow != null)
            {
                PreviewWindow.Close();
                PreviewWindow = null;
            }
        }

        private void ShowingImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowingImageMouseDown(e.GetPosition(CutLine));
        }

        private void ShowingImageMouseDown(System.Windows.Point relativeCoordinateToCutLine)
        {
            MouseDownPoint = relativeCoordinateToCutLine;
        }

        private void ShowingImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowingImageMouseUp(MouseDownPoint, e.GetPosition(CutLine));
        }

        private void ShowingImageMouseUp(System.Windows.Point mouseDown, System.Windows.Point mouseUp)
        {
            if (_CutLine != null)
            {
                // マウスクリックで画像を開いたときにもMouseUpイベントが発行されるのでガード
                if (mouseDown != GetMouseDownInitPoint())
                {
                    _CutLine.ExecuteCommand(mouseDown, mouseUp);
                    CutLineDisplay.Update(this, _ShowingImage, _CutLine);
                }
            }
        }

        private void MenuSaveFileClick(object sender, RoutedEventArgs e)
        {
            if (_OriginalImage == null)
            {
                return;
            }

            string filePath = DialogSaveImageFile.Show(_OriginalImage);
            if (filePath != "")
            {
                SaveImage(filePath);
            }
        }

        private void SaveImage(string filePath)
        {
            try
            {
                ImageProcess.SaveImage(filePath,
                                       _OriginalImage.Path,
                                       _ShowingImage.ToOriginalScale(_CutLine.LeftTop),
                                       _ShowingImage.ToOriginalScale(_CutLine.RightTop),
                                       _ShowingImage.ToOriginalScale(_CutLine.RightBottom),
                                       _ShowingImage.ToOriginalScale(_CutLine.LeftBottom),
                                       _CutLine.Degree);
                ShowSaveResult("画像の保存に成功しました。", "Info");
            }
            catch
            {
                ShowSaveResult("画像の保存に失敗しました。再度保存してください。", "Error");
            }
        }

        private static void ShowSaveResult(string message, string title)
        {
            System.Windows.Forms.MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
