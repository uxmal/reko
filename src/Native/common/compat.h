#ifndef __COMPAT_H
#define __COMPAT_H

#ifdef __cplusplus
extern "C" {
#endif

#ifndef _WINDOWS
void OutputDebugString(char *lpOutputString);
void DebugBreak();
#else
extern void DebugBreak();
#endif

#endif

#ifdef __cplusplus
}

#endif