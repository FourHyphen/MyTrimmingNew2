using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLineRotate : CutLineCommand
    {
        private double MaxRight { get; }

        private double MaxBottom { get; }

        private double RotateDegree { get; set; }

        public CutLineRotate(CutLine cutLine, ShowingImage image, Key key, int keyInputNum) : base(cutLine)
        {
            MaxRight = image.Width;
            MaxBottom = image.Height;
            CalcRotateDegree(key, keyInputNum);
        }

        private void CalcRotateDegree(System.Windows.Input.Key key, int keyInputNum)
        {
            if (key == Key.OemPlus)
            {
                RotateDegree = keyInputNum;
            }
            else if (key == Key.OemMinus)
            {
                RotateDegree = -keyInputNum;
            }
            else
            {
                throw new System.Exception("key should OemPlus or OemMinus.");
            }
        }

        protected override CutLineParameter CalcNewParameterCore()
        {
            return CalcRotate();
        }

        private CutLineParameter CalcRotate()
        {
            return new CutLineParameter(Before.Left, Before.Top, Before.Width, Before.Height);
        }
    }
}