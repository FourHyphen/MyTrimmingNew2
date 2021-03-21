using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public abstract class CutLineChangeSize : CutLineCommand
    {
        protected System.Windows.Point DragStart { get; }

        protected System.Windows.Point DropPoint { get; }

        public CutLineChangeSize(CutLine cutLine, ShowingImage image, System.Windows.Point dragStart, System.Windows.Point dropPoint) : base(cutLine, image)
        {
            DragStart = dragStart;
            DropPoint = dropPoint;
        }

        protected override CutLineParameter CalcNewParameterCore()
        {
            double newWidth = Before.Width;
            double newHeight = Before.Height;
            CalcNewParameterWidthAndHeight(ref newWidth, ref newHeight);

            if (newWidth >= 0.0 && newHeight >= 0.0)
            {
                return CreateNewParameter(newWidth, newHeight);
            }
            else
            {
                // 縮小方向に行き過ぎると、移動点が原点を超えて原点が変わる
                //  -> この場合の処理は複雑なので4隅毎の具象クラスに任せる
                return CreateNewParameterWhenExchangeOrigin(newWidth, newHeight);
            }
        }

        private void CalcNewParameterWidthAndHeight(ref double newWidth, ref double newHeight)
        {
            // 点によってドラッグ＆ドロップの方向と操作の拡大/縮小の方向が異なるので具象クラスに任せる
            double distanceX = GetDistanceX(DragStart, DropPoint);
            double distanceY = GetDistanceY(DragStart, DropPoint);

            double changeSizeY = Before.CalcHeightBaseWidth(distanceX);
            if (Math.Abs(changeSizeY) > Math.Abs(distanceY))
            {
                newWidth += distanceX;
                newHeight += changeSizeY;
            }
            else
            {
                newWidth += Before.CalcWidthBaseHeight(distanceY);
                newHeight += distanceY;
            }
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作が拡大を意図しているなら＋値の距離を、縮小の意図なら－値の距離を返す
        /// </summary>
        /// <param name="dragStart"></param>
        /// <param name="dropPoint"></param>
        /// <returns></returns>
        protected abstract double GetDistanceX(System.Windows.Point dragStart, System.Windows.Point dropPoint);

        /// <summary>
        /// ドラッグ＆ドロップ操作が拡大を意図しているなら＋値の距離を、縮小の意図なら－値の距離を返す
        /// </summary>
        /// <param name="dragStart"></param>
        /// <param name="dropPoint"></param>
        /// <returns></returns>
        protected abstract double GetDistanceY(System.Windows.Point dragStart, System.Windows.Point dropPoint);

        private CutLineParameter CreateNewParameter(double newWidth, double newHeight)
        {
            AdjustWidthAndHeightIfOverShowingImage(ref newWidth, ref newHeight);
            if (Before.Degree == 0)
            {
                // この場合は簡単なので処理の外枠は共通化、4隅座標の計算方法は具象クラスに任せる
                return CreateNewParameterNormal(newWidth, newHeight);
            }
            else
            {
                // この場合は複雑なので処理の外枠も具象クラスに任せる
                return CreateNewParameterRotate(newWidth, newHeight);
            }
        }

        protected abstract void AdjustWidthAndHeightIfOverShowingImage(ref double newWidth, ref double newHeight);

        private CutLineParameter CreateNewParameterNormal(double newWidth, double newHeight)
        {
            System.Windows.Point newLeftTop = GetNewLeftTop(newWidth, newHeight);
            System.Windows.Point newRightTop = GetNewRightTop(newWidth, newHeight);
            System.Windows.Point newLeftBottom = GetNewLeftBottom(newWidth, newHeight);
            System.Windows.Point newRightBottom = GetNewRightBottom(newWidth, newHeight);
            return new CutLineParameter(newLeftTop, newRightTop, newRightBottom, newLeftBottom, Before.Degree);
        }

        protected abstract System.Windows.Point GetNewLeftTop(double newWidth, double newHeight);

        protected abstract System.Windows.Point GetNewRightTop(double newWidth, double newHeight);

        protected abstract System.Windows.Point GetNewLeftBottom(double newWidth, double newHeight);

        protected abstract System.Windows.Point GetNewRightBottom(double newWidth, double newHeight);

        protected abstract CutLineParameter CreateNewParameterRotate(double newWidth, double newHeight);

        protected abstract CutLineParameter CreateNewParameterWhenExchangeOrigin(double newWidth, double newHeight);
    }
}
