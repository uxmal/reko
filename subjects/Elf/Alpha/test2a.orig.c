#include <stdio.h>

static int theNumber = 1337;

int getNumber() {
	return (int)(theNumber * theNumber / 56.2);
}

int magic(int argument) {
	printf("Nice %d!\n", argument);
	return -getNumber();
}

char* numbers(int index) {
	switch (index) {
	case 0: return "zero";
	case 1: return "one";
	case 2: return "two";
	case 3: return "tres";
	case 4: return "quattro";
	case 5: return "fümf";
	case 6: return "shest";
	case 7: return "sju";
	case 8: return "otto";
	case 9: return "nueve";
	case 10: return "dix";
	case 11: return "undici";
	case 12: return "tolv";
	case 13: return "trinatsat";
	case 14: return "quatorze";
	case 15: return "fümfzehn";
	default: return "invalid";
	}
}

int switchy() {
	int baz;
	switch ((baz = getNumber())) {
	case 1338:
		return 1;
	case 1337:
		printf("ok\n");
		return 0;
	case 0:
		return 2;
	case 3:
		printf("no3\n");
		return -2;
	case 4:
		printf("no4\n");
		return -10;
	case 5:
		printf("no5\n");
		return 1;
	case 81:
		magic(baz / 2);
		break;
	case 9012:
		magic(baz * 2);
		break;
	case 2017:
		magic(baz - 42);
		break;
	case 75295:
		magic(baz + 42);
		break;
	}
}

int main() {
	printf("Hello Reko\n");

	int result = switchy();
	char* str = numbers(result % 16);
	return 0;
}
