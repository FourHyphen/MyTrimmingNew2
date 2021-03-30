using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyTrimmingNew2
{
    public class ShowingImage
    {
        private int ImageAreaWidth { get; }
        private int ImageAreaHeight { get; }

        public double Ratio { get; private set; }

        public BitmapSource Source { get; set; }

        public int Width
        {
            get
            {
                return Source.PixelWidth;
            }
        }

        public int Height
        {
            get
            {
                return Source.PixelHeight;
            }
        }

        public ShowingImage(OriginalImage image, int imageAreaWidth, int imageAreaHeight)
        {
            ImageAreaWidth = imageAreaWidth;
            ImageAreaHeight = imageAreaHeight;
            Init(image);
        }

        private void Init(OriginalImage image)
        {
            CreateShowingImageSourceFitImageArea(image);
        }

        private void CreateShowingImageSourceFitImageArea(OriginalImage image)
        {
            Ratio = CalcRatioOfFittingImageAreaKeepingImageRatio(image.Width, image.Height);
            double fitWidth = (double)image.Width * Ratio;
            double fitHeight = (double)image.Height * Ratio;
            Source = ImageProcess.GetShowImage(image.Path, (int)fitWidth, (int)fitHeight);
        }

        private double CalcRatioOfFittingImageAreaKeepingImageRatio(int imageWidth, int imageHeight)
        {
            double ratioBaseWidth = (double)ImageAreaWidth / imageWidth;
            double ratioBaseHeight = (double)ImageAreaHeight / imageHeight;

            // 倍率が大きいと ImageArea から縦(横)だけがはみ出る
            if (ratioBaseWidth < ratioBaseHeight)
            {
                return ratioBaseWidth;
            }
            return ratioBaseHeight;
        }

        public System.Windows.Point ToOriginalScale(System.Windows.Point p)
        {
            return new System.Windows.Point(p.X / Ratio, p.Y / Ratio);
        }
    }
}
