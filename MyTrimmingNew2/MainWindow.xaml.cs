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
            CutLineDisplay.Update(this, _CutLine);
        }

        private void MainWindowKeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Input.Key key = e.Key;
            InputKey(key);
        }

        private void InputKey(System.Windows.Input.Key key)
        {
            if (_CutLine != null)
            {
                _CutLine.SetCommand(key);
                _CutLine.ExecuteCommand();
                CutLineDisplay.Update(this, _CutLine);
            }
        }

        private void ShowingImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowingImageMouseDown(e.GetPosition(CutLine));
        }

        private void ShowingImageMouseDown(System.Windows.Point relativeCoordinateToCutLine)
        {
            if (_CutLine != null)
            {
                _CutLine.SetCommand(relativeCoordinateToCutLine);
            }
        }

        private void ShowingImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowingImageMouseUp(e.GetPosition(CutLine));
        }

        private void ShowingImageMouseUp(System.Windows.Point relativeCoordinateToCutLine)
        {
            if (_CutLine != null)
            {
                _CutLine.ExecuteCommand(relativeCoordinateToCutLine);
                CutLineDisplay.Update(this, _CutLine);
            }
        }
    }
}
