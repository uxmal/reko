// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// #include "targetver.h"

#if _WINDOWS
# define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
# include <windows.h>

# include <stdio.h>
# include <stdint.h>
# include <objbase.h>
# include <stdarg.h>
#else
# include <cstring>
# include "types.h"
#endif

#include "../../../external/Capstone/X86/include/capstone.h"

// TODO: reference additional headers your program requires here
