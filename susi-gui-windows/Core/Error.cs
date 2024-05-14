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
