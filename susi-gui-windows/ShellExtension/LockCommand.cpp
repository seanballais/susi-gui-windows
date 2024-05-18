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
#include <new>
#include <thread>

#include <atlfile.h>
#include <atlstr.h>
#include <ShlObj.h>
#include <Shlwapi.h>
#include <strsafe.h>
#include <Windows.h>

#include "dll.hpp"
#include "logging.hpp"
#include "utils.hpp"

#define PIPE_BUFFER_SIZE 4096 * 4

static WCHAR const c_szVerbDisplayName[] = L"Lock File";

class CExplorerCommandLock : public IExplorerCommand,
	public IInitializeCommand,
	public IObjectWithSite
{
public:
	CExplorerCommandLock()
		: _cRef(1)
		, _punkSite(NULL)
		, _hwnd(NULL)
		, _pstmShellItemArray(NULL)
		, _pipeServerHandle(INVALID_HANDLE_VALUE)
		, _pipeName(L"\\\\.\\pipe\\susi-FD2694FA-4BB3-4ABD-8CF7-0CCCAFA32347")
	{
		DllAddRef();
	}

	// IUnknown
	IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv)
	{
		static const QITAB qit[] = {
			QITABENT(CExplorerCommandLock, IExplorerCommand),
			QITABENT(CExplorerCommandLock, IInitializeCommand),
			{ 0 }
		};

		return QISearch(this, qit, riid, ppv);
	}

	IFACEMETHODIMP_(ULONG) AddRef()
	{
		return InterlockedIncrement(&_cRef);
	}

	IFACEMETHODIMP_(ULONG) Release()
	{
		long cRef = InterlockedDecrement(&_cRef);
		if (cRef == 0) {
			delete this;
		}

		return cRef;
	}

	// IExplorerCommand
	IFACEMETHODIMP GetTitle(IShellItemArray* /* psiItemArray */, LPWSTR* ppszName)
	{
		return SHStrDup(c_szVerbDisplayName, ppszName);
	}

	// Based on: https://github.com/microsoft
	//							   /PowerToys
	//                             /blob
	//                             /fc214a80c535d15b775a820b34981caa3b31d177
	//                             /src/modules/powerrename
	//                             /PowerRenameContextMenu/dllmain.cpp
	IFACEMETHODIMP GetIcon(IShellItemArray* /* psiItemArray */, LPWSTR* ppszIcon)
	{
		std::wstring iconPath = GetDLLFolderPath();
		iconPath.append(L"\\Assets\\Susi.ico");

		return SHStrDup(iconPath.c_str(), ppszIcon);
	}

	IFACEMETHODIMP GetToolTip(IShellItemArray* /* psiItemArray */, LPWSTR* ppszInfotip)
	{
		*ppszInfotip = NULL;
		return E_NOTIMPL;
	}

	IFACEMETHODIMP GetCanonicalName(GUID* pguidCommandName)
	{
		// Visual Studio 2022 can be a prick, so ignore the red underline here if it shows up.
		// You'll be able to compile safely without much issue.
		*pguidCommandName = __uuidof(this);
		return S_OK;
	}

	IFACEMETHODIMP GetState(IShellItemArray* /* psiItemArray */, BOOL fOkToBeSlow, EXPCMDSTATE* pCmdState)
	{
		HRESULT hr = S_OK;
		return hr;
	}

	IFACEMETHODIMP Invoke(IShellItemArray* psiItemArray, IBindCtx* pbc);

	IFACEMETHODIMP GetFlags(EXPCMDFLAGS* pFlags)
	{
		*pFlags = ECF_DEFAULT;
		return S_OK;
	}

	IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand** ppEnum)
	{
		*ppEnum = NULL;
		return E_NOTIMPL;
	}

	IFACEMETHODIMP Initialize(PCWSTR /* pszCommandName */, IPropertyBag* /* ppb */)
	{
		return S_OK;
	}

	// IObjectWithSite
	IFACEMETHODIMP SetSite(IUnknown* punkSite)
	{
		SetInterface(&_punkSite, punkSite);
		return S_OK;
	}

	IFACEMETHODIMP GetSite(REFIID riid, void** ppv)
	{
		*ppv = NULL;
		return _punkSite ? _punkSite->QueryInterface(riid, ppv) : E_FAIL;
	}

private:
	~CExplorerCommandLock()
	{
		SafeRelease(&_punkSite);
		SafeRelease(&_pstmShellItemArray);
		DllRelease();
	}

	DWORD _ThreadProc();

	HRESULT CreateNamedPipeServerAndWaitForConns();

	static DWORD __stdcall s_ThreadProc(void* pl)
	{
		CExplorerCommandLock* pecl = (CExplorerCommandLock*) pl;
		const DWORD ret = pecl->_ThreadProc();
		pecl->Release();

		return ret;
	}

	long _cRef;
	IUnknown* _punkSite;
	HWND _hwnd;
	IStream* _pstmShellItemArray;
	HANDLE _pipeServerHandle;
	std::wstring _pipeName;
};

DWORD CExplorerCommandLock::_ThreadProc()
{
	logInfo(L"New call to _ThreadProc.");
	IShellItemArray* psia;
	HRESULT hr = CoGetInterfaceAndReleaseStream(_pstmShellItemArray, IID_PPV_ARGS(&psia));
	_pstmShellItemArray = NULL;
	if (SUCCEEDED(hr)) {
		std::wstring exePath = GetDLLFolderPath();
		exePath.append(L"\\Susi.exe");

		logInfo(L"Creating pipe server...");
		auto pipe_creation_thread = std::thread(&CExplorerCommandLock::CreateNamedPipeServerAndWaitForConns, this);
		ShellExecute(_hwnd, L"open", exePath.c_str(), NULL, GetDLLFolderPath().c_str(), SW_SHOW);
		pipe_creation_thread.join();

		if (_pipeServerHandle != INVALID_HANDLE_VALUE) {
			logInfo(L"Pipe server ready.");
			CAtlFile writePipe(_pipeServerHandle);

			DWORD count;
			psia->GetCount(&count);
			logInfo(L"Number of files selected for locking: {}", count);
			for (DWORD i = 0; i < count; i++) {
				IShellItem* psi;
				HRESULT hr = GetShellItemFromArrayAt(psia, i, IID_PPV_ARGS(&psi));
				if (SUCCEEDED(hr)) {
					LPWSTR pszName;
					hr = psi->GetDisplayName(SIGDN_DESKTOPABSOLUTEPARSING, &pszName);
					logInfo(L"Currently selected file for locking: {}", pszName);
					if (SUCCEEDED(hr)) {
						CString fileName(pszName);
						
						// Let's a delimiter to help us separate files later on. We're gonna use
						// `|` since it's an illegal character in paths.
						fileName.Append(_T("|"));

						HRESULT hr = writePipe.Write(fileName, fileName.GetLength() * sizeof(TCHAR));
						if (SUCCEEDED(hr)) {
							logInfo(L"Wrote '{}' to pipe.", pszName);
						}
					}
				}

				psi->Release();
			}

			writePipe.Close();
		}

		psia->Release();
	}

	return 0;
}

IFACEMETHODIMP CExplorerCommandLock::Invoke(IShellItemArray* psia, IBindCtx* /* pbc */)
{
	IUnknown_GetWindow(_punkSite, &_hwnd);

	HRESULT hr = CoMarshalInterThreadInterfaceInStream(__uuidof(psia), psia, &_pstmShellItemArray);
	if (SUCCEEDED(hr)) {
		AddRef();

		if (!SHCreateThread(s_ThreadProc, this, CTF_COINIT_STA | CTF_PROCESS_REF, NULL)) {
			Release();
		}
	}

	return S_OK;
}

// Based on: https://github.com/microsoft
//							   /PowerToys
//                             /blob
//                             /fc214a80c535d15b775a820b34981caa3b31d177
//                             /src/modules/powerrename
//                             /PowerRenameContextMenu/dllmain.cpp
//      and: https://bloomfield.online/posts/introduction-to-win32-named-pipes-cpp/
HRESULT CExplorerCommandLock::CreateNamedPipeServerAndWaitForConns()
{
	_pipeServerHandle = CreateNamedPipe(
		_pipeName.c_str(),
		PIPE_ACCESS_OUTBOUND,
		PIPE_TYPE_MESSAGE | PIPE_WAIT,
		PIPE_UNLIMITED_INSTANCES,
		PIPE_BUFFER_SIZE,
		PIPE_BUFFER_SIZE,
		0,
		NULL
	);

	logInfo(L"_pipeServerHandle: {}", _pipeServerHandle);

	if (_pipeServerHandle == NULL || _pipeServerHandle == INVALID_HANDLE_VALUE) {
		logInfo(L"_pipeServerHandle, oh no.");
		return E_FAIL;
	}

	logInfo(L"_pipeServerHandle 111: {}", _pipeServerHandle);

	BOOL isClientConnected = ConnectNamedPipe(_pipeServerHandle, NULL);
	logInfo(L"_pipeServerHandle 222: {}", _pipeServerHandle);
	if (!isClientConnected) {
		logInfo(L"_pipeServerHandle 333: {}", _pipeServerHandle);
		if (GetLastError() == ERROR_PIPE_CONNECTED) {
			logInfo(L"_pipeServerHandle 444: {}", _pipeServerHandle);
			return S_OK;
		}
		else {
			logInfo(L"_pipeServerHandle closing.");
			CloseHandle(_pipeServerHandle);
		}
		logInfo(L"_pipeServerHandle, ahhh!");
		return E_FAIL;
	}

	return S_OK;
}

HRESULT CExplorerCommandLock_CreateInstance(REFIID riid, void** ppv)
{
	initLogging();

	*ppv = NULL;
	CExplorerCommandLock* pLock = new (std::nothrow) CExplorerCommandLock();
	HRESULT hr = pLock ? S_OK : E_OUTOFMEMORY;
	if (SUCCEEDED(hr)) {
		pLock->QueryInterface(riid, ppv);
		pLock->Release();
	}

	return hr;
}
