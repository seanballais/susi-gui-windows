using System.Drawing;
using System.IO;

using Microsoft.UI.Xaml.Media.Imaging;

using susi_gui_windows.Core;

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
    }
}
