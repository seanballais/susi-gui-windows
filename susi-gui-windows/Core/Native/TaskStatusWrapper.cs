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
        public readonly nuint NumProcessedBytes { get { return Status.numProcessedBytes; } }
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
