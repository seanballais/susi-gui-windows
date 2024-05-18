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

using Microsoft.UI.Xaml;
using WinRT.Interop;

using susi_gui_windows.OS.Win32;

namespace susi_gui_windows.OS
{
    public sealed class WindowHandle : IEquatable<WindowHandle>
    {
        private readonly WindowHandleWrapper handle;

        public static WindowHandle GetFromWindow(Window window)
        {
            var wrapper = new WindowHandleWrapper(WindowNative.GetWindowHandle(window));
            return new WindowHandle(wrapper);
        }

        private WindowHandle(WindowHandleWrapper wrapper)
        {
            handle = wrapper;
        }

        public WindowHandleWrapper InternalData { get { return handle; } }

        public bool Equals(WindowHandle other)
        {
            return handle.Equals(other.handle);
        }
    }
}
