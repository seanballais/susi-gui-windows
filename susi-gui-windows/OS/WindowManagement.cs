using susi_gui_windows.OS.Win32;

namespace susi_gui_windows.OS
{
    internal static class WindowManagement
    {
        public static ShowWindowResult ShowWindow(WindowHandle window, ShowWindowCommand command)
        {
            bool isWindowPrevHidden = WindowManagementFFI.ShowWindow(
                window.InternalData,
                ShowWindowCommandNativeExtensions.FromShowWindowCommand(command)
            );
            if (isWindowPrevHidden)
            {
                return ShowWindowResult.PreviouslyHidden;
            }
            else
            {
                return ShowWindowResult.PreviouslyVisible;
            }
        }

        public static void SetForegroundWindow(WindowHandle window)
        {
            bool isWindowForegrounded = WindowManagementFFI.SetForegroundWindow(window.InternalData);
            if (!isWindowForegrounded)
            {
                throw new WindowForegroundingFailedException();
            }
        }
    }
}
