namespace MyTrimmingNew2
{
    public class ImageTrimProgressManager
    {
        private readonly static double Complete = 100.0;

        public double Progress { get; private set; } = 0.0;

        private int _RotateRateLineNum = 0;

        public int RotateRateLineNum
        {
            get
            {
                return _RotateRateLineNum;
            }
            set
            {
                _RotateRateLineNum = value;
                double rate = DoUnsharpMask ? 50.0 : 100.0;
                RotateRate = rate / (double)_RotateRateLineNum;
            }
        }

        private double RotateRate { get; set; }

        private int _UnsharpMaskLineNum = 0;

        public int UnsharpMaskLineNum
        {
            get
            {
                return _UnsharpMaskLineNum;
            }
            set
            {
                _UnsharpMaskLineNum = value;
                UnsharpMaskRate = 100.0 / (double)_UnsharpMaskLineNum;
            }
        }

        private double UnsharpMaskRate { get; set; }

        private bool DoUnsharpMask { get; set; }

        public ImageTrimProgressManager(double unsharpMask)
        {
            Init(unsharpMask);
        }

        private void Init(double unsharpMask)
        {
            DoUnsharpMask = (unsharpMask == 0.0) ? false : true;
        }

        public void AddProgressRotate()
        {
            Progress += RotateRate;
        }

        public void SetCompleteRotate()
        {
            if (DoUnsharpMask)
            {
                Progress = Complete / 2.0;
            }
            else
            {
                Progress = Complete;
            }
        }

        public void AddProgressUnsharpMask()
        {
            Progress += UnsharpMaskRate;
        }

        public void SetCompleteUnsharpMask()
        {
            Progress = Complete;
        }
    }
}