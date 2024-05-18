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

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using susi_gui_windows.Core;
using susi_gui_windows.Utilities;
using susi_gui_windows.ViewModels;

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

        private Action<string> primaryButtonAction;
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
            get { return targetFile; }
            set {
                targetFile = value;
                if (targetFile.OperationType == FileOperationType.Encryption)
                {
                    PrimaryButtonText = "Lock File";

                    confirmPasswordBox.Visibility = Visibility.Visible;
                    confirmPasswordTextbox.IsEnabled = true;
                    confirmPasswordErrorInfoBar.IsEnabled = true;
                }
                else
                {
                    PrimaryButtonText = "Unlock File";

                    confirmPasswordBox.Visibility = Visibility.Collapsed;
                    confirmPasswordTextbox.IsEnabled = false;
                    confirmPasswordErrorInfoBar.IsEnabled = false;
                }
                Bindings.Update();
            }
        }

        public Action<string> PrimaryButtonAction
        {
            get { return primaryButtonAction; }
            set { primaryButtonAction = value; }
        }

        [RelayCommand(CanExecute = nameof(CanRunPrimaryButtonAction))]
        private void RunPrimaryButtonAction(string password)
        {
            if (resourcePadlock.Lock(PrimaryButtonAction) == ResourcePadlockStatus.Locked)
            {
                PrimaryButtonAction(password);
                ClearTextBoxes();

                resourcePadlock.Unlock(PrimaryButtonAction);
            }
        }

        private async void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ContentDialogButtonClickDeferral deferral = args.GetDeferral();

            bool canProceed = false;
            if (string.IsNullOrEmpty(passwordTextbox.Password))
            {
                passwordErrorInfoBar.Message = "Password is required.";
                passwordErrorInfoBar.IsOpen = true;

                canProceed = false;
            }

            if (targetFile.OperationType == FileOperationType.Encryption)
            {
                if (string.IsNullOrEmpty(confirmPasswordTextbox.Password))
                {
                    confirmPasswordErrorInfoBar.Message = "Password confirmation is required.";
                    confirmPasswordErrorInfoBar.IsOpen = true;

                    canProceed = false;
                }
                else
                {
                    canProceed = true;
                }
            }
            else
            {
                bool originalPrimaryButtonAvailability = IsPrimaryButtonEnabled;
                string originalPrimaryButtonText = PrimaryButtonText;
                bool originalPasswordTextboxAvailability = passwordTextbox.IsEnabled;

                IsPrimaryButtonEnabled = false;
                PrimaryButtonText = "Checking Password...";
                passwordTextbox.IsEnabled = false;

                Bindings.Update();

                string password = passwordTextbox.Password;
                bool isPasswordCorrect = await System.Threading.Tasks.Task.Run(() =>
                {
                    return Verification.IsPasswordCorrectForFile(targetFile.FilePath, password);
                });

                IsPrimaryButtonEnabled = originalPrimaryButtonAvailability;
                PrimaryButtonText = originalPrimaryButtonText;
                passwordTextbox.IsEnabled = originalPasswordTextboxAvailability;

                Bindings.Update();

                if (isPasswordCorrect)
                {
                    canProceed = true;
                }
                else
                {
                    passwordTextbox.SelectAll();
                    passwordTextbox.Focus(FocusState.Programmatic);

                    passwordErrorInfoBar.Message = "Password is incorrect.";
                    passwordErrorInfoBar.IsOpen = true;

                    canProceed = false;
                }
            }

            args.Cancel = !canProceed;
            
            deferral.Complete();
        }

        private void Dialog_CancelButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ClearTextBoxes();
        }

        private void PasswordTextbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool isPasswordLengthOkay = passwordTextbox.Password.Length >= Constants.MinimumPasswordLength;
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
            string password = passwordTextbox.Password;
            string passwordConfirmation = confirmPasswordTextbox.Password;

            bool isPasswordSet = !string.IsNullOrEmpty(password);
            bool isPasswordProperLength = password.Length >= Constants.MinimumPasswordLength;
            bool condition = isPasswordSet && isPasswordProperLength;
            if (targetFile is not null && targetFile.OperationType == FileOperationType.Encryption)
            {
                bool isPasswordConfirmationSet = !string.IsNullOrEmpty(passwordConfirmation);
                bool arePasswordsMatching = password.Equals(passwordConfirmation);
                condition = condition && isPasswordConfirmationSet && arePasswordsMatching;
            }

            return condition;
        }
    }
}
