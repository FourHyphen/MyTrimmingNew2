using System;
using System.Threading.Tasks;
using System.Windows;

namespace MyTrimmingNew2
{
    public static class ImageSaving
    {
        public static void Execute(OriginalImage _OriginalImage,
                                   ShowingImage _ShowingImage,
                                   CutLine _CutLine,
                                   string filePath,
                                   ImageProcess.Interpolate interpolate,
                                   double unsharpMask)
        {
            ImageTrim it = new ImageTrim(_OriginalImage.Path,
                                         _ShowingImage.ToOriginalScale(_CutLine.LeftTop),
                                         _ShowingImage.ToOriginalScale(_CutLine.RightTop),
                                         _ShowingImage.ToOriginalScale(_CutLine.RightBottom),
                                         _ShowingImage.ToOriginalScale(_CutLine.LeftBottom),
                                         _CutLine.Degree);
            System.Drawing.Bitmap saveBitmap = it.Create(interpolate, unsharpMask);
            saveBitmap.Save(filePath);
            saveBitmap.Dispose();
        }
    }
}
