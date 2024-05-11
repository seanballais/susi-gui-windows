using System;
using System.Runtime.InteropServices;
using System.Text;

namespace susi_gui_windows.Core.Native
{
    internal class TaskFFI
    {
        public static TaskIDWrapper QueueEncryptionTask(string srcFile, string password)
        {
            byte[] utf8EncodedSrcFile = Encoding.UTF8.GetBytes(srcFile);
            byte[] utf8EncodedPassword = Encoding.UTF8.GetBytes(password);

            IntPtr ptr = queue_encryption_task(utf8EncodedSrcFile, utf8EncodedPassword);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Failed to queue encryption task due to a core library error.");
            }

            var taskID = Marshal.PtrToStructure<TaskIDNative>(ptr);
            return new TaskIDWrapper(taskID, ptr);
        }

        public static TaskIDWrapper QueueDecryptionTask(string srcFile, string password)
        {
            byte[] utf8EncodedSrcFile = Encoding.UTF8.GetBytes(srcFile);
            byte[] utf8EncodedPassword = Encoding.UTF8.GetBytes(password);

            IntPtr ptr = queue_decryption_task(utf8EncodedSrcFile, utf8EncodedPassword);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Failed to queue decryption task due to a core library error.");
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

        // We're using a byte[] here instead of a string since we should be passing a UTF-8 string.
        [DllImport(Constants.CoreDLLName, CharSet = )]
        private static extern IntPtr queue_encryption_task(byte[] target_file, byte[] password);

        // We're using a byte[] here instead of a string since we should be passing a UTF-8 string.
        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr queue_decryption_task(byte[] target_file, byte[] password);

        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr get_task_status(IntPtr id);

        [DllImport(Constants.CoreDLLName)]
        private static extern void drop_task_status(IntPtr status);

        [DllImport(Constants.CoreDLLName)]
        private static extern void drop_task_id(IntPtr id);
    }
}
