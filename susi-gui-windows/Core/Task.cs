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
using System.Runtime.InteropServices;
using susi_gui_windows.Core.Native;

namespace susi_gui_windows.Core
{
    public class Task : IEquatable<Task>
    {
        private TaskID id;

        public Task(TaskType type, string target_file, string password)
        {
            if (type == TaskType.Encryption)
            {
                id = TaskFFI.QueueEncryptionTask(target_file, password);
            }
            else if (type == TaskType.Decryption)
            {
                id = TaskFFI.QueueDecryptionTask(target_file, password);
            }
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
