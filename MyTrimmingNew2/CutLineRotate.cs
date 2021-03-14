using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLineRotate : CutLineCommand
    {
        private double MaxRight { get; }

        private double MaxBottom { get; }

        private double BeforeDegree { get; set; }

        private double RotateDegree { get; set; }

        public CutLineRotate(CutLine cutLine, ShowingImage image, Key key, int keyInputNum) : base(cutLine)
        {
            MaxRight = image.Width;
            MaxBottom = image.Height;
            BeforeDegree = cutLine.Degree;
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
            double newDegree = BeforeDegree + RotateDegree;
            return new CutLineParameter(Before.Left, Before.Top, Before.Width, Before.Height, newDegree);
        }
    }
}