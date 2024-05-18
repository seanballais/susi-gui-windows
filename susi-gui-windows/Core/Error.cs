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
using System.Runtime.InteropServices;
using System.Text;

namespace susi_gui_windows.Core
{
    public class Error
    {
        public static string GetLastErrorMessage()
        {
            // Based on: https://www.michaelfbryan.com/rust-ffi-guide/errors/return_types.html
            int errorLength = Native.ErrorFFI.get_last_error_message_length();

            if (errorLength == 0)
            {
                return string.Empty;
            }

            byte[] buffer = new byte[errorLength];
            int ret = Native.ErrorFFI.get_last_error_message(buffer, buffer.Length);
            if (ret <= 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public static bool HasError()
        {
            return Native.ErrorFFI.has_error();
        }
    }
}
