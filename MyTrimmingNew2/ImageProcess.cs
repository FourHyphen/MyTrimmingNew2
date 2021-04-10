using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;

namespace MyTrimmingNew2
{
    public class ImageProcess
    {
        /// <summary>
        /// 表示用画像
        /// </summary>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapSource GetShowImage(string imagePath, int width, int height)
        {
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                return GetShowImage(bitmap, width, height);
            }
        }

        public static System.Windows.Media.Imaging.BitmapSource GetShowImage(Bitmap bitmap, int width, int height)
        {
            using (Bitmap resized = CreateResizeBitmap(bitmap, width, height))
            {
                return CreateBitmapSourceImage(resized);
            }
        }

        private static System.Drawing.Bitmap CreateResizeBitmap(System.Drawing.Bitmap bitmap, int newWidth, int newHeight)
        {
            Bitmap reductionImage = new Bitmap(newWidth, newHeight);

            // 縮小画像作成(TODO: 最も画像劣化の少ない縮小アルゴリズムの選定)
            using (Graphics g = Graphics.FromImage(reductionImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
            }

            return reductionImage;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern bool DeleteObject(IntPtr hObject);

        private static System.Windows.Media.Imaging.BitmapSource CreateBitmapSourceImage(Bitmap bitmapImage)
        {
            // 参考: http://qiita.com/KaoruHeart/items/dc130d5fc00629c1b6ea
            IntPtr handle = bitmapImage.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.
                    CreateBitmapSourceFromHBitmap(handle,
                                                  IntPtr.Zero,
                                                  System.Windows.Int32Rect.Empty,
                                                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle);
            }
        }

        public static void SaveImage(string savePath,
                                     string originalImagePath,
                                     System.Windows.Point leftTop,
                                     System.Windows.Point rightTop,
                                     System.Windows.Point rightBottom,
                                     System.Windows.Point leftBottom,
                                     double degree)
        {
            System.Drawing.Bitmap saveBitmap = CreateTrimBitmap(originalImagePath,
                                                                leftTop,
                                                                rightTop,
                                                                rightBottom,
                                                                leftBottom,
                                                                degree);
            saveBitmap.Save(savePath);
            saveBitmap.Dispose();
        }

        public static System.Drawing.Bitmap CreateTrimBitmap(string originalImagePath,
                                                             System.Windows.Point leftTop,
                                                             System.Windows.Point rightTop,
                                                             System.Windows.Point rightBottom,
                                                             System.Windows.Point leftBottom,
                                                             double degree)
        {
            if (degree == 0)
            {
                return CreateTrimBitmapCore(originalImagePath, leftTop, rightBottom);
            }
            else
            {
                // TODO: Nearest Neighbor, pixel mixing, pixel mixing -> unsharp の3種類用意する
                // unsharpマスクはデフォルト0.5のMax1.0とする
                return CreateTrimBitmapCore(originalImagePath, leftTop, rightTop, rightBottom, leftBottom, degree);
            }
        }

        private static System.Drawing.Bitmap CreateTrimBitmapCore(string originalImagePath,
                                                                  System.Windows.Point leftTop,
                                                                  System.Windows.Point rightBottom)
        {
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(originalImagePath))
            {
                return CreateTrimBitmapRotateWithoutMargin(bitmap, (int)leftTop.X, (int)leftTop.Y, (int)rightBottom.X, (int)rightBottom.Y);
            }
        }

        private static System.Drawing.Bitmap CreateTrimBitmapCore(string originalImagePath,
                                                                  System.Windows.Point leftTop,
                                                                  System.Windows.Point rightTop,
                                                                  System.Windows.Point rightBottom,
                                                                  System.Windows.Point leftBottom,
                                                                  double degree,
                                                                  double? unsharpK = null)
        {
            int minX, minY, maxX, maxY;
            System.Drawing.Bitmap trimBitmapWithMargin;

            CreateTrimBitmapRotateWithMargin(originalImagePath,
                                             leftTop,
                                             rightTop,
                                             rightBottom,
                                             leftBottom,
                                             degree,
                                             out trimBitmapWithMargin,
                                             out minX,
                                             out minY,
                                             out maxX,
                                             out maxY);

            System.Drawing.Bitmap trimBitmap = CreateTrimBitmapRotateWithoutMargin(trimBitmapWithMargin, minX, minY, maxX, maxY);
            trimBitmapWithMargin.Dispose();

            if (unsharpK == null)
            {
                return trimBitmap;
            }
            else
            {
                System.Drawing.Bitmap unsharp = ApplyUnsharpMasking(trimBitmap, (double)unsharpK);
                trimBitmap.Dispose();
                return unsharp;
            }
        }

        private static void CreateTrimBitmapRotateWithMargin(string originalImagePath,
                                                             System.Windows.Point leftTop,
                                                             System.Windows.Point rightTop,
                                                             System.Windows.Point rightBottom,
                                                             System.Windows.Point leftBottom,
                                                             double degree,
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
                        // Nearest Neighbor
                        //int rotateX = (int)Math.Round(rotate.X, MidpointRounding.AwayFromZero);
                        //int rotateY = (int)Math.Round(rotate.Y, MidpointRounding.AwayFromZero);
                        //System.Drawing.Color c = bitmap.GetPixel(rotateX, rotateY);

                        // Pixel Mixingもどき
                        System.Drawing.Color c = GetPixelColorFakePixelMixing(bitmap, rotate);

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

        public static System.Drawing.Color GetPixelColorFakePixelMixing(Bitmap bitmap, System.Windows.Point rotate)
        {
            int x = (int)Math.Round(rotate.X, MidpointRounding.AwayFromZero);
            int y = (int)Math.Round(rotate.Y, MidpointRounding.AwayFromZero);
            double directionX = rotate.X - (double)x;
            double directionY = rotate.Y - (double)y;
            System.Drawing.Color c1 = bitmap.GetPixel(x - 1, y - 1);
            System.Drawing.Color c2 = bitmap.GetPixel(x, y - 1);
            System.Drawing.Color c3 = bitmap.GetPixel(x + 1, y - 1);
            System.Drawing.Color c4 = bitmap.GetPixel(x - 1, y);
            System.Drawing.Color c5 = bitmap.GetPixel(x, y);
            System.Drawing.Color c6 = bitmap.GetPixel(x + 1, y);
            System.Drawing.Color c7 = bitmap.GetPixel(x - 1, y + 1);
            System.Drawing.Color c8 = bitmap.GetPixel(x, y + 1);
            System.Drawing.Color c9 = bitmap.GetPixel(x + 1, y + 1);

            if (directionX == 0.0 && directionY == 0.0)
            {
                return c5;
            }

            System.Windows.Point p = new System.Windows.Point(directionX, directionY);
            double rd = 0.0, gd = 0.0, bd = 0.0;

            if (directionX < 0.0 && directionY < 0.0)
            {
                // c1, c2, c4, c5
                double d1 = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, -1.0), p);
                double d2 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, -1.0), p);
                double d3 = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, 0.0), p);
                double d4 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
                double sum = d1 + d2 + d3 + d4;
                rd = c1.R * d1 / sum + c2.R * d2 / sum + c4.R * d3 / sum + c5.R * d4 / sum;
                gd = c1.G * d1 / sum + c2.G * d2 / sum + c4.G * d3 / sum + c5.G * d4 / sum;
                bd = c1.B * d1 / sum + c2.B * d2 / sum + c4.B * d3 / sum + c5.B * d4 / sum;
            }
            else if (directionX >= 0.0 && directionY < 0.0)
            {
                // c2, c3, c5, c6
                double d1 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, -1.0), p);
                double d2 = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, -1.0), p);
                double d3 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
                double d4 = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, 0.0), p);
                double sum = d1 + d2 + d3 + d4;
                rd = c2.R * d1 / sum + c3.R * d2 / sum + c5.R * d3 / sum + c6.R * d4 / sum;
                gd = c2.G * d1 / sum + c3.G * d2 / sum + c5.G * d3 / sum + c6.G * d4 / sum;
                bd = c2.B * d1 / sum + c3.B * d2 / sum + c5.B * d3 / sum + c6.B * d4 / sum;
            }
            else if (directionX < 0.0 && directionY >= 0.0)
            {
                // c4, c5, c7, c8
                double d1 = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, 0.0), p);
                double d2 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
                double d3 = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, 1.0), p);
                double d4 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 1.0), p);
                double sum = d1 + d2 + d3 + d4;
                rd = c4.R * d1 / sum + c5.R * d2 / sum + c7.R * d3 / sum + c8.R * d4 / sum;
                gd = c4.G * d1 / sum + c5.G * d2 / sum + c7.G * d3 / sum + c8.G * d4 / sum;
                bd = c4.B * d1 / sum + c5.B * d2 / sum + c7.B * d3 / sum + c8.B * d4 / sum;
            }
            else
            {
                // c5, c6, c8, c9
                double d1 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
                double d2 = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, 0.0), p);
                double d3 = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 1.0), p);
                double d4 = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, 1.0), p);
                double sum = d1 + d2 + d3 + d4;
                rd = c5.R * d1 / sum + c6.R * d2 / sum + c8.R * d3 / sum + c9.R * d4 / sum;
                gd = c5.G * d1 / sum + c6.G * d2 / sum + c8.G * d3 / sum + c9.G * d4 / sum;
                bd = c5.B * d1 / sum + c6.B * d2 / sum + c8.B * d3 / sum + c9.B * d4 / sum;
            }

            byte r = (rd > 255.0) ? (byte)255 : (byte)rd;
            byte g = (gd > 255.0) ? (byte)255 : (byte)gd;
            byte b = (bd > 255.0) ? (byte)255 : (byte)bd;
            return System.Drawing.Color.FromArgb(r, g, b);
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

        private static System.Drawing.Bitmap ApplyUnsharpMasking(Bitmap bitmap, double k = 1)
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
                    System.Drawing.Color c = ApplyUnsharpFilter(new List<System.Drawing.Color>() { c1, c2, c3, c4, c5, c6, c7, c8, c9 }, k);
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

        private static System.Drawing.Color ApplyUnsharpFilter(List<System.Drawing.Color> cs, double k)
        {
            byte r = ApplyUnsharpFilter(new List<byte>() { cs[0].R, cs[1].R, cs[2].R, cs[3].R, cs[4].R, cs[5].R, cs[6].R, cs[7].R, cs[8].R }, k);
            byte g = ApplyUnsharpFilter(new List<byte>() { cs[0].G, cs[1].G, cs[2].G, cs[3].G, cs[4].G, cs[5].G, cs[6].G, cs[7].G, cs[8].G }, k);
            byte b = ApplyUnsharpFilter(new List<byte>() { cs[0].B, cs[1].B, cs[2].B, cs[3].B, cs[4].B, cs[5].B, cs[6].B, cs[7].B, cs[8].B }, k);
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        private static byte ApplyUnsharpFilter(List<byte> bytes, double k)
        {
            double aroundRate = -k / 9.0;
            double centerRate = (8.0 * k + 9.0) / 9.0;
            double tmp1 = bytes[0] * aroundRate + bytes[1] * aroundRate + bytes[2] * aroundRate;
            double tmp2 = bytes[3] * aroundRate + bytes[4] * centerRate + bytes[5] * aroundRate;
            double tmp3 = bytes[6] * aroundRate + bytes[7] * aroundRate + bytes[8] * aroundRate;
            double result = tmp1 + tmp2 + tmp3;

            if (result < 0.0) return (byte)0;
            if (result > 255.0) return (byte)255;
            return (byte)result;
        }
    }
}
