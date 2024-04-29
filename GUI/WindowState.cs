using Microsoft.UI.Xaml;

namespace susi_gui_windows.GUI
{
    internal class WindowState
    {
        private readonly Window window;
        private readonly WindowStatus windowStatus;

        public WindowState(Window window, WindowStatus windowStatus)
        {
            this.window = window;
            this.windowStatus = windowStatus;
        }

        public Window Window { get { return window; } }
        public WindowStatus WindowStatus { get { return windowStatus; } }
    }
}
