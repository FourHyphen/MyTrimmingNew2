using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyTrimmingNew2
{
    public class CutLine
    {
        private ShowingImage _ShowingImage { get; set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Left { get; private set; }

        public int Top { get; private set; }

        public int Right
        {
            get
            {
                return Left + Width;
            }
        }

        public int Bottom
        {
            get
            {
                return Top + Height;
            }
        }

        public CutLine(ShowingImage showingImage)
        {
            _ShowingImage = showingImage;
            Init();
        }

        private void Init()
        {
            InitOrigin();
            InitSize();
        }

        private void InitOrigin()
        {
            Left = 0;
            Top = 0;
        }

        private void InitSize()
        {
            double width = _ShowingImage.Width;
            double height = width * 9.0 / 16.0;
            if (height > _ShowingImage.Height)
            {
                height = _ShowingImage.Height;
                width = height * 16.0 / 9.0;
            }

            Width = (int)width;
            Height = (int)height;
        }

        public void MoveY(int yDirection)
        {
            int newTop = Top + yDirection;
            int newBottom = Top + Height + yDirection;
            bool doStickOutTop = DoTopStickOutOfImage(newTop);
            bool doStickOutBottom = DoBottomStickOutOfImage(newBottom);

            if (IsCutLineInsideImage(doStickOutTop, doStickOutBottom))
            {
                Top = newTop;
            }
            else
            {
                AdjustTop(doStickOutTop, doStickOutBottom);
            }
        }

        private bool DoTopStickOutOfImage(int top)
        {
            return (top < 0);
        }

        private bool DoBottomStickOutOfImage(int bottom)
        {
            return (bottom > _ShowingImage.Height);
        }

        private bool IsCutLineInsideImage(bool doStickOutTop, bool doStickOutBottom)
        {
            return (!doStickOutTop && !doStickOutBottom);
        }

        private void AdjustTop(bool doStickOutTop, bool doStickOutBottom)
        {
            if (doStickOutTop)
            {
                Top = 0;
            }
            else if (doStickOutBottom)
            {
                Top = _ShowingImage.Height - Height;
            }
        }

        public void ChangeSizeBaseRightBottom(Point dragStart, Point dropPoint)
        {
            double distanceX = dropPoint.X - dragStart.X;
            double distanceY = dropPoint.Y - dragStart.Y;
            double changeSizeY = distanceX * 9.0 / 16.0;
            if (Math.Abs(changeSizeY) > Math.Abs(distanceY))
            {
                Width += (int)distanceX;
                Height += (int)changeSizeY;
            }
            else
            {
                Width += (int)(distanceY * 16.0 / 9.0);
                Height += (int)distanceY;
            }
        }
    }
}
