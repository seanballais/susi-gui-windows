using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

using susi_gui_windows.ViewModels;
using Windows.Graphics;

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

            // Set to a strict window size.
            this.AppWindow.Resize(new SizeInt32(550, 400));

            // We don't want the window to be resizable nor maximizable.
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;

            this.viewModel = viewModel;
        }
    }
}
