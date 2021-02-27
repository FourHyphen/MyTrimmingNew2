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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyTrimmingNew2
{
    public partial class MainWindow : Window
    {
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
            ImageAreaWidth.Content = ((int)ImageArea.ActualWidth).ToString();
            ImageAreaHeight.Content = ((int)ImageArea.ActualHeight).ToString();
        }

        private void MenuOpenFileClick(object sender, RoutedEventArgs e)
        {
            string filePath = DialogOpenImageFile.Show();
            if (filePath != "")
            {
                OpenImage(filePath);
            }
        }

        private void OpenImage(string imagePath)
        {
            SetOriginalImageSize(imagePath);

            BitmapSource bs = MyImage.GetShowImage(imagePath);
            ShowingImage.Source = bs;
            ShowingImageWidth.Content = bs.PixelWidth.ToString();
            ShowingImageHeight.Content = bs.PixelHeight.ToString();
        }

        private void SetOriginalImageSize(string imagePath)
        {
            // 画像を開く際に検証処理しないことで高速に読み込む
            using (System.IO.FileStream fs = System.IO.File.OpenRead(imagePath)) {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(fs, false, false))
                {
                    OriginalImageWidth.Content = image.Width.ToString();
                    OriginalImageHeight.Content = image.Height.ToString();
                }
            }
        }
    }
}
