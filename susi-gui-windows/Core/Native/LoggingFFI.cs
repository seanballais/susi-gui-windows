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

namespace susi_gui_windows.Core.Native
{
    internal class LoggingFFI
    {
        public static void LogInfo(string message)
        {
            byte[] utf8EncodedMessage = Encoding.UTF8.GetBytes(message);
            log_info(utf8EncodedMessage);
        }

        // We're using a byte[] here instead of a string since we should be passing a UTF-8 string.
        [DllImport(Constants.CoreDLLName)]
        private static extern void log_info(byte[] message);
    }
}
