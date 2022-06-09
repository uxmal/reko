/*
* Copyright (C) 1999-2022 John Källén.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; either version 2, or (at your option)
* any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; see the file COPYING.  If not, write to
* the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
*/

#ifndef __TYPES_H
#define __TYPES_H

#ifdef _WINDOWS

#include <objbase.h>
#define DLLEXPORT __declspec(dllexport)

#else

#define STDAPICALLTYPE
#define STDMETHODCALLTYPE
#define STDMETHODIMP int
#define STDMETHODIMP_(type) type
#define STDMETHOD(name) HRESULT STDMETHODCALLTYPE name
#define STDMETHOD_(type,name) type STDMETHODCALLTYPE name
#define DLLEXPORT

#include "common/guids.h"

typedef unsigned long UINT;
typedef int INT;
typedef long BOOL;
typedef unsigned char BYTE;
typedef long LONG;
typedef unsigned long ULONG;
typedef unsigned short WORD;
typedef unsigned long DWORD;
typedef unsigned short VARTYPE;
typedef unsigned short USHORT;
typedef DWORD LCID;
typedef LONG SCODE;
typedef short SHORT;
typedef wchar_t WCHAR;
typedef WCHAR TCHAR;
typedef WCHAR OLECHAR;

typedef GUID IID;
typedef IID* LPIID;

typedef /*[ptr]*/ void* HWND;
typedef /*[ptr]*/ void* HMENU;
typedef /*[ptr]*/ void* HANDLE;

typedef int HRESULT;

typedef char* LPWSTR;

struct IUnknown {
     virtual HRESULT QueryInterface(REFIID, void ** out) = 0;
     virtual ULONG AddRef() = 0;
     virtual ULONG Release() = 0;
};

#define S_OK    0
#define S_FALSE 1
#define E_NOINTERFACE 0x80004002
#define E_NOTIMPL 0x80004001
 
#endif 

#endif
