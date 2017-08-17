// Types

typedef unsigned int size_t;
typedef void (*PVFV)(void);
typedef int (*_onexit_t)(void);
typedef int time_t;
typedef struct _iob FILE;
typedef int (*_HANDLE_MATH_ERROR)(struct _exception *);

// Functions

void * malloc(size_t);
void free(void *);
char *_ltoa(long value,char *str,int radix );
int rand();
int strncpy(char *,char *,size_t);
void * calloc(size_t, size_t);
int atol(char *);
time_t time(time_t * ptime);
void srand(unsigned int);
int _ftol([[reko::arg(fpu)]]double);
double floor(double);
int printf(char *, ...);
int scanf(char * format, ...);
int sprintf(char *, char *, ...);
FILE * fopen(const char * file, const char * mode);
int fprintf(FILE * fp, char * format,...);
int fputs(const char *str, FILE *fp);
int fscanf(FILE * fp, char * format,...);
size_t fread(void *buffer, size_t size, size_t count, FILE *stream);
int fclose(FILE * fp);
int _mkdir(const char *dirname);
double _CIpow([[reko::arg(fpu)]]double, [[reko::arg(fpu)]] double); 
double _CIacos([[reko::arg(fpu)]]double);
double _CIasin([[reko::arg(fpu)]]double);
double _CIatan([[reko::arg(fpu)]]double);
double _CIatan2([[reko::arg(fpu)]]double, [[reko::arg(fpu)]] double);
double _CIcos([[reko::arg(fpu)]]double);
double _CIfmod([[reko::arg(fpu)]]double, [[reko::arg(fpu)]] double);
double _CIsin([[reko::arg(fpu)]]double);
double _CIsqrt([[reko::arg(fpu)]]double);
char * strncpy(char *,char *,size_t);
unsigned int _controlfp(unsigned int new, unsigned int mask);
_onexit_t __dllonexit(_onexit_t func, PVFV **pbegin, PVFV **pend);
void __set_app_type(int);
int * __p__fmode();
int * __p__commode();
int __getmainargs(int *, char ***, char ***, int);
void __setusermatherr(_HANDLE_MATH_ERROR pf);
void exit(int code);
void _assert(char cond, char * file, int line);

void _amsg_exit(int n);
void _initterm(PVFV * fStart, PVFV * fEnd);
int _initterm_e(PVFV * fStart, PVFV * fEnd);
void * _encoded_null();
void * _decode_pointer(void *);
