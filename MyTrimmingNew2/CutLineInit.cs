namespace MyTrimmingNew2
{
    public class CutLineInit : CutLineCommand
    {
        private ShowingImage _ShowingImage { get; }

        public CutLineInit(CutLine cutLine, ShowingImage image) : base(cutLine, image)
        {
            _ShowingImage = image;
        }

        protected override CutLineParameter CalcNewParameterCore()
        {
            return new CutLineParameter(_ShowingImage);
        }
    }
}