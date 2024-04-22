using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    internal readonly struct TaskIDWrapper
    {
        public readonly TaskIDNative MarshalledData { get; }
        public readonly IntPtr Pointer { get; }

        public TaskIDWrapper(TaskIDNative MarshalledData, IntPtr Pointer)
        {
            this.MarshalledData = MarshalledData;
            this.Pointer = Pointer;
        }
    }
}
