#ifndef __COMPAT_H
#define __COMPAT_H

#ifdef __cplusplus
extern "C" {
#endif

#ifndef _MSC_VER
void OutputDebugString(char *lpOutputString);
void DebugBreak();

template < typename T, size_t N >
size_t countof( T ( & arr )[ N ] )
{
    return std::extent< T[ N ] >::value;
}

#else
#define countof _countof
#endif

#endif

#ifdef __cplusplus
}

#endif