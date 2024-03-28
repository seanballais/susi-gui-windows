using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows
{
    internal class CoreWrapper
    {
        public static String GetLastErrorMessage()
        {
            // Based on: https://www.michaelfbryan.com/rust-ffi-guide/errors/return_types.html
            Int32 errorLength = FFI.get_last_error_message_length();

            if (errorLength == 0)
            {
                return String.Empty;
            }

            byte[] buffer = new byte[errorLength];
            int ret = FFI.get_last_error_message(buffer, buffer.Length);
            if (ret <= 0)
            {
                return String.Empty;
            }

            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public static String GetLogDir()
        {
            Int32 logDirLength = FFI.get_log_dir_length();
            if (logDirLength == 0)
            {
                return String.Empty;
            }

            byte[] buffer = new byte[logDirLength];
            int ret = FFI.get_log_dir(buffer, buffer.Length);
            if (ret <= 0)
            {
                return String.Empty;
            }

            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }
    }
}
