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
            ShowingImage _ShowingImage = new ShowingImage(imagePath, (int)ImageArea.ActualWidth, (int)ImageArea.ActualHeight);
            OriginalImageWidth.Content = _ShowingImage.OriginalImageWidth.ToString();
            OriginalImageHeight.Content = _ShowingImage.OriginalImageHeight.ToString();

            ShowingImage.Source = _ShowingImage.Source;
            ShowingImageWidth.Content = _ShowingImage.Width.ToString();
            ShowingImageHeight.Content = _ShowingImage.Height.ToString();
        }
    }
}
