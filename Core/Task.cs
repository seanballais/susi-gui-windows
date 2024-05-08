using System;
using System.Runtime.InteropServices;
using susi_gui_windows.Core.Native;

namespace susi_gui_windows.Core
{
    public class Task : IEquatable<Task>
    {
        private TaskID id;

        public Task(TaskType type, string target_file, string password)
        {
            this.id = TaskFFI.QueueEncryptionTask(target_file, password);
        }

        public TaskID ID { get { return id; } }

        public bool Equals(Task other)
        {
            return id.Equals(other.id);
        }

        public TaskStatus GetStatus()
        {
            TaskStatus status;
            try
            {
                status = TaskFFI.GetTaskStatus(id.InternalData);
            }
            catch (NullFFIPointerException)
            {
                status = null;
            }

            return status;
        }
    }
}
