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
#include "ThumbRewriter.h"


ThumbRewriter::ThumbRewriter(
	const uint8_t * rawBytes,
	size_t availableBytes,
	uint64_t address,
	INativeRtlEmitter * emitter,
	INativeTypeFactory * ntf,
	INativeRewriterHost * host)
	:
	rawBytes(rawBytes),
	available(availableBytes),
	address(address),
	m(*emitter),
	ntf(*ntf),
	host(host),
	cRef(1),
	instr(nullptr)
{
	//Dump(".ctor: %08x", this);
	auto ec = cs_open(CS_ARCH_ARM, CS_MODE_THUMB , &hcapstone);
	ec = cs_option(hcapstone, CS_OPT_DETAIL, CS_OPT_ON);
	this->instr = cs_malloc(hcapstone);
	//++s_count;
}

STDMETHODIMP ThumbRewriter::QueryInterface(REFIID riid, void ** ppvOut)
{
	//Dump("QI: %08x %d", this, cRef);
	*ppvOut = nullptr;
	if (riid == IID_IUnknown || riid == IID_INativeRewriter)
	{
		AddRef();
		*ppvOut = static_cast<INativeRewriter *>(this);
		return S_OK;
	}
	return E_NOINTERFACE;
}


STDMETHODIMP ThumbRewriter::Next()
{
	Dump("Next: %08x", this);
	if (available == 0)
		return S_FALSE;			// No more work to do.
	auto addrInstr = address;
	bool f = cs_disasm_iter(hcapstone, &rawBytes, &available, &address, instr);
	if (!f)
	{
		// Failed to disassemble the instruction because it was invalid.
		m.Invalid();
		m.FinishCluster(RtlClass::Invalid, addrInstr, 4);
		return S_OK;
	}
	// Most instructions are linear.
	rtlClass = RtlClass::Linear;
	return S_FALSE;

}



int32_t ThumbRewriter::GetCount()
{
	return 0;
}

//int ThumbRewriter::s_count = 0;
