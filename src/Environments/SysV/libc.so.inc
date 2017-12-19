typedef struct _iob FILE;
typedef struct _io_iob _IO_FILE;
typedef void (*sighandler_t)(int);
typedef __int64 off64_t;
struct timespec
{
	long	tv_sec;
	long	tv_nsec;
};
typedef unsigned int size_t;
typedef unsigned long long off_t;
typedef int clockid_t;
typedef long time_t;
typedef void * va_list;	
typedef int mode_t;
typedef int uid_t;
typedef int gid_t;
struct passwd {
    char   *pw_name;
    char   *pw_passwd;
    uid_t   pw_uid;   
    gid_t   pw_gid;   
    char   *pw_gecos; 
    char   *pw_dir;   
    char   *pw_shell; 
};
struct stat
{
};
typedef int pid_t;

int __libc_start_main(int (*main) (int, char **, char **), int argc, char ** ubp_av, void (*init) (void), void (*fini) (void), void (*rtld_fini) (void), void (* stack_end));
int scanf(char * format, ...);
int __isoc99_scanf(char * format, ...);
int printf(char * format, ...);
int sprintf(char * buffer, char * format, ...);
void free(void * p);

void __assert_fail(const char * assertion, const char * file, unsigned int line, const char * function);

int atoi(const char *);
long atol(const char *);

int getpagesize(void);


void *mmap(void *addr, size_t length, int prot, int flags, int fd, off_t offset);
void *mmap2(void *addr, size_t length, int prot, int flags, int fd, off_t pgoffset);
int munmap(void *addr, size_t length);

int mprotect(void *addr, size_t len, int prot);

int mlock(const void *addr, size_t len);
int mlock2(const void *addr, size_t len, int flags);
int munlock(const void *addr, size_t len);
int mlockall(int flags);
int munlockall(void);



void * malloc(size_t size);
size_t strlen(char *);
char * strchr(char *, char);
char * strrchr(char *, char);
void * memcpy(void *, void *, size_t);
int strcmp(char *,char *);
char * dcgettext(char *, char *, int);
void exit(int);
char * getenv(char *);
int fputc(int char, FILE * stream);
int fputs(const char * s, FILE * stream);
int putc(int c, FILE * stream);
int putchar(int c);
int puts(const char * s);
int getc(FILE *);
int fgetc(FILE *);
char * fgets(char *, int, FILE *);
int strncasecmp(char *, char *, int n);
int strcasecmp(char *, char *);
int strncpy(char *, char *, int n);
char * strcat(char *, char *);
int fcntl(int fd, int cmd);
void * calloc(size_t num, size_t size);
void abort();
int open(char * filename, int oflag);
int open64(char * filename, int oflag);
FILE * fopen64(char * filename, char * mode);
FILE* fdopen(int fd, char * mode);
sighandler_t signal(int signal, sighandler_t handler);
int close(int);
void * realloc(void *, size_t);
int clock_gettime(clockid_t, struct timespec *);
long sysconf(int name);
void * memset(void *, int value, size_t num);
int fflush(FILE * fp);
char * strtok(char * str, const char * delimiters);
char * strcpy(char * dst, char * src);
int  strncmp(char * dst, char * src, int n);
int fclose(FILE * fp);
ssize_t read(int fd, void * buf, size_t count);
ssize_t write(int fd, const void* buf, size_t count);
int strtol(char * s);
int socket(int dom, int type, int proto);
int feof(FILE * fp);
int fprintf(FILE * fp, char * format,...);
int setvbuf(FILE * fp, char * buf, int mode, size_t size);
int _IO_getc(FILE * fp);
int fileno(FILE*);
int ferror(FILE *);

char * strerror ( int errnum );
int mkdir(char *path,int);
size_t fread ( void * ptr, size_t size, size_t count, FILE * stream );
size_t fwrite (  void * ptr, size_t size, size_t count, FILE * stream );
int __printf_chk(int flag, const char * format);
int __fprintf_chk(FILE *, int flag, const char * format);
int __sprintf_chk(char * str, int flag, size_t strlen, const char * format);
int __snprintf_chk(char * str, size_t maxlen, int flag, size_t strlen, const char * format);
int * __errno_location();
void __assert_fail(const char * assertion, const char * file, unsigned int line, const char * function);
char * strpbrk(char *, char *);
void * __rawmemchr(const void * s, int c);
void __stack_chk_fail(void);
int _IO_putc(int __c, _IO_FILE * __fp);
void *mmap64(void *addr, size_t len, int prot, int flags, int fildes, off64_t off);
int memcmp(void *, void *, size_t);
char * strstr(char *, char *);
struct tm  * localtime(time_t t);
int mbtowc (wchar_t* pwc, const char* pmb, size_t max);
void * memmovve(void *, void *src, size_t cb);
int tcsetattr(int fd, int optional_actions, struct termios *termios_p);
int munmap(void *addr, size_t length);
int utime(const char *filename,  struct utimbuf *times);
time_t time (time_t *result);
int __vfprintf_chk(FILE * fp, int flag, const char * format, va_list ap);
int unlink(char * path);
int chmod(const char *pathname, mode_t mode);
int fnmatch(const char *pattern, const char *string, int flags);
FILE * fopen(const char * file, const char * mode);
void perror(const char *msg);
void atexit(void(*func)(void));
int _xstat(int version, const char * path, struct stat * statbuf);
uid_t getuid(void);
uid_t geteuid(void);
struct passwd *getpwuid(uid_t uid);
int putchar(int ch);
int fputs_unlocked(const char *s, FILE *stream);
void *memmove(void *dest, const void *src, size_t n);
void *mempcpy(void *dest, const void *src, size_t n);
unsigned long int strtoul(const char *nptr, char **endptr, int base);
int tcsetpgrp(int fd, pid_t pgrp);
char * textdomain (const char * domainname);
int ungetc(int c, FILE *stream);
size_t wcstombs(char *dest, const wchar_t *src, size_t n);
int wcswidth(const wchar_t *s, size_t n);
int wcwidth(wchar_t c);
