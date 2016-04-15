#include<stdio.h>
main()
{
    double d = 0.01;
    __int64 ll = *(__int64 *) &d;
    printf("%lg %I64x", d, ll);
}
