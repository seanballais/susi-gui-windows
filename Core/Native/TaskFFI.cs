using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core.Native
{
    internal class TaskFFI
    {
        public static TaskIDWrapper queueEncryptionTask(string src_file, string password)
        {
            IntPtr ptr = queue_encryption_task(src_file, password);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Failed to queue encryption task due to a core library error.");
            }

            var taskID = Marshal.PtrToStructure<TaskIDNative>(ptr);
            return new TaskIDWrapper(taskID, ptr);
        }

        public static void dropTaskID(TaskIDWrapper taskID)
        {
            drop_task_id(taskID.Pointer);
        }

        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr queue_encryption_task(string target_file, string password);

        [DllImport(Constants.CoreDLLName)]
        public static extern IntPtr get_task_status(IntPtr id);

        [DllImport(Constants.CoreDLLName)]
        public static extern void drop_task_status(IntPtr status);

        [DllImport(Constants.CoreDLLName)]
        private static extern void drop_task_id(IntPtr id);
    }
}
