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
        public Preview()
        {
            InitializeComponent();
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
