using System.Collections.Generic;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;

using susi_gui_windows.GUI;
using susi_gui_windows.Messages;
using susi_gui_windows.Models;

namespace susi_gui_windows.ViewModels
{
    internal partial class MainWindowViewModel : ObservableRecipient
    {
        private RangeObservableCollection<FileOperation> fileOperations;
        private RangeObservableCollection<TargetFile> unsecuredFiles;

        private TaskRepository taskRepository;
        private readonly DispatcherQueue dispatcherQueue;

        public MainWindowViewModel(TaskRepository taskRepository)
        {
            fileOperations = new RangeObservableCollection<FileOperation>();
            unsecuredFiles = new RangeObservableCollection<TargetFile>();

            this.taskRepository = taskRepository;
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            WeakReferenceMessenger.Default.Register<NewUnsecuredFilesMessage>(this, (r, m) =>
            {
                var recipient = (MainWindowViewModel) r;
                recipient.NewUnsecuredFilesMessageCallback(m);
            });
        }

        public RangeObservableCollection<FileOperation> FileOperations { get { return fileOperations; } }
        public RangeObservableCollection<TargetFile> UnsecuredFiles { get { return unsecuredFiles; } }

        public void AddFileOperation(TargetFile targetFile)
        {
            fileOperations.Add(new FileOperation(targetFile.FilePath, targetFile.OperationType));
        }

        private void NewUnsecuredFilesMessageCallback(NewUnsecuredFilesMessage message)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                lock (unsecuredFiles)
                {
                    string[] newUnsecuredFilePaths = message.Value;
                    List<TargetFile> newUnsecuredFiles = [];
                    foreach (string path in newUnsecuredFilePaths)
                    {
                        var targetFile = new TargetFile(path, FileOperationType.Encryption);
                        newUnsecuredFiles.Add(targetFile);
                    }
                    unsecuredFiles.AddRange(newUnsecuredFiles);
                }
            });
        }
    }
}
