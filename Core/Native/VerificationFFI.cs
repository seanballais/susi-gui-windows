using System.Runtime.InteropServices;
using System.Text;

namespace susi_gui_windows.Core.Native
{
    internal class VerificationFFI
    {
        public static bool IsPasswordCorrectForFile(string filePath, string password)
        {
            byte[] utf8FilePath = Encoding.UTF8.GetBytes(filePath);
            byte[] utf8Password = Encoding.UTF8.GetBytes(password);
            return is_password_correct_for_file(utf8FilePath, utf8Password);
        }

        // We're using a byte[] here instead of a string since we should be passing a UTF-8 string.
        [DllImport(Constants.CoreDLLName)]
        private static extern bool is_password_correct_for_file(byte[] target_file, byte[] password);
    }
}
