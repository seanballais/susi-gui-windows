using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;

using susi_gui_windows.Core;

namespace susi_gui_windows
{
    internal class PipeClient
    {
        private readonly NamedPipeClientStream pipeClient;
        private Action<string> callback;
        private Thread thread;

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
            thread = new Thread(new ThreadStart(this.RunTask));
        }

        public void Start()
        {
            thread.Start();
        }

        private void RunTask()
        {
            pipeClient.Connect();

            var reader = new StreamReader(pipeClient);
            while (true)
            {
                char[] buffer = new char[1024];
                int numRead = reader.ReadBlock(buffer, 0, 1023);
                Logging.Info($"Received data: {numRead}");
            }
        }
    }
}
