using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

using susi_gui_windows.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows
{
    internal sealed partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            this.InitializeComponent();

            // We don't want the window to be resizable.
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;

            this.viewModel = viewModel;
        }
    }
}
