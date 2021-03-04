using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineCommandFactory
    {
        private CutLineCommandFactory() { }

        public static CutLineCommand Create(CutLine cutLine, ShowingImage image, double xDirection, double yDirection)
        {
            return new CutLineMove(cutLine, image, xDirection, yDirection);
        }

        public static CutLineCommand Create(CutLine cutLine, ShowingImage image, System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            return new CutLineChangeSize(cutLine, image, dragStart, dropPoint);
        }
    }
}
