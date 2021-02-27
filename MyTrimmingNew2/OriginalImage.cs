using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class OriginalImage
    {
        public string Path { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public OriginalImage(string imagePath)
        {
            Path = imagePath;
            Init();
        }

        private void Init()
        {
            // 画像を開く際に検証処理しないことで高速に読み込む
            using (System.IO.FileStream fs = System.IO.File.OpenRead(Path))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(fs, false, false))
                {
                    Width = image.Width;
                    Height = image.Height;
                }
            }
        }
    }
}
