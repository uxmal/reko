//$TODO: port over all the definitions from msvcrt.xml here;
// we have a C => XML translator, why not use it?

typedef (*PVFV)();
typedef int time_t;


void * malloc(size_t);
void free(void *);
char *_ltoa(long value,char *str,int radix );
int rand();
int strncpy(char *,char *,size_t);
void * calloc(size_t, size_t);
int atol(char *);
time_t time(time_t * ptime);
void srand(unsigned int);
int _ftol(double);
int printf(char *, ...);
int sprintf(char *, char *, ...);
// double _CIpow(double, double); //$HUH: args passed on FPU stack, non-standard!
char * strncpy(char *,char *,size_t);
void __set_app_type(int);
int * __p__fmode();
int * __p__commode();
int __getmainargs(int *, char ***, char ***, int);
void exit(int code);
void _assert(char cond, char * file, int line);

void _amsg_exit(int n);
void _initterm(PVFV fStart, PVFV fEnd);
int _initterm_e(PVFV fStart, PVFV fEnd);
void * _encoded_null();
void * _decode_pointer(void *);
