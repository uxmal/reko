#include <windows.h>

void reverse_itoa(unsigned int i, char * buf, int size);
int my_strlen(char * s);
int fib(int n);

int WinMain(HINSTANCE hinst, HINSTANCE hinstPrev, LPSTR cmdLine, int nShow)
{
	char buffer[100];
	reverse_itoa(fib(10), buffer, sizeof(buffer));
}

int fib(int n)
{
	if (n < 1)
		return 1;
	else
		return fib(n-1) + fib(n-2);
}

int my_strlen(char * s)
{
	int c = 0;
	while (*s++)
		++c;
	return c;
}

void reverse_itoa(unsigned int i, char * buf, int size)
{
	char * p = buf;
	while (p < buf+size-1)
	{
		int digit = i % 10;
		i /= 10;
		*p++ = digit + '0';
	}
	*p = '\0'; 
}