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
#pragma once

#include <string>
#include <ShlObj.h>
#include <Shlwapi.h>
#include <Windows.h>

#include "dll.hpp"

inline std::wstring GetModuleFolderPath(HMODULE module = nullptr)
{
	wchar_t buffer[MAX_PATH + 1];
	DWORD actualLength = GetModuleFileNameW(module, buffer, MAX_PATH);
	if (GetLastError() == ERROR_INSUFFICIENT_BUFFER) {
		const DWORD longPathLength = 0xFFFF; // Should always be enough.
		std::wstring longFilename(longPathLength, L'\0');
		actualLength = GetModuleFileNameW(module, longFilename.data(), longPathLength);

		PathRemoveFileSpecW(longFilename.data());

		longFilename.resize(std::wcslen(longFilename.data()));
		longFilename.shrink_to_fit();

		return longFilename;
	}

	PathRemoveFileSpecW(buffer);

	return { buffer, static_cast<uint64_t>(lstrlenW(buffer)) };
}

// The following functions use the COM API and are used inside coclasses or
// user-defined COM functions. Hence, we use the COM API style in naming the
// functions.

// Based on: https://github.com/microsoft
//                             /Windows-AppConsult-Samples-DesktopBridge
//                             /blob/main/Docs-ContextMenuSample
//                             /ExplorerCommandVerb/ShellHelpers.h
__inline HRESULT GetShellItemFromArrayAt(IShellItemArray* psia, DWORD i, REFIID riid, void** ppv)
{
	*ppv = NULL;
	IShellItem* psi = NULL;
	HRESULT hr = psia ? psia->GetItemAt(i, &psi) : E_NOINTERFACE;
	if (SUCCEEDED(hr)) {
		hr = psi->QueryInterface(riid, ppv);
		psi->Release();
	}

	return hr;
}

template <class T> void SafeRelease(T** ppT)
{
	if (*ppT) {
		(*ppT)->Release();
		*ppT = NULL;
	}
}

// Assign an interface pointer, release the old one, and capture reference to new.
// Can be used to set the ref (?) to zero too.
template <class T> HRESULT SetInterface(T** ppT, IUnknown* punk)
{
	SafeRelease(ppT);
	return punk ? punk->QueryInterface(ppT) : E_NOINTERFACE;
}

// The following now are our custom functions.
inline std::wstring GetDLLFolderPath()
{
	return GetModuleFolderPath(g_hInst);
}
