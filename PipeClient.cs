using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;

using susi_gui_windows.Core;

namespace susi_gui_windows
{
    internal class PipeClient
    {
        private readonly NamedPipeClientStream pipeClient;
        private Action<string> callback;
        private Thread thread;
        private bool shouldThreadStop;

        public PipeClient(Action<string> callback)
        {
            pipeClient = new NamedPipeClientStream(
                ".",
                "susi-FD2694FA-4BB3-4ABD-8CF7-0CCCAFA32347",
                PipeDirection.In,
                PipeOptions.Asynchronous,
                TokenImpersonationLevel.Impersonation
            );
            this.callback = callback;
            shouldThreadStop = false;
        }

        public void Start()
        {
            thread = new Thread(new ThreadStart(this.RunTask));
            thread.Start();
        }

        public void Stop()
        {
            shouldThreadStop = true;
            thread.Join();
        }

        private void RunTask()
        {
            pipeClient.Connect();

            var readBytes = new List<byte>();
            while (!shouldThreadStop)
            {
                const int ReadBufferSize = 1024;
                byte[] tempBuffer = new byte[ReadBufferSize];
                int numRead = pipeClient.Read(tempBuffer, 0, ReadBufferSize);
                if (numRead == 0)
                {
                    // No more things to read from the pipe, so it's time to process
                    // any data we received.
                    if (readBytes.Count > 0)
                    {
                        string readData = Encoding.Unicode.GetString(readBytes.ToArray(), 0, readBytes.Count);
                        callback(readData);
                        readBytes.Clear();
                    }
                }
                else
                {
                    readBytes.AddRange(tempBuffer[0..numRead]);
                }
            }
        }
    }
}
