/* file system independent - volume parameters */

struct vpfsi {
    unsigned long  vpi_vid;        /* 32 bit volume ID */
    unsigned long  vpi_hDEV;       /* handle to device driver */
    unsigned short vpi_bsize;      /* sector size in bytes */
    unsigned long  vpi_totsec;     /* total number of sectors */
    unsigned short vpi_trksec;     /* sectors / track */
    unsigned short vpi_nhead;      /* number of heads */
    char           vpi_text[12];   /* ASCIIZ volume name */
    void __far *   vpi_pDCS;       /* device capability structure */
    void __far *   vpi_pVCS;       /* volume characteristics */
    unsigned char  vpi_drive;      /* drive (0=A) */
    unsigned char  vpi_unit;       /* unit code */
};

/* file system dependent - volume parameters */

struct vpfsd {
    char           vpd_work[36];   /* work area */
};

/* bit values for cdi_flags (state of cdfsd structure */
/* //$TODO Reko's C Parser can't handle preprocessor direct yet
#define CDI_ISVALID 0x80		// format is known
#define CDI_ISROOT  0x40		// cur dir == root 
#define CDI_ISCURRENT	0x20
*/

/* file system independent - current directories */

struct cdfsi {
    unsigned short cdi_hVPB;           /* VPB handle for associated device */
    unsigned short cdi_end;            /* offset to root of path */
    char           cdi_flags;          /* FS independent flags */
    char           cdi_curdir[260];    /* text of current directory */
};

/* file system dependent - current directories */

struct cdfsd {
    char           cdd_work[8];        /* work area */
};

/* file system independent - file instance */

struct sffsi {
    unsigned long   sfi_mode;       /* access/sharing mode */
    unsigned short  sfi_hVPB;       /* volume info. */
    unsigned short  sfi_ctime;      /* file creation time */
    unsigned short  sfi_cdate;      /* file creation date */
    unsigned short  sfi_atime;      /* file access time */
    unsigned short  sfi_adate;      /* file access date */
    unsigned short  sfi_mtime;      /* file modification time */
    unsigned short  sfi_mdate;      /* file modification date */
    unsigned long   sfi_size;       /* size of file */
    unsigned long   sfi_position;   /* read/write pointer */

/* the following may be of use in sharing checks */

    unsigned short  sfi_UID;        /* user ID of initial opener */
    unsigned short  sfi_PID;        /* process ID of initial opener */
    unsigned short  sfi_PDB;        /* PDB (in 3.x box) of initial opener */
    unsigned short  sfi_selfsfn;    /* system file number of file instance */
    unsigned char   sfi_tstamp;     /* time stamp flags */
    unsigned short  sfi_type;       /* type of object opened */
    unsigned long   sfi_pPVDBFil;   /* performance counter data block pointer */
    unsigned char   sfi_DOSattr;    /* DOS file attributes D/S/A/H/R */
};

/* file system dependent - file instance */

struct sffsd {
    char            sfd_work[30];   /* work area */
};

/* file system independent - file search parameters */

struct fsfsi {
        unsigned short  fsi_hVPB;       /* volume info. */
};

/* file system dependent - file search parameters */

struct fsfsd {
        char            fsd_work[24];   /* work area */
};

struct FileLock {
    unsigned long FileOffset;   /* offset where the lock/unlock begins  */
    unsigned long RangeLength;  /* length of region locked/unlocked     */
};

struct PageCmdHeader {
   unsigned char  InFlags;      /* Input Flags                        */
   unsigned char  OutFlags;     /* Output Flags - must be 0 on entry  */
   unsigned char  OpCount;      /* Number of operations               */
   unsigned char  Pad;          /* Pad for DWORD alignment            */
   unsigned long  Reserved1;    /* Currently Unused                   */
   unsigned long  Reserved2;    /* Currently Unused                   */
   unsigned long  Reserved3;    /* Currently Unused                   */
   struct PageCmd PageCmdList;  /* Currently Unused                   */ 
};

struct PageCmd {    
   unsigned char Cmd;           /* Cmd Code (Read,Write,Verify)       */    
   unsigned char Priority;      /* Same values as for req packets     */    
   unsigned char Status;        /* Status byte                        */    
   unsigned char Error;         /* I24 error code                     */
   unsigned long Addr;          /* Physical(0:32) or Virtual(16:16)   */
   unsigned long FileOffset;    /* Byte Offset in page file  */
};

struct CmdLock {
    unsigned short Cmd;       /* 0 for lock operations         */
    unsigned short LockCnt;   /* number of locks that follow   */
    unsigned long  TimeOut;   /* ms time-out for lock success  */
};

struct Lock {
    unsigned short Share;       /* 0 for exclusive, 1 for read-only  */
    long           Start;       /* start of lock region              */
    long           Length;      /* length of lock region             */
};

struct CmdUnlock {
    unsigned short Cmd;         /* 1 for unlock operations        */
    unsigned short UnlockCnt;   /* Number of unlocks that follow  */
};

struct UnLock {
    long Start;                 /* start of locked region         */
    long Length;                /* length of locked region        */
};

struct CmdSeek {
    unsigned short Cmd;       /* 2 for seek operation         */
    unsigned short Method;    /* 0 for absolute               */
                              /* 1 for relative to current    */
                              /* 2 for relative to EOF        */
    long           Position;  /* file seek position or delta  */
    long           Actual;    /* actual position seeked to    */
};

struct CmdIO {
    unsigned short Cmd;        /* 3 for read, 4 for write      */
    void __far * Buffer;       /* pointer to the data buffer   */
    unsigned short BufferLen;  /* number of bytes requested    */
    unsigned short Actual;     /* number of bytes transferred  */
};

struct FileFindBuf {
    unsigned short dateCreate;
    unsigned short timeCreate;
    unsigned short dateAccess;
    unsigned short timeAccess;
    unsigned short dateWrite;
    unsigned short timeWrite;
    long           cbEOF;
    long           cbAlloc;
    unsigned short attr;
    unsigned char  cbName;
    unsigned char  szName[];
};

struct FileFromFindBuf {
    long           position;    /* position given to FSD on following */
                                /* FS_FINDFROMNAME call               */
    unsigned short dateCreate;
    unsigned short timeCreate;
    unsigned short dateAccess;
    unsigned short timeAccess;
    unsigned short dateWrite;
    unsigned short timeWrite;
    long           cbEOF;
    long           cbAlloc;
    unsigned short attr;
    unsigned char  cbName;
    unsigned char  szName[];
};

union argdat {

    /* pArgType = 1, FileHandle directed case */

    struct sf {
        struct sffsi __far * psffsi;
        struct sffsd __far * psffsd;
    };

    /* pArgType = 2, Pathname directed case */

    struct cd {
        struct cdfsi __far * pcdfsi;
        struct cdfsd __far * pcdfsd;
        char __far *         pPath;
        unsigned short       iCurDirEnd;
    };

    /* pArgType = 3, FSD Name directed case */
    /* pArgDat is Null                      */
};

struct EASizeBufStruc {
    unsigned short easb_MaxEASize;     /* Max size of an individual EA */
    unsigned long easb_MaxEAListSize;  /* Max full EA list size        */
};

union npoper {
    struct phs_param phs;
    struct npi_param npi;
    struct npr_param npr;
    struct npw_param npw;
    struct npq_param npq;
    struct npx_param npx;
    struct npp_param npp;
    struct npt_param npt;
    struct qnps_param qnps;
    struct npc_param npc;
    struct npd_param npd;
};

/* Get/SetPhandState parameter block */

struct phs_param {
    short phs_len;
    short phs_dlen;
    short phs_pmode; /* pipe mode set or returned */
};

/* DosQNmPipeInfo parameter block */
struct npi_param {
    short npi_len;
    short npi_dlen;
    short npi_level; /* information level desired */
};

/* DosRawReadNmPipe parameters */
/* data is buffer addr         */

struct npr_param {
    short npr_len;
    short npr_dlen;
    short npr_nbyt; /* number of bytes read */
};


/* DosRawWriteNmPipe parameters */
/* data is buffer addr          */

struct npw_param {
    short npw_len;
    short npw_dlen;
    short npw_nbyt; /* number of bytes written */
};

/* NPipeWait parameters */

struct npq_param {
    short npq_len;
    short npq_dlen;
    long npq_timeo;  /* time-out in milliseconds */
    short npq_prio;  /* priority of caller       */
};

/* DosCallNmPipe parameters */
/* data is in-buffer addr   */

struct npx_param {
    short npx_len;
    unsigned short npx_ilen;  /* length of in-buffer      */
    char __far * npx_obuf;    /* pointer to out-buffer    */
    unsigned short npx_ilen;  /* length of out-buffer     */
    unsigned short npx_nbyt;  /* number of bytes read     */
    long npx_timeo;           /* time-out in milliseconds */
};

/* PeekPipe parameters, data is buffer addr */

struct npp_param {
    short npp_len;
    unsigned short npp_dlen;
    unsigned short npp_nbyt;   /* number of bytes read      */
    unsigned short npp_av10;   /* bytes left in pipe        */
    unsigned short npp_av11;   /* bytes left in current msg */
    unsigned short npp_state;  /* pipe state                */
};

/* DosTransactNmPipe parameters */
/* data is in-buffer addr       */

struct npt_param {
    short npt_len;
    unsigned short npt_ilen;  /* length of in-buffer   */
    char __far * npt_obuf;    /* pointer to out-buffer */
    unsigned short npt_olen;  /* length of out-buffer  */
    unsigned short npt_nbyt;  /* number of bytes read  */
};

/* QNmPipeSemState parameter block */ /* data is user data buffer */

struct qnps_param {
    unsigned short qnps_len;   /* length of parameter block      */
    unsigned short qnps_dlen;  /* length of supplied data block */
    long qnps_semh;            /* system semaphore handle       */
    unsigned short qnps_nbyt;  /* number of bytes returned      */
};

/* ConnectPipe parameter block, no data block */

struct npc_param {
    unsigned short npc_len;   /* length of parameter block */
    unsigned short npc_dlen;  /* length of data block      */
};

/* DisconnectPipe parameter block, no data block */

struct npd_param {
    unsigned short npd_len;   /* length of parameter block */
    unsigned short npd_dlen;  /* length of data block      */
};

struct filelock {
    unsigned long FileOffset;   /* offset where the lock/unlock begins  */
    unsigned long RangeLength;  /* length of region locked/unlocked     */
};

/* //$TODO: Reko's C Parser doesn't handle preprocessor directives
#define ST_SCREAT 1
#define ST_PCREAT 2
#define ST_SWRITE 4
#define ST_PWRITE 8
#define ST_SREAD 16
#define ST_PREAD 32

#define NMP_GetPHandState    0x21
#define NMP_SetPHandState    0x01
#define NMP_PipeQInfo        0x22
#define NMP_PeekPipe         0x23
#define NMP_ConnectPipe      0x24
#define NMP_DisconnectPipe   0x25
#define NMP_TransactPipe     0x26
#define NMP_ReadRaw          0x11
#define NMP_WriteRaw         0x31
#define NMP_WaitPipe         0x53
#define NMP_CallPipe         0x54
#define NMP_QNmPipeSemState  0x58
*/

int __far __pascal FS_ALLOCATEPAGESPACE(struct sffsi __far * psffsi, struct sffsd __far * psffsd, unsigned long ulsize, unsigned short ulWantContig);
int __far __pascal FS_ATTACH(unsigned short flag, char __far * pDev, struct vpfsd __far *pvpfsd, struct cdfsd __far * pcdfsd, char __far * pParm, unsigned short __far * pLen);
int __far __pascal FS_CANCELLOCKREQUEST(struct sffsi __far * psffsi, struct sffsd __far * psffsd, struct filelock __far * pLockRange);
int __far __pascal FS_CHDIR(unsigned short flag, struct cdsfi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pDir, unsigned short iCurDirEnd);
int __far __pascal FS_CHGFILEPTR(struct sffsi __far * psffsi, struct sffsd __far * psffsd, long offset, unsigned short type, unsigned short IOflag);
int __far __pascal FS_CLOSE(unsigned short type, unsigned short IOflag, struct sffsi __far * psffsi, struct sffsd __far * psffsd);
int __far __pascal FS_COMMIT(unsigned short type, unsigned short IOflag, struct sffsi __far * psffsi, struct sffsd __far * psffsd);
int __far __pascal FS_COPY(unsigned short flag, struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pSrc, unsigned short iSrcCurDirEnd, char __far * pDst,
                       unsigned short iDstCurDirEnd, unsigned short nameType);
int __far __pascal FS_DELETE(struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pFile, unsigned short iCurDirEnd);
int __far __pascal FS_DOPAGEIO(struct sffsi __far * psffsi, struct sffsd __far * psffsd, struct PageCmdHeader __far * pList);
void __far __pascal FS_EXIT(unsigned short uid, unsigned short pid, unsigned short pdb);
int __far __pascal FS_FILEATTRIBUTE(unsigned short flag, struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pName, unsigned short iCurDirEnd, unsigned short __far * pAttr);
int __far __pascal FS_FILEINFO(unsigned short flag, struct sffsi __far * psffsi, struct sffsd __far * psffsd, unsigned short level, char __far * pData, unsigned short cbData, unsigned short IOflag);
int __far __pascal FS_FILEIO (struct sffsi __far * psffsi, struct sffsd __far * psffsd, char __far * pCmdList, unsigned short cbCmdList, unsigned short __far * poError, unsigned short IOflag);
int __far __pascal FS_FINDFIRST(struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pName, unsigned short iCurDirEnd, unsigned short attr, struct fsfsi __far * pfsfsi,
                            struct fsfsd __far * pfsfsd, char __far * pData, unsigned short cbData, unsigned short __far * pcMatch, unsigned short level, unsigned short flags);
int __far __pascal FS_FINDFROMNAME(struct fsfsi __far * pfsfsi, struct fsfsd __far * pfsfsd, char __far * pData, unsigned short cbData, unsigned short __far * pcMatch, unsigned short level,
                               unsigned long position, char __far * pName, unsigned short flags);
int __far __pascal FS_FINDNEXT(struct fsfsi __far * pfsfsi, struct fsfsd __far * pfsfsd, char __far * pData, unsigned short cbData, unsigned short __far * pcMatch, unsigned short level, unsigned short flags);
int __far __pascal FS_FINDNOTIFYCLOSE(unsigned short handle);
int __far __pascal FS_FINDNOTIFYFIRST(struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pName, unsigned short iCurDirEnd,
              unsigned short attr, unsigned short __far * pHandle, char __far * pData, unsigned short cbData, unsigned short __far * pMatch, unsigned short level, unsigned long timeout);
int __far __pascal FS_FINDNOTIFYNEXT(unsigned short handle, char __far * pData, unsigned short cbData, unsigned short __far * pcMatch, unsigned short level, unsigned long timeout);
int __far __pascal FS_FLUSHBUF(unsigned short hVPB, unsigned short flag);
int __far __pascal FS_FSCTL(union argdat __far * pArgDat, unsigned short iArgType, unsigned short func, char __far * pParm, unsigned short lenParm, unsigned short __far * plenParmIO,
                        char __far * pData, unsigned short lenData, unsigned short __far * plenDataIO);
int __far __pascal FS_FSINFO(unsigned short flag, unsigned short hVPB, char __far * pData, unsigned short cbData, unsigned short level);
int __far __pascal FS_INIT(char __far * szParm, unsigned long DevHelp, unsigned long __far * pMiniFSD);
int __far __pascal FS_IOCTL(struct sffsi __far * psffsi, struct sffsd __far * psffsd, unsigned short cat, unsigned short func, char __far * pParm, unsigned short lenMaxParm,
                        unsigned short * plenInOutParm, char __far * pData, unsigned short lenMaxData, unsigned short * plenInOutData);
int __far __pascal FS_MKDIR(struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pName, unsigned short iCurDirEnd, char __far * pEABuf, unsigned short flags);
int __far __pascal FS_MOVE(struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pSrc, unsigned short iSrcCurDirEnd, char __far * pDst, unsigned short iDstCurDirEnd, unsigned short flags);
int __far __pascal FS_NEWSIZE(struct sffsi __far * psffsi, struct sffsd __far * psffsd, unsigned long len, unsigned short IOflag);
int __far __pascal FS_NMPIPE(struct sffsi __far * psffsi, struct sffsd __far * psffsd, unsigned short OpType, union npoper __far * pOpRec, char __far * pData, char __far * pName);
int __far __pascal FS_OPENCREATE(struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pName, unsigned short iCurDirEnd, struct sffsi __far * psffsi,
                             struct sffsd __far * psffsd, unsigned long ulOpenMode, unsigned short usOpenFlag, unsigned short __far * pusAction,
                             unsigned short usAttr, char __far * pcEABuf, unsigned short __far * pfgenflag);
int __far __pascal FS_OPENPAGEFILE(unsigned long __far * pFlag, unsigned long __far * pcMaxReq, char __far * pName, struct sffsi __far * psffsi, struct sffsd __far * psffsi, unsigned short usOpenMode, unsigned short usOpenFlag, unsigned short usAttr, unsigned long Reserved);
int __far __pascal FS_PATHINFO(unsigned short flag, struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pName, unsigned short iCurDirEnd, unsigned short level, char __far * pData, unsigned short cbData);
int __far __pascal FS_PROCESSNAME(char __far * pNameBuf);
int __far __pascal FS_READ(struct sffsi __far * psffsi, struct sffsd __far * psffsd, char __far * pData, unsigned short __far * pLen, unsigned short IOflag);
int __far __pascal FS_RMDIR(struct cdfsi __far * pcdfsi, struct cdfsd __far * pcdfsd, char __far * pName, unsigned short iCurDirEnd);
int __far __pascal FS_SETSWAP(struct sffsi __far * psffsi, struct sffsd __far * psffsd);
int __far __pascal FS_SHUTDOWN(unsigned short type, unsigned long reserved);
int __far __pascal FS_VERIFYUNCNAME(unsigned short flag, char __far * pName);
int __far __pascal FS_WRITE(struct sffsi __far * psffsi, struct sffsd __far * psffsd, char __far * pData, unsigned short __far * pLen, unsigned short IOflag);
int __far __pascal FS_MOUNT(unsigned short flag, struct vpfsi __far * pvpfsi, struct vpfsd __far * pvpfsd, unsigned short hVPB, char __far * pBoot);
int __far __pascal FS_FINDCLOSE(struct fsfsi __far * pfsfsi, struct fsfsd __far * pfsfsd);
int __far __pascal FS_FILELOCKS(struct sffsi __far * psffsi, struct sffsd __far * psffsd, struct filelock __far * pUnLockRange, struct filelock __far * pLockRange, unsigned long timeout, unsigned long flags);
