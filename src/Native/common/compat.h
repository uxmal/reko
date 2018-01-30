#ifndef __COMPAT_H
#define __COMPAT_H

#ifndef _WINDOWS
void OutputDebugString(char *lpOutputString);
void DebugBreak();
#else
extern void DebugBreak();
#endif

#endif