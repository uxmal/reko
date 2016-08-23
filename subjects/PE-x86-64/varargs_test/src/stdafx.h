// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// Yes, we're disabling this so we can build with scanf, an
// inherently dangerous thing to do because of the risk of
// buffer overflows.
#define _CRT_SECURE_NO_WARNINGS

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>

