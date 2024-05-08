using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using susi_gui_windows.Core;
using susi_gui_windows.ViewModels;

namespace susi_gui_windows.GUI
{
    internal static class FileOperationGUIUtils
    {
        public static string CreateInfoBarTitle(TaskProgress state)
        {
            if (state == TaskProgress.Done
                || state == TaskProgress.Failed
                || state == TaskProgress.Interrupted)
            {
                return $"{state}.";
            }
            else
            {
                return null;
            }
        }

        public static InfoBarSeverity GetInfoBarSeverity(TaskProgress state)
        {
            if (state == TaskProgress.Done)
            {
                return InfoBarSeverity.Success;
            }
            else if (state == TaskProgress.Interrupted)
            {
                return InfoBarSeverity.Warning;
            }
            else if (state == TaskProgress.Failed)
            {
                return InfoBarSeverity.Error;
            }
            else
            {
                return InfoBarSeverity.Informational;
            }
        }

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

        public static bool ShouldInfoBarBeOpen(TaskProgress state)
        {
            if (state == TaskProgress.Done
                || state == TaskProgress.Failed
                || state == TaskProgress.Interrupted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
