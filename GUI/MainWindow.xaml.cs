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

            // We should listen to changes to the unsecured files collection.
            this.viewModel.UnsecuredFiles.CollectionChanged += UnsecuredFiles_CollectionChanged;
        }

        private async void AskPasswordForFilesViaDialog()
        {
            areWeAskingPasswordsForFiles = true;

            while (this.viewModel.UnsecuredFiles.Count > 0)
            {
                int numQueuedFiles = this.viewModel.UnsecuredFiles.Count;
                TargetFile targetFile = this.viewModel.UnsecuredFiles[0];

                ContentDialog dialog = new ContentDialog()
                {
                    XamlRoot = rootPanel.XamlRoot,
                    Title = $"Set Password for File ({numQueuedFiles} Files Queued)",
                    Content = $"Set the password for {targetFile.FileName}?",
                    PrimaryButtonText = "Set Password",
                    SecondaryButtonText = "Cancel Locking"
                };

                ContentDialogResult result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // HOI, GAGONG TRAGICO ROMANTICO! THIS IS TEMPORARY. DO NOT FORGET TO UPDATE.
                    this.viewModel.FileOperations.Add(
                        new FileOperation(targetFile.FilePath, FileOperationType.Encryption)
                    );
                }

                this.viewModel.UnsecuredFiles.RemoveAt(0);
            }

            areWeAskingPasswordsForFiles = false;
        }

        private void UnsecuredFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!areWeAskingPasswordsForFiles)
            {
                AskPasswordForFilesViaDialog();
            }
        }
    }
}
