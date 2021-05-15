namespace MyTrimmingNew2
{
    public class CutLineCommandFactory
    {
        private CutLineCommandFactory() { }

        public static CutLineCommand Create(CutLine cutLine, ShowingImage image)
        {
            return new CutLineInit(cutLine, image);
        }

        public static CutLineCommand Create(CutLine cutLine,
                                            ShowingImage image,
                                            System.Windows.Input.Key key,
                                            System.Windows.Input.ModifierKeys modifierKey,
                                            int keyInputNum)
        {
            if (AppKey.IsPurposeMove(key))
            {
                return new CutLineMove(cutLine, image, key, modifierKey, keyInputNum);
            }
            else if (AppKey.IsPurposeRotate(key))
            {
                return new CutLineRotate(cutLine, image, key, modifierKey, keyInputNum);
            }

            return null;
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
            else if (cutLine.IsPointNearRightTop(dragStart))
            {
                return new CutLineChangeSizeRightTop(cutLine, image, dragStart, dropPoint);
            }
            else if (cutLine.IsPointNearLeftBottom(dragStart))
            {
                return new CutLineChangeSizeLeftBottom(cutLine, image, dragStart, dropPoint);
            }
            else if (cutLine.IsPointInside(dragStart))
            {
                return new CutLineMove(cutLine, image, dragStart, dropPoint);
            }

            return null;
        }
    }
}
