using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class SaveImage
    {
        public OriginalImage _OriginalImage { get; }

        public ShowingImage _ShowingImage { get; }

        public CutLine _CutLine { get; }

        public SaveImage(OriginalImage originalImage, ShowingImage showingImage, CutLine cutLine)
        {
            _OriginalImage = originalImage;
            _ShowingImage = showingImage;
            _CutLine = cutLine;
        }

        public void Execute(string filePath)
        {
            ImageProcess.SaveImage(filePath,
                                   _OriginalImage.Path,
                                   _ShowingImage.ToOriginalScale(_CutLine.LeftTop),
                                   _ShowingImage.ToOriginalScale(_CutLine.RightTop),
                                   _ShowingImage.ToOriginalScale(_CutLine.RightBottom),
                                   _ShowingImage.ToOriginalScale(_CutLine.LeftBottom),
                                   _CutLine.Degree);
        }
    }
}
