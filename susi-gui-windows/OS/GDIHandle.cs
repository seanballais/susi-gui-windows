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

using susi_gui_windows.OS.Win32;

namespace susi_gui_windows.OS
{
    internal sealed class GDIHandle : IEquatable<GDIHandle>, IDisposable
    {
        private readonly GDIHandleWrapper handle;

        private bool isDisposed;

        public GDIHandle(GDIHandleWrapper wrapper)
        {
            handle = wrapper;
            isDisposed = false;
        }

        public static implicit operator GDIHandle(GDIHandleWrapper wrapper) => new GDIHandle(wrapper);

        public GDIHandleWrapper InternalData { get { return handle; } }

        public void Dispose()
        {
            if (isDisposed) { return; }

            GDIFFI.DeleteGDIObject(handle);

            isDisposed = true;
        }

        public bool Equals(GDIHandle other)
        {
            return handle.Equals(other.handle);
        }
    }
}
