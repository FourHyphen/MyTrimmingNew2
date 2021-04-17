using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;

namespace MyTrimmingNew2
{
    public class ImageProcess
    {
        public enum Interpolate
        {
            NearestNeighbor,
            PixelMixing
        }

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

        public static System.Drawing.Color GetPixelColorFakePixelMixing(Bitmap bitmap, System.Windows.Point rotate)
        {
            int x = (int)Math.Round(rotate.X, MidpointRounding.AwayFromZero);
            int y = (int)Math.Round(rotate.Y, MidpointRounding.AwayFromZero);
            double directionX = rotate.X - (double)x;
            double directionY = rotate.Y - (double)y;
            System.Drawing.Color c5 = bitmap.GetPixel(x, y);

            if (directionX == 0.0 && directionY == 0.0)
            {
                return c5;
            }

            System.Windows.Point p = new System.Windows.Point(directionX, directionY);
            System.Drawing.Color ca, cb, cc, cd;
            double da, db, dc, dd;

            if (directionX < 0.0 && directionY < 0.0)
            {
                // c1, c2, c4, c5
                ca = bitmap.GetPixel(x - 1, y - 1);
                cb = bitmap.GetPixel(x, y - 1);
                cc = bitmap.GetPixel(x - 1, y);
                cd = c5;
                da = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, -1.0), p);
                db = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, -1.0), p);
                dc = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, 0.0), p);
                dd = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
            }
            else if (directionX >= 0.0 && directionY < 0.0)
            {
                // c2, c3, c5, c6
                ca = bitmap.GetPixel(x, y - 1);
                cb = bitmap.GetPixel(x + 1, y - 1);
                cc = c5;
                cd = bitmap.GetPixel(x + 1, y);
                da = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, -1.0), p);
                db = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, -1.0), p);
                dc = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
                dd = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, 0.0), p);
            }
            else if (directionX < 0.0 && directionY >= 0.0)
            {
                // c4, c5, c7, c8
                ca = bitmap.GetPixel(x - 1, y);
                cb = c5;
                cc = bitmap.GetPixel(x - 1, y + 1);
                cd = bitmap.GetPixel(x, y + 1);
                da = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, 0.0), p);
                db = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
                dc = 1.0 / Common.CalcDistance(new System.Windows.Point(-1.0, 1.0), p);
                dd = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 1.0), p);
            }
            else
            {
                // c5, c6, c8, c9
                ca = c5;
                cb = bitmap.GetPixel(x + 1, y);
                cc = bitmap.GetPixel(x, y + 1);
                cd = bitmap.GetPixel(x + 1, y + 1);
                da = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 0.0), p);
                db = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, 0.0), p);
                dc = 1.0 / Common.CalcDistance(new System.Windows.Point(0.0, 1.0), p);
                dd = 1.0 / Common.CalcDistance(new System.Windows.Point(1.0, 1.0), p);
            }

            double sum = da + db + dc + dd;
            double rd = ca.R * da / sum + cb.R * db / sum + cc.R * dc / sum + cd.R * dd / sum;
            double gd = ca.G * da / sum + cb.G * db / sum + cc.G * dc / sum + cd.G * dd / sum;
            double bd = ca.B * da / sum + cb.B * db / sum + cc.B * dc / sum + cd.B * dd / sum;

            byte r = (rd > 255.0) ? (byte)255 : (byte)rd;
            byte g = (gd > 255.0) ? (byte)255 : (byte)gd;
            byte b = (bd > 255.0) ? (byte)255 : (byte)bd;
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        public static System.Drawing.Color ApplyUnsharpFilter(List<System.Drawing.Color> cs, double k)
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
