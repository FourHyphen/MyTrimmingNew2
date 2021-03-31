using System;
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
            Bitmap resized = CreateBitmap(imagePath, width, height);
            return CreateBitmapSourceImage(resized);
        }

        private static Bitmap CreateBitmap(string imagePath, int newWidth, int newHeight)
        {
            Bitmap reductionImage = new Bitmap(newWidth, newHeight);

            // 縮小画像作成(TODO: 最も画像劣化の少ない縮小アルゴリズムの選定)
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                using (Graphics g = Graphics.FromImage(reductionImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
                }
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
            if (degree == 0)
            {

            }
            else
            {
                SaveImageRotate(savePath, originalImagePath, leftTop, rightTop, rightBottom, leftBottom, degree);
            }
        }

        private static void SaveImageRotate(string savePath,
                                            string originalImagePath,
                                            System.Windows.Point leftTop,
                                            System.Windows.Point rightTop,
                                            System.Windows.Point rightBottom,
                                            System.Windows.Point leftBottom,
                                            double degree)
        {
            int minX, minY, maxX, maxY;
            System.Drawing.Bitmap trimBitmapWithMargin;

            CreateTrimImageWithMargin(originalImagePath,
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

            System.Drawing.Bitmap saveBitmap = CreateTrimBitmapWithoutMargin(trimBitmapWithMargin, minX, minY, maxX, maxY);
            saveBitmap.Save(savePath);
        }

        private static void CreateTrimImageWithMargin(string originalImagePath,
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
                        System.Drawing.Color c = bitmap.GetPixel((int)rotate.X, (int)rotate.Y);
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
        }

        private static System.Drawing.Bitmap CreateTrimBitmapWithoutMargin(Bitmap bitmap,
                                                                           int minX,
                                                                           int minY,
                                                                           int maxX,
                                                                           int maxY)
        {
            int width = maxX - minX;
            int height = maxY - minY;

            Bitmap trimBitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(trimBitmap);
            Rectangle trim = new Rectangle(minX, minY, width, height);
            Rectangle draw = new Rectangle(0, 0, width, height);
            g.DrawImage(bitmap, draw, trim, GraphicsUnit.Pixel);
            g.Dispose();

            return trimBitmap;
        }
    }
}
