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
