using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyTrimmingNew2
{
    public partial class Preview : Window
    {
        public Preview(OriginalImage originalImage, ShowingImage showingImage, CutLine cutLine)
        {
            InitializeComponent();
            Init(originalImage, showingImage, cutLine);
        }

        private void Init(OriginalImage originalImage, ShowingImage showingImage, CutLine cutLine)
        {
            int willSaveWidth, willSaveHeight;
            BitmapSource previewImage;
            CreatePreviewImage(originalImage, showingImage, cutLine, out previewImage, out willSaveWidth, out willSaveHeight);

            PreviewImage.Source = previewImage;
            TrimImageWidth.Content = willSaveWidth.ToString();
            TrimImageHeight.Content = willSaveHeight.ToString();
        }

        private void CreatePreviewImage(OriginalImage originalImage,
                                        ShowingImage showingImage,
                                        CutLine cutLine,
                                        out BitmapSource source,
                                        out int willSaveWidth,
                                        out int willSaveHeight)
        {
            // 1/1サイズを作成
            System.Drawing.Bitmap trimBitmapOriginalScale = CreateTrimBitmap(originalImage, showingImage, cutLine);
            willSaveWidth = trimBitmapOriginalScale.Width;
            willSaveHeight = trimBitmapOriginalScale.Height;

            // プレビュー画面用に縮小
            double showWidth = this.Width * 4.0 / 5.0;    // 画面配置Gridの比率
            double showHeight = showWidth * cutLine.Ratio;
            source = ImageProcess.GetShowImage(trimBitmapOriginalScale, (int)showWidth, (int)showHeight);
        }

        private System.Drawing.Bitmap CreateTrimBitmap(OriginalImage originalImage, ShowingImage showingImage, CutLine cutLine)
        {
            return ImageProcess.CreateTrimBitmap(originalImage.Path,
                                                 showingImage.ToOriginalScale(cutLine.LeftTop),
                                                 showingImage.ToOriginalScale(cutLine.RightTop),
                                                 showingImage.ToOriginalScale(cutLine.RightBottom),
                                                 showingImage.ToOriginalScale(cutLine.LeftBottom),
                                                 cutLine.Degree);
        }

        private void PreviewWindowKeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Input.Key key = e.Key;
            System.Windows.Input.ModifierKeys modifierKeys = e.KeyboardDevice.Modifiers;
            InputKey(key, modifierKeys);
        }

        private void InputKey(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKeys)
        {
            if (AppKey.IsPurposeClosePreview(key, modifierKeys))
            {
                Close();
            }
        }
    }
}
