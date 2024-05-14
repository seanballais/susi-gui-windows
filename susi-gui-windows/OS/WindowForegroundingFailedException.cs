using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.OS
{
    public class WindowForegroundingFailedException : Exception
    {
        public WindowForegroundingFailedException() : base("Failed to foreground window.") {}
    }
}
