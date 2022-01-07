#include <stdio.h>
#include <stdlib.h>

__declspec(dllexport)
int reko_array_byref(char *code_buf){
	for(int i=0; i<32; i++){
		code_buf[i] = i;
	}
	return 0;
}

unsigned char gvar_n[32];

__declspec(dllexport)
int reko_array_n(){
	reko_array_byref(&gvar_n[0]);
	return 0;
}

__declspec(dllexport)
int reko_array_local(){
        unsigned char code_buf[32];

	for(int i=0; i<sizeof(code_buf); i++){
                code_buf[i] = i;
        }
        return 0;
}


