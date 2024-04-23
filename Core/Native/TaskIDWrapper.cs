using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    public struct TaskIDWrapper : IDisposable
    {
        public readonly TaskIDNative MarshalledData { get; }
        public readonly IntPtr Pointer { get; }

        private bool isDisposed;

        public TaskIDWrapper(TaskIDNative MarshalledData, IntPtr Pointer)
        {
            this.MarshalledData = MarshalledData;
            this.Pointer = Pointer;
            this.isDisposed = false;
        }

        public void Dispose()
        {
            if (isDisposed) return;

            TaskFFI.dropTaskID(this);

            isDisposed = true;
        }
    }
}
