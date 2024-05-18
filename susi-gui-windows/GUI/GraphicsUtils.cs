// Susi
// Copyright (C) 2024  Sean Francis N.Ballais
//
// This program is free software : you can redistribute it and /or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see < http://www.gnu.org/licenses/>.
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
