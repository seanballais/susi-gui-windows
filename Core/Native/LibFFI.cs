using System.Runtime.InteropServices;

namespace susi_gui_windows.Core.Native
{
    internal class LibFFI
    {
        [DllImport(Constants.CoreDLLName)]
        public static extern void init_susi_core();
    }
}
