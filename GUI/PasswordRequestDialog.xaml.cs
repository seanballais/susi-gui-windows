using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using susi_gui_windows.Core;
using susi_gui_windows.Utilities;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace susi_gui_windows.GUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class PasswordRequestDialog : ContentDialog
    {
        private TargetFile targetFile;

        private Action primaryButtonAction;
        private ResourcePadlock resourcePadlock;

        public PasswordRequestDialog(XamlRoot xamlRoot)
        {
            InitializeComponent();
            XamlRoot = xamlRoot;

            // We need to set the PrimaryButtonCommand here, since it doesn't
            // get automatically set when we set it via binding it in the
            // XAML file.
            PrimaryButtonCommand = RunPrimaryButtonActionCommand;
            IsPrimaryButtonEnabled = CanRunPrimaryButtonAction();
            PrimaryButtonCommand.CanExecuteChanged += (_, _) =>
            {
                IsPrimaryButtonEnabled = CanRunPrimaryButtonAction();
            };

            resourcePadlock = new ResourcePadlock();
        }

        public TargetFile TargetFile
        {
            set {
                targetFile = value;
                if (targetFile.OperationType == FileOperationType.Encryption)
                {
                    PrimaryButtonText = "Lock File";
                }
            }
        }

        public Action PrimaryButtonAction
        {
            get { return primaryButtonAction; }
            set { primaryButtonAction = value; }
        }

        [RelayCommand(CanExecute = nameof(CanRunPrimaryButtonAction))]
        private void RunPrimaryButtonAction()
        {
            if (resourcePadlock.Lock(PrimaryButtonAction) == ResourcePadlockStatus.Locked)
            {
                if (RunPrimaryButtonActionCommand.CanExecute(null))
                {
                    PrimaryButtonAction();
                    ClearTextBoxes();
                }

                resourcePadlock.Unlock(PrimaryButtonAction);
            }
        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrEmpty(passwordTextbox.Password))
            {
                passwordErrorInfoBar.Message = "Password is required.";
                passwordErrorInfoBar.IsOpen = true;
            }

            if (string.IsNullOrEmpty(confirmPasswordTextbox.Password))
            {
                confirmPasswordErrorInfoBar.Message = "Password confirmation is required.";
                confirmPasswordErrorInfoBar.IsOpen = true;
            }

            ClearTextBoxes();
        }

        private void Dialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ClearTextBoxes();
        }

        private void PasswordTextbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            const int MIN_PASSWORD_LENGTH = 12;
            bool isPasswordLengthOkay = passwordTextbox.Password.Length >= MIN_PASSWORD_LENGTH;
            if (string.IsNullOrEmpty(passwordTextbox.Password) || isPasswordLengthOkay)
            {
                // Let's clear out this one for now. The user might just be re-entering his/her
                // password.
                passwordErrorInfoBar.Message = string.Empty;
                passwordErrorInfoBar.IsOpen = false;
            }
            else
            {
                if (!isPasswordLengthOkay)
                {
                    passwordErrorInfoBar.Message = "Passwords must have 12 characters or more.";
                    passwordErrorInfoBar.IsOpen = true;
                }
            }

            RunPrimaryButtonActionCommand.NotifyCanExecuteChanged();
        }

        private void ConfirmPasswordTextbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool arePasswordsMatching = passwordTextbox.Password.Equals(confirmPasswordTextbox.Password);
            if (string.IsNullOrEmpty(confirmPasswordTextbox.Password) || arePasswordsMatching)
            {
                // Let's clear out this one for now. The user might just be re-entering his/her
                // password.
                confirmPasswordErrorInfoBar.Message = string.Empty;
                confirmPasswordErrorInfoBar.IsOpen = false;
            }
            else
            {
                if (!arePasswordsMatching)
                {
                    confirmPasswordErrorInfoBar.Message = "Passwords entered do not match each other.";
                    confirmPasswordErrorInfoBar.IsOpen = true;
                }
            }

            RunPrimaryButtonActionCommand.NotifyCanExecuteChanged();
        }

        private void ClearTextBoxes()
        {
            passwordTextbox.Password = "";
            confirmPasswordTextbox.Password = "";
        }

        private bool CanRunPrimaryButtonAction()
        {
            Logging.Info("CanRunPrimaryButtonAction triggered");
            string password = passwordTextbox.Password;
            string passwordConfirmation = confirmPasswordTextbox.Password;

            bool isPasswordSet = !string.IsNullOrEmpty(password);
            bool isPasswordConfirmationSet = !string.IsNullOrEmpty(passwordConfirmation);
            bool arePasswordsMatching = password.Equals(passwordConfirmation);
            bool isPrimaryButtonActionSet = PrimaryButtonAction is not null;
            bool isPrimaryButtonActionRunning = (
                isPrimaryButtonActionSet && resourcePadlock.IsResourceLocked(PrimaryButtonAction)
            );

            return isPasswordSet
                && isPasswordConfirmationSet
                && arePasswordsMatching
                && !isPrimaryButtonActionRunning;
        }
    }
}
