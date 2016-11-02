#pragma once


#define MYAPI __declspec(dllimport)

// Exported global variables
extern struct _RTL_CRITICAL_SECTION MYAPI exported_critical_section;
extern int MYAPI exported_int;

// Exported functions
int MYAPI slow_and_safe_increment(int delta);
