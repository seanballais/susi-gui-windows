using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using susi_gui_windows.Core;
using susi_gui_windows.Messages;

namespace susi_gui_windows.ViewModels
{
    internal class NewFilesReceiver : IDisposable
    {
        private ShellExtensionPipeClient pipeNewFilesClient;

        public NewFilesReceiver()
        {
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
            WeakReferenceMessenger.Default.Send(new NewUnsecuredFilesMessage(items));
        }
    }
}
