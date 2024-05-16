namespace susi_gui_windows.Core
{
    public class Lib
    {
        public static void Initialize()
        {
            Native.LibFFI.init_susi_core();
        }
    }
}
