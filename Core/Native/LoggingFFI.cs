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
