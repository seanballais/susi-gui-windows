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
using System.Runtime.InteropServices;
using System.Text;

namespace susi_gui_windows.Core.Native
{
    internal class TaskFFI
    {
        public static TaskIDWrapper QueueEncryptionTask(string srcFile, string password)
        {
            byte[] utf8EncodedSrcFile = Encoding.UTF8.GetBytes(srcFile);
            byte[] utf8EncodedPassword = Encoding.UTF8.GetBytes(password);

            IntPtr ptr = queue_encryption_task(utf8EncodedSrcFile, utf8EncodedPassword);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Failed to queue encryption task due to a core library error.");
            }

            var taskID = Marshal.PtrToStructure<TaskIDNative>(ptr);
            return new TaskIDWrapper(taskID, ptr);
        }

        public static TaskIDWrapper QueueDecryptionTask(string srcFile, string password)
        {
            byte[] utf8EncodedSrcFile = Encoding.UTF8.GetBytes(srcFile);
            byte[] utf8EncodedPassword = Encoding.UTF8.GetBytes(password);

            IntPtr ptr = queue_decryption_task(utf8EncodedSrcFile, utf8EncodedPassword);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Failed to queue decryption task due to a core library error.");
            }

            var taskID = Marshal.PtrToStructure<TaskIDNative>(ptr);
            return new TaskIDWrapper(taskID, ptr);
        }

        public static TaskStatusWrapper GetTaskStatus(TaskIDWrapper taskID)
        {
            var ptr = get_task_status(taskID.Pointer);
            if (ptr == IntPtr.Zero)
            {
                throw new NullFFIPointerException("Task does not exist.");
            }

            var taskStatus = Marshal.PtrToStructure<TaskStatusNative>(ptr);
            return new TaskStatusWrapper(taskStatus, ptr);
        }

        public static void DropTaskStatus(TaskStatusWrapper taskStatus)
        {
            drop_task_status(taskStatus.Pointer);
        }

        public static void DropTaskID(TaskIDWrapper taskID)
        {
            drop_task_id(taskID.Pointer);
        }

        // We're using a byte[] here instead of a string since we should be passing a UTF-8 string.
        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr queue_encryption_task(byte[] target_file, byte[] password);

        // We're using a byte[] here instead of a string since we should be passing a UTF-8 string.
        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr queue_decryption_task(byte[] target_file, byte[] password);

        [DllImport(Constants.CoreDLLName)]
        private static extern IntPtr get_task_status(IntPtr id);

        [DllImport(Constants.CoreDLLName)]
        private static extern void drop_task_status(IntPtr status);

        [DllImport(Constants.CoreDLLName)]
        private static extern void drop_task_id(IntPtr id);
    }
}
