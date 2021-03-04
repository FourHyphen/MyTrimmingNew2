using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public abstract class CutLineCommand
    {
        public CutLineParameter Before { get; private set; }

        public CutLineParameter After { get; private set; }

        public CutLineCommand(CutLine cutLine)
        {
            Before = new CutLineParameter(cutLine.Left, cutLine.Top, cutLine.Width, cutLine.Height);
        }

        public CutLineParameter CalcNewParameter()
        {
            After = CalcNewParameterCore();
            return After;
        }

        protected abstract CutLineParameter CalcNewParameterCore();
    }
}
