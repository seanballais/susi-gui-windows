// Susi
// Copyright (C) 2024  Sean Francis N.Ballais
//
// This program is free software : you can redistribute it and /or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see < http://www.gnu.org/licenses/>.
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

        public long NumReadBytes { get { return (long)status.NumReadBytes; } }
        public long NumWrittenBytes { get { return (long)status.NumWrittenBytes; } }
        public long NumProcessedBytes { get { return (long)status.NumProcessedBytes; } }
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
