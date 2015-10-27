#include <windows.h>

int WinMain(HINSTANCE hinst, HINSTANCE hinstPrev, LPSTR cmdLine, int nShow)
{
    char buf[30];
	if (!cmdLine || !cmdLine[0]) {
		return 0;
	}
	
	LPSTR next = CharNext(cmdLine);
    if (*next != 0)
    {
		OutputDebugString("Yes");
    } 
    else 
    {
		OutputDebugString("No");
    }
    return 0;
}
