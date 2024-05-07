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
            if (IsFinished())
            {
                // Operation is done. So, no need to update any of the properties.
                // If we force ourselves to still call `task.GetStatus()`, exceptions
                // will be thrown from down the stack in the function since the status
                // object of the task that this file operation is associated with will
                // already have been deleted by the time this operation is done *and*
                // when we got its final status. The absence of the status object will
                // cause a NullFFIPointerException to be thrown internally in our FFI
                // layer.
                //
                // These exceptions, fortunately, are caught internally. But, we have to
                // remember that this function, `UpdateStatus()`, gets called multiple
                // times per second by a DispatchQueueTimer. Exception handling is also
                // expensive. So, any exceptions thrown inside `task.GetStatus()` will
                // contribute to a degradation of the app's performance.
                //
                // Returning this function once we know that the operation is done prevents
                // us from calling `task.GetStatus()`, averting the issues we would have
                // encountered as mentioned earlier.
                return;
            }

            TaskStatus taskStatus = task.GetStatus();            
            double numWrittenBytes = (double) taskStatus.NumWrittenBytes;
            progressRatio = Math.Min(numWrittenBytes / (double) fileSize, 1.0);

            NotifyPropertyChanged(nameof(ProgressRatio));

            if (taskStatus.Progress != state)
            {
                state = taskStatus.Progress;
                NotifyPropertyChanged(nameof(State));
            }
        }

        private bool IsFinished()
        {
            return state == TaskProgress.Done
                || state == TaskProgress.Failed
                || state == TaskProgress.Interrupted;
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
