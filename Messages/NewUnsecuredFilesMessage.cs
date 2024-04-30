using CommunityToolkit.Mvvm.Messaging.Messages;

namespace susi_gui_windows.Messages
{
    internal class NewUnsecuredFilesMessage(string[] newFilePaths) : ValueChangedMessage<string[]>(newFilePaths) {}
}
