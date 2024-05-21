# Susi

![A screenshot of Susi.](docs/screenshots/app-screenshot.png)

Susi is a utitlity app by [Sean Ballais](https://seanballais.com) that lets you encrypt (and decrypt) files to protect them from unauthorized parties. This is considered to be alpha software, so use this at your own risk.

![Susi Demonstration](docs/app-demo.gif)

## Supported Platforms
Susi fully supports Windows 11, but has partial support for Windows 10. Only file unlocking/decryption is supported in Windows 10 at the moment. Additionally, only 64-bit versions of the aformentioned operating systems are supported.

## Components
Susi is composed of two main components: (a) Susi GUI, and (b) Susi Core. Susi GUI is the GUI component of the app (this repository holds the code for the Windows version), while [Susi Core](https://github.com/seanballais/susi-core) is where most of the major operations, such as encryption and decryption, are done.

## Built With
Susi GUI is built with C# and a small, but important, component in C++. Susi Core is built completely in Rust. The installer for Susi GUI is built with [InnoSetup](https://jrsoftware.org/isinfo.php).

## Development
If you are developing 

## Installation
Susi GUI is exported as an MSIX package. However, it depends on the latest Visual C++ 2022 Redistributable, .NET 8 Desktop Runtime, and Windows App SDK Runtime. A certificate is also required to allow sideloading/installing the app. These steps are cumbersome for users. So, to make the installation process easy, we are providing a setup file that is a chain installer. It does all the aforementioned steps on the behalf of the user.

You can simply grab a setup file from the [Releases](https://github.com/seanballais/susi-gui-windows/releases) page. Once you have a setup file, simply run it and follow the steps. You don't have to do much to install it! The demo below shows the installation process. Note that this is shortened since the dependencies were already instaleld.

![Installation Demo](docs/app-installation-demo.gif)

We recommend restarting your machine, however, once the installation is done, so that our shell extension to the File Explorer gets loaded in. Alternatively, you can just restart Explorer from the Task Manager.

One important thing to note is that Susi is installed using MSIX. So, it will be installed inside `C:\Program Files\WindowsApps`.

Beware that the app and installer are both alpha software. So, again, use at your own risk.

### Uninstallation
Uninstallation of Susi is simply done by uninstalling the app from the Start Menu or the Settings app.



However, this only removes the app itself. You will have to manually uninstall the dependencies if you want them off of your machine. We don't necessarily recommend that since other apps may already be dependending on them.

The certificate for the app will still remain unless removed manually. You can remove it by performing the following steps:

1. Open "Manage user certificates" in the Control Panel. This can also be opened directly from the Start Menu.
2. Open "Trusted People" and then "Certificates".
3. Select the certificate that was issued by "Sean Francis N. Ballais", and either click on the red "X" button on the toolbar or right-click the selected item and click "Delete". You will be asked if you would like to delete the selected certificate. Press "Yes".
4. Done!

You may also view the demo below to learn how to remove the certificate.



## Using Susi
Susi provides two features -- encryption _and_ decryption of files. The app encrypts to and decrypts from a custom Susi Encrypted File (a `.ssef` file). It does not support any other encryption formats. You may check [Susi Specifications](#susi-specifications) for details on the custom `.ssef` file format.

### Encryption
Encrypting a file with Susi is easy. The following steps will guide you how to encrypt a file.

1. Simply select a file from the Windows Explorer or desktop.
2. Right-click the selected file, and select "Lock File".
3. Susi will show up and will ask for a password to encrypt the file. Enter the desired password for the file. You will have to enter the same password in the "Confirm Password" textbox.
4. After entering the password, Susi will start encrypting the file. It may take a while depending on your computer. After encryption is complete, the original file will be **removed** and you are left with an encrypted `.ssef` file in the **same directory** as the original file.

The following video provides a visual demonstration on how to encrypt a file with Susi.



### Decryption
Decrypting a Susi Encrypted File with Susi is easy as well. Remember that Susi can only decrpyt a Susi Encrypted File (`.ssef` file). The following steps will guide you how to decrpyt a file.

1. Open a Susi Encrypted File (`.ssef` file) from Windows Explorer or the desktop. If you are asked to select a program to open the file with, make sure you select Susi and, as a recommendation, set it as the default app.
2. Susi will show up and will ask for the password used to encrypt the file. Enter the correct password for the file. Susi will inform you if you entered the wrong password.
3. After entering the password, Susi will start decrypting the file. It may take a while depending on your computer. After decryption is complete, the encrypted file will be **removed** and you are left with the original file with its original file name in the **same directory** as the encrypted file.

The following video provides a visual demonstration on how to decrypt a Susi Encrypted File with Susi.



### Remarks
Make sure you remember the password you set for a file when you encrypt it. You will **not** be able to decrypt it without the correct password. Additionally, note that a Susi Encrypted File is portable and can be renamed. However, upon decryption, it will revert back to its original file name.

## Susi Software Specifications
Software specifications for Susi have been written to guide us with the development of the project. It also contains the specifications of the file format of a Susi Encrypted File (`.ssef`). If you are interested in reading it or learning about the file format, you may obtain and read the latest specifications from the [software specifications's GitHub repository](https://github.com/seanballais/susi-software-specs/releases).

## License
This project, Susi GUI (for Windows), is licensed under the GNU General Public License v3. See [`LICENSE.md`](/LICENSE.md) for details.

Some parts of the app was based on third-party code. See [`THIRD-PARTY-LICENSES.txt`](/THIRD-PARTY-LICENSES.txt) for details. Some code used in this project are based from [StackOverflow](https://stackoverflow.com) answers. The credits for such usage are stored in the individual files with code that is based off of StackOverflow answers.

## Contact
Sean Francis N. Ballais - [@seanballais](https://twitter.com/seanballais) - [sean@seanballais.com](mailto:sean@seanballais.com)
