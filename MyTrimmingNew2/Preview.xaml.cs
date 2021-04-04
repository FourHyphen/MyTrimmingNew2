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
            BitmapSource source;

            CreatePreviewImage(originalImage, showingImage, cutLine, out source, out willSaveWidth, out willSaveHeight);
            PreviewImage.Source = source;
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
            double showWidth = this.Width * 4.0 / 5.0;
            double showHeight = showWidth * cutLine.Ratio;

            ImageProcess.CreateTrimImage(originalImage.Path,
                                         showingImage.ToOriginalScale(cutLine.LeftTop),
                                         showingImage.ToOriginalScale(cutLine.RightTop),
                                         showingImage.ToOriginalScale(cutLine.RightBottom),
                                         showingImage.ToOriginalScale(cutLine.LeftBottom),
                                         cutLine.Degree,
                                         (int)showWidth,
                                         (int)showHeight,
                                         out source,
                                         out willSaveWidth,
                                         out willSaveHeight);
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
