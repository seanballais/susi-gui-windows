using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using susi_gui_windows.Core;
using susi_gui_windows.Messages;

namespace susi_gui_windows.Models
{
    internal class TaskRepository : IDisposable
    {
        private List<string> unsecuredFiles;
        private ShellExtensionPipeClient pipeNewFilesClient;

        // Stores info on whether a client is aware that the data we have here
        // has been updated. Clients are identified by their GUID.
        private Dictionary<Guid, TaskRepositoryClientDataState> clientDataState;

        public TaskRepository()
        {
            unsecuredFiles = [];
            pipeNewFilesClient = new ShellExtensionPipeClient(PipeNewFilesClient_Callback);
            clientDataState = new Dictionary<Guid, TaskRepositoryClientDataState>();
        }

        public void Dispose()
        {
            pipeNewFilesClient.Stop();
        }

        public void ListenToNewlyPassedFiles()
        {
            pipeNewFilesClient.Start();
        }

        public Guid ConnectClient()
        {
            Guid clientGUID = Guid.NewGuid();
            clientDataState.Add(clientGUID, TaskRepositoryClientDataState.NeedsUpdating);
            return clientGUID;
        }

        public bool DoesClientDataNeedUpdating(Guid clientGUID)
        {
            if (clientDataState.ContainsKey(clientGUID))
            {
                return clientDataState[clientGUID] == TaskRepositoryClientDataState.NeedsUpdating;
            } else
            {
                throw new UnknownTaskRepositoryClientException(clientGUID);
            }
        }

        public List<string> GetUnsecuredFiles(Guid clientGUID)
        {
            SetClientDataStateToUpdated(clientGUID);
            return unsecuredFiles;
        }

        private void PipeNewFilesClient_Callback(string[] items)
        {
            lock (unsecuredFiles)
            {
                lock (clientDataState)
                {
                    unsecuredFiles.AddRange(items);
                    foreach (Guid clientGUID in clientDataState.Keys)
                    {
                        clientDataState[clientGUID] = TaskRepositoryClientDataState.NeedsUpdating;
                    }
                }
            }
        }

        private void SetClientDataStateToUpdated(Guid clientGUID)
        {
            SetClientDataState(clientGUID, TaskRepositoryClientDataState.Updated);
        }

        private void SetClientDataStateToNeedsUpdating(Guid clientGUID)
        {
            SetClientDataState(clientGUID, TaskRepositoryClientDataState.NeedsUpdating);
        }

        private void SetClientDataState(Guid clientGUID, TaskRepositoryClientDataState state)
        {
            if (clientDataState.ContainsKey(clientGUID))
            {
                clientDataState[clientGUID] = state;
            }
            else
            {
                throw new UnknownTaskRepositoryClientException(clientGUID);
            }
        }
    }
}
