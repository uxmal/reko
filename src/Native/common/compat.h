#ifndef __COMPAT_H
#define __COMPAT_H

#ifdef __cplusplus
extern "C" {
#endif

#ifndef _MSC_VER
void OutputDebugString(char *lpOutputString);
void DebugBreak();
#define countof(_arr) (sizeof(arr)/sizeof arr[0])
#else
#define countof _countof
#endif

#endif

#ifdef __cplusplus
}

#endif