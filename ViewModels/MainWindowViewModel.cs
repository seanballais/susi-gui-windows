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
        public ObservableCollection<FileOperation> fileOperations;
        public ObservableCollection<TargetFile> unsecuredFiles;

        private TaskRepository taskRepository;
        private readonly DispatcherQueue dispatcherQueue;

        public MainWindowViewModel(TaskRepository taskRepository)
        {
            fileOperations = new ObservableCollection<FileOperation>();
            unsecuredFiles = new ObservableCollection<TargetFile>();

            this.taskRepository = taskRepository;
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            WeakReferenceMessenger.Default.Register<NewUnsecuredFilesMessage>(this, (r, m) =>
            {
                var recipient = (MainWindowViewModel) r;
                recipient.NewUnsecuredFilesMessageCallback(m);
            });
        }

        public ObservableCollection<FileOperation> FileOperations { get { return fileOperations; } }
        public ObservableCollection<TargetFile> UnsecuredFiles { get { return unsecuredFiles; } }

        private void NewUnsecuredFilesMessageCallback(NewUnsecuredFilesMessage message)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                lock (unsecuredFiles)
                {
                    string[] newUnsecuredFilePaths = message.Value;
                    foreach (string path in newUnsecuredFilePaths)
                    {
                        var targetFile = new TargetFile(path, FileOperationType.Encryption);
                        unsecuredFiles.Add(targetFile);
                    }
                }
            });
        }
    }
}
