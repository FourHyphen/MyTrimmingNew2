using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyTrimmingNew2
{
    public class ImageTrim
    {
        private string OriginalImagePath { get; }

        private System.Windows.Point LeftTop { get; }

        private System.Windows.Point RightTop { get; }

        private System.Windows.Point RightBottom { get; }

        private System.Windows.Point LeftBottom { get; }

        private double Degree { get; }

        public double Progress
        {
            get
            {
                return ((ProgressManager == null) ? 0.0 : ProgressManager.Progress);
            }
        }

        private ImageTrimProgressManager ProgressManager { get; set; }

        public ImageTrim(string originalImagePath,
                         System.Windows.Point leftTop,
                         System.Windows.Point rightTop,
                         System.Windows.Point rightBottom,
                         System.Windows.Point leftBottom,
                         double degree)
        {
            OriginalImagePath = originalImagePath;
            LeftTop = leftTop;
            RightTop = rightTop;
            RightBottom = rightBottom;
            LeftBottom = leftBottom;
            Degree = degree;
        }

        public System.Drawing.Bitmap Create(ImageProcess.Interpolate interpolate, double unsharpMask)
        {
            System.Drawing.Bitmap original = new Bitmap(OriginalImagePath);
            ProgressManager = new ImageTrimProgressManager(original, unsharpMask);

            System.Drawing.Bitmap trim;
            if (Degree == 0)
            {
                trim = CreateCore(original);
            }
            else
            {
                trim = CreateCore(original, interpolate, unsharpMask);
            }

            ProgressManager.SetComplete();
            original.Dispose();
            return trim;
        }

        private System.Drawing.Bitmap CreateCore(System.Drawing.Bitmap original)
        {
            return TrimMargin(original, (int)LeftTop.X, (int)LeftTop.Y, (int)RightBottom.X, (int)RightBottom.Y);
        }

        private System.Drawing.Bitmap CreateCore(System.Drawing.Bitmap original, ImageProcess.Interpolate interpolate, double unsharpMask)
        {
            int minX, minY, maxX, maxY;
            System.Drawing.Bitmap rotateBitmapWithMargin;
            RotateWithMargin(original, interpolate, out rotateBitmapWithMargin, out minX, out minY, out maxX, out maxY);

            System.Drawing.Bitmap trimBitmap = TrimMargin(rotateBitmapWithMargin, minX, minY, maxX, maxY);
            rotateBitmapWithMargin.Dispose();

            if (unsharpMask == 0.0)
            {
                return trimBitmap;
            }
            else
            {
                System.Drawing.Bitmap unsharp = ApplyUnsharpMasking(trimBitmap, unsharpMask);
                trimBitmap.Dispose();
                return unsharp;
            }
        }

        private void RotateWithMargin(Bitmap original,
                                      ImageProcess.Interpolate interpolate,
                                      out System.Drawing.Bitmap rotateBitmapWithMargin,
                                      out int minX,
                                      out int minY,
                                      out int maxX,
                                      out int maxY)
        {
            // 切り抜き画像作成(余白あり)
            // rotateBitmapWithMargin = 切り抜き線に沿って切り抜いた領域を、傾いてない画像として白いキャンバスに貼り付けた画像
            // minおよびmax変数 = 白いキャンバスに存在する切り抜き後画像領域の場所

            // 回転パラメーター準備
            double centerX = Common.CalcCenterX(LeftTop, RightBottom);
            double centerY = Common.CalcCenterY(LeftBottom, RightTop);
            double radian = (double)Degree * Math.PI / 180;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            RectLine rectLine = new RectLine(LeftTop, RightTop, RightBottom, LeftBottom);
            rotateBitmapWithMargin = new Bitmap(original.Width, original.Height);
            minX = original.Width;
            minY = original.Height;
            maxX = 0;
            maxY = 0;

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    System.Windows.Point rotate = Common.CalcRotatePoint(new System.Windows.Point(x, y), centerX, centerY, cos, sin);
                    if (rectLine.IsInside(rotate))
                    {
                        System.Drawing.Color c;
                        if (interpolate == ImageProcess.Interpolate.PixelMixing)
                        {
                            c = ImageProcess.GetPixelColorFakePixelMixing(original, rotate);
                        }
                        else
                        {
                            // Nearest Neighbor
                            int rotateX = (int)Math.Round(rotate.X, MidpointRounding.AwayFromZero);
                            int rotateY = (int)Math.Round(rotate.Y, MidpointRounding.AwayFromZero);
                            c = original.GetPixel(rotateX, rotateY);
                        }

                        rotateBitmapWithMargin.SetPixel(x, y, c);
                        if (x < minX)
                        {
                            minX = x;
                        }
                        if (x > maxX)
                        {
                            maxX = x;
                        }
                        if (y < minY)
                        {
                            minY = y;
                        }
                        if (y > maxY)
                        {
                            maxY = y;
                        }
                    }
                }

                ProgressManager.AddProgressPerHeight();
            }
        }

        private System.Drawing.Bitmap TrimMargin(Bitmap bitmap, int minX, int minY, int maxX, int maxY)
        {
            int width = maxX - minX;
            int height = maxY - minY;

            Bitmap trimBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(trimBitmap))
            {
                Rectangle trim = new Rectangle(minX, minY, width, height);
                Rectangle draw = new Rectangle(0, 0, width, height);
                g.DrawImage(bitmap, draw, trim, GraphicsUnit.Pixel);
            }

            return trimBitmap;
        }

        private System.Drawing.Bitmap ApplyUnsharpMasking(Bitmap bitmap, double k)
        {
            System.Drawing.Bitmap unsharp = new Bitmap(bitmap.Width, bitmap.Height);

            // 3x3カーネルを適用するため端を無視
            for (int y = 1; y < bitmap.Height - 1; y++)
            {
                for (int x = 1; x < bitmap.Width - 1; x++)
                {
                    // 参考: https://imagingsolution.blog.fc2.com/blog-entry-114.html
                    System.Drawing.Color c1 = bitmap.GetPixel(x - 1, y - 1);
                    System.Drawing.Color c2 = bitmap.GetPixel(x, y - 1);
                    System.Drawing.Color c3 = bitmap.GetPixel(x + 1, y - 1);
                    System.Drawing.Color c4 = bitmap.GetPixel(x - 1, y);
                    System.Drawing.Color c5 = bitmap.GetPixel(x, y);
                    System.Drawing.Color c6 = bitmap.GetPixel(x + 1, y);
                    System.Drawing.Color c7 = bitmap.GetPixel(x - 1, y + 1);
                    System.Drawing.Color c8 = bitmap.GetPixel(x, y + 1);
                    System.Drawing.Color c9 = bitmap.GetPixel(x + 1, y + 1);
                    System.Drawing.Color c = ImageProcess.ApplyUnsharpFilter(new List<System.Drawing.Color>() { c1, c2, c3, c4, c5, c6, c7, c8, c9 }, k);
                    unsharp.SetPixel(x, y, c);
                }

                ProgressManager.AddProgressPerHeight();
            }

            // 3x3カーネル適用時に無視した端を埋める
            for (int x = 0; x < bitmap.Width; x++)
            {
                unsharp.SetPixel(x, 0, bitmap.GetPixel(x, 0));
                unsharp.SetPixel(x, bitmap.Height - 1, bitmap.GetPixel(x, bitmap.Height - 1));
            }
            for (int y = 0; y < bitmap.Height; y++)
            {
                unsharp.SetPixel(0, y, bitmap.GetPixel(0, y));
                unsharp.SetPixel(bitmap.Width - 1, y, bitmap.GetPixel(bitmap.Width - 1, y));
            }

            return unsharp;
        }
    }
}
