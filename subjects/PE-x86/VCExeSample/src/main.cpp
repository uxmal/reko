
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
