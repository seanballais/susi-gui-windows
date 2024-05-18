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
        private long numProcessedBytes;
        private double progressRatio;
        private TaskProgress state;
        private string errorMessage;

        // Easiest way to let our XAML code access the view model commands.
        private MainWindowViewModel viewModel;

        public FileOperation(
            string filePath,
            FileOperationType type,
            string password,
            MainWindowViewModel viewModel)
        {
            this.filePath = filePath;
            
            Icon gdiFileIcon = Icon.ExtractAssociatedIcon(this.filePath);
            fileIcon = GraphicsUtils.ConvertGDIIconToWinUIBitmapSource(gdiFileIcon);

            this.type = type;
            fileSize = new FileInfo(this.filePath).Length;
            progressRatio = 0;
            state = TaskProgress.Queued;
            errorMessage = null;
            this.viewModel = viewModel;

            TaskType taskType = (type == FileOperationType.Encryption)
                ? TaskType.Encryption
                : TaskType.Decryption;
            task = new Task(taskType, filePath, password);
        }

        // Forced to have this property since, in WinUI 3, we can't get
        // the current item inside a DataTemplate.
        public FileOperation Self { get { return this; } }

        public string FileName { get { return Path.GetFileName(filePath); } }
        public string FilePath { get { return filePath; } }
        public long FileSize { get { return fileSize; } }
        public BitmapImage FileIcon { get { return fileIcon; } }
        public FileOperationType OperationType { get { return type; } }
        public long NumProcessedBytes { get { return numProcessedBytes; } }
        public double ProgressRatio { get { return progressRatio; } }
        public TaskProgress State { get { return state; } }
        public string ErrorMessage { get { return errorMessage; } }
        public MainWindowViewModel ViewModel { get { return viewModel; } }

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

            if (taskStatus.NumProcessedBytes != numProcessedBytes)
            {
                numProcessedBytes = taskStatus.NumProcessedBytes;
                progressRatio = numProcessedBytes / (double)fileSize;

                NotifyPropertyChanged(nameof(NumProcessedBytes));
                NotifyPropertyChanged(nameof(ProgressRatio));
            }

            if (taskStatus.Progress != state)
            {
                state = taskStatus.Progress;
                NotifyPropertyChanged(nameof(State));

                // For our clear button, we are using a command
                // (RemoveFileOperationCommand) that takes a FileOperation
                // as a parameter. To bind that command while also making
                // sure that we get to this pass this instance of
                // FileOperation, we first need to provide access to this
                // instance via the Self property we created. This property
                // is bound to the command parameter of our clear button,
                // which is passed to the button's command. To ensure that
                // the binding gets updated whenever the operation's state
                // changes, we need to call this notifier.
                NotifyPropertyChanged(nameof(Self));
            }

            if ($"{taskStatus.LastError}." != errorMessage)
            {
                errorMessage = $"{taskStatus.LastError}.";
                NotifyPropertyChanged(nameof(ErrorMessage));
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
