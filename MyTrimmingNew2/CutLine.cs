﻿using System.Windows;
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

        public double Ratio { get { return Parameter.Ratio; } }

        public double Degree { get { return Parameter.Degree; } }

        public System.Windows.Point LeftTop { get { return Parameter.LeftTop; } }

        public System.Windows.Point RightTop { get { return Parameter.RightTop; } }

        public System.Windows.Point RightBottom { get { return Parameter.RightBottom; } }

        public System.Windows.Point LeftBottom { get { return Parameter.LeftBottom; } }

        public CutLine(ShowingImage showingImage)
        {
            _ShowingImage = showingImage;
            ExecuteCommandInit();
        }

        public CutLineParameter CloneParameter()
        {
            if (Parameter == null)
            {
                return null;
            }
            return new CutLineParameter(Parameter.LeftTop, Parameter.RightTop, Parameter.RightBottom, Parameter.LeftBottom, Parameter.Degree);
        }

        private void ExecuteCommandInit()
        {
            CutLineCommand command = CutLineCommandFactory.Create(this, _ShowingImage);
            ExecuteCommandCore(command);
        }

        public void ExecuteCommand(Key key, int keyInputNum = 1)
        {
            ExecuteCommand(key, ModifierKeys.None, keyInputNum);
        }

        public void ExecuteCommand(Key key, System.Windows.Input.ModifierKeys modifierKeys, int keyInputNum = 1)
        {
            if (AppKey.IsPurposeUndo(key, modifierKeys))
            {
                ExecuteUndo(keyInputNum);
            }
            else if (AppKey.IsPurposeRedo(key, modifierKeys))
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

        private void ExecuteUndo(int undoNum)
        {
            Parameter = CommandHistory.Undo(undoNum);
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
            RectLine rl = new RectLine(LeftTop, RightTop, RightBottom, LeftBottom);
            return (rl.IsInside((p.X, p.Y)));
        }
    }
}
