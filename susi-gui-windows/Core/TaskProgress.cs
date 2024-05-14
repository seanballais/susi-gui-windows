using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    public enum TaskProgress
    {
        Queued,
        Processing,
        Finalizing,
        Done,
        Failed,
        Interrupted
    }
}
