using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = FFI.has_error();
            if (hasError)
            {
                textContent.Text = $"Error: {CoreWrapper.GetLastErrorMessage()}";
            } else
            {
                textContent.Text = "No error.";
            }

            Log.Information("Testing things out");
        }

        private void startEncryption_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                string src_file = "C:/Users/sean/bitmap.png";
                string password = "heyheyheyheyhey";
                FFI.queue_encryption_task(src_file, password);
            }).Start();
        }
    }
}
