using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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

        public static void Copy(Bitmap src, out byte[] dst)
        {
            BitmapData data = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                                           ImageLockMode.ReadWrite,
                                           PixelFormat.Format32bppArgb);
            dst = new byte[src.Width * 4 * src.Height];
            Marshal.Copy(data.Scan0, dst, 0, dst.Length);
            src.UnlockBits(data);
        }

        public static void Copy(byte[] src, int bitmapWidth, int bitmapHeight, out Bitmap dst)
        {
            dst = new Bitmap(bitmapWidth, bitmapHeight);
            Copy(src, dst);
        }

        public static void Copy(byte[] src, Bitmap dst)
        {
            BitmapData dataResult = dst.LockBits(new Rectangle(0, 0, dst.Width, dst.Height),
                                                 ImageLockMode.ReadWrite,
                                                 PixelFormat.Format32bppArgb);
            Marshal.Copy(src, 0, dataResult.Scan0, src.Length);
            dst.UnlockBits(dataResult);
        }

        public static System.Drawing.Color GetPixelColorFakePixelMixing(byte[] rBuf, byte[] gBuf, byte[] bBuf, int x, int y, int width, int height, System.Windows.Point rotate)
        {
            double directionX = rotate.X - (double)x;
            double directionY = rotate.Y - (double)y;

            if (!DoFitPointConsideringFilterSize(width, height, x, y, 1))
            {
                return Color.FromArgb(rBuf[4], gBuf[4], bBuf[4]);
            }

            System.Windows.Point p = new System.Windows.Point(directionX, directionY);
            int ia, ib, ic, id;
            System.Windows.Point pa, pb, pc, pd;

            if (directionX < 0.0 && directionY < 0.0)
            {
                // c1, c2, c4, c5
                ia = 0;
                ib = 1;
                ic = 3;
                id = 4;
                pa = new System.Windows.Point(-1.0, -1.0);
                pb = new System.Windows.Point(0.0, -1.0);
                pc = new System.Windows.Point(-1.0, 0.0);
                pd = new System.Windows.Point(0.0, 0.0);
            }
            else if (directionX >= 0.0 && directionY < 0.0)
            {
                // c2, c3, c5, c6
                ia = 1;
                ib = 2;
                ic = 4;
                id = 5;
                pa = new System.Windows.Point(0.0, -1.0);
                pb = new System.Windows.Point(1.0, -1.0);
                pc = new System.Windows.Point(0.0, 0.0);
                pd = new System.Windows.Point(1.0, 0.0);
            }
            else if (directionX < 0.0 && directionY >= 0.0)
            {
                // c4, c5, c7, c8
                ia = 3;
                ib = 4;
                ic = 6;
                id = 7;
                pa = new System.Windows.Point(-1.0, 0.0);
                pb = new System.Windows.Point(0.0, 0.0);
                pc = new System.Windows.Point(-1.0, 1.0);
                pd = new System.Windows.Point(0.0, 1.0);
            }
            else
            {
                // c5, c6, c8, c9
                ia = 4;
                ib = 5;
                ic = 7;
                id = 8;
                pa = new System.Windows.Point(0.0, 0.0);
                pb = new System.Windows.Point(1.0, 0.0);
                pc = new System.Windows.Point(0.0, 1.0);
                pd = new System.Windows.Point(1.0, 1.0);
            }

            double da = 1.0 / Common.CalcDistance(pa, p);
            double db = 1.0 / Common.CalcDistance(pb, p);
            double dc = 1.0 / Common.CalcDistance(pc, p);
            double dd = 1.0 / Common.CalcDistance(pd, p);
            double sum = da + db + dc + dd;
            double rd = rBuf[ia] * da / sum + rBuf[ib] * db / sum + rBuf[ic] * dc / sum + rBuf[id] * dd / sum;    // sumでの除算をまとめるとテストに失敗する
            double gd = gBuf[ia] * da / sum + gBuf[ib] * db / sum + gBuf[ic] * dc / sum + gBuf[id] * dd / sum;
            double bd = bBuf[ia] * da / sum + bBuf[ib] * db / sum + bBuf[ic] * dc / sum + bBuf[id] * dd / sum;

            byte r = (rd > 255.0) ? (byte)255 : (byte)rd;
            byte g = (gd > 255.0) ? (byte)255 : (byte)gd;
            byte b = (bd > 255.0) ? (byte)255 : (byte)bd;
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        private static bool DoFitPointConsideringFilterSize(int width, int height, int x, int y, int filter)
        {
            int xMax = width - filter;
            int yMax = height - filter;
            return (filter < x && x < xMax) && (filter < y && y < yMax);
        }

        public static System.Drawing.Color ApplyUnsharpFilter(byte[] rBuf, byte[] gBuf, byte[] bBuf, double k)
        {
            // 参考: https://imagingsolution.blog.fc2.com/blog-entry-114.html
            double aroundRate = -k / 9.0;
            double centerRate = (8.0 * k + 9.0) / 9.0;
            byte r = ApplyUnsharpFilter(rBuf[0], rBuf[1], rBuf[2], rBuf[3], rBuf[4], rBuf[5], rBuf[6], rBuf[7], rBuf[8], aroundRate, centerRate);
            byte g = ApplyUnsharpFilter(gBuf[0], gBuf[1], gBuf[2], gBuf[3], gBuf[4], gBuf[5], gBuf[6], gBuf[7], gBuf[8], aroundRate, centerRate);
            byte b = ApplyUnsharpFilter(bBuf[0], bBuf[1], bBuf[2], bBuf[3], bBuf[4], bBuf[5], bBuf[6], bBuf[7], bBuf[8], aroundRate, centerRate);
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        private static byte ApplyUnsharpFilter(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, double aroundRate, double centerRate)
        {
            double tmp1 = b1 * aroundRate + b2 * aroundRate + b3 * aroundRate;
            double tmp2 = b4 * aroundRate + b5 * centerRate + b6 * aroundRate;
            double tmp3 = b7 * aroundRate + b8 * aroundRate + b9 * aroundRate;
            double result = tmp1 + tmp2 + tmp3;

            if (result < 0.0) return (byte)0;
            if (result > 255.0) return (byte)255;
            return (byte)result;
        }
    }
}
