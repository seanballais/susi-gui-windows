using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

using Microsoft.UI.Xaml.Media.Imaging;

using susi_gui_windows.Core;
using susi_gui_windows.GUI;

namespace susi_gui_windows.ViewModels
{
    internal class FileOperation : INotifyPropertyChanged
    {
        private readonly Task task;
        private readonly FileOperationType type;
        private string filePath;
        private BitmapImage fileIcon;
        private long fileSize;
        private double progressRatio;
        private TaskProgress state;

        public FileOperation(string filePath, FileOperationType type, string password)
        {
            this.filePath = filePath;
            
            Icon gdiFileIcon = Icon.ExtractAssociatedIcon(this.filePath);
            fileIcon = GraphicsUtils.ConvertGDIIconToWinUIBitmapSource(gdiFileIcon);

            this.type = type;
            fileSize = new FileInfo(this.filePath).Length;
            progressRatio = 0;
            state = TaskProgress.Queued;

            TaskType taskType = (type == FileOperationType.Encryption)
                ? TaskType.Encryption
                : TaskType.Decryption;
            task = new Task(taskType, filePath, password);
        }

        public string FileName { get { return Path.GetFileName(filePath); } }
        public string FilePath { get { return filePath; } }
        public long FileSize { get { return fileSize; } }
        public BitmapImage FileIcon { get { return fileIcon; } }
        public FileOperationType OperationType { get { return type; } }
        public double ProgressRatio { get { return progressRatio; } }
        public TaskProgress State { get { return state; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateStatus()
        {
            TaskStatus taskStatus = task.GetStatus();
            if (taskStatus is null)
            {
                // No use in updating the properties. The task would be done already.
                return;
            }
            
            double numWrittenBytes = (double) taskStatus.NumWrittenBytes;
            progressRatio = Math.Min(numWrittenBytes / (double) fileSize, 1.0);

            NotifyPropertyChanged(nameof(ProgressRatio));

            if (taskStatus.Progress != state)
            {
                state = taskStatus.Progress;
                NotifyPropertyChanged(nameof(State));
            }
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
