using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    public class TaskException : Exception
    {
        public TaskException(string message) : base(message) {}
    }
}
