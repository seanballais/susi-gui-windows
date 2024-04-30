using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;

using susi_gui_windows.Core;
using susi_gui_windows.Messages;

namespace susi_gui_windows.ViewModels
{
    public partial class MainWindowViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<string> pendingFilePaths;

        private readonly DispatcherQueue dispatcherQueue;

        public MainWindowViewModel()
        {
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Messenger.Register<MainWindowViewModel, NewUnsecuredFilesMessage>(
                this,
                (r, m) => r.NewUnsecuredFilesMessage_Receiver(m)
            );
        }

        private void NewUnsecuredFilesMessage_Receiver(NewUnsecuredFilesMessage message)
        {
            string[] newFiles = message.Value;
            foreach (string filePath in newFiles)
            {
                // We need to update pendingFilePaths from the UI thread.
                // This function is getting called in a different thread.
                Logging.Info($"Got {filePath}.");
                dispatcherQueue.TryEnqueue(() =>
                {
                    PendingFilePaths.Add(filePath);
                });
            }
        }
    }
}
