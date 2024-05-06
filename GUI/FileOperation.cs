using System.ComponentModel;
using System.Drawing;
using System.IO;

using Microsoft.UI.Xaml.Media.Imaging;

using susi_gui_windows.Core;

namespace susi_gui_windows.GUI
{
    internal class FileOperation : INotifyPropertyChanged
    {
        private readonly Task task;
        private TaskStatus taskStatus;
        private readonly FileOperationType type;
        private string filePath;
        private BitmapImage fileIcon;
        private long fileSize;

        public FileOperation(string filePath, FileOperationType type, string password)
        {
            this.filePath = filePath;
            
            Icon gdiFileIcon = Icon.ExtractAssociatedIcon(this.filePath);
            fileIcon = GraphicsUtils.ConvertGDIIconToWinUIBitmapSource(gdiFileIcon);

            this.type = type;
            fileSize = new FileInfo(this.filePath).Length;

            TaskType taskType = (type == FileOperationType.Encryption)
                ? TaskType.Encryption
                : TaskType.Decryption;
            task = new Task(taskType, filePath, password);
            taskStatus = task.GetStatus();
        }

        public string FileName { get { return Path.GetFileName(filePath); } }
        public string FilePath { get { return filePath; } }
        public long FileSize { get { return fileSize; } }
        public BitmapImage FileIcon { get { return fileIcon; } }
        public TaskStatus Status { get { return taskStatus; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateStatus()
        {
            taskStatus = task.GetStatus();
            NotifyPropertyChanged(nameof(Status));
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
