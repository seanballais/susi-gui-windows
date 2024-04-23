using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    public struct TaskStatusWrapper : IDisposable
    {
        public readonly TaskStatusNative Status;
        public readonly IntPtr Pointer;

        private bool isDisposed;

        public TaskStatusWrapper(TaskStatusNative status, IntPtr ptr)
        {
            this.Status = status;
            this.Pointer = ptr;
        }

        public readonly nuint NumReadBytes { get { return Status.numReadBytes; } }
        public readonly nuint NumWrittenBytes { get { return Status.numWrittenBytes; } }
        public readonly bool ShouldStop { get { return Status.shouldStop; } }
        public readonly string LastError { get { return Status.lastError; } }
        public readonly TaskProgressNative Progress { get { return Status.progress; } }
        
        public void Dispose()
        {
            if (isDisposed) { return; }

            TaskFFI.DropTaskStatus(this);

            isDisposed = true;
        }
    }
}
