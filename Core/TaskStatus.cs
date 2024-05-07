using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using susi_gui_windows.Core.Native;

namespace susi_gui_windows.Core
{
    public sealed class TaskStatus : IEquatable<TaskStatus>
    {
        private readonly TaskStatusWrapper status;

        public TaskStatus(TaskStatusWrapper wrapper)
        {
            status = wrapper;
        }

        public static implicit operator TaskStatus(TaskStatusWrapper status) => new TaskStatus(status);

        public long NumReadBytes { get { return (long) status.NumReadBytes; } }
        public long NumWrittenBytes { get { return (long) status.NumWrittenBytes; } }
        public bool ShouldStop { get { return status.ShouldStop; } }
        public string LastError { get { return status.LastError; } }
        public TaskProgress Progress { get { return TaskProgressExtensions.FromTaskProgressNative(status.Progress); } }

        ~TaskStatus()
        {
            status.Dispose();
        }

        public bool Equals(TaskStatus other)
        {
            return status.Equals(other.status);
        }
    }
}
