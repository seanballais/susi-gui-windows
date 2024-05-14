using susi_gui_windows.Core.Native;

namespace susi_gui_windows.Core
{
    internal class Verification
    {
        public static bool IsPasswordCorrectForFile(string filePath, string password)
        {
            return VerificationFFI.IsPasswordCorrectForFile(filePath, password);            
        }
    }
}
