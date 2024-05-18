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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace susi_gui_windows.Utilities
{
    /// <summary>
    /// A padlock for a resource to ensure that a resource is only used once. This uses
    /// Interlocked underneath. As such, we can't just lock out a resource without
    /// modifying, which do not want to do sometimes. To resolve that, all resources that
    /// will be locked will be given a representative integer we will use for locking.
    /// </summary>
    internal class ResourcePadlock
    {
        private Dictionary<object, int> resourceFlags;

        public ResourcePadlock()
        {
            resourceFlags = new Dictionary<object, int>();
        }

        public ResourcePadlockStatus Lock(object resource)
        {
            lock (resourceFlags)
            {
                // NOTE: Default value of an int is 0.
                ref int flag = ref CollectionsMarshal.GetValueRefOrAddDefault(resourceFlags, resource, out bool exists);

                if (Interlocked.Exchange(ref flag, 1) == 0)
                {
                    return ResourcePadlockStatus.Locked;
                } else
                {
                    return ResourcePadlockStatus.UnableToLock;
                }
            }
        }

        public void Unlock(object resource)
        {
            lock (resourceFlags)
            {
                ref int flag = ref CollectionsMarshal.GetValueRefOrNullRef(resourceFlags, resource);
                if (Unsafe.IsNullRef(ref flag))
                {
                    throw new ArgumentNullException("resource");
                }

                Interlocked.Exchange(ref flag, 0);
            }
        }

        public bool IsResourceLocked(object resource)
        {
            lock (resourceFlags)
            {
                ref int flag = ref CollectionsMarshal.GetValueRefOrNullRef(resourceFlags, resource);
                if (Unsafe.IsNullRef(ref flag))
                {
                    return false;
                }

                // NOTE: 1 means resource is locked, and 0 means that the resource is unlocked.
                if (Interlocked.Exchange(ref flag, 1) == 1)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
    }
}
