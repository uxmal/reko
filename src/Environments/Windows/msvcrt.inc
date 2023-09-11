// Types

typedef unsigned int size_t;
typedef size_t intptr_t;
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
[[reko::returns(register,"eax")]]int _ftol([[reko::arg(fpu)]]double);
double floor(double);
int puts(const char * s);
int printf(char *, ...);
int scanf(char * format, ...);
int sprintf(char *, char *, ...);
FILE * fopen(const char * file, const char * mode);
int fflush(FILE * fp);
int fprintf(FILE * fp, char * format,...);
int fputs(const char *str, FILE *fp);
int fscanf(FILE * fp, char * format,...);
size_t fread(void *buffer, size_t size, size_t count, FILE *stream);
int fclose(FILE * fp);
int _fcloseall();
int _mkdir(const char *dirname);

[[reko::returns(fpu)]] double _CIpow([[reko::arg(fpu)]]double, [[reko::arg(fpu)]] double); 
[[reko::returns(fpu)]] double _CIacos([[reko::arg(fpu)]]double);
[[reko::returns(fpu)]] double _CIasin([[reko::arg(fpu)]]double);
[[reko::returns(fpu)]] double _CIatan([[reko::arg(fpu)]]double);
[[reko::returns(fpu)]] double _CIatan2([[reko::arg(fpu)]]double, [[reko::arg(fpu)]] double);
[[reko::returns(fpu)]] double _CIcos([[reko::arg(fpu)]]double);
[[reko::returns(fpu)]] double _CIfmod([[reko::arg(fpu)]]double, [[reko::arg(fpu)]] double);
[[reko::returns(fpu)]] double _CIsin([[reko::arg(fpu)]]double);
[[reko::returns(fpu)]] double _CIsqrt([[reko::arg(fpu)]]double);
[[reko::returns(fpu)]] double _CIexp([[reko::arg(fpu)]]double);
char * strncpy(char *,char *,size_t);
void _fpreset();
unsigned int _controlfp(unsigned int new, unsigned int mask);
unsigned int _control87(unsigned int new, unsigned int mask);
_onexit_t __dllonexit(_onexit_t func, PVFV **pbegin, PVFV **pend);
void __set_app_type(int);
int * __p__fmode();
int * __p__commode();
int __getmainargs(int *, char ***, char ***, int);
void __setusermatherr(_HANDLE_MATH_ERROR pf);
void exit(int code);
void _assert(char cond, char * file, int line);


void (__cdecl *signal(  
   int sig,   
   void (__cdecl *func ) (int, ...)))   
   (int);  


void _amsg_exit(int n);
void _initterm(PVFV * fStart, PVFV * fEnd);
int _initterm_e(PVFV * fStart, PVFV * fEnd);
void * _encoded_null();
void * _decode_pointer(void *);

int _chdir(const char *dirname);
intptr_t _findfirst(const char * filespec, struct _finddata_t * fileinfo);
int _findnext(intptr_t handle, struct _finddata_t * fileinfo);
char * _getcwd(char * buffer, int maxlen);
int _isnan(double x);  
int _stat(const char *path,struct _stat *buffer); 
double cos(double);
double fabs(double);
int fgetc(FILE *);
char * fgets(char * buf, int size, FILE *);
int fputc(int, FILE *);
int fseek(FILE *, long offset, int origin);
long ftell(FILE *);
size_t fwrite(const void *, size_t size, size_t count, FILE *);
double ldexp(double,int);
double log(double);
double pow(double, double);
char * setlocale(int category, const char *locale);
double sin(double);
double sqrt(double);
int sscanf(char *, const char  * fmt, ...);
