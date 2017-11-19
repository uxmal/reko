// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

#if _WINDOWS
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}
#else
IID IID_IUnknown = (IID){0x000000000,0x0000,0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46};
#endif

