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
            BitmapSource bs = MyImage.GetShowImage(imagePath);
            ShowImage.Source = bs;
            OriginalImageWidth.Content = bs.PixelWidth.ToString();
            OriginalImageHeight.Content = bs.PixelHeight.ToString();
        }
    }
}
