// Susi
// Copyright (C) 2024  Sean Francis N.Ballais
//
// This program is free software : you can redistribute it and /or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see < http://www.gnu.org/licenses/>.
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
