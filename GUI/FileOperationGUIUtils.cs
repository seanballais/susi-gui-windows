using Microsoft.UI.Xaml;
using susi_gui_windows.Core;

namespace susi_gui_windows.GUI
{
    internal static class FileOperationGUIUtils
    {
        public static Visibility ShouldProgressBarBeVisible(TaskProgress state)
        {
            if (state != TaskProgress.Done
                && state != TaskProgress.Failed
                && state != TaskProgress.Interrupted)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
}
