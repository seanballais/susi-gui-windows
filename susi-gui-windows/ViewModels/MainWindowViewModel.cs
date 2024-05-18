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
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;

using susi_gui_windows.Core;
using susi_gui_windows.GUI;
using susi_gui_windows.Messages;
using susi_gui_windows.Utilities;

namespace susi_gui_windows.ViewModels
{
    internal partial class MainWindowViewModel : ObservableRecipient, IDisposable
    {
        private ObservableCollection<FileOperation> fileOperations;
        private RangeObservableCollection<TargetFile> unsecuredFiles;
        private ResourcePadlock resourcePadlock;

        private readonly DispatcherQueue dispatcherQueue;
        private readonly DispatcherQueueTimer fileOperationsStatusTimer;

        public MainWindowViewModel()
        {
            fileOperations = new ObservableCollection<FileOperation>();
            unsecuredFiles = new RangeObservableCollection<TargetFile>();
            resourcePadlock = new ResourcePadlock();

            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            fileOperationsStatusTimer = dispatcherQueue.CreateTimer();

            fileOperationsStatusTimer.Interval = TimeSpan.FromMilliseconds(16);
            fileOperationsStatusTimer.Tick += (s, e) =>
            {
                if (resourcePadlock.Lock(fileOperations) == ResourcePadlockStatus.Locked)
                {
                    foreach (FileOperation operation in fileOperations)
                    {
                        operation.UpdateStatus();
                    }

                    resourcePadlock.Unlock(fileOperations);
                }
            };
            fileOperationsStatusTimer.Start();

            WeakReferenceMessenger.Default.Register<NewUnsecuredFilesMessage>(this, (r, m) =>
            {
                // NOTE: Files received from IPC are for encryption.
                var recipient = (MainWindowViewModel) r;
                recipient.NewUnsecuredFilesMessageCallback(m);
            });
        }
        public void Dispose()
        {
            fileOperationsStatusTimer.Stop();
        }

        public ObservableCollection<FileOperation> FileOperations { get { return fileOperations; } }
        public RangeObservableCollection<TargetFile> UnsecuredFiles { get { return unsecuredFiles; } }

        public void AddFileOperation(TargetFile targetFile, string password)
        {
            fileOperations.Add(
                new FileOperation(targetFile.FilePath, targetFile.OperationType, password, this));
        }

        public void AddNewUnsecuredFiles(string[] targetFilePaths, FileOperationType type)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                lock (unsecuredFiles)
                {
                    List<TargetFile> newUnsecuredFiles = [];
                    foreach (string path in targetFilePaths)
                    {
                        var targetFile = new TargetFile(path, type);
                        newUnsecuredFiles.Add(targetFile);
                    }
                    unsecuredFiles.AddRange(newUnsecuredFiles);
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanRemoveFileOperation))]
        public void RemoveFileOperation(FileOperation operation)
        {
            fileOperations.Remove(operation);
        }

        public bool HasOngoingOperations()
        {
            foreach (var operation in fileOperations)
            {
                if (operation.State == TaskProgress.Queued
                    || operation.State == TaskProgress.Processing
                    || operation.State == TaskProgress.Finalizing)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasQueuedFiles()
        {
            return unsecuredFiles.Count > 0;
        }

        private void NewUnsecuredFilesMessageCallback(NewUnsecuredFilesMessage message)
        {
            string[] newUnsecuredFilePaths = message.Value;
            AddNewUnsecuredFiles(newUnsecuredFilePaths, FileOperationType.Encryption);
        }

        private bool CanRemoveFileOperation(FileOperation operation)
        {
            return operation.State == TaskProgress.Done
                || operation.State == TaskProgress.Interrupted
                || operation.State == TaskProgress.Failed;
        }
    }
}
