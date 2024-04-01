using System;

using System.Runtime.InteropServices;

namespace susi_gui_windows
{
    internal class FFI
    {
        const string __DLL_NAME = "susi.dll";

        [DllImport(__DLL_NAME)]
        public static extern void init_susi_core();

        [DllImport(__DLL_NAME)]
        public static extern void queue_encryption_task(string src_file, string password);

        [DllImport(__DLL_NAME)]
        public static extern bool has_error();

        [DllImport(__DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern Int32 get_last_error_message(byte[] buffer, Int32 length);

        [DllImport(__DLL_NAME)]
        public static extern Int32 get_last_error_message_length();

        [DllImport(__DLL_NAME)]
        public static extern void close_worker_pool();
    }
}
