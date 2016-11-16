
long double _pow(long double a, long double b) {
    long double c = 1;
    for (int i=0; i<b; i++)
        c *= a;
    return c;
}

long double _fact(long double x) {
    long double ret = 1;
    for (int i=1; i<=x; i++) 
        ret *= i;
	__asm__("nop");
    return ret;
}

long double _sin(long double x) {
    long double y = x;
    long double s = -1;
    for (int i=3; i<=100; i+=2) {
        y+=s*(_pow(x,i)/_fact(i));
        s *= -1;
    }  
    return y;
}
long double _cos(long double x) {
    long double y = 1;
    long double s = -1;
    for (int i=2; i<=100; i+=2) {
        y+=s*(_pow(x,i)/_fact(i));
        s *= -1;
    }  
    return y;
}
long double _tan(long double x) {
     return (_sin(x)/_cos(x));  
}



int main()
{
	long double x = 3.14;
	_pow(x,x);
	_fact(x);
	_sin(x);
	_cos(x);
	_tan(x);
return 0;	
}