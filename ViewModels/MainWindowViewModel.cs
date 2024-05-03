using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using susi_gui_windows.GUI;
using susi_gui_windows.Messages;
using susi_gui_windows.Models;

namespace susi_gui_windows.ViewModels
{
    internal partial class MainWindowViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<FileOperation> fileOperations;

        private TaskRepository taskRepository;
        private readonly DispatcherQueue dispatcherQueue;
        private readonly Dictionary<string, Icon> fileDefaultIcons;

        public MainWindowViewModel(TaskRepository taskRepository)
        {
            fileOperations = new ObservableCollection<FileOperation>();

            this.taskRepository = taskRepository;
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            fileDefaultIcons = new Dictionary<string, Icon>();

            WeakReferenceMessenger.Default.Register<NewUnsecuredFilesMessage>(this, (r, m) =>
            {
                var recipient = (MainWindowViewModel) r;
                recipient.NewUnsecuredFilesMessageCallback(m);
            });
        }

        private void NewUnsecuredFilesMessageCallback(NewUnsecuredFilesMessage message)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                lock (FileOperations)
                {
                    string[] newUnsecuredFilePaths = message.Value;
                    foreach (string path in newUnsecuredFilePaths)
                    {
                        var operation = new FileOperation(path, FileOperationType.Encryption);
                        FileOperations.Add(operation);
                    }
                }
            });
        }
    }
}
