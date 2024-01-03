#pragma once
/*
* Copyright (C) 1999-2024 John K�ll�n.
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

class Arm64Disassembler : public ComBase, public INativeDisassembler
{
public:
	Arm64Disassembler(const uint8_t * bytes, size_t length, int offset, uint64_t uAddr);
	~Arm64Disassembler();

	STDMETHODIMP QueryInterface(REFIID iid, void ** ppvItf) override;
	STDMETHODIMP_(ULONG) AddRef() override { return  ComBase::AddRef(); }
	STDMETHODIMP_(ULONG) Release() override { return ComBase::Release(); }

	INativeInstruction * STDAPICALLTYPE NextInstruction() override;
private:
	InstrClass InstrClassFromId(unsigned int armInstrID);

	csh hcapstone;
	const uint8_t * bytes;
	size_t length;
	int offset;
	uint64_t uAddr;
};