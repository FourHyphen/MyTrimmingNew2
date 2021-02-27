﻿using System;
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
        private CutLine _CutLine { get; set; } = null;

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
            _CutLine = new CutLine(image);
            DisplayCutLineCore(_CutLine);
        }

        private void DisplayCutLineCore(CutLine cutLine)
        {
            System.Windows.Point leftTop = new System.Windows.Point(cutLine.Left, cutLine.Top);
            System.Windows.Point rightTop = new System.Windows.Point(cutLine.Right, cutLine.Top);
            System.Windows.Point rightBottom = new System.Windows.Point(cutLine.Right, cutLine.Bottom);
            System.Windows.Point leftBottom = new System.Windows.Point(cutLine.Left, cutLine.Bottom);

            CutLine.Points[0] = leftTop;
            CutLine.Points[1] = rightTop;
            CutLine.Points[2] = rightBottom;
            CutLine.Points[3] = leftBottom;
            CutLine.Points[4] = leftTop;  // 長方形として閉じる

            SetLabelCoordinate(leftTop, CutLineLeftTopX, CutLineLeftTopY);
            SetLabelCoordinate(rightTop, CutLineRightTopX, CutLineRightTopY);
            SetLabelCoordinate(rightBottom, CutLineRightBottomX, CutLineRightBottomY);
            SetLabelCoordinate(leftBottom, CutLineLeftBottomX, CutLineLeftBottomY);

            CutLineWidth.Content = cutLine.Width.ToString();
            CutLineHeight.Content = cutLine.Height.ToString();
        }

        private void SetLabelCoordinate(System.Windows.Point p, Label labelX, Label labelY)
        {
            labelX.Content = p.X.ToString();
            labelY.Content = p.Y.ToString();
        }

        private void MainWindowKeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Input.Key key = e.Key;
            InputKey(key);
        }

        private void InputKey(System.Windows.Input.Key key)
        {
            if (_CutLine == null)
            {
                return;
            }

            if (key == Key.Up)
            {
                _CutLine.MoveY(-1);
            }
            else if (key == Key.Down)
            {
                _CutLine.MoveY(1);
            }

            DisplayCutLineCore(_CutLine);
        }
    }
}
