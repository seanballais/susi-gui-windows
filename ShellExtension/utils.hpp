// Based on: https://github.com/microsoft
//                             /Windows-AppConsult-Samples-DesktopBridge
//                             /blob/main/Docs-ContextMenuSample
//                             /ExplorerCommandVerb/ShellHelpers.h
#include <Windows.h>

__inline HRESULT GetItemAt(IShellItemArray* psia, DWORD i, REFIID riid, void** ppv)
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
