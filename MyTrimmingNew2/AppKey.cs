using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class AppKey
    {
        public static bool IsPurposeOpenImage(Key key, ModifierKeys modifierKey)
        {
            return (key == Key.O && modifierKey == ModifierKeys.Control);
        }

        public static bool IsPurposeSaveImage(Key key, ModifierKeys modifierKey)
        {
            return (key == Key.S && modifierKey == ModifierKeys.Control);
        }

        public static bool IsPurposeShowPreview(Key key, ModifierKeys modifierKey)
        {
            return (key == Key.P && modifierKey == ModifierKeys.Control);
        }

        public static bool IsPurposeClosePreview(Key key, ModifierKeys modifierKey)
        {
            return (key == Key.W && modifierKey == ModifierKeys.Control);
        }

        public static bool IsPurposeUndo(Key key, ModifierKeys modifierKey)
        {
            return (modifierKey == ModifierKeys.Control && key == Key.Z);
        }

        public static bool IsPurposeRedo(Key key, ModifierKeys modifierKey)
        {
            return (modifierKey == ModifierKeys.Control && key == Key.Y);
        }

        public static bool IsPurposeMove(Key key)
        {
            return (key == Key.Up || key == Key.Down || key == Key.Right || key == Key.Left);
        }

        public static bool IsPurposeRotate(Key key)
        {
            return (key == Key.OemPlus || key == Key.OemMinus);
        }
    }
}