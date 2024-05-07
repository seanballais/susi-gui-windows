using susi_gui_windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.GUI
{
    internal static class TextUtils
    {
        public static string SetFileOperationTitle(string fileName, FileOperationType type)
        {
            if (type == FileOperationType.Encryption)
            {
                return $"Locking {fileName}";
            }
            else
            {
                return $"Unlocking {fileName}";
            }
        }
    }
}
