using Microsoft.Win32;
using System;

namespace MyTrimmingNew2
{
    class DialogSaveImageFile
    {
        public static string Show(OriginalImage image)
        {
            SaveFileDialog sfd = CreateSaveFileDialog(image);
            string filePath = "";
            if (sfd.ShowDialog() == true)
            {
                filePath = sfd.FileName;
            }

            return filePath;
        }

        private static SaveFileDialog CreateSaveFileDialog(OriginalImage image)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = System.IO.Path.GetDirectoryName(image.Path);
            sfd.FileName = image.GetSaveImageNameExample(sfd.InitialDirectory);
            sfd.Title = "保存先を指定してください";
            sfd.RestoreDirectory = true;
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = false;

            return sfd;
        }
    }
}