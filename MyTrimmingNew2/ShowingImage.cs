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
        private string ImagePath { get; }
        private int ImageAreaWidth { get; }
        private int ImageAreaHeight { get; }

        public int OriginalImageWidth { get; private set; }

        public int OriginalImageHeight { get; private set; }

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

        public ShowingImage(string imagePath, int imageAreaWidth, int imageAreaHeight)
        {
            ImagePath = imagePath;
            ImageAreaWidth = imageAreaWidth;
            ImageAreaHeight = imageAreaHeight;

            Init();
        }

        private void Init()
        {
            // 画像を開く際に検証処理しないことで高速に読み込む
            using (System.IO.FileStream fs = System.IO.File.OpenRead(ImagePath))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(fs, false, false))
                {
                    OriginalImageWidth = image.Width;
                    OriginalImageHeight = image.Height;
                }
            }

            CreateShowingImageSourceFitImageArea();
        }

        private void CreateShowingImageSourceFitImageArea()
        {
            double ratio = CalcRatioOfFittingImageAreaKeepingImageRatio();
            double fitWidth = (double)OriginalImageWidth * ratio;
            double fitHeight = (double)OriginalImageHeight * ratio;
            Source = ImageProcess.GetShowImage(ImagePath, (int)fitWidth, (int)fitHeight);
        }

        private double CalcRatioOfFittingImageAreaKeepingImageRatio()
        {
            double ratioBaseWidth = (double)ImageAreaWidth / OriginalImageWidth;
            double ratioBaseHeight = (double)ImageAreaHeight / OriginalImageHeight;

            // 倍率が大きいと ImageArea から縦(横)だけがはみ出る
            if (ratioBaseWidth < ratioBaseHeight)
            {
                return ratioBaseWidth;
            }
            return ratioBaseHeight;
        }

    }
}
