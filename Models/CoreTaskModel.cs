using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using susi_gui_windows.Core;

namespace susi_gui_windows.Models
{
    internal class CoreTaskModel : INotifyPropertyChanged
    {
        // We need to mention Core.Task instead of Task to ensure that readers
        // know that we are talking about our core library's Task, instead of
        // the one that is of C#'s.
        private readonly Core.Task task;
        private Core.TaskStatus status;
        private DispatcherTimer dispatcherTimer;

        public CoreTaskModel(Core.Task task)
        {
            this.task = task;
            
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 16);
            dispatcherTimer.Start();
        }

        public nuint NumReadBytes { get { return status.NumReadBytes; } }
        public nuint NumWrittenBytes { get { return status.NumWrittenBytes; } }
        public string LastError { get { return status.LastError; } }
        public TaskProgress Progress { get { return status.Progress; } }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            status = task.GetStatus();
            OnPropertyChanged();
        }
    }
}
