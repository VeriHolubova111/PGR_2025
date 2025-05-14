using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EasyImageEditor
{
    public partial class MainWindow : Window
    {
        private Bitmap originalBitmap;
        private Bitmap currentBitmap;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Obrázky|*.bmp;*.jpg;*.png;*.jpeg";
            if (ofd.ShowDialog() == true)
            {
                originalBitmap = new Bitmap(ofd.FileName);
                currentBitmap = (Bitmap)originalBitmap.Clone();
                DisplayImage(currentBitmap);
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentBitmap == null) return;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG|*.png";
            if (sfd.ShowDialog() == true)
            {
                currentBitmap.Save(sfd.FileName, ImageFormat.Png);
            }
        }

        private void ToGrayscale_Click(object sender, RoutedEventArgs e)
        {
            if (currentBitmap == null) return;
            Bitmap gray = new Bitmap(currentBitmap.Width, currentBitmap.Height);
            for (int y = 0; y < currentBitmap.Height; y++)
            {
                for (int x = 0; x < currentBitmap.Width; x++)
                {
                    System.Drawing.Color c = currentBitmap.GetPixel(x, y);
                    int luma = (int)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
                    gray.SetPixel(x, y, System.Drawing.Color.FromArgb(luma, luma, luma));
                }
            }
            currentBitmap = gray;
            DisplayImage(currentBitmap);
        }

        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            if (currentBitmap == null) return;
            currentBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            DisplayImage(currentBitmap);
        }

        private void SobelFilter_Click(object sender, RoutedEventArgs e)
        {
            if (currentBitmap == null) return;
            Bitmap sobel = Sobel(currentBitmap);
            currentBitmap = sobel;
            DisplayImage(currentBitmap);
        }

        private void DisplayImage(Bitmap bmp)
        {
            using MemoryStream memory = new MemoryStream();
            bmp.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            ImageViewer.Source = bitmapImage;
        }

        private Bitmap Sobel(Bitmap bmp)
        {
            Bitmap gray = new Bitmap(bmp.Width, bmp.Height);
            for (int y = 1; y < bmp.Height - 1; y++)
            {
                for (int x = 1; x < bmp.Width - 1; x++)
                {
                    int gx =
                        -1 * bmp.GetPixel(x - 1, y - 1).R + 1 * bmp.GetPixel(x + 1, y - 1).R +
                        -2 * bmp.GetPixel(x - 1, y).R + 2 * bmp.GetPixel(x + 1, y).R +
                        -1 * bmp.GetPixel(x - 1, y + 1).R + 1 * bmp.GetPixel(x + 1, y + 1).R;

                    int gy =
                        -1 * bmp.GetPixel(x - 1, y - 1).R + -2 * bmp.GetPixel(x, y - 1).R + -1 * bmp.GetPixel(x + 1, y - 1).R +
                         1 * bmp.GetPixel(x - 1, y + 1).R + 2 * bmp.GetPixel(x, y + 1).R + 1 * bmp.GetPixel(x + 1, y + 1).R;

                    int magnitude = Math.Clamp((int)Math.Sqrt(gx * gx + gy * gy), 0, 255);
                    gray.SetPixel(x, y, System.Drawing.Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }
            return gray;
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (originalBitmap != null)
            {
                currentBitmap = (Bitmap)originalBitmap.Clone();
                DisplayImage(currentBitmap);
            }
            else
            {
                MessageBox.Show("Nejdříve načti obrázek.");
            }
        }
        private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            if (currentBitmap == null) return;

                currentBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                DisplayImage(currentBitmap);
        }

        private void FlipVertical_Click(object sender, RoutedEventArgs e)
        {
            if (currentBitmap == null) return;
                currentBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                DisplayImage(currentBitmap);
        }
    }
}
