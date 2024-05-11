using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;

using susi_gui_windows.Core;
using susi_gui_windows.GUI;
using susi_gui_windows.Messages;
using susi_gui_windows.Utilities;

namespace susi_gui_windows.ViewModels
{
    internal partial class MainWindowViewModel : ObservableRecipient, IDisposable
    {
        private ObservableCollection<FileOperation> fileOperations;
        private RangeObservableCollection<TargetFile> unsecuredFiles;
        private ResourcePadlock resourcePadlock;

        private readonly DispatcherQueue dispatcherQueue;
        private readonly DispatcherQueueTimer fileOperationsStatusTimer;

        public MainWindowViewModel()
        {
            fileOperations = new ObservableCollection<FileOperation>();
            unsecuredFiles = new RangeObservableCollection<TargetFile>();
            resourcePadlock = new ResourcePadlock();

            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            fileOperationsStatusTimer = dispatcherQueue.CreateTimer();

            fileOperationsStatusTimer.Interval = TimeSpan.FromMilliseconds(16);
            fileOperationsStatusTimer.Tick += (s, e) =>
            {
                if (resourcePadlock.Lock(fileOperations) == ResourcePadlockStatus.Locked)
                {
                    foreach (FileOperation operation in fileOperations)
                    {
                        operation.UpdateStatus();
                    }

                    resourcePadlock.Unlock(fileOperations);
                }
            };
            fileOperationsStatusTimer.Start();

            WeakReferenceMessenger.Default.Register<NewUnsecuredFilesMessage>(this, (r, m) =>
            {
                // NOTE: Files received from IPC are for encryption.
                var recipient = (MainWindowViewModel) r;
                recipient.NewUnsecuredFilesMessageCallback(m);
            });
        }
        public void Dispose()
        {
            fileOperationsStatusTimer.Stop();
        }

        public ObservableCollection<FileOperation> FileOperations { get { return fileOperations; } }
        public RangeObservableCollection<TargetFile> UnsecuredFiles { get { return unsecuredFiles; } }

        public void AddFileOperation(TargetFile targetFile, string password)
        {
            fileOperations.Add(
                new FileOperation(targetFile.FilePath, targetFile.OperationType, password, this));
        }

        public void AddNewUnsecuredFiles(string[] targetFilePaths, FileOperationType type)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                lock (unsecuredFiles)
                {
                    List<TargetFile> newUnsecuredFiles = [];
                    foreach (string path in targetFilePaths)
                    {
                        var targetFile = new TargetFile(path, FileOperationType.Encryption);
                        newUnsecuredFiles.Add(targetFile);
                    }
                    unsecuredFiles.AddRange(newUnsecuredFiles);
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanRemoveFileOperation))]
        public void RemoveFileOperation(FileOperation operation)
        {
            fileOperations.Remove(operation);
        }

        private void NewUnsecuredFilesMessageCallback(NewUnsecuredFilesMessage message)
        {
            string[] newUnsecuredFilePaths = message.Value;
            AddNewUnsecuredFiles(newUnsecuredFilePaths, FileOperationType.Encryption);
        }

        private bool CanRemoveFileOperation(FileOperation operation)
        {
            return operation.State == TaskProgress.Done
                || operation.State == TaskProgress.Interrupted
                || operation.State == TaskProgress.Failed;
        }
    }
}
