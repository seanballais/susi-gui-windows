// Based on: https://github.com/microsoft
//                             /Windows-AppConsult-Samples-DesktopBridge
//                             /blob/main
//							   /Docs-ContextMenuSample/ExplorerCommandVerb/dll.cpp
#include <new>

#include <Shlwapi.h>
#include <Windows.h>

#include "dll.hpp"

typedef HRESULT (*PFNCREATEINSTANCE)(REFIID riid, void** ppvObject);

struct CLASS_OBJECT_INIT
{
    const CLSID* pClsid;
    PFNCREATEINSTANCE pfnCreate;
};

const CLASS_OBJECT_INIT c_rgClassObjectInit[] = {
    { &__uuidof(CExplorerCommandLock), CExplorerCommandLock_CreateInstance }
};

long g_cRefModule = 0;

HINSTANCE g_hInst = NULL;

void DllAddRef()
{
    InterlockedIncrement(&g_cRefModule);
}

void DllRelease()
{
    InterlockedDecrement(&g_cRefModule);
}

class CClassFactory : public IClassFactory
{
public:
    static HRESULT CreateInstance(REFCLSID clsid,
                                  const CLASS_OBJECT_INIT* pClassObjectInits,
                                  size_t cClassObjectInits,
                                  REFIID riid,
                                  void** ppv)
    {
        *ppv = NULL;
        HRESULT hr = CLASS_E_CLASSNOTAVAILABLE;
        for (size_t i = 0; i < cClassObjectInits; i++) {
            if (clsid == *pClassObjectInits[i].pClsid) {
                IClassFactory* pClassFactory = new (std::nothrow) CClassFactory(pClassObjectInits[i].pfnCreate);
                hr = pClassFactory ? S_OK : E_OUTOFMEMORY;
                if (SUCCEEDED(hr)) {
                    hr = pClassFactory->QueryInterface(riid, ppv);
                    pClassFactory->Release();
                }
                break;
            }
        }

        return hr;
    }

    CClassFactory(PFNCREATEINSTANCE pfnCreate) : _cRef(1), _pfnCreate(pfnCreate)
    {
        DllAddRef();
    }

    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv)
    {
        static const QITAB qit[] = {
            QITABENT(CClassFactory, IClassFactory),
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

    // IClassFactory
    IFACEMETHODIMP CreateInstance(IUnknown* punkOuter, REFIID riid, void** ppv)
    {
        return punkOuter ? CLASS_E_NOAGGREGATION : _pfnCreate(riid, ppv);
    }

    IFACEMETHODIMP LockServer(BOOL fLock)
    {
        if (fLock) {
            DllAddRef();
        } else {
            DllRelease();
        }

        return S_OK;
    }

private:
    ~CClassFactory()
    {
        DllRelease();
    }

    long _cRef;
    PFNCREATEINSTANCE _pfnCreate;
};

STDAPI DllCanUnloadNow()
{
    // Only allow the DLL to be unloaded after all outstanding references have been released.
    return (g_cRefModule == 0) ? S_OK : S_FALSE;
}

STDAPI DllGetClassObject(REFCLSID clsid, REFIID riid, void** ppv)
{
    return CClassFactory::CreateInstance(clsid, c_rgClassObjectInit, ARRAYSIZE(c_rgClassObjectInit), riid, ppv);
}

STDAPI_(BOOL) DllMain(HINSTANCE hInstance, DWORD dwReason, void*)
{
    if (dwReason == DLL_PROCESS_ATTACH) {
        g_hInst = hInstance;
        DisableThreadLibraryCalls(hInstance);
    }

    return TRUE;
}
