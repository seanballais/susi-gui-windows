using System;
using System.Collections.Specialized;

using Microsoft.UI.Xaml.Controls;

using susi_gui_windows.Core;
using susi_gui_windows.Utilities;
using susi_gui_windows.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows.GUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class MainPage : Page
    {
        private MainWindowViewModel viewModel;
        private PasswordRequestDialog passwordRequestDialog;
        private ResourcePadlock resourcePadlock;

        public MainPage(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            resourcePadlock = new ResourcePadlock();

            // We should listen to changes to the unsecured files collection.
            this.viewModel.UnsecuredFiles.CollectionChanged += UnsecuredFiles_CollectionChanged;
        }

        public async System.Threading.Tasks.Task InitializeCustomComponents()
        {
            await InitializePasswordRequestDialog();
        }

        private async System.Threading.Tasks.Task InitializePasswordRequestDialog()
        {
            if (passwordRequestDialog is not null)
            {
                return;
            }

            await WaitUntilXAMLRootIsReady();

            passwordRequestDialog = new PasswordRequestDialog(Content.XamlRoot);
        }

        private void UnsecuredFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AskPasswordForFilesViaDialog();
            }
        }

        private async void AskPasswordForFilesViaDialog()
        {
            // I feel like this can be further improved with something like event handlers, but I can't seem
            // to figure out a way to do it yet.
            // - Sean Ballais (May 6, 2024 2:46 AM)
            await WaitUntilPasswordRequestDialogIsReady();

            // Ensure we only get to use the password request dialog one at a time.
            if (resourcePadlock.Lock(passwordRequestDialog) == ResourcePadlockStatus.Locked)
            {
                Logging.Info("This should only show once.");
                while (viewModel.UnsecuredFiles.Count > 0)
                {
                    int numQueuedFiles = viewModel.UnsecuredFiles.Count;
                    TargetFile targetFile = viewModel.UnsecuredFiles[0];

                    string fileWord = (numQueuedFiles > 1) ? "Files" : "File";
                    string numQueuedFilesSubText = $"{numQueuedFiles} {fileWord} in Queue";
                    passwordRequestDialog.Title = $"Password Required ({numQueuedFilesSubText})";
                    passwordRequestDialog.TargetFile = targetFile;
                    passwordRequestDialog.PrimaryButtonAction = (string password) => {
                        viewModel.AddFileOperation(targetFile, password);
                    };

                    try
                    {
                        await passwordRequestDialog.ShowAsync();
                    }
                    catch (Exception e)
                    {
                        // TODO: This should use an error logging function.
                        Logging.Info($"Exception found: {e}");
                    }

                    viewModel.UnsecuredFiles.RemoveAt(0);
                }

                resourcePadlock.Unlock(passwordRequestDialog);
            }
        }

        private async System.Threading.Tasks.Task WaitUntilXAMLRootIsReady()
        {
            await Execution.AsyncWaitUntil(() => { return Content.XamlRoot is not null; });
        }

        private async System.Threading.Tasks.Task WaitUntilPasswordRequestDialogIsReady()
        {
            await Execution.AsyncWaitUntil(() => { return passwordRequestDialog is not null; });
        }
    }
}
