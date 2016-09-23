
#include <stdio.h>


typedef struct cdecl_class cdecl_class;
typedef struct cdecl_class *cdecl_class_ptr;
typedef struct cdecl_class_vtbl cdecl_class_vtbl;

struct cdecl_class_vtbl
{
    void (*method00)(cdecl_class *);
    void (*method04)(cdecl_class *, int);
    int (*sum)(cdecl_class *, int, int);
};

struct cdecl_class
{
    cdecl_class_vtbl *vtbl;
};

static cdecl_class_ptr gbl_c;

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

extern "C" __declspec(dllexport) void test3(cdecl_class *c)
{
    c->vtbl->method04(c, 1000);
}

extern "C" __declspec(dllexport) void test4()
{
    gbl_c->vtbl->method00(gbl_c);
}

extern "C" __declspec(dllexport) void test5()
{
    (( void (*)(cdecl_class *, int, float))gbl_c->vtbl->method04)(gbl_c, 999, 1000.1);
}

extern "C" __declspec(dllexport) void test6(cdecl_class *c, int a, int b)
{
    int sum;
    sum = c->vtbl->sum(c, a, b);
    c->vtbl->method04(c, sum);
}
