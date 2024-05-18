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
