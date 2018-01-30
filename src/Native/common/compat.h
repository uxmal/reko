#ifndef __COMPAT_H
#define __COMPAT_H

#ifdef __cplusplus
extern "C" {
#endif

#ifndef _MSC_VER
void OutputDebugString(char *lpOutputString);
void DebugBreak();
#endif

#endif

#ifdef __cplusplus
}

#endif