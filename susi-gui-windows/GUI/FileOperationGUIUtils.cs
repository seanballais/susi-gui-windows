// Susi
// Copyright (C) 2024  Sean Francis N.Ballais
//
// This program is free software : you can redistribute it and /or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see < http://www.gnu.org/licenses/>.
using CommunityToolkit.Mvvm.Input;
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

        public static Visibility ShouldClearButtonBeVisible(TaskProgress state)
        {
            if (state == TaskProgress.Done
                || state == TaskProgress.Failed
                || state == TaskProgress.Interrupted)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
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
