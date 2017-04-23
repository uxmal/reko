// ArmNative.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "reko.h"
#include "ArmRewriter.h"

extern "C" {
	__declspec(dllexport)
		INativeRewriter * __cdecl CreateRewriter(const uint8_t * rawBytes, int length, IRtlEmitter * m, IFrame * frame, IRewriterHost * host)
	{
		return new ArmRewriter(rawBytes, length, m, frame, host);
	}
}