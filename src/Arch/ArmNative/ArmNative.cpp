// ArmNative.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "reko.h"
#include "ArmRewriter.h"

IRewriter * CreateRewriter(void * rawBytes, int length, IRtlEmitter * m, IFrame * frame, IRewriterHost * host)
{
	return new ArmRewriter(rawBytes, length, m, frame, host);
}
