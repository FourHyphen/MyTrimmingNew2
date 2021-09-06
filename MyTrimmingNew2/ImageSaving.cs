using System;
using System.Threading.Tasks;
using System.Windows;

namespace MyTrimmingNew2
{
    public class ImageSaving
    {
        private MainWindow Main { get; }

        private OriginalImage _OriginalImage { get; }

        private ShowingImage _ShowingImage { get; }

        private CutLine _CutLine { get; }

        private ImageTrim _ImageTrim { get; set; }

        public double Progress
        {
            get
            {
                return ((_ImageTrim == null) ? 0.0 : _ImageTrim.Progress);
            }
        }

        public ImageSaving(MainWindow main, OriginalImage originalImage, ShowingImage showingImage, CutLine cutLine)
        {
            Main = main;
            _OriginalImage = originalImage;
            _ShowingImage = showingImage;
            _CutLine = cutLine;
        }

        public void Execute(string filePath, ImageProcess.Interpolate interpolate, double unsharpMask)
        {
            DisplaySaveStatusWindow();

            System.Timers.Timer timer = new System.Timers.Timer();
            StartSaveImageTimer(timer, filePath, interpolate, unsharpMask);
            WaitToFinishSaveImageTimer(timer);

            HideSaveStatusWindow();
        }

        private void DisplaySaveStatusWindow()
        {
            Main.SaveStatus.Dispatcher.Invoke(() =>
            {
                Main.SaveProgressLabel.Content = "0.0";
                Main.Menu.IsEnabled = false;
                Main.ImageArea.IsEnabled = false;
                Main.SaveProgressBar.Value = 0.0;
                Main.SaveStatus.Visibility = Visibility.Visible;
            });
        }

        private void StartSaveImageTimer(System.Timers.Timer timer, string filePath, ImageProcess.Interpolate interpolate, double unsharpMask)
        {
            SaveImageAsync(filePath, interpolate, unsharpMask);

            timer.Interval = 500;  // [ms]
            timer.Elapsed += (s, e) =>
            {
                Main.SaveStatus.Dispatcher.Invoke(() =>
                {
                    Main.SaveProgressBar.Value = Progress;
                    Main.SaveProgressLabel.Content = Math.Round(Progress, 1).ToString();
                });

                if (Progress >= 100.0)
                {
                    timer.Stop();
                    timer.Enabled = false;
                }
            };

            timer.Start();
        }

        private void WaitToFinishSaveImageTimer(System.Timers.Timer countdownTimer)
        {
            Task task = Task.Run(() =>
            {
                while (true)
                {
                    if (!countdownTimer.Enabled)
                    {
                        return;
                    }
                }
            });

            Task.WaitAll(task);
        }

        private void HideSaveStatusWindow()
        {
            Main.SaveStatus.Dispatcher.Invoke(() =>
            {
                Main.ImageArea.IsEnabled = true;
                Main.Menu.IsEnabled = true;
                Main.SaveStatus.Visibility = Visibility.Hidden;
            });
        }

        private async void SaveImageAsync(string filePath, ImageProcess.Interpolate interpolate, double unsharpMask)
        {
            await Task.Run(() =>
            {
                Save(filePath, interpolate, unsharpMask);
            });
        }

        private void Save(string filePath, ImageProcess.Interpolate interpolate, double unsharpMask)
        {
            _ImageTrim = new ImageTrim(_OriginalImage.Path,
                                       _ShowingImage.ToOriginalScale(_CutLine.LeftTop),
                                       _ShowingImage.ToOriginalScale(_CutLine.RightTop),
                                       _ShowingImage.ToOriginalScale(_CutLine.RightBottom),
                                       _ShowingImage.ToOriginalScale(_CutLine.LeftBottom),
                                       _CutLine.Degree);
            System.Drawing.Bitmap saveBitmap = _ImageTrim.Create(interpolate, unsharpMask);
            saveBitmap.Save(filePath);
            saveBitmap.Dispose();
        }
    }
}
