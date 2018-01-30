/*
* Copyright (C) 1999-2017 John Källén.
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


void STDMETHODCALLTYPE ThumbArchitecture::GetAllRegisters(int regKind, int * pcRegs, const NativeRegister ** ppRegs)
{
	*pcRegs = ARM_REG_ENDING;
	*ppRegs = &ArmArchitecture::aRegs[0];
}

INativeDisassembler * STDMETHODCALLTYPE ThumbArchitecture::CreateDisassembler(
	const uint8_t * bytes, int length, int offset, uint64_t uAddr)
{
	auto dasm = new ThumbDisassembler(bytes + offset, length - offset, offset, uAddr);
	dasm->AddRef();
	return dasm;
}

INativeRewriter * STDAPICALLTYPE ThumbArchitecture::CreateRewriter(
	const uint8_t * rawBytes,
	uint32_t length,
	uint32_t offset,
	uint64_t address,
	INativeRtlEmitter * m,
	INativeTypeFactory * typeFactory,
	INativeRewriterHost * host)
{
	return new ThumbRewriter(rawBytes + offset, length - offset, address, m, typeFactory, host);
}
