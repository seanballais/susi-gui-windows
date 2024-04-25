#include <new>

#include <ShlObj.h>
#include <Shlwapi.h>
#include <strsafe.h>
#include <Windows.h>

#include "dll.hpp"
#include "utils.hpp"

static WCHAR const c_szVerbDisplayName[] = L"Lock File";

class CExplorerCommandLock : public IExplorerCommand,
	public IInitializeCommand,
	public IObjectWithSite
{
public:
	CExplorerCommandLock() : _cRef(1), _punkSite(NULL), _hwnd(NULL), _pstmShellItemArray(NULL)
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

	IFACEMETHODIMP GetIcon(IShellItemArray* /* psiItemArray */, LPWSTR* ppszIcon)
	{
		*ppszIcon = NULL;
		return E_NOTIMPL;
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
};

DWORD CExplorerCommandLock::_ThreadProc()
{
	IShellItemArray* psia;
	HRESULT hr = CoGetInterfaceAndReleaseStream(_pstmShellItemArray, IID_PPV_ARGS(&psia));
	_pstmShellItemArray = NULL;
	if (SUCCEEDED(hr)) {
		DWORD count;
		psia->GetCount(&count);

		IShellItem2* psi;
		HRESULT hr = GetItemAt(psia, 0, IID_PPV_ARGS(&psi));
		if (SUCCEEDED(hr)) {
			PWSTR pszName;
			hr = psi->GetDisplayName(SIGDN_DESKTOPABSOLUTEPARSING, &pszName);
			if (SUCCEEDED(hr)) {
				WCHAR szMsg[128];
				StringCchPrintf(szMsg, ARRAYSIZE(szMsg), L"%d item(s), first item is named %s", count, pszName);

				MessageBox(_hwnd, szMsg, L"ExplorerCommand Lock", MB_OK);

				WCHAR szAnotherMsg[255];
				StringCchPrintf(szAnotherMsg, ARRAYSIZE(szAnotherMsg), L"DLL Folder Path: %s", getDLLFolderPath().c_str());

				MessageBox(_hwnd, szAnotherMsg, L"Test", MB_OK);

				CoTaskMemFree(pszName);
			}

			psi->Release();
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

HRESULT CExplorerCommandLock_CreateInstance(REFIID riid, void** ppv)
{
	*ppv = NULL;
	CExplorerCommandLock* pLock = new (std::nothrow) CExplorerCommandLock();
	HRESULT hr = pLock ? S_OK : E_OUTOFMEMORY;
	if (SUCCEEDED(hr)) {
		pLock->QueryInterface(riid, ppv);
		pLock->Release();
	}

	return hr;
}
