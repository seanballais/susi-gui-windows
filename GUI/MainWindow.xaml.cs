using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

using susi_gui_windows.Core;
using susi_gui_windows.ViewModels;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            this.InitializeComponent();

            this.viewModel = viewModel;
        }
    }
}
