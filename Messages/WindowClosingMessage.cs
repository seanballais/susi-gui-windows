using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;
using susi_gui_windows.GUI;

namespace susi_gui_windows.Messages
{
    internal class WindowClosingMessage : ValueChangedMessage<WindowState>
    {
        public WindowClosingMessage(WindowState windowState) : base(windowState) {}

        public Window Window { get { return Value.Window; } }
        public WindowStatus WindowStatus { get { return Value.WindowStatus; } }
    }
}
