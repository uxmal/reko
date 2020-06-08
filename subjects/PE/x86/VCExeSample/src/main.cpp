
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

class thiscall_class
{
public:
    virtual void set_double(double) = 0;
    virtual double modify_double(int,double) = 0;
};

struct nested_structs_type
{
    int a;
    struct {
        int b;
        int c;
    } str;
    int d;
};

static cdecl_class_ptr gbl_c;
static thiscall_class *gbl_thiscall;

static int remainder;
static int quotient;

static nested_structs_type gbl_nested_structs;

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

extern "C" __declspec(dllexport) double test7(double d)
{
    if (d > 1.0)
        gbl_thiscall->set_double(d);
    return gbl_thiscall->modify_double(13, d);
}

extern "C" __declspec(dllexport) void nested_if_blocks_test8(double d)
{
    gbl_thiscall->modify_double(-1, d);
    if (d != 123.0)
        if (d < 321.0)
            gbl_thiscall->set_double(d);
    test6(gbl_c, 6, 7);
}

extern "C" __declspec(dllexport) void loop_test9(float f)
{
    for(int i = 0; i < gbl_thiscall->modify_double(i, f); i++)
        gbl_thiscall->set_double(f);
}

extern "C" __declspec(dllexport) void const_div_test10(int f)
{
    __asm
    {
        mov eax, 0x0000000A
        mov ecx, 0x00000003
        mov edx, f
        test edx, edx
        jz store
        xor edx, edx
        div ecx
        mov ecx, edx
store:
        mov remainder, ecx
        mov quotient, eax
    }
}

extern "C" __declspec(dllexport) void loop_test11(double d)
{
    int i = 5;
    while (i > 0)
    {
        if ((i % 2) == 0)
            loop_test9(d);
        else
            nested_if_blocks_test8(d);
        i--;
    }
}

extern "C" __declspec(dllexport) void nested_structs_test12(nested_structs_type *str)
{
    str->a = 1;
    str->str.b = 2;
    str->str.c = 3;
    str->d = 4;
}

extern "C" __declspec(dllexport) void nested_structs_test13(nested_structs_type *str)
{
    nested_structs_test12(str);
}

extern "C" __declspec(dllexport) void gbl_nested_structs_test14()
{
    gbl_nested_structs.a = 5;
    gbl_nested_structs.str.b = 6;
    gbl_nested_structs.str.c = 7;
    gbl_nested_structs.d = 8;
}
