using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLine
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Left { get; private set; }

        public int Top { get; private set; }

        public CutLine(ShowingImage showingImage)
        {
            Init(showingImage);
        }

        private void Init(ShowingImage showingImage)
        {
            InitOrigin();
            InitSize(showingImage);
        }

        private void InitOrigin()
        {
            Left = 0;
            Top = 0;
        }

        private void InitSize(ShowingImage showingImage)
        {
            double width = showingImage.Width;
            double height = width * 9.0 / 16.0;
            if (height > showingImage.Height)
            {
                height = showingImage.Height;
                width = height * 16.0 / 9.0;
            }

            Width = (int)width;
            Height = (int)height;
        }
    }
}
