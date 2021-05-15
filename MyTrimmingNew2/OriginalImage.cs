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

        public string GetSaveImageNameExample(string dirPath)
        {
            string fileName = System.IO.Path.GetFileName(Path);
            string ext = System.IO.Path.GetExtension(fileName);
            string fileNameBase = fileName.Replace(ext, "");

            for (int i = 1; i <= 99; i++)
            {
                string exampleName = fileNameBase + "_resize_" + i.ToString().PadLeft(2, '0') + ext;
                string examplePath = System.IO.Path.Combine(dirPath, exampleName);
                if (!System.IO.File.Exists(examplePath))
                {
                    return exampleName;
                }
            }

            return fileNameBase + "_resize" + ext;
        }
    }
}
