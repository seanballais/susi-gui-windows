using susi_gui_windows.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    internal class Validation
    {
        public static bool IsPasswordCorrectForFile(string filePath, string password)
        {
            bool isCorrect = ValidationFFI.IsPasswordCorrectForFile(filePath, password);
            if (Error.HasError())
            {
                throw new IncorrectPasswordFFIException(Error.GetLastErrorMessage());
            }

            return isCorrect;
        }
    }
}
