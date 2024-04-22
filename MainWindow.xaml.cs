using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Serilog;
using susi_gui_windows.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT;
using WinRT.Interop;

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

        private async void OnClosing(object sender, AppWindowClosingEventArgs e)
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

            Log.Information("Testing things out");
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
                textContent.Text = $"{status.numReadBytes}";
            }
        }
    }
}
