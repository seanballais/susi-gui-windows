using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    public class Lib
    {
        public static void Initialize()
        {
            Native.LibFFI.init_susi_core();
        }
    }
}
