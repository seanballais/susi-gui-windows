using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

using susi_gui_windows.Core;
using susi_gui_windows.GUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow appWindow;
        private Core.Task task;
        private string arguments;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void SetText(string str)
        {
            arguments = str;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            textContent.Text = arguments;
        }

        private void startEncryption_Click(object sender, RoutedEventArgs e)
        {
            string src_file = "C:/Users/sean/Packages/Shared Development Packages/All/Test Sized Files/200MB.zip";
            string password = "heyheyheyheyhey";
            task = new Core.Task(TaskType.Encryption, src_file, password);
        }

        private void getNumReadBytes_Click(object sender, RoutedEventArgs e)
        {
            if (task != null)
            {
                var status = task.GetStatus();
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
