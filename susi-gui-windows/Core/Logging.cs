using susi_gui_windows.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    public sealed class Logging
    {
        public static void Info(string message)
        {
            LoggingFFI.LogInfo(message);
        }
    }
}
