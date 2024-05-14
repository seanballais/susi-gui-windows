using System;
using System.Threading.Tasks;

namespace susi_gui_windows.Utilities
{
    internal static class Execution
    {
        public static async Task AsyncWaitUntil(Func<bool> condition)
        {
            while (!condition())
            {
                await Task.Delay(50);
            }
        }
    }
}
