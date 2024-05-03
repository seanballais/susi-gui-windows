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
