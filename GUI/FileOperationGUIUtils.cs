using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using susi_gui_windows.Core;
using susi_gui_windows.ViewModels;

namespace susi_gui_windows.GUI
{
    internal static class FileOperationGUIUtils
    {
        public static string CreateFileOperationTitle(string fileName, FileOperationType type, TaskProgress status)
        {
            string rootVerb;
            if (type == FileOperationType.Encryption)
            {
                rootVerb = "Lock";
            }
            else
            {
                rootVerb = "Unlock";
            }

            string adverb = "";
            if (status == TaskProgress.Queued)
            {
                adverb = "Preparing to ";
            }
            else if (status == TaskProgress.Processing)
            {
                // We don't need an adverb here, but
                // we do need to change the tense of our verb.
                rootVerb = $"{rootVerb}ing";
            }
            else if (status == TaskProgress.Finalizing)
            {
                adverb = "Finalizing ";
                rootVerb = $"{rootVerb}ing";
            }
            else if (status == TaskProgress.Done)
            {
                // Just like when we are processing a task, we
                // don't need an adverb here, but we do need to
                // change the tense of our verb.
                rootVerb = $"{rootVerb}ed";
            }
            else if (status == TaskProgress.Failed)
            {
                adverb = "Failed to ";
            }
            else
            {
                adverb = "Stopped ";
                rootVerb = $"{rootVerb}ing";
            }

            return $"{adverb}{rootVerb} {fileName}";
        }

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

        public static string CreateOperationProgressString(long currValue, long maxValue)
        {
            return $"{TextUtils.GetSizeString(currValue)} of {TextUtils.GetSizeString(maxValue)}";
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
            if (state == TaskProgress.Failed)
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
