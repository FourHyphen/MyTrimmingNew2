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
            ProgressManager = new ImageTrimProgressManager(unsharpMask);

            System.Drawing.Bitmap trim;
            if (Degree == 0)
            {
                trim = CreateCore(original);
            }
            else
            {
                trim = CreateCore(original, interpolate, unsharpMask);
            }

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
            double radian = Common.ToRadian((double)Degree);
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            // 探索範囲の制限
            LimitSearchArea(original, centerX, centerY, out int xMin, out int yMin, out int xMax, out int yMax);
            ProgressManager.RotateRateLineNum = yMax - yMin + 1;

            // Bitmap.GetPixel() と SetPixel() は遅いので byte 配列を使用する
            ImageProcess.Copy(original, out byte[] buf);

            int originalWidth = original.Width;
            int originalHeight = original.Height;
            int xRoundMax = originalWidth - 1;
            int yRoundMax = originalHeight - 1;

            int widthIndex = originalWidth * 4;
            byte[] resultBuf = new byte[widthIndex * originalHeight];
            RectLine rectLine = new RectLine(LeftTop, RightTop, RightBottom, LeftBottom);

            int[] rangeX = new int[originalWidth];
            int[] rangeY = new int[originalHeight];
            System.Threading.Tasks.Parallel.For(yMin, yMax, y =>
            {
                byte[] rBuf = new byte[9];
                byte[] gBuf = new byte[9];
                byte[] bBuf = new byte[9];
                for (int x = xMin; x < xMax; x++)
                {
                    (double X, double Y) rotate = Common.CalcRotatePoint(x, y, centerX, centerY, cos, sin);
                    if (rectLine.IsInside(rotate))
                    {
                        System.Drawing.Color c;
                        if (interpolate == ImageProcess.Interpolate.PixelMixing)
                        {
                            // 予備実験の際、bitmap.Width = 3840 で rotate.X = 3839.59 の場合があった(再現不可)
                            int xRound = Min(Round(rotate.X), xRoundMax);
                            int yRound = Min(Round(rotate.Y), yRoundMax);
                            CreateFilter(buf, xRound, yRound, widthIndex, ref rBuf, ref gBuf, ref bBuf);
                            c = ImageProcess.GetPixelColorFakePixelMixing(rBuf, gBuf, bBuf, xRound, yRound, originalWidth, originalHeight, rotate);
                        }
                        else
                        {
                            // Nearest Neighbor
                            int rotateX = Round(rotate.X);
                            int rotateY = Round(rotate.Y);
                            CreateFilter(buf, rotateX, rotateY, widthIndex, ref rBuf, ref gBuf, ref bBuf);
                            c = Color.FromArgb(rBuf[4], gBuf[4], bBuf[4]);
                        }

                        int i = y * widthIndex + x * 4;
                        resultBuf[i] = c.R;
                        resultBuf[i + 1] = c.G;
                        resultBuf[i + 2] = c.B;
                        resultBuf[i + 3] = 255;    // A が 0 だと画像が真っ黒になる

                        rangeX[x]++;
                        rangeY[y]++;
                    }
                }

                ProgressManager.AddProgressRotate();
            });

            minX = Array.FindIndex(rangeX, value => value > 0);
            minY = Array.FindIndex(rangeY, value => value > 0);
            maxX = Array.FindLastIndex(rangeX, value => value > 0);
            maxY = Array.FindLastIndex(rangeY, value => value > 0);

            ImageProcess.Copy(resultBuf, original.Width, original.Height, out rotateBitmapWithMargin);
            ProgressManager.SetCompleteRotate();
        }

        private void LimitSearchArea(Bitmap original, double centerX, double centerY, out int xMin, out int yMin, out int xMax, out int yMax)
        {
            double rectWidth = Common.CalcDistance(LeftTop, RightTop);
            double rectHeight = Common.CalcDistance(LeftTop, LeftBottom);
            xMin = Math.Max(0, (int)(centerX - (rectWidth / 2.0)) - 1);
            yMin = Math.Max(0, (int)(centerY - (rectHeight / 2.0)) - 1);
            xMax = Math.Min(original.Width, (int)(centerX + (rectWidth / 2.0)) + 1);
            yMax = Math.Min(original.Height, (int)(centerY + (rectHeight / 2.0)) + 1);
        }

        private int Round(double d)
        {
            double tmp = d + 0.5;
            return (int)tmp;
        }

        private int Min(int a, int b)
        {
            return a < b ? a : b;
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
            ProgressManager.UnsharpMaskLineNum = bitmap.Height;
            System.Drawing.Bitmap result = ApplyUnsharpMaskingUnmanaged(bitmap, k);
            ProgressManager.SetCompleteUnsharpMask();

            return result;
        }

        private System.Drawing.Bitmap ApplyUnsharpMaskingUnmanaged(Bitmap bitmap, double k)
        {
            System.Drawing.Bitmap unsharp = new Bitmap(bitmap);
            ImageProcess.Copy(unsharp, out byte[] buf);

            byte[] resultBuf = new byte[unsharp.Width * unsharp.Height * 4];
            buf.CopyTo(resultBuf, 0);

            int width = unsharp.Width * 4;

            // 3x3カーネルを適用するため端を無視
            int maxWidth = unsharp.Width - 1;
            int maxHeight = unsharp.Height - 1;
            System.Threading.Tasks.Parallel.For(1, maxHeight, y =>
            {
                byte[] rBuf = new byte[9];
                byte[] gBuf = new byte[9];
                byte[] bBuf = new byte[9];
                for (int x = 1; x < maxWidth; x++)
                {
                    CreateFilter(buf, x, y, width, ref rBuf, ref gBuf, ref bBuf);
                    Color c = ImageProcess.ApplyUnsharpFilter(rBuf, gBuf, bBuf, k);
                    int i = y * width + x * 4;
                    resultBuf[i] = c.R;
                    resultBuf[i + 1] = c.G;
                    resultBuf[i + 2] = c.B;
                }

                ProgressManager.AddProgressUnsharpMask();
            });

            ImageProcess.Copy(resultBuf, unsharp);

            return unsharp;
        }

        private void CreateFilter(byte[] buf, int x, int y, int width, ref byte[] rBuf, ref byte[] gBuf, ref byte[] bBuf)
        {
            int xMinus = (x - 1) * 4;
            int xCenter = x * 4;
            int xPlus = (x + 1) * 4;
            int yMinus = (y - 1) * width;
            int yCenter = y * width;
            int yPlus = (y + 1) * width;
            int i1 = yMinus + xMinus;
            int i2 = yMinus + xCenter;
            int i3 = yMinus + xPlus;
            int i4 = yCenter + xMinus;
            int i5 = yCenter + xCenter;
            int i6 = yCenter + xPlus;
            int i7 = yPlus + xMinus;
            int i8 = yPlus + xCenter;
            int i9 = yPlus + xPlus;
            rBuf[0] = buf[i1]; gBuf[0] = buf[i1 + 1]; bBuf[0] = buf[i1 + 2];
            rBuf[1] = buf[i2]; gBuf[1] = buf[i2 + 1]; bBuf[1] = buf[i2 + 2];
            rBuf[2] = buf[i3]; gBuf[2] = buf[i3 + 1]; bBuf[2] = buf[i3 + 2];
            rBuf[3] = buf[i4]; gBuf[3] = buf[i4 + 1]; bBuf[3] = buf[i4 + 2];
            rBuf[4] = buf[i5]; gBuf[4] = buf[i5 + 1]; bBuf[4] = buf[i5 + 2];
            rBuf[5] = buf[i6]; gBuf[5] = buf[i6 + 1]; bBuf[5] = buf[i6 + 2];
            rBuf[6] = buf[i7]; gBuf[6] = buf[i7 + 1]; bBuf[6] = buf[i7 + 2];
            rBuf[7] = buf[i8]; gBuf[7] = buf[i8 + 1]; bBuf[7] = buf[i8 + 2];
            rBuf[8] = buf[i9]; gBuf[8] = buf[i9 + 1]; bBuf[8] = buf[i9 + 2];
        }
    }
}
