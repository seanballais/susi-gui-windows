using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace susi_gui_windows
{
    internal class PipeClient
    {
        private readonly NamedPipeClientStream pipeClient;

        public PipeClient()
        {
            pipeClient = new NamedPipeClientStream(
                ".",
                "\\\\.\\pipe\\susi-FD2694FA-4BB3-4ABD-8CF7-0CCCAFA32347",
                PipeDirection.In,
                PipeOptions.Asynchronous,
                TokenImpersonationLevel.Impersonation
            );
        }

        public void Start(Action<string> callback)
        {
            pipeClient.Connect();
            callback($"Pipeline Connected: {pipeClient.IsConnected}");
        }

        public async void Listen(Action<string> callback)
        {
            var reader = new StreamReader(pipeClient);
            while (true)
            {
                string data = await reader.ReadLineAsync();
                callback(data);
            }
        }
    }
}
