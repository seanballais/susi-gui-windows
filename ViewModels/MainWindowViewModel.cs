using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using susi_gui_windows.Messages;
using susi_gui_windows.Models;

namespace susi_gui_windows.ViewModels
{
    internal partial class MainWindowViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<string> pendingFilePaths;

        private TaskRepository taskRepository;
        private readonly DispatcherQueue dispatcherQueue;

        public MainWindowViewModel(TaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            pendingFilePaths = new ObservableCollection<string>();

            WeakReferenceMessenger.Default.Register<NewUnsecuredFilesMessage>(this, (r, m) =>
            {
                MainWindowViewModel recipient = (MainWindowViewModel) r;
                recipient.NewUnsecuredFilesMessageCallback(m);
            });
        }

        private void NewUnsecuredFilesMessageCallback(NewUnsecuredFilesMessage message)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                lock(PendingFilePaths)
                {
                    string[] newUnsecuredFilePaths = message.Value;
                    foreach (string path in newUnsecuredFilePaths)
                    {
                        PendingFilePaths.Add(path);
                    }
                }
            });
        }
    }
}
