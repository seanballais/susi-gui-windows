using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;

using susi_gui_windows.Models;

namespace susi_gui_windows.ViewModels
{
    internal partial class MainWindowViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<string> pendingFilePaths;

        private TaskRepository taskRepository;
        private readonly Guid taskRepositoryClientGUID;
        private readonly DispatcherQueue dispatcherQueue;

        public MainWindowViewModel(TaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
            taskRepositoryClientGUID = this.taskRepository.ConnectClient();
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            pendingFilePaths = new ObservableCollection<string>();

            Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        PendingFilePaths.Add("test");
                    });
                }
            });
        }
    }
}
