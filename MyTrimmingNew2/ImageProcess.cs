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
    }
}
