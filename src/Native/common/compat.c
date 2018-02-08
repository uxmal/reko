#include <stdio.h>

#ifndef _MSC_VER
void OutputDebugString(char *lpOutputString){
	fputs(lpOutputString, stderr);
}

void DebugBreak(){
	
}
#endif