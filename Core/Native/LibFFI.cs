using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    internal class LibFFI
    {
        [DllImport(Constants.CoreDLLName)]
        public static extern void init_susi_core();
    }
}
