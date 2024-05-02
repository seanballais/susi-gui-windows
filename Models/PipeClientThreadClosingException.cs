using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Models
{
    internal class PipeClientThreadClosingException : Exception
    {
        public PipeClientThreadClosingException() : base("Shell extension pipe client thread is closing.") { }
    }
}
