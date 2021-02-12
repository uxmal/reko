
#include <stdio.h>


struct struct_type
{
    int i;
    double d;
};

static struct_type *gbl_s;

static struct_type *GetMin(struct_type *s1, struct_type *s2)
{
    if (s1->i < s2->i)
        return s1;
    if (s1->i < s2->i)
        return s2;
    if (s1->d < s2->d)
        return s1;
    else
        return s2;
}

int main(int argc, char *argv[])
{
    struct_type s1, s2, *res;
    s1.i = 0;
    s1.d = 1.0;
    s2.i = 10;
    s2.d = 11.0;
    res = GetMin(&s1, &s2);
    s1.i = 100;
    res->i = 5;
    res->d = 5.0;
    printf("%d %f %d %f\n", s1.i, s1.d, s2.i, s2.d);
    gbl_s = &s2;
    s2.i = 2;
    s2.d = 2.0;
    gbl_s->i = 3;
    gbl_s->d = 3.0;
    printf("%d %f\n", s2.i, s2.d);
}
