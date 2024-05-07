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
        public static TaskIDWrapper QueueEncryptionTask(string src_file, string password)
        {
            IntPtr ptr = queue_encryption_task(src_file, password);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Failed to queue encryption task due to a core library error.");
            }

            var taskID = Marshal.PtrToStructure<TaskIDNative>(ptr);
            return new TaskIDWrapper(taskID, ptr);
        }

        public static TaskStatusWrapper GetTaskStatus(TaskIDWrapper taskID)
        {
            var ptr = get_task_status(taskID.Pointer);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Task does not exist.");
            }

            var taskStatus = Marshal.PtrToStructure<TaskStatusNative>(ptr);
            return new TaskStatusWrapper(taskStatus, ptr);
        }

        public static void DropTaskStatus(TaskStatusWrapper taskStatus)
        {
            drop_task_status(taskStatus.Pointer);
        }

        public static void DropTaskID(TaskIDWrapper taskID)
        {
            drop_task_id(taskID.Pointer);
        }

        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr queue_encryption_task(string target_file, string password);

        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr get_task_status(IntPtr id);

        [DllImport(Constants.CoreDLLName)]
        private static extern void drop_task_status(IntPtr status);

        [DllImport(Constants.CoreDLLName)]
        private static extern void drop_task_id(IntPtr id);
    }
}
