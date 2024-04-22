using susi_gui_windows.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    public class TaskID : IEquatable<TaskID>, IDisposable
    {
        private readonly IntPtr ptr;
        private readonly TaskIDNative internalID;

        public TaskID(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) { throw new ArgumentNullException(nameof(ptr)); }

            this.internalID = Marshal.PtrToStructure<TaskIDNative>(ptr);
            this.ptr = ptr;
        }

        public bool Equals(TaskID other)
        {
            return internalID == other.internalID;
        }

        public void Dispose()
        {
            Native.TaskFFI.drop_task_id(ptr);
        }
    }
}
