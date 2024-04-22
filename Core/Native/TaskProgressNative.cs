using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    public enum TaskProgressNative : UInt32
    {
        Queued,
        Running,
        Done,
        Failed
    }
}
