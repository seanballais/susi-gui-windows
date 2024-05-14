using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.UI.Xaml.Media.Imaging;

namespace susi_gui_windows.GUI
{
    internal static class GraphicsUtils
    {
        public static BitmapImage ConvertGDIIconToWinUIBitmapSource(Icon icon)
        {
            // Based on: https://stackoverflow.com/a/51227400/1116098
            // but a miniscule amount from: https://stackoverflow.com/a/76641464/1116098
            if (icon == null)
            {
                return null;
            }

            Bitmap bmp = icon.ToBitmap();
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                bitmapImage.SetSource(stream.AsRandomAccessStream());
            }

            return bitmapImage;
        }
    }
}
