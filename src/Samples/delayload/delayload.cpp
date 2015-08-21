#include <Windows.h>
#pragma comment(lib, "delayimp")

int WinMain(HINSTANCE hinst, HINSTANCE hinstPrev, LPSTR szArgs, int nCmdShow)
{
	HMODULE module = ::GetModuleHandle(nullptr);
	HWND hwnd = ::GetDesktopWindow();
	hwnd = ::GetForegroundWindow();
	hwnd = ::GetFocus();
	return 0;
}