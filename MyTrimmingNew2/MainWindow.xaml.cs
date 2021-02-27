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
                DisplayImage(filePath);
            }
        }

        private void DisplayImage(string imagePath)
        {
            OriginalImage originalImage = new OriginalImage(imagePath);
            ShowingImage showingImage = new ShowingImage(originalImage, (int)ImageArea.ActualWidth, (int)ImageArea.ActualHeight);
            DisplayImageCore(originalImage, showingImage);
            DisplayCutLine(showingImage);
        }

        private void DisplayImageCore(OriginalImage originalImage, ShowingImage showingImage)
        {
            OriginalImageWidth.Content = originalImage.Width.ToString();
            OriginalImageHeight.Content = originalImage.Height.ToString();

            ShowingImage.Source = showingImage.Source;
            ShowingImageWidth.Content = showingImage.Width.ToString();
            ShowingImageHeight.Content = showingImage.Height.ToString();
        }

        private void DisplayCutLine(ShowingImage image)
        {
            CutLine cl = new CutLine(image);

            DisplayCutLineCore(cl);
            CutLineWidth.Content = cl.Width.ToString();
            CutLineHeight.Content = cl.Height.ToString();
        }

        private void DisplayCutLineCore(CutLine cutLine)
        {
            System.Windows.Point origin = new System.Windows.Point(cutLine.Left, cutLine.Top);
            CutLine.Points[0] = origin;
            CutLine.Points[1] = new System.Windows.Point(cutLine.Width, cutLine.Top);
            CutLine.Points[2] = new System.Windows.Point(cutLine.Width, cutLine.Height);
            CutLine.Points[3] = new System.Windows.Point(cutLine.Left, cutLine.Height);
            CutLine.Points[4] = origin;  // 長方形として閉じる
        }
    }
}
