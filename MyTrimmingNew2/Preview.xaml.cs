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
            ImageSource source = ImageProcess.CreateTrimImage(originalImage.Path,
                                                           showingImage.ToOriginalScale(cutLine.LeftTop),
                                                           showingImage.ToOriginalScale(cutLine.RightTop),
                                                           showingImage.ToOriginalScale(cutLine.RightBottom),
                                                           showingImage.ToOriginalScale(cutLine.LeftBottom),
                                                           cutLine.Degree,
                                                           (int)cutLine.Width,
                                                           (int)cutLine.Height,
                                                           out willSaveWidth,
                                                           out willSaveHeight);
            PreviewImage.Source = source;
            TrimImageWidth.Content = willSaveWidth.ToString();
            TrimImageHeight.Content = willSaveHeight.ToString();
        }

        private void PreviewWindowKeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Input.Key key = e.Key;
            System.Windows.Input.ModifierKeys modifierKeys = e.KeyboardDevice.Modifiers;
            InputKey(key, modifierKeys);
        }

        private void InputKey(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKeys)
        {
            if (IsPurposeClose(key, modifierKeys))
            {
                Close();
            }
        }

        public bool IsPurposeClose(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKeys)
        {
            return (key == Key.W && modifierKeys == ModifierKeys.Control);
        }
    }
}
