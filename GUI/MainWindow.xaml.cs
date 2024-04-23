using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

using susi_gui_windows.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;
        private Core.Task _task;

        public MainWindow()
        {
            this.InitializeComponent();

            this.Closed += OnClosed;

            this._appWindow = GetAppWindowForCurrentWindow();
            this._appWindow.Closing += OnClosing;
        }

        private void OnClosed(object sender, WindowEventArgs e) { }

        private void OnClosing(object sender, AppWindowClosingEventArgs e)
        {
            this.Close();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error.HasError())
            {
                textContent.Text = $"Error: {Error.GetLastErrorMessage()}";
            }
            else
            {
                textContent.Text = "No error.";
            }
        }

        private void startEncryption_Click(object sender, RoutedEventArgs e)
        {
            string src_file = "C:/Users/sean/Packages/Shared Development Packages/All/Test Sized Files/200MB.zip";
            string password = "heyheyheyheyhey";
            _task = new Core.Task(TaskType.Encryption, src_file, password);
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(myWndId);
        }

        private void getNumReadBytes_Click(object sender, RoutedEventArgs e)
        {
            if (_task != null)
            {
                var status = _task.GetStatus();
                if (status == null)
                {
                    textContent.Text = "None";
                } else
                {
                    textContent.Text = $"{status.Progress} | Read Bytes: {status.NumReadBytes} | Error: {status.LastError}";
                }
            }
        }
    }
}
