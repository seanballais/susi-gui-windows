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

extern HINSTANCE g_hInst;

void DllAddRef();
void DllRelease();

class __declspec(uuid("D56A16FE-D6DD-41A4-A2E1-08C05AB083C7")) CExplorerCommandLock;

HRESULT CExplorerCommandLock_CreateInstance(REFIID riid, void** ppv);
