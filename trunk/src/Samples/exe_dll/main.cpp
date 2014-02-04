#include <stdio.h>
#include "types.h"

int main(int arch, char * argv[])
{
	SampleClass c(argv[1]);
	int n = c.atoi() + c.cdecl_method() + c.stdcall_method();
	printf("Result: {0}\n", n);
}
