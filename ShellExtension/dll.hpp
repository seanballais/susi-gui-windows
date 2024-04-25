#pragma once

void DllAddRef();
void DllRelease();

class __declspec(uuid("D56A16FE-D6DD-41A4-A2E1-08C05AB083C7")) CExplorerCommandLock;

HRESULT CExplorerCommandLock_CreateInstance(REFIID riid, void** ppv);
