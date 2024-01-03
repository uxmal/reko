/*
* Copyright (C) 1999-2024 John Källén.
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

#include "stdafx.h"

#include "reko.h"

#include "functions.h"
#include "ComBase.h" 
#include "ThumbDisassembler.h"
#include "ArmRewriter.h"
#include "ThumbRewriter.h"
#include "ThumbArchitecture.h"
#include "ArmArchitecture.h"

ThumbArchitecture::ThumbArchitecture()
{
	AddRef();
}


STDMETHODIMP ThumbArchitecture::QueryInterface(REFIID riid, void ** ppvObject)
{
	if (riid == IID_INativeArchitecture ||
		riid == IID_IAgileObject ||
		riid == IID_IUnknown)
	{
		AddRef();
		*ppvObject = static_cast<INativeArchitecture *>(this);
		return S_OK;
	}
	*ppvObject = nullptr;
	return E_NOINTERFACE;
}


STDMETHODIMP ThumbArchitecture::GetAllRegisters(int regKind, int * pcRegs, void ** ppRegs)
{
	*pcRegs = ARM_REG_ENDING;
	*ppRegs = const_cast<NativeRegister*>(&ArmArchitecture::aRegs[0]);
	return S_OK;
}

INativeDisassembler * STDMETHODCALLTYPE ThumbArchitecture::CreateDisassembler(
	void * bytes, int length, int offset, uint64_t uAddr)
{
	auto dasm = new ThumbDisassembler(reinterpret_cast<uint8_t*>(bytes) + offset, length - offset, offset, uAddr);
	dasm->AddRef();
	return dasm;
}

INativeRewriter * STDAPICALLTYPE ThumbArchitecture::CreateRewriter(
	void * rawBytes,
	int32_t length,
	int32_t offset,
	uint64_t address,
	INativeRtlEmitter * m,
	INativeTypeFactory * typeFactory,
	INativeRewriterHost * host)
{
	auto rw = new ThumbRewriter(reinterpret_cast<uint8_t*>(rawBytes) + offset, length - offset, address, m, typeFactory, host);
	rw->AddRef();
	return rw;
}
