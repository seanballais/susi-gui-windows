using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

using Windows.Graphics;

using susi_gui_windows.ViewModels;
using susi_gui_windows.GUI;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace susi_gui_windows
{
    internal sealed partial class MainWindow : Window
    {
        private MainPage mainPage;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            // Set to a strict window size.
            AppWindow.Resize(new SizeInt32(550, 600));

            // We don't want the window to be resizable nor maximizable.
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;

            mainPage = new MainPage(viewModel);
            Content = mainPage;
        }

        public async Task InitializeCustomComponents()
        {
            await mainPage.InitializeCustomComponents();
        }
    }
}
