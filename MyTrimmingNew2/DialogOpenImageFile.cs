using Microsoft.Win32;

namespace MyTrimmingNew2
{
    class DialogOpenImageFile
    {
        public static string Show()
        {
            OpenFileDialog ofd = CreateOpenFileDialog();
            string filePath = "";
            if (ofd.ShowDialog() == true)
            {
                filePath = ofd.FileName;
            }

            return filePath;
        }

        private static OpenFileDialog CreateOpenFileDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            // TODO: 表示文字列は全て外部管理(setting.ini？)したい
            ofd.FileName = "";
            ofd.Filter = "Imageファイル(*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png|すべてのファイル(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Title = "開く画像ファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            return ofd;
        }
    }
}
