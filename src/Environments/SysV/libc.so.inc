typedef struct _iob FILE;
typedef struct _io_iob _IO_FILE;
typedef void (*sighandler_t)(int);
typedef __int64 off64_t;
typedef int wint_t;
typedef int int32_t;


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
typedef void * mbstate_t;			// incorrect?
typedef int nl_item;
typedef struct _DIR DIR;
struct sigaction
{
};
typedef struct sigset_t sigset_t;
typedef struct jmp_buf jmp_buf;
typedef struct _acl * acl_t;
typedef unsigned int acl_tag_t;
struct acl_entry 
{
	acl_tag_t		ae_tag;
};
typedef struct acl_entry acl_entry_t;
typedef char *security_context_t;

const unsigned short * * __ctype_b_loc (void);
size_t __ctype_get_mb_cur_max(void);
int32_t * * __ctype_tolower_loc(void);
int32_t * * __ctype_toupper_loc(void);
int __cxa_atexit(void (*func) (void *), void * arg, void * dso_handle);

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

char * strerror ( int errnum );
void error(int status, int errnum, const char *format, ...);
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
int _xstat(int version, const char * path, struct stat * statbuf);
void *mmap64(void *addr, size_t len, int prot, int flags, int fildes, off64_t off);
char * strstr(char *, char *);
struct tm  * localtime(time_t t);
int mbtowc (wchar_t* pwc, const char* pmb, size_t max);
void * memmovve(void *, void *src, size_t cb);
int tcsetattr(int fd, int optional_actions, struct termios *termios_p);
int munmap(void *addr, size_t length);
int utime(const char *filename,  struct utimbuf *times);
time_t time (time_t *result);
int __vfprintf_chk(FILE * fp, int flag, const char * format, va_list ap);
int chmod(const char *pathname, mode_t mode);
int ferror(FILE *);
int fileno(FILE*);
int fnmatch(const char *pattern, const char *string, int flags);
FILE * fopen(const char * file, const char * mode);
int fputs_unlocked(const char *s, FILE *stream);
void atexit(void(*func)(void));
uid_t getuid(void);
uid_t geteuid(void);
struct passwd *getpwuid(uid_t uid);

int ioctl(int fd, unsigned long request, ...);
int isatty(int fd);
int iswcntrl(wint_t ch);
int iswprint( wint_t ch );
int lgetfilecon(const char *path, security_context_t *con);
struct lconv* localeconv (void);
off_t lseek(int fd, off_t offset, int whence);

size_t mbrtowc( wchar_t* pwc, const char* s, size_t n, mbstate_t* ps );
int mbsinit(const mbstate_t *ps);
size_t mbstowcs(wchar_t *dest, const char *src, size_t n);
time_t mktime(struct tm *tm);
int memcmp(void *, void *, size_t);
void *memmove(void *dest, const void *src, size_t n);
void *mempcpy(void *dest, const void *src, size_t n);
char *nl_langinfo(nl_item item);
DIR *opendir(const char *name);
void perror(const char *msg);
int putchar(int ch);
int raise(int sig);
struct dirent *readdir(DIR *dirp);
ssize_t readlink(const char *path, char *buf, size_t bufsiz);
char *setlocale(int category, const char *locale);
int sigaction(int signum, const struct sigaction *act, struct sigaction *oldact);
int sigaddset(sigset_t *set, int signum);
int sigemptyset(sigset_t *set); 
int sigismember(const sigset_t *set, int signum); 
int sigprocmask(int how, const sigset_t *set, sigset_t *oldset);
char *stpcpy(char *dest, const char *src);
char *stpncpy(char *dest, const char *src, size_t n);
int strcoll ( const char * str1, const char * str2 );
size_t strftime(char *s, size_t max, const char *format, const struct tm *tm);
unsigned long int strtoul(const char *nptr, char **endptr, int base);
pid_t tcgetpgrp(int fd);
int tcsetpgrp(int fd, pid_t pgrp);
char * textdomain (const char * domainname);
int ungetc(int c, FILE *stream);
int unlink(char * path);
size_t wcstombs(char *dest, const wchar_t *src, size_t n);
int wcswidth(const wchar_t *s, size_t n);
int wcwidth(wchar_t c);

size_t __fpending(FILE *stream);
int __freading(FILE *stream);
int __fxstat (int vers, int fd, struct stat *buf);
void * __memcpy_chk(void * dest, const void * src, size_t len, size_t destlen);
void * __mempcpy_chk(void * dest, const void * src, size_t len, size_t destlen);
int __lxstat(int version, char * __path, struct stat * __statbuf);
int __overflow(FILE *, int c);
unsigned long int __strtoul_internal(const char * __nptr, char * * __endptr, int __base, int __group);
int __uflow(FILE *f);
int __xstat(int ver, const char * path, struct stat * stat_buf);
void _exit(int status);
int _obstack_begin(struct obstack *, int, int, void *(*) (long), void (*) (void *));
void _obstack_newchunk(struct obstack *, int);
int _setjmp(jmp_buf env); 
int acl_extended_file(const char *path_p);
int acl_get_entry(acl_t acl, int entry_id, acl_entry_t *entry_p); 
int acl_get_tag_type(acl_entry_t entry_d, acl_tag_t *tag_type_p); 
long int atol(const char *);
char * bindtextdomain (const char * domainname, const char * dirname);
int closedir(DIR *dirp);
int dirfd(DIR *dirp);
void error(int status, int errnum, const char *format, ...);
int fflush_unlocked(FILE *stream);
int fgetfilecon(int fd, security_context_t *con); 
void freecon(security_context_t con); 
int fscanf ( FILE * stream, const char * format, ... );
int fseeko(FILE *stream, off_t offset, int whence);
size_t fwrite_unlocked(const void *ptr, size_t size, size_t n, FILE *stream);
int getfilecon(const char *path, security_context_t *con);
struct group *getgrgid(gid_t gid);
struct group *getgrnam(const char *name);
int getopt_long(int argc, char * const argv[], const char *optstring, const struct option *longopts, int *longindex);
struct passwd *getpwnam(const char *name);
int gettimeofday(struct timeval *tv, struct timezone *tz);
