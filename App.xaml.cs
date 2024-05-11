using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

using susi_gui_windows.Core;
using susi_gui_windows.GUI;
using susi_gui_windows.OS;
using susi_gui_windows.ViewModels;
using Windows.ApplicationModel.Activation;

namespace susi_gui_windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;
        private MainWindowViewModel mainWindowViewModel;
        private NewFilesReceiver newFilesReceiver;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            Lib.Initialize();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Based on: https://gist.github.com/andrewleader/5adc742fe15b06576c1973ea6e999552
            var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var instance = AppInstance.FindOrRegisterForKey("susi-sfb");

            // Check if our current instance is the main instance. If it's not, we bounce.
            if (!instance.IsCurrent)
            {
                // Redirect activation to the main instance.
                await instance.RedirectActivationToAsync(activationArgs);

                // And we exit our instance.
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            // Well, we're the main instance then. Let's register for activation redirection.
            AppInstance.GetCurrent().Activated += App_Activated;

            mainWindowViewModel = new MainWindowViewModel();

            ProcessedPassedFiles(activationArgs);

            newFilesReceiver = new NewFilesReceiver();
            newFilesReceiver.ListenToNewlyPassedFiles();

            mainWindow = new MainWindow(mainWindowViewModel);
            mainWindow.Closed += MainWindow_Closed;
            await mainWindow.InitializeCustomComponents();
            mainWindow.Activate();
        }

        private void App_Activated(object sender, AppActivationArguments args)
        {
            ProcessedPassedFiles(args);

            // Based on: https://github.com/microsoft/microsoft-ui-xaml/issues/7595#issuecomment-1514604263
            var windowHandle = WindowHandle.GetFromWindow(mainWindow);
            WindowManagement.ShowWindow(windowHandle, ShowWindowCommand.ShowNormal);
            WindowManagement.SetForegroundWindow(windowHandle);
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            newFilesReceiver.Dispose();
            mainWindowViewModel.Dispose();
        }

        private void ProcessedPassedFiles(AppActivationArguments args)
        {
            // NOTE: Passed files during activation are for decryption.
            if (args.Kind is ExtendedActivationKind.File
                && args.Data is IFileActivatedEventArgs fileActivatedEventArgs)
            {
                string[] targetFilePaths = new string[fileActivatedEventArgs.Files.Count];
                for (int i = 0; i < fileActivatedEventArgs.Files.Count; i++)
                {
                    targetFilePaths[i] = fileActivatedEventArgs.Files[i].Path;
                }

                mainWindowViewModel.AddNewUnsecuredFiles(targetFilePaths, FileOperationType.Decryption);
            }
        }
    }
}
