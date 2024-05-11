using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    internal class IncorrectPasswordFFIException : Exception
    {
        public IncorrectPasswordFFIException(string message) : base(message) { }
    }
}
