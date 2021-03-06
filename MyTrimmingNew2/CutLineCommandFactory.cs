﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineCommandFactory
    {
        private CutLineCommandFactory() { }

        public static CutLineCommand Create(CutLine cutLine, ShowingImage image, System.Windows.Input.Key key, int keyInputNum)
        {
            return new CutLineMove(cutLine, image, key, keyInputNum);
        }

        public static CutLineCommand Create(CutLine cutLine, ShowingImage image, System.Windows.Point dragStart, System.Windows.Point dropPoint)
        {
            if (cutLine.IsPointNearRightBottom(dragStart))
            {
                return new CutLineChangeSize(cutLine, image, dragStart, dropPoint);
            }
            else if (cutLine.IsPointInside(dragStart))
            {
                return new CutLineMove(cutLine, image, dragStart, dropPoint);
            }

            return null;
        }
    }
}
