#include <stdio.h>
#include <stdlib.h>
#include <string.h>

struct foo {
	void *(*ptr1)(int x, int y);
	int (*ptr2)(void *foo, char bar);
};

void *my1(int x, int y){
	void *mem = calloc(x, y);
	return mem;
}

int my2(void *foo, char bar){
	memcpy(foo, &bar, sizeof(bar));
	return 1;
}

int branches(int check, int against){
	if(
		check < against &&
		check * 2 < against * 2 &&
		check * 3 < against * 3 &&
		check * 4 < against * 4 &&
		check / 2 < against / 2
	){
		void *mem = my1(check, against);
		free(mem);
		return 0;
	}
	return -1;
}

int main(int argc, char *argv[]){
	struct foo x = {
		.ptr1 = &my1,
		.ptr2 = &my2
	};

	struct foo *px = &x;

	void *mem = px->ptr1(1, 5);
	px->ptr2(mem, 'x');

	puts("done\n");
	free(mem);
	return 0;
}

