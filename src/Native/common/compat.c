#include <stdio.h>

#ifndef _WINDOWS
void OutputDebugString(char *lpOutputString){
	fputs(lpOutputString, stderr);
}

void DebugBreak(){

}
#endif