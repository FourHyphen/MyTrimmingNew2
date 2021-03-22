using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineCommandFactory
    {
        private CutLineCommandFactory() { }

        public static CutLineCommand Create(CutLine cutLine,
                                            ShowingImage image,
                                            System.Windows.Input.Key key,
                                            System.Windows.Input.ModifierKeys modifierKeys,
                                            int keyInputNum)
        {
            if (IsPurposeMove(key))
            {
                return new CutLineMove(cutLine, image, key, modifierKeys, keyInputNum);
            } else if (IsPurposeRotate(key))
            {
                return new CutLineRotate(cutLine, image, key, keyInputNum);
            }

            return null;
        }

        private static bool IsPurposeMove(System.Windows.Input.Key key)
        {
            return (key == System.Windows.Input.Key.Up ||
                    key == System.Windows.Input.Key.Down ||
                    key == System.Windows.Input.Key.Right ||
                    key == System.Windows.Input.Key.Left);
        }

        private static bool IsPurposeRotate(System.Windows.Input.Key key)
        {
            return (key == System.Windows.Input.Key.OemPlus ||
                    key == System.Windows.Input.Key.OemMinus);
        }

        public static CutLineCommand Create(CutLine cutLine, ShowingImage image, System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            if (cutLine.IsPointNearRightBottom(dragStart))
            {
                return new CutLineChangeSizeRightBottom(cutLine, image, dragStart, dropPoint);
            }
            else if (cutLine.IsPointNearLeftTop(dragStart))
            {
                return new CutLineChangeSizeLeftTop(cutLine, image, dragStart, dropPoint);
            }
            else if (cutLine.IsPointInside(dragStart))
            {
                return new CutLineMove(cutLine, image, dragStart, dropPoint);
            }

            return null;
        }
    }
}
