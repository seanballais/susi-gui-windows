using susi_gui_windows.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows.Core
{
    public static class TaskProgressExtensions
    {
        public static TaskProgress FromTaskProgressNative(TaskProgressNative nativeProgress) => nativeProgress switch
        {
            TaskProgressNative.Queued  => TaskProgress.Queued,
            TaskProgressNative.Running => TaskProgress.Running,
            TaskProgressNative.Failed  => TaskProgress.Failed,
            TaskProgressNative.Done    => TaskProgress.Done,
            _ => throw new ArgumentOutOfRangeException(nameof(nativeProgress), $"TaskProgress value not expected: {nativeProgress}"),
        };        
    }
}
