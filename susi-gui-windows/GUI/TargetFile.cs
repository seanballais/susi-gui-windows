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
using System.IO;

using Microsoft.UI.Xaml.Media.Imaging;

using susi_gui_windows.ViewModels;

namespace susi_gui_windows.GUI
{
    internal class TargetFile
    {
        private string filePath;
        private BitmapImage fileIcon;
        private readonly FileOperationType type;

        public TargetFile(string filePath, FileOperationType type)
        {
            this.filePath = filePath;

            Icon gdiFileIcon = Icon.ExtractAssociatedIcon(this.filePath);
            fileIcon = GraphicsUtils.ConvertGDIIconToWinUIBitmapSource(gdiFileIcon);

            this.type = type;
        }

        public string FileName { get { return Path.GetFileName(filePath); } }
        public string FilePath { get { return filePath; } }
        public BitmapImage FileIcon { get { return fileIcon; } }
        public FileOperationType OperationType { get { return type; } }
    }
}
