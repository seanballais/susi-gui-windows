using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using susi_gui_windows.Core;
using susi_gui_windows.Messages;

namespace susi_gui_windows.Models
{
    internal class TaskRepository : IDisposable
    {
        private List<string> unsecuredFiles;
        private ShellExtensionPipeClient pipeNewFilesClient;

        public TaskRepository()
        {
            unsecuredFiles = [];
            pipeNewFilesClient = new ShellExtensionPipeClient(PipeNewFilesClient_Callback);
        }

        public void Dispose()
        {
            pipeNewFilesClient.Stop();
        }

        public void ListenToNewlyPassedFiles()
        {
            pipeNewFilesClient.Start();
        }

        private void PipeNewFilesClient_Callback(string[] items)
        {
            unsecuredFiles.AddRange(items);
            WeakReferenceMessenger.Default.Send(new NewUnsecuredFilesMessage(items));
        }
    }
}
