using System.Drawing;
using System.IO;

using Microsoft.UI.Xaml.Media.Imaging;

using susi_gui_windows.Core;

namespace susi_gui_windows.GUI
{
    internal class FileOperation
    {
        private readonly Core.TaskID taskID;
        private readonly FileOperationType type;
        private string filePath;
        private BitmapImage fileIcon;
        private string password;

        public FileOperation(string filePath, FileOperationType type, string password)
        {
            this.filePath = filePath;
            
            Icon gdiFileIcon = Icon.ExtractAssociatedIcon(this.filePath);
            fileIcon = GraphicsUtils.ConvertGDIIconToWinUIBitmapSource(gdiFileIcon);

            this.password = password;
            this.type = type;
        }

        public string FileName { get { return Path.GetFileName(filePath); } }
        public string FilePath { get { return filePath; } }
        public BitmapImage FileIcon { get { return fileIcon; } }
    }
}
