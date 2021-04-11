using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public System.Drawing.Bitmap CreateTrimBitmap(ImageProcess.Interpolate interpolate,
                                                      double unsharpMask)
        {
            if (Degree == 0)
            {
                return CreateTrimBitmapCore(OriginalImagePath, LeftTop, RightBottom);
            }
            else
            {
                return CreateTrimBitmapCore(OriginalImagePath, LeftTop, RightTop, RightBottom, LeftBottom, Degree, interpolate, unsharpMask);
            }
        }

        private System.Drawing.Bitmap CreateTrimBitmapCore(string originalImagePath,
                                                           System.Windows.Point leftTop,
                                                           System.Windows.Point rightBottom)
        {
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(originalImagePath))
            {
                return CreateTrimBitmapRotateWithoutMargin(bitmap, (int)leftTop.X, (int)leftTop.Y, (int)rightBottom.X, (int)rightBottom.Y);
            }
        }

        private System.Drawing.Bitmap CreateTrimBitmapCore(string originalImagePath,
                                                           System.Windows.Point leftTop,
                                                           System.Windows.Point rightTop,
                                                           System.Windows.Point rightBottom,
                                                           System.Windows.Point leftBottom,
                                                           double degree,
                                                           ImageProcess.Interpolate interpolate,
                                                           double unsharpMask)
        {
            int minX, minY, maxX, maxY;
            System.Drawing.Bitmap trimBitmapWithMargin;

            CreateTrimBitmapRotateWithMargin(originalImagePath,
                                             leftTop,
                                             rightTop,
                                             rightBottom,
                                             leftBottom,
                                             degree,
                                             interpolate,
                                             out trimBitmapWithMargin,
                                             out minX,
                                             out minY,
                                             out maxX,
                                             out maxY);

            System.Drawing.Bitmap trimBitmap = CreateTrimBitmapRotateWithoutMargin(trimBitmapWithMargin, minX, minY, maxX, maxY);
            trimBitmapWithMargin.Dispose();

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

        private void CreateTrimBitmapRotateWithMargin(string originalImagePath,
                                                      System.Windows.Point leftTop,
                                                      System.Windows.Point rightTop,
                                                      System.Windows.Point rightBottom,
                                                      System.Windows.Point leftBottom,
                                                      double degree,
                                                      ImageProcess.Interpolate interpolate,
                                                      out System.Drawing.Bitmap trimBitmapWithMargin,
                                                      out int minX,
                                                      out int minY,
                                                      out int maxX,
                                                      out int maxY)
        {
            // 切り抜き画像作成(余白あり)
            // trimBitmapWithMargin = 切り抜き線に沿って切り抜いた領域を、傾いてない画像として白いキャンバスに貼り付けた画像
            // minおよびmax変数 = 白いキャンバスに存在する切り抜き後画像領域の場所

            // 回転パラメーター準備
            double centerX = Common.CalcCenterX(leftTop, rightBottom);
            double centerY = Common.CalcCenterY(leftBottom, rightTop);
            double radian = (double)degree * Math.PI / 180;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            Bitmap bitmap = new Bitmap(originalImagePath);
            RectLine rectLine = new RectLine(leftTop, rightTop, rightBottom, leftBottom);
            trimBitmapWithMargin = new Bitmap(bitmap.Width, bitmap.Height);
            minX = bitmap.Width;
            minY = bitmap.Height;
            maxX = 0;
            maxY = 0;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    System.Windows.Point rotate = Common.CalcRotatePoint(new System.Windows.Point(x, y), centerX, centerY, cos, sin);
                    if (rectLine.IsInside(rotate))
                    {
                        System.Drawing.Color c;
                        if (interpolate == ImageProcess.Interpolate.PixelMixing)
                        {
                            c = ImageProcess.GetPixelColorFakePixelMixing(bitmap, rotate);
                        }
                        else
                        {
                            // Nearest Neighbor
                            int rotateX = (int)Math.Round(rotate.X, MidpointRounding.AwayFromZero);
                            int rotateY = (int)Math.Round(rotate.Y, MidpointRounding.AwayFromZero);
                            c = bitmap.GetPixel(rotateX, rotateY);
                        }

                        trimBitmapWithMargin.SetPixel(x, y, System.Drawing.Color.FromArgb(c.R, c.G, c.B));
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
            }

            bitmap.Dispose();
        }

        private static System.Drawing.Bitmap CreateTrimBitmapRotateWithoutMargin(Bitmap bitmap,
                                                                         int minX,
                                                                         int minY,
                                                                         int maxX,
                                                                         int maxY)
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

        private static System.Drawing.Bitmap ApplyUnsharpMasking(Bitmap bitmap, double k)
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
