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
            using (Bitmap resized = CreateResizeBitmap(imagePath, width, height))
            {
                return CreateBitmapSourceImage(resized);
            }
        }

        private static Bitmap CreateResizeBitmap(string imagePath, int newWidth, int newHeight)
        {
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                return CreateResizeBitmap(bitmap, newWidth, newHeight);
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

        private static System.Drawing.Bitmap CreateTrimBitmap(string originalImagePath,
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
                                                                  double degree)
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
            return trimBitmap;
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

        public static System.Windows.Media.Imaging.BitmapSource CreateTrimImage(string originalImagePath,
                                                                                System.Windows.Point leftTop,
                                                                                System.Windows.Point rightTop,
                                                                                System.Windows.Point rightBottom,
                                                                                System.Windows.Point leftBottom,
                                                                                double degree,
                                                                                int fitWidth,
                                                                                int fitHeight,
                                                                                out int willSaveWidth,
                                                                                out int willSaveHeight)
        {
            System.Drawing.Bitmap trimBitmap = CreateTrimBitmap(originalImagePath,
                                                                leftTop,
                                                                rightTop,
                                                                rightBottom,
                                                                leftBottom,
                                                                degree);
            willSaveWidth = trimBitmap.Width;
            willSaveHeight = trimBitmap.Height;

            using (System.Drawing.Bitmap resize = CreateResizeBitmap(trimBitmap, fitWidth, fitHeight))
            {
                trimBitmap.Dispose();
                return CreateBitmapSourceImage(resize);
            }
        }
    }
}
