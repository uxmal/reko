/*
* Copyright (C) 1999-2022 John K�ll�n.
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

// ArmNative.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "types.h"
#include "reko.h"

#include "functions.h"
#include "ComBase.h"
#include "ArmRewriter.h"
#include "ArmArchitecture.h"
#include "Arm64Architecture.h"
#include "ThumbArchitecture.h"

extern "C" {
	
	DLLEXPORT INativeRewriter *
		CreateNativeRewriter(
			const uint8_t * rawBytes,
			uint32_t length,	
			uint32_t offset, 
			uint64_t address, 
			INativeRtlEmitter * m,
			INativeTypeFactory * typeFactory,
			INativeRewriterHost * host)
	{
		auto rw = new ArmRewriter(cs_mode::CS_MODE_ARM, rawBytes + offset, length - offset, address, m, typeFactory, host);
		rw->AddRef();
		return rw;
	}

	DLLEXPORT INativeArchitecture *
		CreateNativeArchitecture(const char * archName)
	{
		if (strcmp(archName, "arm") == 0)
			return new ArmArchitecture();
		else if (strcmp(archName, "arm-thumb") == 0)
			return new ThumbArchitecture();
		else if (strcmp(archName, "arm-64") == 0)
			return new Arm64Architecture();
		else
			return nullptr;
	}
}