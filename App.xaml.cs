using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using susi_gui_windows.Core;
using susi_gui_windows.GUI;
using susi_gui_windows.Messages;
using susi_gui_windows.OS;

namespace susi_gui_windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;
        private PipeClient pipeClient;

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
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Based on: https://gist.github.com/andrewleader/5adc742fe15b06576c1973ea6e999552
            var activationArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            var instance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("susi-sfb");

            using (StreamWriter outputFile = new StreamWriter("C:/Users/sean/log.txt", true))
            {
                outputFile.WriteLine($"test1");
            }

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
            Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().Activated += App_Activated;

            mainWindow = new MainWindow();
            mainWindow.Closed += MainWindow_Closed;
            mainWindow.Activate();

            pipeClient = new PipeClient(mainWindow.SetText);
            pipeClient.Start();
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            pipeClient.Stop();
        }

        private void App_Activated(object sender, AppActivationArguments args)
        {
            // Based on: https://github.com/microsoft/microsoft-ui-xaml/issues/7595#issuecomment-1514604263
            var windowHandle = WindowHandle.GetFromWindow(mainWindow);
            WindowManagement.ShowWindow(windowHandle, ShowWindowCommand.ShowNormal);
            WindowManagement.SetForegroundWindow(windowHandle);
        }
    }
}
