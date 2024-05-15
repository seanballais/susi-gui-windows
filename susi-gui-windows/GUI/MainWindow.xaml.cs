using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.Graphics;

using susi_gui_windows.ViewModels;
using susi_gui_windows.GUI;
using Microsoft.UI.Xaml.Media;
using susi_gui_windows.Utilities;

namespace susi_gui_windows
{
    internal sealed partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private MainPage mainPage;
        private ContentDialog windowCloseConfirmationDialog;
        private ResourcePadlock resourcePadlock;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            AppWindow.SetIcon(Constants.AppIconPath);

            // Set to a strict window size.
            AppWindow.Resize(new SizeInt32(550, 600));

            // We don't want the window to be resizable nor maximizable.
            var appWindowPresenter = AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;

            AppWindow.Closing += MainWindow_Closing;

            resourcePadlock = new ResourcePadlock();

            this.viewModel = viewModel;

            mainPage = new MainPage(viewModel);
            Content = mainPage;

            windowCloseConfirmationDialog = new ContentDialog
            {
                Title = "Close Susi?",
                Content = "",
                DefaultButton = ContentDialogButton.Secondary,
                PrimaryButtonText = "Yes!",
                SecondaryButtonText = "Cancel"
            };
        }

        private async void MainWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            args.Cancel = true;
            if (resourcePadlock.Lock(windowCloseConfirmationDialog) == ResourcePadlockStatus.Locked)
            {
                bool appHasOngoingOperations = viewModel.HasOngoingOperations();
                bool appHasQueuedFiles = viewModel.HasQueuedFiles();

                if (appHasOngoingOperations || appHasQueuedFiles)
                {
                    string[] currentSituationTexts = { null, null };
                    if (appHasOngoingOperations)
                    {
                        currentSituationTexts[0] = "ongoing operations";
                    }

                    if (appHasQueuedFiles)
                    {
                        currentSituationTexts[1] = "queued files";
                    }

                    string additionalDialogText = string.Join(
                        " and ",
                        currentSituationTexts.Where(s => !string.IsNullOrEmpty(s))
                    );
                    windowCloseConfirmationDialog.Content = (
                        "Are you sure you want to close Susi? "
                        + $"You have {additionalDialogText}. "
                        + "If you continue with closing the app, they will be cancelled."
                    );

                    bool hasOpenDialogs = AreContentDialogsOpen();

                    if (hasOpenDialogs)
                    {
                        mainPage.HideCurrentDialog();
                        WaitUntilContentDialogsAreClosed();
                    }

                    ContentDialogResult result = await windowCloseConfirmationDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        Close();
                    }
                    else
                    {
                        mainPage.UnhideCurrentDialog();
                    }
                }
                else
                {
                    Close();
                }

                resourcePadlock.Unlock(windowCloseConfirmationDialog);
            }
        }

        public async Task InitializeCustomComponents()
        {
            await mainPage.InitializeCustomComponents();

            windowCloseConfirmationDialog.XamlRoot = Content.XamlRoot;
        }

        private void WaitUntilContentDialogsAreClosed()
        {
            while (AreContentDialogsOpen()) {}
        }

        private bool AreContentDialogsOpen()
        {
            var openedPopups = VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot);
            foreach (var popup in openedPopups)
            {
                if (popup.Child is ContentDialog)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
