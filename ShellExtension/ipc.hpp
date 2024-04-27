#pragma once

#include <string>
#include <Windows.h>

// Based on the buffer size for IPC specified in PowerRename, as of April 27, 2024.
#define PIPE_BUFFER_SIZE 4096 * 4

HRESULT CreateNamedPipeServerAndWaitForConns(HANDLE pipeServerHandle, std::wstring pipeName);
