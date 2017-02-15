#include <stdio.h>
#include <windows.h>

int main(int argc, char *argv[]){
    HMODULE library = LoadLibraryA(argv[1]);
    printf("handle: 0x%x\n", library);
    DWORD error = GetLastError();
    printf("error: 0x%x\n", error);
    return 0;
}
