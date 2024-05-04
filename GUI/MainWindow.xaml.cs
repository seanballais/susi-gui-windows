using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using susi_gui_windows.Core;
using susi_gui_windows.GUI;
using susi_gui_windows.ViewModels;
using System;
using System.Collections.Specialized;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows
{
    internal sealed partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private bool areWeAskingPasswordsForFiles;
        private DispatcherQueue dispatcherQueue;

        public MainWindow(MainWindowViewModel viewModel)
        {
            this.InitializeComponent();

            // Set to a strict window size.
            this.AppWindow.Resize(new SizeInt32(550, 400));

            // We don't want the window to be resizable nor maximizable.
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;

            this.viewModel = viewModel;
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            // We should listen to changes to the unsecured files collection.
            viewModel.UnsecuredFiles.CollectionChanged += UnsecuredFiles_CollectionChanged;
        }

        private async void AskPasswordForFilesViaDialog()
        {
            areWeAskingPasswordsForFiles = true;

            while (viewModel.UnsecuredFiles.Count > 0)
            {
                int numQueuedFiles = viewModel.UnsecuredFiles.Count;
                TargetFile targetFile = viewModel.UnsecuredFiles[0];

                var dialog = new PasswordRequestDialog(targetFile, this.Content.XamlRoot);

                string fileWord = (numQueuedFiles > 1) ? "Files" : "File";
                string numQueuedFilesSubText = $"{numQueuedFiles} {fileWord} Queued";
                dialog.Title = $"Password Required ({numQueuedFilesSubText})";

                try
                {
                    ContentDialogResult result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        // HOI, GAGONG TRAGICO ROMANTICO! THIS IS TEMPORARY. DO NOT FORGET TO UPDATE.
                        viewModel.FileOperations.Add(
                            new FileOperation(targetFile.FilePath, FileOperationType.Encryption)
                        );
                    }
                }
                catch (Exception e)
                {
                    Logging.Info($"Exception found: {e.ToString()}");
                }

                viewModel.UnsecuredFiles.RemoveAt(0);
            }

            areWeAskingPasswordsForFiles = false;
        }

        private async void UnsecuredFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!areWeAskingPasswordsForFiles)
            {
                // Make sure this window's XAML root exists before we start the dialogs.
                while (this.Content.XamlRoot is null)
                {
                    await System.Threading.Tasks.Task.Delay(50);
                }

                AskPasswordForFilesViaDialog();
            }
        }
    }
}
