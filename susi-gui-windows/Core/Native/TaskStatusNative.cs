using System.Runtime.InteropServices;

namespace susi_gui_windows.Core.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TaskStatusNative
    {
        public nuint numReadBytes;
        public nuint numWrittenBytes;
        public nuint numProcessedBytes;
        public bool shouldStop;
        [MarshalAs(UnmanagedType.LPUTF8Str)] public string lastError;
        public TaskProgressNative progress;
    }
}
