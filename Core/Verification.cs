using susi_gui_windows.Core.Native;

namespace susi_gui_windows.Core
{
    internal class Verification
    {
        public static bool IsPasswordCorrectForFile(string filePath, string password)
        {
            bool isCorrect = VerificationFFI.IsPasswordCorrectForFile(filePath, password);
            if (Error.HasError())
            {
                throw new PasswordVerificationException(Error.GetLastErrorMessage());
            }

            return isCorrect;
        }
    }
}
