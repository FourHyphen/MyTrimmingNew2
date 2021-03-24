using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyTrimmingNew2
{
    public class CutLine
    {
        private ShowingImage _ShowingImage { get; set; }

        private CutLineParameter Parameter { get; set; }

        private CutLineCommandHistory CommandHistory { get; set; } = new CutLineCommandHistory();

        private static double NearRange = 20.0;

        public double Width { get { return Parameter.Width; } }

        public double Height { get { return Parameter.Height; } }

        public double Degree { get { return Parameter.Degree; } }

        public System.Windows.Point LeftTop { get { return Parameter.LeftTop; } }

        public System.Windows.Point RightTop { get { return Parameter.RightTop; } }

        public System.Windows.Point RightBottom { get { return Parameter.RightBottom; } }

        public System.Windows.Point LeftBottom { get { return Parameter.LeftBottom; } }

        public CutLine(ShowingImage showingImage)
        {
            _ShowingImage = showingImage;
            Init();
        }

        private void Init()
        {
            Parameter = new CutLineParameter(_ShowingImage);
        }

        public CutLineParameter CloneParameter()
        {
            return new CutLineParameter(Parameter.LeftTop, Parameter.RightTop, Parameter.RightBottom, Parameter.LeftBottom, Parameter.Degree);
        }

        public void ExecuteCommand(Key key, int keyInputNum = 1)
        {
            ExecuteCommand(key, ModifierKeys.None, keyInputNum);
        }

        public void ExecuteCommand(Key key, System.Windows.Input.ModifierKeys modifierKeys, int keyInputNum = 1)
        {
            if (IsPurposeUndo(key, modifierKeys))
            {
                ExecuteUndo(keyInputNum);
            }
            else if (IsPurposeRedo(key, modifierKeys))
            {
                ExecuteRedo(keyInputNum);
            }
            else
            {
                CutLineCommand command = CutLineCommandFactory.Create(this, _ShowingImage, key, modifierKeys, keyInputNum);
                ExecuteCommandCore(command);
            }
        }

        public void ExecuteCommand(Point dragStart, Point dropPoint)
        {
            CutLineCommand command = CutLineCommandFactory.Create(this, _ShowingImage, dragStart, dropPoint);
            ExecuteCommandCore(command);
        }

        private void ExecuteCommandCore(CutLineCommand command)
        {
            if (command != null)
            {
                Parameter = CommandHistory.Execute(command);
            }
        }

        private bool IsPurposeUndo(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKey)
        {
            return (modifierKey == ModifierKeys.Control && key == Key.Z);
        }

        private void ExecuteUndo(int undoNum)
        {
            Parameter = CommandHistory.Undo(undoNum);
        }

        private bool IsPurposeRedo(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKey)
        {
            return (modifierKey == ModifierKeys.Control && key == Key.Y);
        }

        private void ExecuteRedo(int redoNum)
        {
            Parameter = CommandHistory.Redo(redoNum);
        }

        public bool IsPointNearLeftTop(Point p)
        {
            return (IsPointNear(Parameter.LeftTop, p));
        }

        public bool IsPointNearRightTop(Point p)
        {
            return (IsPointNear(Parameter.RightTop, p));
        }

        public bool IsPointNearLeftBottom(Point p)
        {
            return (IsPointNear(Parameter.LeftBottom, p));
        }

        public bool IsPointNearRightBottom(Point p)
        {
            return (IsPointNear(Parameter.RightBottom, p));
        }

        private bool IsPointNear(Point baseP, Point p)
        {
            return (IsPointNearX(baseP.X, p.X, NearRange)) && (IsPointNearY(baseP.Y, p.Y, NearRange));
        }

        private bool IsPointNearX(double baseX, double x, double range)
        {
            return ((x - range) <= baseX) && (baseX <= (x + range));
        }

        private bool IsPointNearY(double baseY, double y, double range)
        {
            return ((y - range) <= baseY) && (baseY <= (y + range));
        }

        public bool IsPointInside(Point p)
        {
            if (p.X < LeftTop.X || p.X > RightTop.X)
            {
                return false;
            }
            if (p.Y < LeftTop.Y || p.Y > RightBottom.Y)
            {
                return false;
            }

            return true;
        }
    }
}
