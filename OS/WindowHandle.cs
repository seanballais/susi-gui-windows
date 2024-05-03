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
