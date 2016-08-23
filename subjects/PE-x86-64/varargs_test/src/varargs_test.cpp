// varargs_test.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

void printfTest(
	int i,
	char *s,
	wchar_t *ws,
	float f,
	double lf,
	char c,
	wchar_t wc)
{
	printf("%i %s %S %f %lf %c %C", i,s, ws, f, lf, c, wc);
}

void scanfTest(
	int * i,
	char *s,
	wchar_t *ws,
	float * f,
	double *lf,
	char *c,
	wchar_t *wc)
{
	scanf("%i %30s %S %f %lf %c %C", i, s,  ws, f, lf, c, wc);
}


int main()
{
	printfTest(3, "hello", L"Hello!", 3.0F, 3.14, 'c', L'W');
	int i;
	char sBuf[30];	// Get your buffer overflow right here.
	wchar_t wsBuf[30];
	float f;
	double lf;
	char c;
	wchar_t wc;
	scanfTest(&i, sBuf, wsBuf, &f, &lf, &c, &wc);
    return 0;
}

