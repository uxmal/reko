int fib10;

int fib(int n)
{
	if (n > 1)
	{
		return fib(n-1) + fib(n-2);
	}
	else
		return n;
}

int main()
{
	fib10 = fib(10);
}
