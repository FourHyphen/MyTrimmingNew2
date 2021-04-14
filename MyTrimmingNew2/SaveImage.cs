using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class SaveImage
    {
        private OriginalImage _OriginalImage { get; }

        private ShowingImage _ShowingImage { get; }

        private CutLine _CutLine { get; }

        private ImageTrim _ImageTrim { get; set; }

        public double Progress
        {
            get
            {
                return ((_ImageTrim == null) ? 0.0 : _ImageTrim.Progress);
            }
        }

        public SaveImage(OriginalImage originalImage, ShowingImage showingImage, CutLine cutLine)
        {
            _OriginalImage = originalImage;
            _ShowingImage = showingImage;
            _CutLine = cutLine;
        }

        public void Execute(string filePath, ImageProcess.Interpolate interpolate, double unsharpMask)
        {
            _ImageTrim = new ImageTrim(_OriginalImage.Path,
                                       _ShowingImage.ToOriginalScale(_CutLine.LeftTop),
                                       _ShowingImage.ToOriginalScale(_CutLine.RightTop),
                                       _ShowingImage.ToOriginalScale(_CutLine.RightBottom),
                                       _ShowingImage.ToOriginalScale(_CutLine.LeftBottom),
                                       _CutLine.Degree);
            System.Drawing.Bitmap saveBitmap = _ImageTrim.Create(interpolate, unsharpMask);
            saveBitmap.Save(filePath);
            saveBitmap.Dispose();
        }
    }
}
