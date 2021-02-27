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
            int originalImageWidth = 0, originalImageHeight = 0;

            // 画像を開く際に検証処理しないことで高速に読み込む
            using (System.IO.FileStream fs = System.IO.File.OpenRead(imagePath))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(fs, false, false))
                {
                    originalImageWidth = image.Width;
                    originalImageHeight = image.Height;
                }
            }

            OriginalImageWidth.Content = originalImageWidth.ToString();
            OriginalImageHeight.Content = originalImageHeight.ToString();

            BitmapSource bs = CreateBitmapSourceFitImageArea(imagePath, originalImageWidth, originalImageHeight);
            ShowingImage.Source = bs;
            ShowingImageWidth.Content = bs.PixelWidth.ToString();
            ShowingImageHeight.Content = bs.PixelHeight.ToString();
        }

        private BitmapSource CreateBitmapSourceFitImageArea(string imagePath, int imageWidth, int imageHeight)
        {
            double ratio = CalcRatioOfFittingImageAreaKeepingImageRatio(imageWidth, imageHeight);
            double fitWidth = (double)imageWidth * ratio;
            double fitHeight = (double)imageHeight * ratio;
            return MyImage.GetShowImage(imagePath, (int)fitWidth, (int)fitHeight);
        }

        private double CalcRatioOfFittingImageAreaKeepingImageRatio(int imageWidth, int imageHeight)
        {
            double ratioBaseWidth = (double)ImageArea.ActualWidth / imageWidth;
            double ratioBaseHeight = (double)ImageArea.ActualHeight / imageHeight;

            // 倍率が大きいと ImageArea から縦(横)だけがはみ出る
            if (ratioBaseWidth < ratioBaseHeight)
            {
                return ratioBaseWidth;
            }
            return ratioBaseHeight;
        }
    }
}
