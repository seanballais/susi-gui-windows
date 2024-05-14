using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    internal class ErrorFFI
    {
        [DllImport(Constants.CoreDLLName, CharSet = CharSet.Unicode)]
        public static extern int get_last_error_message(byte[] buffer, int length);

        [DllImport(Constants.CoreDLLName)]
        public static extern int get_last_error_message_length();

        [DllImport(Constants.CoreDLLName)]
        public static extern bool has_error();
    }
}
