using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public class TaskIDNative
    {
        public ulong upperID;
        public ulong lowerID;
    }
}
