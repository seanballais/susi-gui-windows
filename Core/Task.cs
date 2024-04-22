using System;
using System.Runtime.InteropServices;
using susi_gui_windows.Core.Native;

namespace susi_gui_windows.Core
{
    public class Task
    {
        private TaskIDWrapper id;

        public Task(TaskType type, string target_file, string password)
        {
            this.id = TaskFFI.queueEncryptionTask(target_file, password);
        }

        // TODO: Use a public access modifier here after making a C# wrapper for TaskStatusNative.
        internal Native.TaskStatusNative GetStatus()
        {
            var ptr = Native.TaskFFI.get_task_status(id.Pointer);
            return Marshal.PtrToStructure<TaskStatusNative>(ptr);
        }
    }
}
