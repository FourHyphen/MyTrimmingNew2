using System.Drawing;

namespace MyTrimmingNew2
{
    public class ImageTrimProgressManager
    {
        public double Progress { get; private set; } = 0.0;

        private double Complete { get { return 100.0; } }

        private double RatePerHeight { get; set; } = 0.0;

        public ImageTrimProgressManager(Bitmap original, double unsharpMask)
        {
            Init(original.Height, unsharpMask);
        }

        private void Init(double imageHeight, double unsharpMask)
        {
            double baseHeight = imageHeight - 2;    // 3x3マスク適用のため端を無視する
            if (unsharpMask == 0.0)
            {
                RatePerHeight = 100.0 / baseHeight;
            }
            else
            {
                RatePerHeight = 50.0 / baseHeight;
            }
        }

        public void AddProgressPerHeight()
        {
            Progress += RatePerHeight;
        }

        public void SetComplete()
        {
            Progress = Complete;
        }
    }
}