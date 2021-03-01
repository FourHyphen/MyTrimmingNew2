﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLine
    {
        private ShowingImage _ShowingImage { get; set; }

        private static double RatioWidth = 16.0;

        private static double RatioHeight = 9.0;

        private static int NearRange = 20;

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
            double height = CalcHeightBaseWidth(width);
            if (height > _ShowingImage.Height)
            {
                height = _ShowingImage.Height;
                width = CalcWidthBaseHeight(height);
            }

            Width = (int)width;
            Height = (int)height;
        }

        private double CalcHeightBaseWidth(double width)
        {
            return width * RatioHeight / RatioWidth;
        }

        private double CalcWidthBaseHeight(double height)
        {
            return height * RatioWidth / RatioHeight;
        }

        public void Move(Key key, int num = 1)
        {
            if (key == Key.Left)
            {
                MoveX(-1 * num);
            }
            else if (key == Key.Right)
            {
                MoveX(1 * num);
            }
            else if (key == Key.Up)
            {
                MoveY(-1 * num);
            }
            else if (key == Key.Down)
            {
                MoveY(1 * num);
            }
        }

        private void MoveX(int xDirection)
        {
            int newLeft = Left + xDirection;
            int newRight = Right + xDirection;
            bool doStickOutLeft = DoLeftStickOutOfImage(newLeft);
            bool doStickOutRight = DoRightStickOutOfImage(newRight);

            if (IsCutLineInsideImageHorizontal(doStickOutLeft, doStickOutRight))
            {
                Left = newLeft;
            }
            else
            {
                AdjustLeft(doStickOutLeft, doStickOutRight);
            }
        }

        private bool DoLeftStickOutOfImage(int left)
        {
            return (left < 0);
        }

        private bool DoRightStickOutOfImage(int right)
        {
            return (right > _ShowingImage.Width);
        }

        private bool IsCutLineInsideImageHorizontal(bool doStickOutLeft, bool doStickOutRight)
        {
            return (!doStickOutLeft && !doStickOutRight);
        }

        private void AdjustLeft(bool doStickOutLeft, bool doStickOutRight)
        {
            if (doStickOutLeft)
            {
                Left = 0;
            }
            else if (doStickOutRight)
            {
                Left = _ShowingImage.Width - Width;
            }
        }

        private void MoveY(int yDirection)
        {
            int newTop = Top + yDirection;
            int newBottom = Top + Height + yDirection;
            bool doStickOutTop = DoTopStickOutOfImage(newTop);
            bool doStickOutBottom = DoBottomStickOutOfImage(newBottom);

            if (IsCutLineInsideImageVertical(doStickOutTop, doStickOutBottom))
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

        private bool IsCutLineInsideImageVertical(bool doStickOutTop, bool doStickOutBottom)
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

        public bool IsPointNearRightBottom(Point p)
        {
            return (IsPointNearRightBottomX((int)p.X, NearRange)) && (IsPointNearRightBottomY((int)p.Y, NearRange));
        }

        private bool IsPointNearRightBottomX(int x, int range)
        {
            return ((x - range) <= Right) && (Right <= (x + range));
        }

        private bool IsPointNearRightBottomY(int y, int range)
        {
            return ((y - range) <= Bottom) && (Bottom <= (y + range));
        }

        public void ChangeSizeBaseRightBottom(Point dragStart, Point dropPoint)
        {
            double distanceX = dropPoint.X - dragStart.X;
            double distanceY = dropPoint.Y - dragStart.Y;
            double changeSizeY = CalcHeightBaseWidth(distanceX);
            if (Math.Abs(changeSizeY) > Math.Abs(distanceY))
            {
                Width += (int)distanceX;
                Height += (int)changeSizeY;
            }
            else
            {
                Width += (int)CalcWidthBaseHeight(distanceY);
                Height += (int)distanceY;
            }

            // 拡大し過ぎると切り抜き線が画像をはみ出すのでその対応
            if (Right > _ShowingImage.Width)
            {
                Width = _ShowingImage.Width - Left;
                Height = (int)CalcHeightBaseWidth((double)Width);
            }

            if (Bottom > _ShowingImage.Height)
            {
                Height = _ShowingImage.Height - Top;
                Width = (int)CalcWidthBaseHeight((double)Height);
            }
        }
    }
}
