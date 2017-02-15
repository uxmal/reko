#include <stdio.h>

#include "driver.h"

int main()
{
	MathService math;
	math.add(3);
	math.sub(1);
	math.mul(5);

	printf("math: count: %d, accum %d\n", MathService::count, math.total());
    return 0;
}

