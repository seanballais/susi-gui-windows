using susi_gui_windows.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    public sealed class TaskID : IEquatable<TaskID>
    {
        private readonly TaskIDWrapper id;

        public TaskID(TaskIDWrapper wrapper)
        {
            id = wrapper;
        }

        public static implicit operator TaskID(TaskIDWrapper id) => new TaskID(id);


        ~TaskID()
        { 
           id.Dispose();
        }

        public TaskIDWrapper InternalData { get { return id; } }
        
        public bool Equals(TaskID other)
        {
            return id.Equals(other.id);
        }
    }
}
