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
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using susi_gui_windows.Core;

namespace susi_gui_windows.ViewModels
{
    internal class ShellExtensionPipeClient
    {
        private NamedPipeClientStream pipeClient;
        private Action<string[]> callback;
        private Thread thread;
        private bool shouldThreadStop;

        public ShellExtensionPipeClient(Action<string[]> callback)
        {
            this.callback = callback;
            shouldThreadStop = false;
        }

        public void Start()
        {
            thread = new Thread(new ThreadStart(RunTask));
            thread.Start();
        }

        public void Stop()
        {
            shouldThreadStop = true;
            thread.Join();
        }

        private void RunTask()
        {
            while (!shouldThreadStop)
            {
                try
                {
                    ConnectToPipe();
                }
                catch (PipeClientThreadClosingException)
                {
                    // The thread is already shutting down. We better close now.
                    pipeClient.Dispose();
                    break;
                }

                bool checkingForMoreData = true;
                var readBytes = new List<byte>();
                const int ReadBufferSize = 1024;
                byte[] tempBuffer = new byte[ReadBufferSize];
                while (checkingForMoreData)
                {
                    int numRead = pipeClient.Read(tempBuffer, 0, ReadBufferSize);
                    if (numRead == 0)
                    {
                        // No more things to read from the pipe, so it's time to process
                        // any data we received.
                        checkingForMoreData = false;

                        if (readBytes.Count > 0)
                        {
                            string readData = Encoding.Unicode.GetString(readBytes.ToArray(), 0, readBytes.Count);
                            string[] items = readData.Split('|', StringSplitOptions.RemoveEmptyEntries);

                            callback(items);
                        }
                    }
                    else
                    {
                        readBytes.AddRange(tempBuffer[0..numRead]);
                    }
                }

                // At this point, we shouldn't be receiving any additional files, and the shell extension's
                // IPC server will close as well. So, we should disconnect and just attempt to recreate another
                // pipe instance. We will get reconnected when there is a new IPC server that pops up when the
                // shell extension is called again.
                //
                // However, if we are closing the thread in the middle of receiving data, then this should clean
                // up our pipe.
                pipeClient.Dispose();
            }
        }

        private void ConnectToPipe()
        {
            pipeClient = new NamedPipeClientStream(
                ".",
                "susi-FD2694FA-4BB3-4ABD-8CF7-0CCCAFA32347",
                PipeDirection.In,
                PipeOptions.Asynchronous,
                TokenImpersonationLevel.Impersonation
            );
            while (!pipeClient.IsConnected && !shouldThreadStop)
            {
                try
                {
                    pipeClient.Connect(50);
                }
                // Ignore the timeout exception. Wala tayong pake kasi uulit-uulitin lang
                // nating magtry kumonnect kahit kung wala na siya sa piling mo. Malay
                // natin bumalik pa. Try lang natin. Tapos, if it doesn't work then, at
                // least we tried.
                //
                // See https://twitter.com/sineclips/status/1593423987375120385
                // for the Filipino meme reference, and contact any Filipino programmer for
                // the cultural translation into English.
                catch (TimeoutException) { }
            }

            if (shouldThreadStop)
            {
                throw new PipeClientThreadClosingException();
            }
        }
    }
}
