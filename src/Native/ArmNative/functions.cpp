/*
* Copyright (C) 1999-2025 John Källén.
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

#include "common/compat.h"

// Stand-alone functions.

#if _DEBUG || DEBUG
void Dump(const char * fmt, ...)
{
	va_list args;
	va_start(args, fmt);
#if _WIN32||_WIN64
	char buf[512];
	vsnprintf(buf, countof(buf), fmt, args);
	::strcat_s(buf, "\r\n");
	::OutputDebugStringA(buf);
// Use MessageBox for Release mode debugging.
//	::MessageBoxA(nullptr, buf, "Dump", MB_OK);
#else
	vfprintf(stderr, fmt, args);
	fputs("\n", stderr);
#endif
	va_end(args);
}
#endif
