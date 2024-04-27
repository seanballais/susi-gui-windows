#include "ipc.hpp"

// Based on: https://github.com/microsoft
//							   /PowerToys
//							   /blob/main/src/modules
//							   /powerrename/PowerRenameContextMenu/dllmain.cpp
//      and: https://bloomfield.online/posts/introduction-to-win32-named-pipes-cpp/
HRESULT CreateNamedPipeServerAndWaitForConns(HANDLE pipeServerHandle, std::wstring pipeName)
{
	pipeServerHandle = CreateNamedPipe(
		pipeName.c_str(),
		PIPE_ACCESS_OUTBOUND,
		PIPE_TYPE_MESSAGE | PIPE_WAIT,
		PIPE_UNLIMITED_INSTANCES,
		PIPE_BUFFER_SIZE,
		PIPE_BUFFER_SIZE,
		0,
		NULL
	);
	if (pipeServerHandle == NULL || pipeServerHandle == INVALID_HANDLE_VALUE) {
		return E_FAIL;
	}

	BOOL isClientConnected = ConnectNamedPipe(pipeServerHandle, NULL);
	if (!isClientConnected) {
		if (GetLastError() == ERROR_PIPE_CONNECTED) {
			return S_OK;
		} else {
			CloseHandle(pipeServerHandle);
		}

		return E_FAIL;
	}

	return S_OK;
}
