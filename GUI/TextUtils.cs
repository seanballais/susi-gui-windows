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

        public static string CreateOperationProgressString(long currValue, long maxValue)
        {
            return $"{GetSizeString(currValue)} of {GetSizeString(maxValue)}";
        }

        /// <summary>
        /// Creates the size string from a number of bytes.
        /// </summary>
        /// <param name="size">Number of bytes that will be converted into a different unit.</param>
        /// <returns></returns>
        public static string GetSizeString(long size)
        {
            // Let's follow the IEC standard (KiB, MiB, etc.) since it seems that people think of
            // a kilobyte as 1,024 bytes and so on.
            long divisor = 1;
            string unit = "B";
            if (size >= 1099511627776)
            {
                divisor = 1099511627776;
                unit = "TiB";
            }
            else if (size >= 1073741824)
            {
                divisor = 1073741824;
                unit = "GiB";
            }
            else if (size >= 1048576)
            {
                divisor = 1048576;
                unit = "MiB";
            }
            else if (size >= 1024)
            {
                divisor = 1024;
                unit = "KiB";
            }

            double sizeInNewUnit = (double) size / divisor;
            return $"{sizeInNewUnit:F2} {unit:F2}";
        }
    }
}
