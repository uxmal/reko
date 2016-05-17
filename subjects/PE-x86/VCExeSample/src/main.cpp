
#include <stdio.h>


extern void test1(char *arg1, int arg2, char *arg3, float arg4);


int main(int argc, char *argv[])
{
    test1(argv[0], argc, "test123", 1.0);
}

void test1(char *arg1, int arg2, char *arg3, float arg4)
{
    printf("%s %d %s %f", arg1, arg2, arg3, arg4);
}

extern "C" __declspec(dllexport) void test2(int a)
{
    test1("1", 2, "3", 4.123);
    if (a == 0)
        test1("5", 6, "7", 8.567);
}
