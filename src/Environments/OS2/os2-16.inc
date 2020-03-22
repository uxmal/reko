#line 1 "OS21X.H"



















 #pragma read_only_file



















 #pragma read_only_file






 

















typedef unsigned char   UCHAR;
typedef unsigned char   BYTE;
typedef unsigned short  USHORT;
typedef unsigned int    UINT;
typedef unsigned long   ULONG;

typedef unsigned short  SHANDLE;
typedef void __far        *LHANDLE;

typedef SHANDLE    HFILE;
typedef HFILE __far  *PHFILE;

typedef char __far   *PSZ;
typedef char __near  *NPSZ;

typedef char __far   *PCH;
typedef char __near  *NPCH;

typedef unsigned short  BOOL;
typedef BOOL __far        *PBOOL;






























typedef char __far     *PCHAR;
typedef short __far    *PSHORT;
typedef int __far      *PINT;
typedef long __far     *PLONG;
typedef UCHAR __far    *PUCHAR;
typedef USHORT __far   *PUSHORT;
typedef UINT __far     *PUINT;
typedef ULONG __far    *PULONG;

typedef void __far     *PVOID;

typedef UCHAR __far    *PBYTE;
typedef BYTE __near    *NPBYTE;

typedef int (__pascal __far  *PFN)();
typedef int (__pascal __near *NPFN)();
typedef PFN __far *PPFN;

typedef USHORT       SEL;
typedef SEL __far      *PSEL;

typedef USHORT       HMODULE;
typedef HMODULE __far  *PHMODULE;
typedef USHORT       PID;
typedef PID __far      *PPID;
typedef USHORT       TID;
typedef TID __far      *PTID;
typedef void __far     *HSEM;
typedef HSEM __far     *PHSEM;

typedef LHANDLE   HPS, __far *PHPS;
typedef LHANDLE   HDC, __far *PHDC;
typedef LHANDLE   HRGN, __far *PHRGN;
typedef LHANDLE   HBITMAP, __far *PHBITMAP;
typedef LHANDLE   HMF, __far *PHMF;
typedef long      COLOR, __far *PCOLOR;

typedef struct _POINTL {
    long x;
    long y;
} POINTL, __far  *PPOINTL, __near *NPPOINTL;

typedef struct _POINTS {
    short x;
    short y;
} POINTS, __far *PPOINTS;

typedef struct _RECTL {
    long xLeft;
    long yBottom;
    long xRight;
    long yTop;
} RECTL, __far  *PRECTL, __near *NPRECTL;

typedef struct _WPOINT {
    short x;
    short dummy1;
    short y;
    short dummy2;
} WPOINT, __far *PWPOINT, __near *NPWPOINT;

typedef struct _WRECT {
    short xLeft;
    short dummy1;
    short yBottom;
    short dummy2;
    short xRight;
    short dummy3;
    short yTop;
    short dummy4;
} WRECT, __far *PWRECT, __near *NPWRECT;

typedef char STR8[8], __far *PSTR8;

typedef struct _DRIVDATA {
    long cb;
    long lVersion;
    char szDeviceName[32];
    char abGeneralData[1];
} DRIVDATA, __far *PDRIVDATA;

typedef struct _DEVOPENSTRUC {
    PSZ       pszLogAddress;
    PSZ       pszDriverName;
    PDRIVDATA pdriv;
    PSZ       pszDataType;
    PSZ       pszComment;
    PSZ       pszQueueProcName;
    PSZ       pszQueueProcParams;
    PSZ       pszSpoolerParams;
    PSZ       pszNetworkParams;
} DEVOPENSTRUC, __far *PDEVOPENSTRUC;
















typedef struct _FATTRS {
    USHORT usRecordLength;
    USHORT fsSelection;
    long   lMatch;
    char   szFacename[32];
    USHORT idRegistry;
    USHORT usCodePage;
    long   lMaxBaselineExt;
    long   lAveCharWidth;
    USHORT fsType;
    USHORT fsFontUse;
} FATTRS, __far *PFATTRS;

















typedef struct _FONTMETRICS {
    char   szFamilyname[32];
    char   szFacename[32];
    USHORT idRegistry;
    USHORT usCodePage;
    long   lEmHeight;
    long   lXHeight;
    long   lMaxAscender;
    long   lMaxDescender;
    long   lLowerCaseAscent;
    long   lLowerCaseDescent;
    long   lInternalLeading;
    long   lExternalLeading;
    long   lAveCharWidth;
    long   lMaxCharInc;
    long   lEmInc;
    long   lMaxBaselineExt;
    short  sCharSlope;
    short  sInlineDir;
    short  sCharRot;
    USHORT usWeightClass;
    USHORT usWidthClass;
    short  sXDeviceRes;
    short  sYDeviceRes;
    short  sFirstChar;
    short  sLastChar;
    short  sDefaultChar;
    short  sBreakChar;
    short  sNominalPointSize;
    short  sMinimumPointSize;
    short  sMaximumPointSize;
    USHORT fsType;
    USHORT fsDefn;
    USHORT fsSelection;
    USHORT fsCapabilities;
    long   lSubscriptXSize;
    long   lSubscriptYSize;
    long   lSubscriptXOffset;
    long   lSubscriptYOffset;
    long   lSuperscriptXSize;
    long   lSuperscriptYSize;
    long   lSuperscriptXOffset;
    long   lSuperscriptYOffset;
    long   lUnderscoreSize;    long   lUnderscorePosition;
    long   lStrikeoutSize;
    long   lStrikeoutPosition;
    short  sKerningPairs;
    short  sFamilyClass;
    long   lMatch;
} FONTMETRICS, __far *PFONTMETRICS;


















 #pragma read_only_file





  
  
  
  
  

















 #pragma read_only_file





  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  








































typedef struct _RESULTCODES {
    USHORT codeTerminate;
    USHORT codeResult;
} RESULTCODES, __far *PRESULTCODES;

typedef struct _PIDINFO {
    PID pid;
    TID tid;
    PID pidParent;
} PIDINFO, __far *PPIDINFO;

typedef void (__far *PFNTHREAD)(void);
typedef void (__pascal __far *PFNEXITLIST)(USHORT);

USHORT __pascal __far DosBeep(USHORT,USHORT);
USHORT __pascal __far DosCreateThread(PFNTHREAD,PTID,PBYTE);
USHORT __pascal __far DosCwait(USHORT,USHORT,PRESULTCODES,PPID,PID);
USHORT __pascal __far DosEnterCritSec(void);
USHORT __pascal __far DosExecPgm(PCHAR,short,USHORT,PSZ,PSZ,PRESULTCODES,PSZ);
void   __pascal __far DosExit(USHORT,USHORT);
USHORT __pascal __far DosExitCritSec(void);
USHORT __pascal __far DosExitList(USHORT,PFNEXITLIST);
USHORT __pascal __far DosGetPID(PPIDINFO);
USHORT __pascal __far DosGetPPID(USHORT,PUSHORT);
USHORT __pascal __far DosGetPrty(USHORT,PUSHORT,USHORT);
USHORT __pascal __far DosKillProcess(USHORT,PID);
USHORT __pascal __far DosResumeThread(TID);
USHORT __pascal __far DosSetPrty(USHORT,USHORT,short,USHORT);
USHORT __pascal __far DosSleep(ULONG);
USHORT __pascal __far DosSuspendThread(TID);

































































































typedef SHANDLE     HDIR;
typedef HDIR __far    *PHDIR;

typedef struct _DENA1 {
    UCHAR  reserved;
    UCHAR  cbName;
    USHORT cbValue;
    UCHAR  szName[1];
} DENA1;

typedef struct _FDATE {
    unsigned day   : 5;
    unsigned month : 4;
    unsigned year  : 7;
} FDATE;

typedef struct _FTIME {
    unsigned twosecs : 5;
    unsigned minutes : 6;
    unsigned hours   : 5;
} FTIME;

typedef struct _FILEFINDBUF {
    FDATE  fdateCreation;
    FTIME  ftimeCreation;
    FDATE  fdateLastAccess;
    FTIME  ftimeLastAccess;
    FDATE  fdateLastWrite;
    FTIME  ftimeLastWrite;
    ULONG  cbFile;
    ULONG  cbFileAlloc;
    USHORT attrFile;
    UCHAR  cchName;
    char   achName[256];
} FILEFINDBUF, __far *PFILEFINDBUF;

typedef struct _FILEFINDBUF2 {
    FDATE  fdateCreation;
    FTIME  ftimeCreation;
    FDATE  fdateLastAccess;
    FTIME  ftimeLastAccess;
    FDATE  fdateLastWrite;
    FTIME  ftimeLastWrite;
    ULONG  cbFile;
    ULONG  cbFileAlloc;
    USHORT attrFile;
    ULONG  cbList;
    UCHAR  cchName;
    char   achName[256];
} FILEFINDBUF2, __far *PFILEFINDBUF2;

typedef struct _FILESTATUS {
    FDATE  fdateCreation;
    FTIME  ftimeCreation;
    FDATE  fdateLastAccess;
    FTIME  ftimeLastAccess;
    FDATE  fdateLastWrite;
    FTIME  ftimeLastWrite;
    ULONG  cbFile;
    ULONG  cbFileAlloc;
    USHORT attrFile;
} FILESTATUS, __far *PFILESTATUS;

typedef struct _FSALLOCATE {
    ULONG  idFileSystem;
    ULONG  cSectorUnit;
    ULONG  cUnit;
    ULONG  cUnitAvail;
    USHORT cbSector;
} FSALLOCATE, __far *PFSALLOCATE;

typedef struct _GEA {
    BYTE cbName;
    char szName[1];
} GEA, __far *PGEA;

typedef struct _GEALIST {
    ULONG  cbList;
    GEA list[1];
} GEALIST, __far *PGEALIST;

typedef struct _FEA {
    BYTE fEA;
    BYTE cbName;
    USHORT cbValue;
} FEA, __far *PFEA;

typedef struct _FEALIST {
    ULONG  cbList;
    FEA list[1];
} FEALIST, __far *PFEALIST;

typedef struct _EAOP {
    PGEALIST fpGEAList;
    PFEALIST fpFEAList;
    ULONG    oError;
} EAOP, __far *PEAOP;

typedef struct _VOLUMELABEL {
    BYTE cch;
    char szVolLabel[12];
} VOLUMELABEL, __far *PVOLUMELABEL;

typedef struct _FSINFO {
    FDATE fdateCreation;
    FTIME ftimeCreation;
    VOLUMELABEL vol;
} FSINFO, __far *PFSINFO;

USHORT __pascal __far DosBufReset(HFILE);
USHORT __pascal __far DosChDir(PSZ,ULONG);
USHORT __pascal __far DosChgFilePtr(HFILE,long,USHORT,PULONG);
USHORT __pascal __far DosClose(HFILE);
USHORT __pascal __far DosCopy(PSZ,PSZ,USHORT,ULONG);
USHORT __pascal __far DosDelete(PSZ,ULONG);
USHORT __pascal __far DosDupHandle(HFILE,PHFILE);
USHORT __pascal __far DosEditName(USHORT,PSZ,PSZ,PBYTE,USHORT);
USHORT __pascal __far DosEnumAttribute(USHORT,PVOID,ULONG,PVOID,ULONG,PULONG,ULONG,ULONG);
USHORT __pascal __far DosFileIO(HFILE,PBYTE,USHORT,PLONG);
USHORT __pascal __far DosFileLocks(HFILE,PLONG,PLONG);
USHORT __pascal __far DosFindClose(HDIR);
USHORT __pascal __far DosFindFirst(PSZ,PHDIR,USHORT,PFILEFINDBUF,USHORT,PUSHORT,ULONG);
USHORT __pascal __far DosFindFirst2(PSZ,PHDIR,USHORT,PVOID,USHORT,PUSHORT,USHORT,ULONG);
USHORT __pascal __far DosFindNext(HDIR,PFILEFINDBUF,USHORT,PUSHORT);
USHORT __pascal __far DosFSAttach(PSZ,PSZ,PBYTE,USHORT,USHORT,ULONG);
USHORT __pascal __far DosFSCtl(PBYTE,USHORT,PUSHORT,PBYTE,USHORT,PUSHORT,USHORT,PSZ,HFILE,USHORT,ULONG);
USHORT __pascal __far DosMkDir(PSZ,ULONG);
USHORT __pascal __far DosMkDir2(PSZ,PEAOP,ULONG);
USHORT __pascal __far DosMove(PSZ,PSZ,ULONG);
USHORT __pascal __far DosNewSize(HFILE,ULONG);
USHORT __pascal __far DosOpen(PSZ,PHFILE,PUSHORT,ULONG,USHORT,USHORT,USHORT,ULONG);
USHORT __pascal __far DosOpen2(PSZ,PHFILE,PUSHORT,ULONG,USHORT,USHORT,USHORT,PEAOP,ULONG);
USHORT __pascal __far DosQCurDir(USHORT,PBYTE,PUSHORT);
USHORT __pascal __far DosQCurDisk(PUSHORT,PULONG);
USHORT __pascal __far DosQFHandState(HFILE,PUSHORT);
USHORT __pascal __far DosQFileInfo(HFILE,USHORT,PBYTE,USHORT);
USHORT __pascal __far DosQFileMode(PSZ,PUSHORT,ULONG);
USHORT __pascal __far DosQFSAttach(PSZ,USHORT,USHORT,PBYTE,PUSHORT,ULONG);
USHORT __pascal __far DosQFSInfo(USHORT,USHORT,PBYTE,USHORT);
USHORT __pascal __far DosQHandType(HFILE,PUSHORT,PUSHORT);
USHORT __pascal __far DosQPathInfo(PSZ,USHORT,PBYTE,USHORT,ULONG);
USHORT __pascal __far DosQVerify(PUSHORT);
USHORT __pascal __far DosRead(HFILE,PVOID,USHORT,PUSHORT);
USHORT __pascal __far DosReadAsync(HFILE,PULONG,PUSHORT,PVOID,USHORT,PUSHORT);
USHORT __pascal __far DosRmDir(PSZ,ULONG);
USHORT __pascal __far DosSelectDisk(USHORT);
USHORT __pascal __far DosSetFHandState(HFILE,USHORT);
USHORT __pascal __far DosSetFileInfo(HFILE,USHORT,PBYTE,USHORT);
USHORT __pascal __far DosSetFileMode(PSZ,USHORT,ULONG);
USHORT __pascal __far DosSetFSInfo(USHORT,USHORT,PBYTE,USHORT);
USHORT __pascal __far DosSetMaxFH(USHORT);
USHORT __pascal __far DosSetPathInfo(PSZ,USHORT,PBYTE,USHORT,USHORT,ULONG);
USHORT __pascal __far DosSetVerify(USHORT);
USHORT __pascal __far DosShutdown(ULONG);
USHORT __pascal __far DosWrite(HFILE,PVOID,USHORT,PUSHORT);
USHORT __pascal __far DosWriteAsync(HFILE,PULONG,PUSHORT,PVOID,USHORT,PUSHORT);










USHORT __pascal __far DosAllocHuge(USHORT,USHORT,PSEL,USHORT,USHORT);
USHORT __pascal __far DosAllocSeg(USHORT,PSEL,USHORT);
USHORT __pascal __far DosAllocShrSeg(USHORT,PSZ,PSEL);
USHORT __pascal __far DosCreateCSAlias(SEL,PSEL);
USHORT __pascal __far DosFreeSeg(SEL);
USHORT __pascal __far DosGetHugeShift(PUSHORT);
USHORT __pascal __far DosGetSeg(SEL);
USHORT __pascal __far DosGetShrSeg(PSZ,PSEL);
USHORT __pascal __far DosGiveSeg(SEL,PID,PSEL);
USHORT __pascal __far DosLockSeg(SEL);
USHORT __pascal __far DosMemAvail(PULONG);
USHORT __pascal __far DosReallocHuge(USHORT,USHORT,SEL);
USHORT __pascal __far DosReallocSeg(USHORT,SEL);
USHORT __pascal __far DosSizeSeg(SEL,PULONG);
USHORT __pascal __far DosSubAlloc(SEL,PUSHORT,USHORT);
USHORT __pascal __far DosSubFree(SEL,USHORT,USHORT);
USHORT __pascal __far DosSubSet(SEL,USHORT,USHORT);
USHORT __pascal __far DosUnlockSeg(SEL);





USHORT __pascal __far DosFreeModule(HMODULE);
USHORT __pascal __far DosGetModHandle(PSZ,PHMODULE);
USHORT __pascal __far DosGetModName(HMODULE,USHORT,PCHAR);
USHORT __pascal __far DosGetProcAddr(HMODULE,PSZ,PFN __far *);
USHORT __pascal __far DosLoadModule(PSZ,USHORT,PSZ,PHMODULE);





typedef SHANDLE     HPIPE;
typedef HPIPE __far   *PHPIPE;

USHORT __pascal __far DosCallNmPipe(PSZ,PBYTE,USHORT,PBYTE,USHORT,PUSHORT,ULONG);
USHORT __pascal __far DosConnectNmPipe(HPIPE);
USHORT __pascal __far DosDisConnectNmPipe(HPIPE);
USHORT __pascal __far DosMakeNmPipe(PSZ,PHPIPE,USHORT,USHORT,USHORT,USHORT,ULONG);
USHORT __pascal __far DosPeekNmPipe(HPIPE,PBYTE,USHORT,PUSHORT,PUSHORT,PUSHORT);
USHORT __pascal __far DosQNmPHandState(HPIPE,PUSHORT);
USHORT __pascal __far DosQNmPipeInfo(HPIPE,USHORT,PBYTE,USHORT);
USHORT __pascal __far DosQNmPipeSemState(HSEM,PBYTE,USHORT);
USHORT __pascal __far DosSetNmPHandState(HPIPE,USHORT);
USHORT __pascal __far DosSetNmPipeSem(HPIPE,HSEM,USHORT);
USHORT __pascal __far DosTransactNmPipe(HPIPE,PBYTE,USHORT,PBYTE,USHORT,PUSHORT);
USHORT __pascal __far DosWaitNmPipe(PSZ,ULONG);





typedef SHANDLE     HQUEUE;
typedef HQUEUE __far  *PHQUEUE;

USHORT __pascal __far DosCloseQueue(HQUEUE);
USHORT __pascal __far DosCreateQueue(PHQUEUE,USHORT,PSZ);
USHORT __pascal __far DosMakePipe(PHFILE,PHFILE,USHORT);
USHORT __pascal __far DosOpenQueue(PUSHORT,PHQUEUE,PSZ);
USHORT __pascal __far DosPeekQueue(HQUEUE,PULONG,PUSHORT,PULONG,PUSHORT,UCHAR,PBYTE,ULONG);
USHORT __pascal __far DosPurgeQueue(HQUEUE);
USHORT __pascal __far DosQueryQueue(HQUEUE,PUSHORT);
USHORT __pascal __far DosReadQueue(HQUEUE,PULONG,PUSHORT,PULONG,USHORT,UCHAR,PBYTE,HSEM);
USHORT __pascal __far DosWriteQueue(HQUEUE,USHORT,USHORT,PBYTE,UCHAR);








typedef LHANDLE     HSYSSEM;
typedef HSYSSEM __far *PHSYSSEM;

typedef struct _DOSFSRSEM {
    USHORT cb;
    PID    pid;
    TID    tid;
    USHORT cUsage;
    USHORT client;
    ULONG  sem;
} DOSFSRSEM, __far *PDOSFSRSEM;

typedef struct _MUXSEM {
    USHORT zero;
    HSEM   hsem;
} MUXSEM, __far *PMUXSEM;

typedef struct _MUXSEMLIST {
    USHORT  cmxs;
    MUXSEM  amxs[16];
} MUXSEMLIST, __far *PMUXSEMLIST;

USHORT __pascal __far DosCloseSem(HSEM);
USHORT __pascal __far DosCreateSem(USHORT,PHSYSSEM,PSZ);
USHORT __pascal __far DosFSRamSemClear(PDOSFSRSEM);
USHORT __pascal __far DosFSRamSemRequest(PDOSFSRSEM,long);
USHORT __pascal __far DosMuxSemWait(PUSHORT,PVOID,long);
USHORT __pascal __far DosOpenSem(PHSEM,PSZ);
USHORT __pascal __far DosSemClear(HSEM);
USHORT __pascal __far DosSemRequest(HSEM,long);
USHORT __pascal __far DosSemSet(HSEM);
USHORT __pascal __far DosSemSetWait(HSEM,long);
USHORT __pascal __far DosSemWait(HSEM,long);





typedef SHANDLE       HMONITOR;
typedef HMONITOR __far  *PHMONITOR;

USHORT __pascal __far DosMonClose(HMONITOR);
USHORT __pascal __far DosMonOpen(PSZ,PHMONITOR);
USHORT __pascal __far DosMonRead(PBYTE,USHORT,PBYTE,PUSHORT);
USHORT __pascal __far DosMonReg(HMONITOR,PBYTE,PBYTE,USHORT,USHORT);
USHORT __pascal __far DosMonWrite(PBYTE,PBYTE,USHORT);
















typedef struct _GINFOSEG {
    ULONG   time;
    ULONG   msecs;
    UCHAR   hour;
    UCHAR   minutes;
    UCHAR   seconds;
    UCHAR   hundredths;    USHORT  timezone;
    USHORT  cusecTimerInterval;
    UCHAR   day;
    UCHAR   month;
    USHORT  year;
    UCHAR   weekday;
    UCHAR   uchMajorVersion;
    UCHAR   uchMinorVersion;
    UCHAR   chRevisionLetter;
    UCHAR   sgCurrent;
    UCHAR   sgMax;
    UCHAR   cHugeShift;
    UCHAR   fProtectModeOnly;
    USHORT  pidForeground;
    UCHAR   fDynamicSched;
    UCHAR   csecMaxWait;
    USHORT  cmsecMinSlice;
    USHORT  cmsecMaxSlice;
    USHORT  bootdrive;
    UCHAR   amecRAS[32];
    UCHAR   csgWindowableVioMax;
    UCHAR   csgPMMax;
} GINFOSEG, __far *PGINFOSEG;

typedef struct _LINFOSEG {
    PID     pidCurrent;
    PID     pidParent;
    USHORT  prtyCurrent;
    TID     tidCurrent;
    USHORT  sgCurrent;
    UCHAR   rfProcStatus;
    UCHAR   dummy1;
    BOOL    fForeground;
    UCHAR   typeProcess;
    UCHAR   dummy2;
    SEL     selEnvironment;
    USHORT  offCmdLine;
    USHORT  cbDataSegment;
    USHORT  cbStack;
    USHORT  cbHeap;
    HMODULE hmod;
    SEL     selDS;
} LINFOSEG, __far *PLINFOSEG;

USHORT __pascal __far DosGetInfoSeg(PSEL,PSEL);





typedef struct _COUNTRYCODE {
    USHORT country;
    USHORT codepage;
} COUNTRYCODE, __far *PCOUNTRYCODE;

typedef struct _COUNTRYINFO {
    USHORT country;
    USHORT codepage;
    USHORT fsDateFmt;
    char   szCurrency[5];
    char   szThousandsSeparator[2];
    char   szDecimal[2];
    char   szDateSeparator[2];
    char   szTimeSeparator[2];
    UCHAR  fsCurrencyFmt;
    UCHAR  cDecimalPlace;
    UCHAR  fsTimeFmt;
    USHORT abReserved1[2];
    char   szDataSeparator[2];
    USHORT abReserved2[5];
} COUNTRYINFO, __far *PCOUNTRYINFO;

USHORT __pascal __far DosCaseMap(USHORT,PCOUNTRYCODE,PCHAR);
USHORT __pascal __far DosGetCollate(USHORT,PCOUNTRYCODE,PCHAR,PUSHORT);
USHORT __pascal __far DosGetCp(USHORT,PUSHORT,PUSHORT);
USHORT __pascal __far DosGetCtryInfo(USHORT,PCOUNTRYCODE,PCOUNTRYINFO,PUSHORT);
USHORT __pascal __far DosGetDBCSEv(USHORT,PCOUNTRYCODE,PCHAR);
USHORT __pascal __far DosSetCp(USHORT,USHORT);
USHORT __pascal __far DosSetProcCp(USHORT,USHORT);






























typedef void (__pascal __far *PFNSIGHANDLER)(USHORT,USHORT);

USHORT __pascal __far DosFlagProcess(PID,USHORT,USHORT,USHORT);
USHORT __pascal __far DosHoldSignal(USHORT);
USHORT __pascal __far DosSendSignal(USHORT,USHORT);
USHORT __pascal __far DosSetSigHandler(PFNSIGHANDLER,PFNSIGHANDLER __far *,PUSHORT,USHORT,USHORT);





USHORT __pascal __far DosFreeResource(PBYTE);
USHORT __pascal __far DosGetResource(HMODULE,USHORT,USHORT,PSEL);
USHORT __pascal __far DosGetResource2(HMODULE,USHORT,USHORT,PULONG);





typedef struct _DATETIME {
    UCHAR   hours;
    UCHAR   minutes;
    UCHAR   seconds;
    UCHAR   hundredths;
    UCHAR   day;
    UCHAR   month;
    USHORT  year;
    short   timezone;
    UCHAR   weekday;
} DATETIME, __far *PDATETIME;

USHORT __pascal __far DosGetDateTime(PDATETIME);
USHORT __pascal __far DosSetDateTime(PDATETIME);





typedef SHANDLE     HTIMER;
typedef HTIMER __far  *PHTIMER;

USHORT __pascal __far DosTimerAsync(ULONG,HSEM,PHTIMER);
USHORT __pascal __far DosTimerStart(ULONG,HSEM,PHTIMER);
USHORT __pascal __far DosTimerStop(HTIMER);





USHORT __pascal __far DosErrClass(USHORT,PUSHORT,PUSHORT,PUSHORT);
USHORT __pascal __far DosError(USHORT);
USHORT __pascal __far DosGetEnv(PUSHORT,PUSHORT);
USHORT __pascal __far DosGetMachineMode(PBYTE);
USHORT __pascal __far DosGetMessage(PCHAR __far *,USHORT,PCHAR,USHORT,USHORT,PSZ,PUSHORT);
USHORT __pascal __far DosGetVersion(PUSHORT);
USHORT __pascal __far DosInsMessage(PCHAR __far *,USHORT,PSZ,USHORT,PCHAR,USHORT,PUSHORT);
USHORT __pascal __far DosPutMessage(USHORT,USHORT,PCHAR);
USHORT __pascal __far DosPtrace(PBYTE);
USHORT __pascal __far DosQSysInfo(USHORT,PBYTE,USHORT);
USHORT __pascal __far DosScanEnv(PSZ,PSZ __far *);
USHORT __pascal __far DosSearchPath(USHORT,PSZ,PSZ,PBYTE,USHORT);
USHORT __pascal __far DosSetVec(USHORT,PFN,PFN __far *);





typedef struct _STATUSDATA {
  USHORT Length;
  USHORT SelectInd;
  USHORT BondInd;
} STATUSDATA, __far *PSTATUSDATA;

typedef struct _STARTDATA {
    USHORT Length;
    USHORT Related;
    USHORT FgBg;
    USHORT TraceOpt;
    PSZ    PgmTitle;
    PSZ    PgmName;
    PBYTE  PgmInputs;
    PBYTE  TermQ;
    PBYTE  Environment;
    USHORT InheritOpt;
    USHORT SessionType;
    PSZ    IconFile;
    ULONG  PgmHandle;
    USHORT PgmControl;
    USHORT InitXPos;
    USHORT InitYPos;
    USHORT InitXSize;
    USHORT InitYSize;
} STARTDATA, __far *PSTARTDATA;

USHORT __pascal __far DosQAppType(PSZ,PUSHORT);
USHORT __pascal __far DosSelectSession(USHORT,ULONG);
USHORT __pascal __far DosSetSession(USHORT,PSTATUSDATA);
USHORT __pascal __far DosStartSession(PSTARTDATA,PUSHORT,PUSHORT);
USHORT __pascal __far DosStopSession(USHORT,USHORT,ULONG);





void   __pascal __far DosCallback(PFN);
USHORT __pascal __far DosCLIAccess(void);
USHORT __pascal __far DosDevConfig(PVOID,USHORT,USHORT);
USHORT __pascal __far DosDevIOCtl(PVOID,PVOID,USHORT,USHORT,HFILE);
USHORT __pascal __far DosDevIOCtl2(PVOID,USHORT,PVOID,USHORT,USHORT,USHORT,HFILE);
USHORT __pascal __far DosPhysicalDisk(USHORT,PBYTE,USHORT,PBYTE,USHORT);
USHORT __pascal __far DosPortAccess(USHORT,USHORT,USHORT,USHORT);
USHORT __pascal __far DosR2StackRealloc(USHORT);




















 #pragma read_only_file





    
    
    






















typedef SHANDLE    HKBD;
typedef HKBD __far   *PHKBD;

typedef struct _KBDKEYINFO {
    UCHAR  chChar;
    UCHAR  chScan;
    UCHAR  fbStatus;
    UCHAR  bNlsShift;
    USHORT fsState;
    ULONG  time;
}KBDKEYINFO, __far *PKBDKEYINFO;

typedef struct _KBDHWID {
    USHORT length;
    USHORT kbd_id;
    USHORT reserved1;
    USHORT reserved2;
}KBDHWID, __far *PKBDHWID;

typedef struct _KBDINFO {
    USHORT cb;
    USHORT fsMask;
    USHORT chTurnAround;
    USHORT fsInterim;
    USHORT fsState;
} KBDINFO, __far *PKBDINFO;

typedef struct _STRINGINBUF {
    USHORT cb;
    USHORT cchIn;
} STRINGINBUF, __far *PSTRINGINBUF;

typedef struct _KBDTRANS {
    UCHAR  chChar;
    UCHAR  chScan;
    UCHAR  fbStatus;
    UCHAR  bNlsShift;
    USHORT fsState;
    ULONG  time;
    USHORT fsDD;
    USHORT fsXlate;
    USHORT fsShift;
    USHORT sZero;
} KBDTRANS, __far *PKBDTRANS;

USHORT __pascal __far KbdCharIn(PKBDKEYINFO CharData, USHORT IOWait, HKBD KbdHandle);
USHORT __pascal __far KbdClose(HKBD KbdHandle);
USHORT __pascal __far KbdDeRegister(void);
USHORT __pascal __far KbdFlushBuffer(HKBD KbdHandle);
USHORT __pascal __far KbdFreeFocus(HKBD KbdHandle);
USHORT __pascal __far KbdGetCp(ULONG Reserved, PUSHORT CodePageID, HKBD KbdHandle);
USHORT __pascal __far KbdGetFocus(USHORT IOWait, HKBD KbdHandle);
USHORT __pascal __far KbdGetHWId(PKBDHWID KeyboardID, HKBD KbdHandle);
USHORT __pascal __far KbdGetStatus(PKBDINFO Structure, HKBD KbdHandle);
USHORT __pascal __far KbdOpen(PHKBD KbdHandle);
USHORT __pascal __far KbdPeek(PKBDKEYINFO CharData, HKBD KbdHandle);
USHORT __pascal __far KbdRegister(PSZ ModuleName, PSZ EntryPoint, ULONG FunctionMask);
USHORT __pascal __far KbdSetCp(USHORT Reserved, USHORT CodePageID, HKBD KbdHandle);
USHORT __pascal __far KbdSetCustXt(PUSHORT Xlatetable, HKBD KbdHandle);
USHORT __pascal __far KbdSetFgnd(void);
USHORT __pascal __far KbdSetStatus(PKBDINFO Structure, HKBD KbdHandle);
USHORT __pascal __far KbdStringIn(PCH CharBuffer, PSTRINGINBUF Length, USHORT IOWait, HKBD KbdHandle);
USHORT __pascal __far KbdSynch(USHORT IOWait);
USHORT __pascal __far KbdXlate(PKBDTRANS XlateRecord, HKBD KbdHandle);



































typedef SHANDLE    HMOU;
typedef HMOU __far   *PHMOU;

typedef struct _MOUQUEINFO {
    USHORT cEvents;
    USHORT cmaxEvents;
} MOUQUEINFO, __far *PMOUQUEINFO;

typedef struct _PTRLOC {
    USHORT row;
    USHORT col;
} PTRLOC, __far *PPTRLOC;

typedef struct _PTRSHAPE {
    USHORT cb;
    USHORT col;
    USHORT row;
    USHORT colHot;
    USHORT rowHot;
} PTRSHAPE, __far *PPTRSHAPE;

typedef struct _SCALEFACT {
    USHORT rowScale;
    USHORT colScale;
} SCALEFACT, __far *PSCALEFACT;

typedef struct _MOUEVENTINFO {
    USHORT fs;
    ULONG  time;
    USHORT row;
    USHORT col;
} MOUEVENTINFO, __far *PMOUEVENTINFO;

typedef struct _NOPTRRECT {
    USHORT row;
    USHORT col;
    USHORT cRow;
    USHORT cCol;
} NOPTRRECT, __far *PNOPTRRECT;

USHORT __pascal __far MouClose(HMOU DeviceHandle);
USHORT __pascal __far MouDeRegister(void);
USHORT __pascal __far MouDrawPtr(HMOU DeviceHandle);
USHORT __pascal __far MouFlushQue(HMOU DeviceHandle);
USHORT __pascal __far MouGetDevStatus(PUSHORT DeviceStatus, HMOU DeviceHandle);
USHORT __pascal __far MouGetEventMask(PUSHORT EventMask, HMOU DeviceHandle);
USHORT __pascal __far MouGetNumButtons(PUSHORT NumberOfButtons, HMOU DeviceHandle);
USHORT __pascal __far MouGetNumMickeys(PUSHORT NumberOfMickeys, HMOU DeviceHandle);
USHORT __pascal __far MouGetNumQueEl(PMOUQUEINFO QueDataRecord, HMOU DeviceHandle);
USHORT __pascal __far MouGetPtrPos(PPTRLOC PtrPos, HMOU DeviceHandle);
USHORT __pascal __far MouGetPtrShape(PBYTE PtrBuffer, PPTRSHAPE PtrDefRec, HMOU DeviceHandle);
USHORT __pascal __far MouGetScaleFact(PSCALEFACT ScaleStruct, HMOU DeviceHandle);
USHORT __pascal __far MouInitReal(PSZ DriverName);
USHORT __pascal __far MouOpen(PSZ DriverName, PHMOU DeviceHandle);
USHORT __pascal __far MouReadEventQue(PMOUEVENTINFO Buffer, PUSHORT ReadType, HMOU DeviceHandle);
USHORT __pascal __far MouRegister(PSZ ModuleName, PSZ EntryName, ULONG Mask);
USHORT __pascal __far MouRemovePtr(PNOPTRRECT PtrArea, HMOU DeviceHandle);
USHORT __pascal __far MouSetDevStatus(PUSHORT DeviceStatus, HMOU DeviceHandle);
USHORT __pascal __far MouSetEventMask(PUSHORT EventMask, HMOU DeviceHandle);
USHORT __pascal __far MouSetPtrPos(PPTRLOC PtrPos, HMOU DeviceHandle);
USHORT __pascal __far MouSetPtrShape(PBYTE PtrBuffer, PPTRSHAPE PtrDefRec, HMOU DeviceHandle);
USHORT __pascal __far MouSetScaleFact(PSCALEFACT ScaleStruct, HMOU DeviceHandle);
USHORT __pascal __far MouSynch(USHORT IOWait);















































































typedef SHANDLE    HVIO;
typedef HVIO __far   *PHVIO;

typedef struct _VIOCONFIGINFO {
    USHORT  cb;
    USHORT  adapter;
    USHORT  display;
    ULONG   cbMemory;
    USHORT  Configuration;
    USHORT  VDHVersion;
    USHORT  Flags;
    ULONG   HWBufferSize;
    ULONG   FullSaveSize;
    ULONG   PartSaveSize;
    USHORT  EMAdaptersOFF;
    USHORT  EMDisplaysOFF;
} VIOCONFIGINFO, __far *PVIOCONFIGINFO;

typedef struct _VIOCURSORINFO {
    USHORT   yStart;
    USHORT   cEnd;
    USHORT   cx;
    USHORT   attr;
} VIOCURSORINFO, __far *PVIOCURSORINFO;

typedef struct _VIOFONTINFO {
    USHORT  cb;
    USHORT  type;
    USHORT  cxCell;
    USHORT  cyCell;
    PVOID   pbData;
    USHORT  cbData;
} VIOFONTINFO, __far *PVIOFONTINFO;

typedef struct _VIOMODEINFO {
    USHORT cb;
    UCHAR  fbType;
    UCHAR  color;
    USHORT col;
    USHORT row;
    USHORT hres;
    USHORT vres;
    UCHAR  fmt_ID;
    UCHAR  attrib;
    ULONG  buf_addr;
    ULONG  buf_length;
    ULONG  full_length;
    ULONG  partial_length;
    PCH    ext_data_addr;
} VIOMODEINFO, __far *PVIOMODEINFO;

typedef struct _VIOPHYSBUF {
    PBYTE    pBuf;
    ULONG    cb;
    SEL      asel[1];
} VIOPHYSBUF, __far *PVIOPHYSBUF;

typedef struct _VIOPALSTATE {
    USHORT  cb;
    USHORT  type;
    USHORT  iFirst;
    USHORT  acolor[1];
} VIOPALSTATE, __far *PVIOPALSTATE;

typedef struct _VIOOVERSCAN {
    USHORT  cb;
    USHORT  type;
    USHORT  color;
} VIOOVERSCAN, __far *PVIOOVERSCAN;

typedef struct _VIOINTENSITY {
    USHORT  cb;
    USHORT  type;
    USHORT  fs;
} VIOINTENSITY, __far *PVIOINTENSITY;

typedef struct _VIOCOLORREG {
    USHORT  cb;
    USHORT  type;
    USHORT  firstcolorreg;
    USHORT  numcolorregs;
    PCH     colorregaddr;
} VIOCOLORREG, __far *PVIOCOLORREG;

typedef struct _VIOSETULINELOC {
    USHORT  cb;
    USHORT  type;
    USHORT  scanline;
} VIOSETULINELOC, __far *PVIOSETULINELOC;

typedef struct _VIOSETTARGET {
    USHORT  cb;
    USHORT  type;
    USHORT  defaultalgorithm;
} VIOSETTARGET, __far *PVIOSETTARGET;

USHORT __pascal __far VioDeRegister(void);
USHORT __pascal __far VioEndPopUp(HVIO);
USHORT __pascal __far VioGetAnsi(PUSHORT,HVIO);
USHORT __pascal __far VioGetBuf(PULONG,PUSHORT,HVIO);
USHORT __pascal __far VioGetConfig(USHORT,PVIOCONFIGINFO,HVIO);
USHORT __pascal __far VioGetCp(USHORT,PUSHORT,HVIO);
USHORT __pascal __far VioGetCurPos(PUSHORT,PUSHORT,HVIO);
USHORT __pascal __far VioGetCurType(PVIOCURSORINFO,HVIO);
USHORT __pascal __far VioGetFont(PVIOFONTINFO,HVIO);
USHORT __pascal __far VioGetMode(PVIOMODEINFO,HVIO);
USHORT __pascal __far VioGetPhysBuf(PVIOPHYSBUF,USHORT);
USHORT __pascal __far VioGetState(PVOID,HVIO);
USHORT __pascal __far VioGlobalReg(PSZ,PSZ,ULONG,ULONG,long);
USHORT __pascal __far VioModeUndo(USHORT,USHORT,USHORT);
USHORT __pascal __far VioModeWait(USHORT,PUSHORT,USHORT);
USHORT __pascal __far VioPopUp(PUSHORT,HVIO);
USHORT __pascal __far VioPrtSc(HVIO);
USHORT __pascal __far VioPrtScToggle(HVIO);
USHORT __pascal __far VioReadCellStr(PCH,PUSHORT,USHORT,USHORT,HVIO);
USHORT __pascal __far VioReadCharStr(PCH,PUSHORT,USHORT,USHORT,HVIO);
USHORT __pascal __far VioRegister(PSZ,PSZ,ULONG,ULONG);
USHORT __pascal __far VioSavRedrawUndo(USHORT,USHORT,HVIO);
USHORT __pascal __far VioSavRedrawWait(USHORT,PUSHORT,HVIO);
USHORT __pascal __far VioScrLock(USHORT,PUCHAR,HVIO);
USHORT __pascal __far VioScrollDn(USHORT,USHORT,USHORT,USHORT,USHORT,PBYTE,HVIO);
USHORT __pascal __far VioScrollLf(USHORT,USHORT,USHORT,USHORT,USHORT,PBYTE,HVIO);
USHORT __pascal __far VioScrollRt(USHORT,USHORT,USHORT,USHORT,USHORT,PBYTE,HVIO);
USHORT __pascal __far VioScrollUp(USHORT,USHORT,USHORT,USHORT,USHORT,PBYTE,HVIO);
USHORT __pascal __far VioScrUnLock(HVIO);
USHORT __pascal __far VioSetAnsi(USHORT,HVIO);
USHORT __pascal __far VioSetCp(USHORT,USHORT,HVIO);
USHORT __pascal __far VioSetCurPos(USHORT,USHORT,HVIO);
USHORT __pascal __far VioSetCurType(PVIOCURSORINFO,HVIO);
USHORT __pascal __far VioSetFont(PVIOFONTINFO,HVIO);
USHORT __pascal __far VioSetMode(PVIOMODEINFO,HVIO);
USHORT __pascal __far VioSetState(PVOID,HVIO);
USHORT __pascal __far VioShowBuf(USHORT,USHORT,HVIO);
USHORT __pascal __far VioWrtCellStr(PCH,USHORT,USHORT,USHORT,HVIO);
USHORT __pascal __far VioWrtCharStr(PCH,USHORT,USHORT,USHORT,HVIO);
USHORT __pascal __far VioWrtCharStrAtt(PCH,USHORT,USHORT,USHORT,PBYTE,HVIO);
USHORT __pascal __far VioWrtNAttr(PBYTE,USHORT,USHORT,USHORT,HVIO);
USHORT __pascal __far VioWrtNCell(PBYTE,USHORT,USHORT,USHORT,HVIO);
USHORT __pascal __far VioWrtNChar(PBYTE,USHORT,USHORT,USHORT,HVIO);
USHORT __pascal __far VioWrtTTY(PCH,USHORT,HVIO);




















 #pragma read_only_file










































































































































































































































































































































































































































  















 #pragma read_only_file







































































































































































































































































































































































































typedef struct _DCBINFO {
    USHORT usWriteTimeout;
    USHORT usReadTimeout;
    BYTE   fbCtlHndShake;
    BYTE   fbFlowReplace;
    BYTE   fbTimeout;
    BYTE   bErrorReplacementChar;
    BYTE   bBreakReplacementChar;
    BYTE   bXONChar;
    BYTE   bXOFFChar;
} DCBINFO, __far *PDCBINFO;

typedef struct _TRACKLAYOUT {
    BYTE   bCommand;
    USHORT usHead;
    USHORT usCylinder;
    USHORT usFirstSector;
    USHORT cSectors;
    struct {
        USHORT usSectorNumber;
        USHORT usSectorSize;
    } TrackTable[1];
} TRACKLAYOUT, __far *PTRACKLAYOUT;

typedef struct _BIOSPARAMETERBLOCK {
    USHORT usBytesPerSector;
    BYTE   bSectorsPerCluster;
    USHORT usReservedSectors;
    BYTE   cFATs;
    USHORT cRootEntries;
    USHORT cSectors;
    BYTE   bMedia;
    USHORT usSectorsPerFAT;
    USHORT usSectorsPerTrack;
    USHORT cHeads;
    ULONG  cHiddenSectors;
    ULONG  cLargeSectors;
    BYTE   abReserved[6];
    USHORT cCylinders;
    BYTE   bDeviceType;
    USHORT fsDeviceAttr;
} BIOSPARAMETERBLOCK, __far *PBIOSPARAMETERBLOCK;

typedef struct _SCREENGROUP {
    USHORT idScreenGrp;
    USHORT fTerminate;
} SCREENGROUP, __far *PSCREENGROUP;

typedef struct _FRAME {
    BYTE bCharsPerLine;
    BYTE bLinesPerInch;
} FRAME, __far *PFRAME;

typedef struct _KBDTYPE {
    USHORT usType;
    USHORT reserved1;
    USHORT reserved2;
} KBDTYPE, __far *PKBDTYPE;

typedef struct _LINECONTROL {
    BYTE bDataBits;
    BYTE bParity;
    BYTE bStopBits;
    BYTE fTransBreak;
} LINECONTROL, __far *PLINECONTROL;

typedef struct _MODEMSTATUS {
    BYTE fbModemOn;
    BYTE fbModemOff;
} MODEMSTATUS, __far *PMODEMSTATUS;

typedef struct _TRACKFORMAT {
    BYTE bCommand;
    USHORT usHead;
    USHORT usCylinder;
    USHORT usReserved;
    USHORT cSectors;
    struct {
        BYTE bCylinder;
        BYTE bHead;
        BYTE idSector;
        BYTE bBytesSector;
    } FormatTable[1];
} TRACKFORMAT, __far *PTRACKFORMAT;

typedef struct _RXQUEUE {
    USHORT cch;
    USHORT cb;
} RXQUEUE, __far *PRXQUEUE;

typedef struct _DEVICEPARAMETERBLOCK {
    USHORT reserved1;
    USHORT cCylinders;
    USHORT cHeads;
    USHORT cSectorsPerTrack;
    USHORT reserved2;
    USHORT reserved3;
    USHORT reserved4;
    USHORT reserved5;
} DEVICEPARAMETERBLOCK, __far *PDEVICEPARAMETERBLOCK;

typedef struct _PTRDRAWFUNCTION {
    USHORT usReturnCode;
    PFN pfnDraw;
    PCH pchDataSeg;
} PTRDRAWFUNCTION, __far *PPTRDRAWFUNCTION;

typedef struct _PTRDRAWADDRESS {
    USHORT reserved;
    PTRDRAWFUNCTION ptrdfnc;
} PTRDRAWADDRESS, __far *PPTRDRAWADDRESS;

typedef struct _SHIFTSTATE {
    USHORT fsState;
    BYTE   fNLS;
} SHIFTSTATE, __far *PSHIFTSTATE;

typedef struct _HOTKEY {
    USHORT fsHotKey;
    UCHAR  uchScancodeMake;
    UCHAR  uchScancodeBreak;
    USHORT idHotKey;
} HOTKEY, __far *PHOTKEY;

typedef struct _MONITORPOSITION {
    USHORT fPosition;
    USHORT index;
    ULONG  pbInBuf;
    USHORT offOutBuf;
} MONITORPOSITION, __far *PMONITORPOSITION;

typedef struct _RATEDELAY {
    USHORT usDelay;
    USHORT usRate;
} RATEDELAY, __far *PRATEDELAY;

typedef struct _CODEPAGEINFO {
    PBYTE pbTransTable;
    USHORT idCodePage;
    USHORT idTable;
} CODEPAGEINFO, __far *PCODEPAGEINFO;

typedef struct _CPID {
    USHORT idCodePage;
    USHORT Reserved;
} CPID, __far *PCPID;

typedef struct _LDTADDRINFO {
    PULONG pulPhysAddr;
    USHORT cb;
} LDTADDRINFO, __far *PLDTADDRINFO;

typedef struct _PTRDRAWDATA {
    USHORT cb;
    USHORT usConfig;
    USHORT usFlag;
} PTRDRAWDATA, __far *PPTRDRAWDATA;

typedef struct _GETLINIOCTLDATA {
    ULONG PacketLength;
    ULONG PhysicalAddress;
    ULONG ApertureSize;
    PBYTE LinearAddress;
    ULONG LinearFlags;
} GETLINIOCTLDATA, *PGETLINIOCTLDATA;

typedef struct _BANKINFO {
    ULONG  ulBankLength;
    USHORT usBank;
    USHORT usVideoModeType;
    USHORT usReadWriteMode;
} BANKINFO;

typedef struct _GLOBALIOCTLDATA {
    ULONG ProcessAddress;
    ULONG AddressLength;
    ULONG GlobalAddress;
} GLOBALIOCTLDATA;

typedef struct _OEMSVGAINFO {
    USHORT AdapterType;
    USHORT ChipType;
    ULONG  Memory;
} OEMSVGAINFO;

typedef struct _OEMINFO {
    ULONG  OEMLength;
    USHORT Manufacturer;
    ULONG  ManufacturerData;
} OEMINFO;

typedef struct _GETGLOBALPACKET {
    ULONG   GlobalPktLength;
    GLOBALIOCTLDATA GlobalPktData[1];
} GETGLOBALPACKET;





















 #pragma read_only_file



    
    
    
    
    


















 #pragma read_only_file







    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    



  
  
  















 #pragma read_only_file










































































































typedef long  IPT;
typedef PLONG PIPT;
typedef long  PIX;
typedef ULONG LINE;

typedef struct _FORMATRECT {
    long cxFormat;
    long cyFormat;
} MLEFORMATRECT, __far *PFORMATRECT;

typedef struct _MLECTLDATA {
    USHORT cbCtlData;
    USHORT afIEFormat;
    ULONG  cchText;
    IPT    iptAnchor;
    IPT    iptCursor;
    long   cxFormat;
    long   cyFormat;
    ULONG  afFormatFlags;
} MLECTLDATA, __far *PMLECTLDATA;

typedef struct _MLEOVERFLOW {
    ULONG afErrInd;
    long  nBytesOver;
    long  pixHorzOver;
    long  pixVertOver;
}  MLEOVERFLOW, __far *POVERFLOW;

typedef struct _MLEMARGSTRUCT {
    USHORT afMargins;
    USHORT usMouMsg;
    IPT    iptNear;
} MLEMARGSTRUCT, __far *PMARGSTRUCT;

typedef struct _SEARCH {
    USHORT cb;
    PCHAR  pchFind;
    PCHAR  pchReplace;
    short  cchFind;
    short  cchReplace;
    IPT    iptStart;
    IPT    iptStop;
    USHORT cchFound;
} MLE_SEARCHDATA, __far *PMLE_SEARCHDATA;






  


























































typedef LHANDLE HWND, __far *PHWND;
typedef LHANDLE HAB;
typedef LHANDLE HPOINTER;

typedef void __far    *MPARAM;
typedef MPARAM __far  *PMPARAM;
typedef void __far    *MRESULT;
typedef MRESULT __far *PMRESULT;

typedef MRESULT (__pascal __far *PFNWP)(HWND,USHORT,MPARAM,MPARAM);



















typedef struct _QVERSDATA {
    USHORT   environment;
    USHORT   version;
} QVERSDATA, __far *PQVERSDATA;

HPS     __pascal __far WinBeginPaint(HWND,HPS,PRECTL);
MRESULT __pascal __far WinDefWindowProc(HWND,USHORT,MPARAM,MPARAM);
BOOL    __pascal __far WinDestroyWindow(HWND);
BOOL    __pascal __far WinEndPaint(HPS);
BOOL    __pascal __far WinFillRect(HPS,PRECTL,long);
HPS     __pascal __far WinGetClipPS(HWND,HWND,USHORT);
HPS     __pascal __far WinGetPS(HWND);
HAB     __pascal __far WinInitialize(USHORT);
BOOL    __pascal __far WinIsWindowShowing(HWND);
HDC     __pascal __far WinOpenWindowDC(HWND);
HAB     __pascal __far WinQueryAnchorBlock(HWND);
ULONG   __pascal __far WinQueryVersion(HAB);
BOOL    __pascal __far WinQueryWindowRect(HWND,PRECTL);
BOOL    __pascal __far WinRegisterClass(HAB,PSZ,PFNWP,ULONG,USHORT);
BOOL    __pascal __far WinReleasePS(HPS);
short   __pascal __far WinScrollWindow(HWND,short,short,PRECTL,PRECTL,HRGN,PRECTL,USHORT);
BOOL    __pascal __far WinShowWindow(HWND,BOOL);
BOOL    __pascal __far WinTerminate(HAB);


































































typedef struct _SWP {
    USHORT fs;
    short  cy;
    short  cx;
    short  y;
    short  x;
    HWND   hwndInsertBehind;
    HWND   hwnd;
} SWP, __far *PSWP;

HWND  __pascal __far WinCreateWindow(HWND,PSZ,PSZ,ULONG,short,short,short,
                  short,HWND,HWND,USHORT,PVOID,PVOID);
BOOL  __pascal __far WinDrawBitmap(HPS,HBITMAP,PRECTL,PPOINTL,long,long,USHORT);
BOOL  __pascal __far WinDrawBorder(HPS,PRECTL,short,short,long,long,USHORT);
short __pascal __far WinDrawText(HPS,short,PCH,PRECTL,long,long,USHORT);
BOOL  __pascal __far WinEnableWindow(HWND,BOOL);
BOOL  __pascal __far WinEnableWindowUpdate(HWND,BOOL);
short __pascal __far WinLoadMessage(HAB,HMODULE,USHORT,short,PSZ);
short __pascal __far WinLoadString(HAB,HMODULE,USHORT,short,PSZ);
BOOL  __pascal __far WinInvalidateRect(HWND,PRECTL,BOOL);
BOOL  __pascal __far WinInvalidateRegion(HWND,HRGN,BOOL);
BOOL  __pascal __far WinInvertRect(HPS,PRECTL);
BOOL  __pascal __far WinIsChild(HWND,HWND);
BOOL  __pascal __far WinIsWindow(HAB,HWND);
BOOL  __pascal __far WinIsWindowEnabled(HWND);
BOOL  __pascal __far WinIsWindowVisible(HWND);
short __pascal __far WinMultWindowFromIDs(HWND,PHWND,USHORT,USHORT);
HWND  __pascal __far WinQueryWindow(HWND,short,BOOL);
BOOL  __pascal __far WinQueryWindowPos(HWND,PSWP);
short __pascal __far WinQueryWindowText(HWND,short,PCH);
short __pascal __far WinQueryWindowTextLength(HWND);
BOOL  __pascal __far WinSetMultWindowPos(HAB,PSWP,USHORT);
BOOL  __pascal __far WinSetOwner(HWND,HWND);
BOOL  __pascal __far WinSetParent(HWND,HWND,BOOL);
BOOL  __pascal __far WinSetWindowPos(HWND,HWND,short,short,short,short,USHORT);
BOOL  __pascal __far WinSetWindowText(HWND,PSZ);
HWND  __pascal __far WinQueryDesktopWindow(HAB,HDC);
HWND  __pascal __far WinQueryObjectWindow(HWND);
BOOL  __pascal __far WinQueryWindowProcess(HWND,PPID,PTID);
BOOL  __pascal __far WinUpdateWindow(HWND);
HWND  __pascal __far WinWindowFromID(HWND,USHORT);



BOOL __pascal __far WinSetActiveWindow(HWND,HWND);



























typedef LHANDLE HENUM;

typedef struct _CREATESTRUCT {
    PVOID  pPresParams;
    PVOID  pCtlData;
    USHORT id;
    HWND   hwndInsertBehind;
    HWND   hwndOwner;
    short  cy;
    short  cx;
    short  y;
    short  x;
    ULONG  flStyle;
    PSZ    pszText;
    PSZ    pszClass;
    HWND   hwndParent;
} CREATESTRUCT, __far *PCREATESTRUCT;

typedef struct _CLASSINFO {
    ULONG  flClassStyle;
    PFNWP  pfnWindowProc;
    USHORT cbWindowData;
} CLASSINFO, __far *PCLASSINFO;

HENUM  __pascal __far WinBeginEnumWindows(HWND);
BOOL   __pascal __far WinEndEnumWindows(HENUM);
short  __pascal __far WinExcludeUpdateRegion(HPS,HWND);
HWND   __pascal __far WinGetNextWindow(HENUM);
HPS    __pascal __far WinGetScreenPS(HWND);
BOOL   __pascal __far WinIsThreadActive(HAB);
BOOL   __pascal __far WinLockVisRegions(HWND,BOOL);
HWND   __pascal __far WinLockWindow(HWND,BOOL);
BOOL   __pascal __far WinLockWindowUpdate(HWND,HWND);
BOOL   __pascal __far WinMapWindowPoints(HWND,HWND,PPOINTL,short);
HWND   __pascal __far WinQueryActiveWindow(HWND,BOOL);
BOOL   __pascal __far WinQueryClassInfo(HAB,PSZ,PCLASSINFO);
short  __pascal __far WinQueryClassName(HWND,short,PCH);
HWND   __pascal __far WinQuerySysModalWindow(HWND,BOOL);
BOOL   __pascal __far WinQueryUpdateRect(HWND,PRECTL);
short  __pascal __far WinQueryUpdateRegion(HWND,HRGN);
HDC    __pascal __far WinQueryWindowDC(HWND);
short  __pascal __far WinQueryWindowLockCount(HWND);
PVOID  __pascal __far WinQueryWindowPtr(HWND,short);
USHORT __pascal __far WinQueryWindowUShort(HWND,short);
ULONG  __pascal __far WinQueryWindowULong(HWND,short);
BOOL   __pascal __far WinRegisterWindowDestroy(HWND,BOOL);
BOOL   __pascal __far WinSetSysModalWindow(HWND,HWND);
BOOL   __pascal __far WinSetWindowBits(HWND,short,ULONG,ULONG);
BOOL   __pascal __far WinSetWindowPtr(HWND,short,PVOID);
BOOL   __pascal __far WinSetWindowULong(HWND,short,ULONG);
BOOL   __pascal __far WinSetWindowUShort(HWND,short,USHORT);
PFNWP  __pascal __far WinSubclassWindow(HWND,PFNWP);
BOOL   __pascal __far WinValidateRect(HWND,PRECTL,BOOL);
BOOL   __pascal __far WinValidateRegion(HWND,HRGN,BOOL);
HWND   __pascal __far WinWindowFromPoint(HWND,PPOINTL,BOOL,BOOL);
HWND   __pascal __far WinWindowFromDC(HDC);


















































































typedef LHANDLE HMQ;

typedef struct _COMMANDMSG {
    USHORT source;
    BOOL   fMouse;
    USHORT cmd;
    USHORT unused;
} CMDMSG;

typedef struct _MQINFO {
    USHORT cb;
    PID    pid;
    TID    tid;
    USHORT cmsgs;
    PVOID  pReserved;
} MQINFO, __far *PMQINFO;

typedef struct _QMSG {
    HWND   hwnd;
    USHORT msg;
    MPARAM mp1;
    MPARAM mp2;
    ULONG  time;
    POINTL ptl;
} QMSG, __far *PQMSG;

BOOL    __pascal __far WinCancelShutdown(HMQ,BOOL);
HMQ     __pascal __far WinCreateMsgQueue(HAB,short);
BOOL    __pascal __far WinDestroyMsgQueue(HMQ);
MRESULT __pascal __far WinDispatchMsg(HAB,PQMSG);
BOOL    __pascal __far WinGetMsg(HAB,PQMSG,HWND,USHORT,USHORT);
BOOL    __pascal __far WinPeekMsg(HAB,PQMSG,HWND,USHORT,USHORT,USHORT);
BOOL    __pascal __far WinPostMsg(HWND,USHORT,MPARAM,MPARAM);
BOOL    __pascal __far WinQueryQueueInfo(HMQ,PMQINFO,USHORT);
BOOL    __pascal __far WinRegisterUserDatatype(HAB,short,short,PSHORT);
BOOL    __pascal __far WinRegisterUserMsg(HAB,USHORT,short,short,short,short,short);
MRESULT __pascal __far WinSendMsg(HWND,USHORT,MPARAM,MPARAM);
BOOL    __pascal __far WinSetMsgMode(HAB,PSZ,short);
BOOL    __pascal __far WinSetSynchroMode(HAB,short);
















































typedef struct _WNDPARAMS {
    USHORT fsStatus;
    USHORT cchText;
    PSZ    pszText;
    USHORT cbPresParams;
    PVOID  pPresParams;
    USHORT cbCtlData;
    PVOID  pCtlData;
} WNDPARAMS, __far *PWNDPARAMS;

BOOL   __pascal __far WinBroadcastMsg(HWND,USHORT,MPARAM,MPARAM,USHORT);
BOOL   __pascal __far WinInSendMsg(HAB);
USHORT __pascal __far WinMsgMuxSemWait(PUSHORT,PVOID,long);
USHORT __pascal __far WinMsgSemWait(HSEM,long);
BOOL   __pascal __far WinPostQueueMsg(HMQ,USHORT,MPARAM,MPARAM);
BOOL   __pascal __far WinQueryMsgPos(HAB,PPOINTL);
ULONG  __pascal __far WinQueryMsgTime(HAB);
ULONG  __pascal __far WinQueryQueueStatus(HWND);
BOOL   __pascal __far WinSetClassMsgInterest(HAB,PSZ,USHORT,short);
BOOL   __pascal __far WinSetMsgInterest(HWND,USHORT,short);
BOOL   __pascal __far WinWaitMsg(HAB,USHORT,USHORT);


































































typedef struct _FRAMECDATA {
    USHORT  cb;
    ULONG   flCreateFlags;
    HMODULE hmodResources;
    USHORT  idResources;
} FRAMECDATA, __far *PFRAMECDATA;

HWND  __pascal __far WinCreateStdWindow(HWND,ULONG,PULONG,PSZ,PSZ,ULONG,HMODULE,USHORT,PHWND);




































































BOOL  __pascal __far WinCalcFrameRect(HWND,PRECTL,BOOL);
BOOL  __pascal __far WinCreateFrameControls(HWND,PFRAMECDATA,PSZ);
BOOL  __pascal __far WinFlashWindow(HWND,BOOL);
BOOL  __pascal __far WinGetMaxPosition(HWND,PSWP);
BOOL  __pascal __far WinGetMinPosition(HWND,PSWP,PPOINTL);










































typedef struct _SMHSTRUCT {
    MPARAM mp2;
    MPARAM mp1;
    USHORT msg;
    HWND   hwnd;
} SMHSTRUCT, __far *PSMHSTRUCT;

BOOL __pascal __far WinCallMsgFilter(HAB,PQMSG,USHORT);
BOOL __pascal __far WinReleaseHook(HAB,HMQ,short,PFN,HMODULE);
BOOL __pascal __far WinSetHook(HAB,HMQ,short,PFN,HMODULE);























































BOOL    __pascal __far WinAlarm(HWND,USHORT);
MRESULT __pascal __far WinDefDlgProc(HWND,USHORT,MPARAM,MPARAM);
BOOL    __pascal __far WinDismissDlg(HWND hwndDlg, USHORT usResult);
USHORT  __pascal __far WinDlgBox(HWND,HWND,PFNWP,HMODULE,USHORT,PVOID);
BOOL    __pascal __far WinGetDlgMsg(HWND,PQMSG);
HWND    __pascal __far WinLoadDlg(HWND,HWND,PFNWP,HMODULE,USHORT,PVOID);
USHORT  __pascal __far WinMessageBox(HWND,HWND,PSZ,PSZ,USHORT,USHORT);
BOOL    __pascal __far WinQueryDlgItemShort(HWND,USHORT,PSHORT,BOOL);
USHORT  __pascal __far WinQueryDlgItemText(HWND,USHORT,short,PSZ);
short   __pascal __far WinQueryDlgItemTextLength(HWND,USHORT);
BOOL    __pascal __far WinSetDlgItemShort(HWND,USHORT,USHORT,BOOL);
BOOL    __pascal __far WinSetDlgItemText(HWND,USHORT,PSZ);


























typedef struct _DLGTITEM {
    USHORT fsItemStatus;
    USHORT cChildren;
    USHORT cchClassName;
    USHORT offClassName;
    USHORT cchText;
    USHORT offText;
    ULONG  flStyle;
    short  x;
    short  y;
    short  cx;
    short  cy;
    USHORT id;
    USHORT offPresParams;
    USHORT offCtlData;
} DLGTITEM, __far *PDLGTITEM;

typedef struct _DLGTEMPLATE {
    USHORT   cbTemplate;
    USHORT   type;
    USHORT   codepage;
    USHORT   offadlgti;
    USHORT   fsTemplateStatus;
    USHORT   iItemFocus;
    USHORT   coffPresParams;
    DLGTITEM adlgti[1];
} DLGTEMPLATE, __far *PDLGTEMPLATE;

HWND    __pascal __far WinCreateDlg(HWND,HWND,PFNWP,PDLGTEMPLATE,PVOID);
HWND    __pascal __far WinEnumDlgItem(HWND,HWND,USHORT,BOOL);
BOOL    __pascal __far WinMapDlgPoints(HWND,PPOINTL,USHORT,BOOL);
USHORT  __pascal __far WinProcessDlg(HWND);
MRESULT __pascal __far WinSendDlgItemMsg(HWND,USHORT,USHORT,MPARAM,MPARAM);
USHORT  __pascal __far WinStartDlg(HWND);
short   __pascal __far WinSubstituteStrings(HWND,PSZ,short,PSZ);




















BOOL  __pascal __far WinFocusChange(HWND,HWND,USHORT);
BOOL  __pascal __far WinSetFocus(HWND,HWND);











































































































typedef struct _CHARMSG {
    USHORT chr;
    USHORT vkey;
    USHORT fs;
    UCHAR  cRepeat;
    UCHAR  scancode;
} CHRMSG;

typedef struct _MOUSEMSG {
    USHORT codeHitTest;
    USHORT unused;
    short  x;
    short  y;
} MSEMSG;

BOOL  __pascal __far WinIsPhysInputEnabled(HWND);
short __pascal __far WinGetKeyState(HWND,short);
short __pascal __far WinGetPhysKeyState(HWND,short);
BOOL  __pascal __far WinEnablePhysInput(HWND,BOOL);
HWND  __pascal __far WinQueryCapture(HWND,BOOL);
HWND  __pascal __far WinQueryFocus(HWND,BOOL);
BOOL  __pascal __far WinSetCapture(HWND,HWND);
BOOL  __pascal __far WinSetKeyboardStateTable(HWND,PBYTE,BOOL);


























BOOL   __pascal __far WinCloseClipbrd(HAB);
BOOL   __pascal __far WinEmptyClipbrd(HAB);
USHORT __pascal __far WinEnumClipbrdFmts(HAB,USHORT);
BOOL   __pascal __far WinOpenClipbrd(HAB);
ULONG  __pascal __far WinQueryClipbrdData(HAB,USHORT);
BOOL   __pascal __far WinQueryClipbrdFmtInfo(HAB,USHORT,PUSHORT);
HWND   __pascal __far WinQueryClipbrdOwner(HAB,BOOL);
HWND   __pascal __far WinQueryClipbrdViewer(HAB,BOOL);
BOOL   __pascal __far WinSetClipbrdData(HAB,ULONG,USHORT,USHORT);
BOOL   __pascal __far WinSetClipbrdOwner(HAB,HWND);
BOOL   __pascal __far WinSetClipbrdViewer(HAB,HWND);



















































typedef HMODULE HLIB;
typedef PHMODULE PHLIB;

BOOL  __pascal __far WinDeleteProcedure(HAB,PFNWP);
BOOL  __pascal __far WinDeleteLibrary(HAB,HLIB);
PFNWP __pascal __far WinLoadProcedure(HAB,HLIB,PSZ);
HLIB  __pascal __far WinLoadLibrary(HAB,PSZ);




























































typedef struct _MENUITEM {
    short  iPosition;
    USHORT afStyle;
    USHORT afAttribute;
    USHORT id;
    HWND   hwndSubMenu;
    ULONG  hItem;
} MENUITEM, __far *PMENUITEM;

typedef struct _OWNERITEM {
    HWND   hwnd;
    HPS    hps;
    USHORT fsState;
    USHORT fsAttribute;
    USHORT fsStateOld;
    USHORT fsAttributeOld;
    RECTL  rclItem;
    short  idItem;
    ULONG  hItem;
} OWNERITEM, __far *POWNERITEM;

HWND __pascal __far WinCreateMenu(HWND,PVOID);
HWND __pascal __far WinLoadMenu(HWND,HMODULE,USHORT);
































































































































typedef struct _PARAM {
    ULONG id;
    ULONG cb;
    BYTE  ab[1];
} PARAM, __far  *PPARAM, __near *NPPARAM;

typedef struct _PRESPARAMS {
    ULONG   cb;
    PARAM   aparam[1];
} PRESPARAMS, __far  *PPRESPARAMS, __near *NPPRESPARAMS;

ULONG __pascal __far WinQueryPresParam(HWND,ULONG,ULONG,PULONG,ULONG,PVOID,USHORT);
long  __pascal __far WinQuerySysColor(HWND,long,long);
long  __pascal __far WinQuerySysValue(HWND,short);
BOOL  __pascal __far WinRemovePresParam(HWND,ULONG);
BOOL  __pascal __far WinSetPresParam(HWND,ULONG,ULONG,PVOID);
BOOL  __pascal __far WinSetSysValue(HWND,short,long);
BOOL  __pascal __far WinSetSysColors(HWND,ULONG,ULONG,long,ULONG,PLONG);















typedef LHANDLE HACCEL;

typedef struct _ACCEL {
    USHORT fs;
    USHORT key;
    USHORT cmd;
} ACCEL, __far *PACCEL;

typedef struct _ACCELTABLE {
    USHORT cAccel;
    USHORT codepage;
    ACCEL  aaccel[1];
} ACCELTABLE, __far *PACCELTABLE;

USHORT __pascal __far WinCopyAccelTable(HACCEL,PACCELTABLE,USHORT);
HACCEL __pascal __far WinCreateAccelTable(HAB,PACCELTABLE);
BOOL   __pascal __far WinDestroyAccelTable(HACCEL);
HACCEL __pascal __far WinLoadAccelTable(HAB,HMODULE,USHORT);
HACCEL __pascal __far WinQueryAccelTable(HAB,HWND);
BOOL   __pascal __far WinSetAccelTable(HAB,HACCEL,HWND);
BOOL   __pascal __far WinTranslateAccel(HAB,HWND,HACCEL,PQMSG);







































typedef struct _BTNCDATA {
    USHORT  cb;
    USHORT  fsCheckState;
    USHORT  fsHiliteState;
} BTNCDATA, __far *PBTNCDATA;

typedef struct _USERBUTTON {
    HWND   hwnd;
    HPS    hps;
    USHORT fsState;
    USHORT fsStateOld;
} USERBUTTON, __far *PUSERBUTTON;
































































typedef struct _ENTRYFDATA {
    USHORT cb;
    USHORT cchEditLimit;
    USHORT ichMinSel;
    USHORT ichMaxSel;
} ENTRYFDATA, __far *PENTRYFDATA;





















































typedef struct _SBCDATA {
    USHORT cb;
    USHORT sHilite;
    short  posFirst;
    short  posLast;
    short  posThumb;
    short  cVisible;
    short  cTotal;
} SBCDATA, __far *PSBCDATA;

















typedef struct _TRACKINFO {
    short  cxBorder;
    short  cyBorder;
    short  cxGrid;
    short  cyGrid;
    short  cxKeyboard;
    short  cyKeyboard;
    RECTL  rclTrack;
    RECTL  rclBoundary;
    POINTL ptlMinTrackSize;
    POINTL ptlMaxTrackSize;
    USHORT fs;
} TRACKINFO, __far *PTRACKINFO;

BOOL __pascal __far WinShowTrackRect(HWND,BOOL);
BOOL __pascal __far WinTrackRect(HWND,HPS,PTRACKINFO);





BOOL __pascal __far WinCopyRect(HAB,PRECTL,PRECTL);
BOOL __pascal __far WinEqualRect(HAB,PRECTL,PRECTL);
BOOL __pascal __far WinInflateRect(HAB,PRECTL,short,short);
BOOL __pascal __far WinIntersectRect(HAB,PRECTL,PRECTL,PRECTL);
BOOL __pascal __far WinIsRectEmpty(HAB,PRECTL);
BOOL __pascal __far WinMakePoints(HAB,PWPOINT,USHORT);
BOOL __pascal __far WinMakeRect(HAB,PWRECT);
BOOL __pascal __far WinOffsetRect(HAB,PRECTL,short,short);
BOOL __pascal __far WinPtInRect(HAB,PRECTL,PPOINTL);
BOOL __pascal __far WinSetRect(HAB,PRECTL,short,short,short,short);
BOOL __pascal __far WinSetRectEmpty(HAB,PRECTL);
BOOL __pascal __far WinSubtractRect(HAB,PRECTL,PRECTL,PRECTL);
BOOL __pascal __far WinUnionRect(HAB,PRECTL,PRECTL,PRECTL);














































































typedef struct _POINTERINFO {
    BOOL    fPointer;
    short   xHotspot;
    short   yHotspot;
    HBITMAP hbmPointer;
    HBITMAP hbmColor;
} POINTERINFO, __far *PPOINTERINFO;

HPOINTER __pascal __far WinCreatePointer(HWND,HBITMAP,BOOL,short,short);
HPOINTER __pascal __far WinCreatePointerIndirect(HWND,PPOINTERINFO);
BOOL     __pascal __far WinDestroyPointer(HPOINTER);
BOOL     __pascal __far WinDrawPointer(HPS,short,short,HPOINTER,USHORT);
HBITMAP  __pascal __far WinGetSysBitmap(HWND,USHORT);
HPOINTER __pascal __far WinLoadPointer(HWND,HMODULE,USHORT);
HPOINTER __pascal __far WinQueryPointer(HWND);
BOOL     __pascal __far WinQueryPointerInfo(HPOINTER,PPOINTERINFO);
BOOL     __pascal __far WinQueryPointerPos(HWND,PPOINTL);
HPOINTER __pascal __far WinQuerySysPointer(HWND,short,BOOL);
BOOL     __pascal __far WinSetPointer(HWND,HPOINTER);
BOOL     __pascal __far WinSetPointerPos(HWND,short,short);
BOOL     __pascal __far WinShowPointer(HWND,BOOL);



















 #pragma read_only_file

























typedef LHANDLE HAPP;
typedef LHANDLE HPROGRAM, __far *PHPROGRAM;
typedef LHANDLE HINI, __far *PHINI;

typedef struct _XYWINSIZE {
    short  x;
    short  y;
    short  cx;
    short  cy;
    USHORT fsWindow;
} XYWINSIZE, __far *PXYWINSIZE;

typedef struct _PRFPROFILE {
    ULONG cchUserName;
    PSZ   pszUserName;
    ULONG cchSysName;
    PSZ   pszSysName;
} PRFPROFILE, __far *PPRFPROFILE;

HAPP __pascal __far __loadds WinInstStartApp(HINI,HWND,USHORT,PSZ __far*,PSZ,PVOID,USHORT);
BOOL __pascal __far __loadds WinTerminateApp(HAPP);























typedef struct _HPROGARRAY {
    HPROGRAM ahprog[1];
} HPROGARRAY, __far *PHPROGARRAY;

typedef char PROGCATEGORY, __far *PPROGCATEGORY;
typedef struct _PROGTYPE {
    PROGCATEGORY progc;
    UCHAR        fbVisible;
} PROGTYPE, __far *PPROGTYPE;

typedef struct _PROGRAMENTRY {
    HPROGRAM hprog;
    PROGTYPE progt;
    char     szTitle[60+1];
} PROGRAMENTRY, __far *PPROGRAMENTRY;

typedef struct _PIBSTRUCT {
    PROGTYPE  progt;
    char      szTitle[60+1];
    char      szIconFileName[128+1];
    char      szExecutable[128+1];
    char      szStartupDir[128+1];
    XYWINSIZE xywinInitial;
    USHORT    res1;
    LHANDLE   res2;
    USHORT    cchEnvironmentVars;
    PCH       pchEnvironmentVars;
    USHORT    cchProgramParameter;
    PCH       pchProgramParameter;
} PIBSTRUCT, __far *PPIBSTRUCT;

typedef struct _PROGDETAILS {
    ULONG     Length;
    PROGTYPE  progt;
    USHORT    pad1[3];
    PSZ       pszTitle;
    PSZ       pszExecutable;
    PSZ       pszParameters;
    PSZ       pszStartupDir;
    PSZ       pszIcon;
    PSZ       pszEnvironment;
    SWP       swpInitial;
    USHORT    pad2[5];
} PROGDETAILS, __far *PPROGDETAILS;

typedef struct _PROGTITLE {
    HPROGRAM hprog;
    PROGTYPE progt;
    USHORT   pad1[3];
    PSZ      pszTitle;
} PROGTITLE, __far *PPROGTITLE;

typedef struct _QFEOUTBLK {
    USHORT   Total;
    USHORT   Count;
    HPROGRAM ProgramArr[1];
} QFEOUTBLK, __far *PQFEOUTBLK;

HPROGRAM __pascal __far WinAddProgram(HAB,PPIBSTRUCT,HPROGRAM);
HPROGRAM __pascal __far WinCreateGroup(HAB,PSZ,UCHAR,ULONG,ULONG);
USHORT   __pascal __far WinQueryDefinition(HAB,HPROGRAM,PPIBSTRUCT,USHORT);
BOOL     __pascal __far WinQueryProgramTitles(HAB,HPROGRAM,PPROGRAMENTRY,USHORT,PUSHORT);

HPROGRAM     __pascal __far PrfAddProgram(HINI,PPROGDETAILS,HPROGRAM);
BOOL         __pascal __far PrfChangeProgram(HINI,HPROGRAM,PPROGDETAILS);
HPROGRAM     __pascal __far PrfCreateGroup(HINI,PSZ,UCHAR);
BOOL         __pascal __far PrfDestroyGroup(HINI,HPROGRAM);
ULONG        __pascal __far PrfQueryDefinition(HINI,HPROGRAM,PPROGDETAILS,ULONG);
PROGCATEGORY __pascal __far PrfQueryProgramCategory(HINI,PSZ);
ULONG        __pascal __far PrfQueryProgramHandle(HINI,PSZ,PHPROGARRAY,ULONG,PULONG);
ULONG        __pascal __far PrfQueryProgramTitles(HINI,HPROGRAM,PPROGTITLE,ULONG,PULONG);
BOOL         __pascal __far PrfRemoveProgram(HINI,HPROGRAM);





typedef LHANDLE HSWITCH, __far *PHSWITCH;







typedef struct _SWCNTRL {
    HWND     hwnd;
    HWND     hwndIcon;
    HPROGRAM hprog;
    USHORT   idProcess;
    USHORT   idSession;
    UCHAR    uchVisibility;
    UCHAR    fbJump;
    char     szSwtitle[60+1];
    BYTE     fReserved;
} SWCNTRL, __far *PSWCNTRL;

HSWITCH __pascal __far __loadds WinAddSwitchEntry(PSWCNTRL);
USHORT  __pascal __far __loadds WinRemoveSwitchEntry(HSWITCH);





typedef struct _SWENTRY {
    HSWITCH hswitch;
    SWCNTRL swctl;
} SWENTRY, __far *PSWENTRY;

typedef struct _SWBLOCK {
    USHORT  cswentry;
    SWENTRY aswentry[1];
} SWBLOCK, __far *PSWBLOCK;

USHORT   __pascal __far __loadds WinChangeSwitchEntry(HSWITCH,PSWCNTRL);
HSWITCH  __pascal __far __loadds WinCreateSwitchEntry(HAB,PSWCNTRL);
USHORT   __pascal __far __loadds WinQuerySessionTitle(HAB,USHORT,PSZ,USHORT);
USHORT   __pascal __far __loadds WinQuerySwitchEntry(HSWITCH,PSWCNTRL);
HSWITCH  __pascal __far __loadds WinQuerySwitchHandle(HWND,PID);
USHORT   __pascal __far __loadds WinQuerySwitchList(HAB,PSWBLOCK,USHORT);
USHORT   __pascal __far __loadds WinQueryTaskSizePos(HAB,USHORT,PSWP);
USHORT   __pascal __far __loadds WinQueryTaskTitle(USHORT,PSZ,USHORT);
USHORT   __pascal __far __loadds WinSwitchToProgram(HSWITCH);




    






BOOL   __pascal __far WinQueryProfileData(HAB,PSZ,PSZ,PVOID,PUSHORT);
short  __pascal __far WinQueryProfileInt(HAB,PSZ,PSZ,short);
USHORT __pascal __far WinQueryProfileSize(HAB,PSZ,PSZ,PUSHORT);
USHORT __pascal __far WinQueryProfileString(HAB,PSZ,PSZ,PSZ,PVOID,USHORT);
BOOL   __pascal __far WinWriteProfileData(HAB,PSZ,PSZ,PVOID,USHORT);
BOOL   __pascal __far WinWriteProfileString(HAB,PSZ,PSZ,PSZ);

BOOL   __pascal __far PrfCloseProfile(HINI);
HINI   __pascal __far PrfOpenProfile(HAB,PSZ);
BOOL   __pascal __far PrfQueryProfile(HAB,PPRFPROFILE);
BOOL   __pascal __far PrfQueryProfileData(HINI,PSZ,PSZ,PVOID,PULONG);
short  __pascal __far PrfQueryProfileInt(HINI,PSZ,PSZ,short);
BOOL   __pascal __far PrfQueryProfileSize(HINI,PSZ,PSZ,PULONG);
ULONG  __pascal __far PrfQueryProfileString(HINI,PSZ,PSZ,PSZ,PVOID,ULONG);
BOOL   __pascal __far PrfReset(HAB,PPRFPROFILE);
BOOL   __pascal __far PrfWriteProfileData(HINI,PSZ,PSZ,PVOID,ULONG);
BOOL   __pascal __far PrfWriteProfileString(HINI,PSZ,PSZ,PSZ);






    















 #pragma read_only_file















































































  


















 #pragma read_only_file
































































typedef int HELPSUBTABLE;
typedef int __far *PHELPSUBTABLE;

typedef struct _HELPTABLE {
    USHORT        idAppWindow;
    PHELPSUBTABLE phstHelpSubTable;
    USHORT        idExtPanel;
} HELPTABLE, __far *PHELPTABLE;

typedef struct _HELPINIT {
    USHORT     cb;
    ULONG      ulReturnCode;
    PSZ        pszTutorialName;
    PHELPTABLE phtHelpTable;
    HMODULE    hmodHelpTableModule;
    HMODULE    hmodAccelActionBarModule;
    USHORT     idAccelTable;
    USHORT     idActionBar;
    PSZ        pszHelpWindowTitle;
    USHORT     usShowPanelId;
    PSZ        pszHelpLibraryName;
} HELPINIT, __far *PHELPINIT;

BOOL __pascal __far  WinAssociateHelpInstance(HWND,HWND);
HWND __pascal __far  WinCreateHelpInstance(HAB,PHELPINIT);
BOOL __pascal __far  WinCreateHelpTable(HWND,PHELPTABLE);
BOOL __pascal __far  WinDestroyHelpInstance(HWND);
BOOL __pascal __far  WinLoadHelpTable(HWND,USHORT,HMODULE);
HWND __pascal __far  WinQueryHelpInstance(HWND);












typedef LHANDLE HHEAP;

NPBYTE __pascal __far WinAllocMem(HHEAP,USHORT);
USHORT __pascal __far WinAvailMem(HHEAP,BOOL,USHORT);
HHEAP  __pascal __far WinCreateHeap(SEL,USHORT,USHORT,USHORT,USHORT,USHORT);
HHEAP  __pascal __far WinDestroyHeap(HHEAP);
NPBYTE __pascal __far WinFreeMem(HHEAP,NPBYTE,USHORT);
PVOID  __pascal __far WinLockHeap(HHEAP);
NPBYTE __pascal __far WinReallocMem(HHEAP,NPBYTE,USHORT,USHORT);







typedef LHANDLE HATOMTBL;
typedef USHORT  ATOM;

HATOMTBL __pascal __far WinQuerySystemAtomTable(void);
HATOMTBL __pascal __far WinCreateAtomTable(USHORT,USHORT);
HATOMTBL __pascal __far WinDestroyAtomTable(HATOMTBL);
ATOM     __pascal __far WinAddAtom(HATOMTBL,PSZ);
ATOM     __pascal __far WinFindAtom(HATOMTBL,PSZ);
ATOM     __pascal __far WinDeleteAtom(HATOMTBL,ATOM);
USHORT   __pascal __far WinQueryAtomUsage(HATOMTBL,ATOM);
USHORT   __pascal __far WinQueryAtomLength(HATOMTBL,ATOM);
USHORT   __pascal __far WinQueryAtomName(HATOMTBL,ATOM,PSZ,USHORT);





typedef struct _CATCHBUF {
    ULONG reserved[ 4 ];
} CATCHBUF, __far *PCATCHBUF;

short __pascal __far WinCatch(PCATCHBUF);
void  __pascal __far WinThrow(PCATCHBUF,short);





















 #pragma read_only_file









typedef ULONG   ERRORID;
typedef ERRORID __far *PERRORID;

typedef struct _ERRINFO {
    USHORT  cbFixedErrInfo;
    ERRORID idError;
    USHORT  cDetailLevel;
    USHORT  offaoffszMsg;
    USHORT  offBinaryData;
} ERRINFO, __far *PERRINFO;

BOOL     __pascal __far WinFreeErrorInfo(PERRINFO);
PERRINFO __pascal __far WinGetErrorInfo(HAB);
ERRORID  __pascal __far WinGetLastError(HAB);





















 #pragma read_only_file





    
    
    
    
    
    
    
    
    
    
    
    
    



    












typedef  long FIXED, __far *PFIXED;
typedef  USHORT FIXED88;
typedef  USHORT FIXED114;

typedef struct _SIZEL {
    long cx;
    long cy;
} SIZEL, __far *PSIZEL;























BOOL __pascal __far GpiAssociate(HPS,HDC);
HPS  __pascal __far GpiCreatePS(HAB,HDC,PSIZEL,ULONG);
BOOL __pascal __far GpiDestroyPS(HPS);
BOOL __pascal __far GpiErase(HPS);
HDC  __pascal __far GpiQueryDevice(HPS);
BOOL __pascal __far GpiRestorePS(HPS,long);
long __pascal __far GpiSavePS(HPS);







































long  __pascal __far GpiErrorSegmentData(HPS,PLONG,PLONG);
long  __pascal __far GpiQueryDrawControl(HPS,long);
long  __pascal __far GpiQueryDrawingMode(HPS);
ULONG __pascal __far GpiQueryPS(HPS,PSIZEL);
long  __pascal __far GpiQueryStopDraw(HPS);
BOOL  __pascal __far GpiResetPS(HPS,ULONG);
BOOL  __pascal __far GpiSetDrawControl(HPS,long,long);
BOOL  __pascal __far GpiSetDrawingMode(HPS,long);
BOOL  __pascal __far GpiSetPS(HPS,PSIZEL,ULONG);
BOOL  __pascal __far GpiSetStopDraw(HPS,long);













long  __pascal __far GpiCorrelateChain(HPS,long,PPOINTL,long,long,PLONG);
long  __pascal __far GpiCorrelateFrom(HPS,long,long,long,PPOINTL,long,long,PLONG);
long  __pascal __far GpiCorrelateSegment(HPS,long,long,PPOINTL,long,long,PLONG);
BOOL  __pascal __far GpiQueryBoundaryData(HPS,PRECTL);
BOOL  __pascal __far GpiQueryPickAperturePosition(HPS,PPOINTL);
BOOL  __pascal __far GpiQueryPickApertureSize(HPS,PSIZEL);
BOOL  __pascal __far GpiQueryTag(HPS,PLONG);
BOOL  __pascal __far GpiResetBoundaryData(HPS);
BOOL  __pascal __far GpiSetPickAperturePosition(HPS,PPOINTL);
BOOL  __pascal __far GpiSetPickApertureSize(HPS,long,PSIZEL);
BOOL  __pascal __far GpiSetTag(HPS,long);
























BOOL __pascal __far GpiCloseSegment(HPS);
BOOL __pascal __far GpiDeleteSegment(HPS,long);
BOOL __pascal __far GpiDeleteSegments(HPS,long,long);
BOOL __pascal __far GpiDrawChain(HPS);
BOOL __pascal __far GpiDrawDynamics(HPS);
BOOL __pascal __far GpiDrawFrom(HPS,long,long);
BOOL __pascal __far GpiDrawSegment(HPS,long);
long __pascal __far GpiGetData(HPS,long,PLONG,long,long,PBYTE);
BOOL __pascal __far GpiOpenSegment(HPS,long);
long __pascal __far GpiPutData(HPS,long,PLONG,PBYTE);
long __pascal __far GpiQueryInitialSegmentAttrs(HPS,long);
long __pascal __far GpiQuerySegmentAttrs(HPS,long,long);
long __pascal __far GpiQuerySegmentNames(HPS,long,long,long,PLONG);
long __pascal __far GpiQuerySegmentPriority(HPS,long,long);
BOOL __pascal __far GpiRemoveDynamics(HPS,long,long);
BOOL __pascal __far GpiSetInitialSegmentAttrs(HPS,long,long);
BOOL __pascal __far GpiSetSegmentAttrs(HPS,long,long,long);
BOOL __pascal __far GpiSetSegmentPriority(HPS,long,long,long);









BOOL  __pascal __far GpiBeginElement(HPS,long,PSZ);
BOOL  __pascal __far GpiDeleteElement(HPS);
BOOL  __pascal __far GpiDeleteElementRange(HPS,long,long);
BOOL  __pascal __far GpiDeleteElementsBetweenLabels(HPS,long,long);
BOOL  __pascal __far GpiEndElement(HPS);
long  __pascal __far GpiElement(HPS,long,PSZ,long,PBYTE);
BOOL  __pascal __far GpiLabel(HPS,long);
BOOL  __pascal __far GpiOffsetElementPointer(HPS,long);
long  __pascal __far GpiQueryEditMode(HPS);
long  __pascal __far GpiQueryElement(HPS,long,long,PBYTE);
long  __pascal __far GpiQueryElementPointer(HPS);
long  __pascal __far GpiQueryElementType(HPS,PLONG,long,PSZ);
BOOL  __pascal __far GpiSetEditMode(HPS,long);
BOOL  __pascal __far GpiSetElementPointer(HPS,long);
BOOL  __pascal __far GpiSetElementPointerAtLabel(HPS,long);















typedef struct _MATRIXLF {
    FIXED fxM11;
    FIXED fxM12;
    long  lM13;
    FIXED fxM21;
    FIXED fxM22;
    long  lM23;
    long  lM31;
    long  lM32;
    long  lM33;
} MATRIXLF, __far *PMATRIXLF;

BOOL __pascal __far GpiConvert(HPS,long,long,long,PPOINTL);
long __pascal __far GpiCallSegmentMatrix(HPS,long,long,PMATRIXLF,long);
BOOL __pascal __far GpiQueryDefaultViewMatrix(HPS,long,PMATRIXLF);
BOOL __pascal __far GpiQueryGraphicsField(HPS,PRECTL);
BOOL __pascal __far GpiQueryModelTransformMatrix(HPS,long,PMATRIXLF);
BOOL __pascal __far GpiQueryPageViewport(HPS,PRECTL);
BOOL __pascal __far GpiQuerySegmentTransformMatrix(HPS,long,long,PMATRIXLF);
BOOL __pascal __far GpiQueryViewingTransformMatrix(HPS,long,PMATRIXLF);
BOOL __pascal __far GpiQueryViewingLimits(HPS,PRECTL);
BOOL __pascal __far GpiRotate(HPS,PMATRIXLF,long,FIXED,PPOINTL);
BOOL __pascal __far GpiScale(HPS,PMATRIXLF,long,PFIXED,PPOINTL);
BOOL __pascal __far GpiSetDefaultViewMatrix(HPS,long,PMATRIXLF,long);
BOOL __pascal __far GpiSetGraphicsField(HPS,PRECTL);
BOOL __pascal __far GpiSetModelTransformMatrix(HPS,long,PMATRIXLF,long);
BOOL __pascal __far GpiSetPageViewport(HPS,PRECTL);
BOOL __pascal __far GpiSetSegmentTransformMatrix(HPS,long,long,PMATRIXLF,long);
BOOL __pascal __far GpiSetViewingLimits(HPS,PRECTL);
BOOL __pascal __far GpiSetViewingTransformMatrix(HPS,long,PMATRIXLF,long);
BOOL __pascal __far GpiTranslate(HPS,PMATRIXLF,long,PPOINTL);















BOOL __pascal __far GpiBeginPath(HPS,long);
BOOL __pascal __far GpiCloseFigure(HPS);
BOOL __pascal __far GpiEndPath(HPS);
long __pascal __far GpiFillPath(HPS,long,long);
BOOL __pascal __far GpiModifyPath(HPS,long,long);
long __pascal __far GpiOutlinePath(HPS,long,long);
BOOL __pascal __far GpiSetClipPath(HPS,long,long);
long __pascal __far GpiStrokePath(HPS,long,ULONG);

























BOOL  __pascal __far GpiCreateLogColorTable(HPS,ULONG,long,long,long,PLONG);
BOOL  __pascal __far GpiQueryColorData(HPS,long,PLONG);
long  __pascal __far GpiQueryColorIndex(HPS,ULONG,long);
long  __pascal __far GpiQueryLogColorTable(HPS,ULONG,long,long,PLONG);
long  __pascal __far GpiQueryNearestColor(HPS,ULONG,long);
long  __pascal __far GpiQueryRealColors(HPS,ULONG,long,long,PLONG);
long  __pascal __far GpiQueryRGBColor(HPS,ULONG,long);
BOOL  __pascal __far GpiRealizeColorTable(HPS);
BOOL  __pascal __far GpiUnrealizeColorTable(HPS);







































































BOOL  __pascal __far GpiBeginArea(HPS,ULONG);
long  __pascal __far GpiBox(HPS,long,PPOINTL,long,long);
long  __pascal __far GpiCharString(HPS,long,PCH);
long  __pascal __far GpiCharStringAt(HPS,PPOINTL,long,PCH);
long  __pascal __far GpiEndArea(HPS);
long  __pascal __far GpiLine(HPS,PPOINTL);
BOOL  __pascal __far GpiMove(HPS,PPOINTL);
long  __pascal __far GpiPolyLine(HPS,long,PPOINTL);
long  __pascal __far GpiQueryColor(HPS);
long  __pascal __far GpiQueryPattern(HPS);
BOOL  __pascal __far GpiSetColor(HPS,long);
BOOL  __pascal __far GpiSetPattern(HPS,long);































































































































































typedef PVOID PBUNDLE;

typedef struct _ARCPARAMS {
    long lP;
    long lQ;
    long lR;
    long lS;
} ARCPARAMS, __far *PARCPARAMS;

typedef struct _SIZEF {
    FIXED cx;
    FIXED cy;
} SIZEF, __far *PSIZEF;

typedef struct _GRADIENTL {
    long x;
    long y;
} GRADIENTL, __far *PGRADIENTL;

typedef struct _LINEBUNDLE {
    long   lColor;
    long   lReserved;
    USHORT usMixMode;
    USHORT usReserved;
    FIXED  fxWidth;
    long   lGeomWidth;
    USHORT usType;
    USHORT usEnd;
    USHORT usJoin;
} LINEBUNDLE;

typedef struct _CHARBUNDLE {
    long   lColor;
    long   lBackColor;
    USHORT usMixMode;
    USHORT usBackMixMode;
    USHORT usSet;
    USHORT usPrecision;
    SIZEF  sizfxCell;
    POINTL ptlAngle;
    POINTL ptlShear;
    USHORT usDirection;
} CHARBUNDLE;

typedef struct _MARKERBUNDLE {
    long   lColor;
    long   lBackColor;
    USHORT usMixMode;
    USHORT usBackMixMode;
    USHORT usSet;
    USHORT usSymbol;
    SIZEF  sizfxCell;
} MARKERBUNDLE;

typedef struct _AREABUNDLE {
    long   lColor;
    long   lBackColor;
    USHORT usMixMode;
    USHORT usBackMixMode;
    USHORT usSet;
    USHORT usSymbol;
    POINTL ptlRefPoint ;
} AREABUNDLE;

typedef struct _IMAGEBUNDLE {
    long   lColor;
    long   lBackColor;
    USHORT usMixMode;
    USHORT usBackMixMode;
} IMAGEBUNDLE;

long  __pascal __far GpiCharStringPos(HPS,PRECTL,ULONG,long,PCH,PLONG);
long  __pascal __far GpiCharStringPosAt(HPS,PPOINTL,PRECTL,ULONG,long,PCH,PLONG);
BOOL  __pascal __far GpiComment(HPS,long,PBYTE);
long  __pascal __far GpiFullArc(HPS,long,FIXED);
long  __pascal __far GpiImage(HPS,long,PSIZEL,long,PBYTE);
long  __pascal __far GpiMarker(HPS,PPOINTL);
long  __pascal __far GpiPartialArc(HPS,PPOINTL,FIXED,FIXED,FIXED);
long  __pascal __far GpiPointArc(HPS,PPOINTL);
long  __pascal __far GpiPolyFillet(HPS,long,PPOINTL);
long  __pascal __far GpiPolyFilletSharp(HPS,long,PPOINTL,PFIXED);
long  __pascal __far GpiPolyMarker(HPS,long,PPOINTL);
long  __pascal __far GpiPolySpline(HPS,long,PPOINTL);
BOOL  __pascal __far GpiPop(HPS,long);
long  __pascal __far GpiPtVisible(HPS,PPOINTL);
BOOL  __pascal __far GpiQueryArcParams(HPS,PARCPARAMS);
long  __pascal __far GpiQueryAttrMode(HPS);
long  __pascal __far GpiQueryAttrs(HPS,long,ULONG,PBUNDLE);
long  __pascal __far GpiQueryBackColor(HPS);
long  __pascal __far GpiQueryBackMix(HPS);
BOOL  __pascal __far GpiQueryCharAngle(HPS,PGRADIENTL);
BOOL  __pascal __far GpiQueryCharBox(HPS,PSIZEF);
long  __pascal __far GpiQueryCharDirection(HPS);
long  __pascal __far GpiQueryCharMode(HPS);
long  __pascal __far GpiQueryCharSet(HPS);
BOOL  __pascal __far GpiQueryCharShear(HPS,PPOINTL);
BOOL  __pascal __far GpiQueryCharStringPos(HPS,ULONG,long,PCH,PLONG,PPOINTL);
BOOL  __pascal __far GpiQueryCharStringPosAt(HPS,PPOINTL,ULONG,long,PCH,PLONG,PPOINTL);
BOOL  __pascal __far GpiQueryCurrentPosition(HPS,PPOINTL);
BOOL  __pascal __far GpiQueryDefCharBox(HPS,PSIZEL);
long  __pascal __far GpiQueryLineEnd(HPS);
long  __pascal __far GpiQueryLineJoin(HPS);
long  __pascal __far GpiQueryLineType(HPS);
FIXED __pascal __far GpiQueryLineWidth(HPS);
long  __pascal __far GpiQueryLineWidthGeom(HPS);
long  __pascal __far GpiQueryMarker(HPS);
BOOL  __pascal __far GpiQueryMarkerBox(HPS,PSIZEF);
long  __pascal __far GpiQueryMarkerSet(HPS);
long  __pascal __far GpiQueryMix(HPS);
BOOL  __pascal __far GpiQueryPatternRefPoint(HPS,PPOINTL);
long  __pascal __far GpiQueryPatternSet(HPS);
BOOL  __pascal __far GpiQueryTextBox(HPS,long,PCH,long,PPOINTL);
long  __pascal __far GpiRectVisible(HPS,PRECTL);
BOOL  __pascal __far GpiSetArcParams(HPS,PARCPARAMS);
BOOL  __pascal __far GpiSetAttrMode(HPS,long);
BOOL  __pascal __far GpiSetAttrs(HPS,long,ULONG,ULONG,PBUNDLE);
BOOL  __pascal __far GpiSetBackColor(HPS,long);
BOOL  __pascal __far GpiSetBackMix(HPS,long);
BOOL  __pascal __far GpiSetCharAngle(HPS,PGRADIENTL);
BOOL  __pascal __far GpiSetCharBox(HPS,PSIZEF);
BOOL  __pascal __far GpiSetCharDirection(HPS,long);
BOOL  __pascal __far GpiSetCharMode(HPS,long);
BOOL  __pascal __far GpiSetCharSet(HPS,long);
BOOL  __pascal __far GpiSetCharShear(HPS,PPOINTL);
BOOL  __pascal __far GpiSetCurrentPosition(HPS,PPOINTL);
BOOL  __pascal __far GpiSetLineEnd(HPS,long);
BOOL  __pascal __far GpiSetLineJoin(HPS,long);
BOOL  __pascal __far GpiSetLineType(HPS,long);
BOOL  __pascal __far GpiSetLineWidth(HPS,FIXED);
BOOL  __pascal __far GpiSetLineWidthGeom(HPS,long);
BOOL  __pascal __far GpiSetMarker(HPS,long);
BOOL  __pascal __far GpiSetMarkerBox(HPS,PSIZEF);
BOOL  __pascal __far GpiSetMarkerSet(HPS,long);
BOOL  __pascal __far GpiSetMix(HPS,long);
BOOL  __pascal __far GpiSetPatternRefPoint(HPS,PPOINTL);
BOOL  __pascal __far GpiSetPatternSet(HPS,long);
















typedef char FFDESCS[2][32], __far *PFFDESCS;

typedef struct _KERNINGPAIRS {
    short sFirstChar;
    short sSecondChar;
    short sKerningAmount;
} KERNINGPAIRS, __far *PKERNINGPAIRS;

long   __pascal __far GpiCreateLogFont(HPS,PSTR8,long,PFATTRS);
BOOL   __pascal __far GpiDeleteSetId(HPS,long);
BOOL   __pascal __far GpiLoadFonts(HAB,PSZ);
USHORT __pascal __far GpiQueryCp(HPS);
long   __pascal __far GpiQueryFontFileDescriptions(HAB,PSZ,PLONG,PFFDESCS);
long   __pascal __far GpiQueryFonts(HPS,ULONG,PSZ,PLONG,long,PFONTMETRICS);
BOOL   __pascal __far GpiQueryFontMetrics(HPS,long,PFONTMETRICS);
long   __pascal __far GpiQueryKerningPairs(HPS,long,PKERNINGPAIRS);
long   __pascal __far GpiQueryNumberSetIds(HPS);
BOOL   __pascal __far GpiQuerySetIds(HPS,long,PLONG,PSTR8,PLONG);
BOOL   __pascal __far GpiQueryWidthTable(HPS,long,long,PLONG);
BOOL   __pascal __far GpiSetCp(HPS,USHORT);
BOOL   __pascal __far GpiUnloadFonts(HAB,PSZ);



























long    __pascal __far GpiBitBlt(HPS,HPS,long,PPOINTL,long,ULONG);
BOOL    __pascal __far GpiDeleteBitmap(HBITMAP);
HBITMAP __pascal __far GpiLoadBitmap(HPS,HMODULE,USHORT,long,long);
HBITMAP __pascal __far GpiSetBitmap(HPS,HBITMAP);
long    __pascal __far GpiWCBitBlt(HPS,HBITMAP,long,PPOINTL,long,ULONG);









typedef struct _BITMAPINFOHEADER {
    ULONG  cbFix;
    USHORT cx;
    USHORT cy;
    USHORT cPlanes;
    USHORT cBitCount;
} BITMAPINFOHEADER, __far *PBITMAPINFOHEADER;

typedef struct _RGB {
    BYTE bBlue;
    BYTE bGreen;
    BYTE bRed;
} RGB;

typedef struct _BITMAPINFO {
    ULONG  cbFix;
    USHORT cx;
    USHORT cy;
    USHORT cPlanes;
    USHORT cBitCount;
    RGB    argbColor[1];
} BITMAPINFO, __far *PBITMAPINFO;

HBITMAP __pascal __far GpiCreateBitmap(HPS,PBITMAPINFOHEADER,ULONG,PBYTE,PBITMAPINFO);
long    __pascal __far GpiQueryBitmapBits(HPS,long,long,PBYTE,PBITMAPINFO);
BOOL    __pascal __far GpiQueryBitmapDimension(HBITMAP,PSIZEL);
HBITMAP __pascal __far GpiQueryBitmapHandle(HPS,long);
BOOL    __pascal __far GpiQueryBitmapParameters(HBITMAP,PBITMAPINFOHEADER);
BOOL    __pascal __far GpiQueryDeviceBitmapFormats(HPS,long,PLONG);
long    __pascal __far GpiQueryPel(HPS,PPOINTL);
long    __pascal __far GpiSetBitmapBits(HPS,long,long,PBYTE,PBITMAPINFO);
BOOL    __pascal __far GpiSetBitmapDimension(HBITMAP,PSIZEL);
BOOL    __pascal __far GpiSetBitmapId(HPS,HBITMAP,long);
long    __pascal __far GpiSetPel(HPS,PPOINTL);




































typedef struct _RGNRECT {
    USHORT ircStart;
    USHORT crc;
    USHORT crcReturned;
    USHORT usDirection;
} RGNRECT, __far *PRGNRECT;

long  __pascal __far GpiCombineRegion(HPS,HRGN,HRGN,HRGN,long);
HRGN  __pascal __far GpiCreateRegion(HPS,long,PRECTL);
BOOL  __pascal __far GpiDestroyRegion(HPS,HRGN);
long  __pascal __far GpiEqualRegion(HPS,HRGN,HRGN);
long  __pascal __far GpiExcludeClipRectangle(HPS,PRECTL);
long  __pascal __far GpiIntersectClipRectangle(HPS,PRECTL);
long  __pascal __far GpiOffsetClipRegion(HPS,PPOINTL);
BOOL  __pascal __far GpiOffsetRegion(HPS,HRGN,PPOINTL);
long  __pascal __far GpiPaintRegion(HPS,HRGN);
long  __pascal __far GpiPtInRegion(HPS,HRGN,PPOINTL);
long  __pascal __far GpiQueryRegionBox(HPS,HRGN,PRECTL);
BOOL  __pascal __far GpiQueryRegionRects(HPS,HRGN,PRECTL,PRGNRECT,PRECTL);
long  __pascal __far GpiRectInRegion(HPS,HRGN,PRECTL);
BOOL  __pascal __far GpiSetRegion(HPS,HRGN,long,PRECTL);
long  __pascal __far GpiSetClipRegion(HPS,HRGN,PHRGN);
HRGN  __pascal __far GpiQueryClipRegion(HPS);
long  __pascal __far GpiQueryClipBox(HPS,PRECTL);









































HMF   __pascal __far GpiCopyMetaFile(HMF);
BOOL  __pascal __far GpiDeleteMetaFile(HMF);
HMF   __pascal __far GpiLoadMetaFile(HAB,PSZ);
long  __pascal __far GpiPlayMetaFile(HPS,HMF,long,PLONG,PLONG,long,PSZ);
BOOL  __pascal __far GpiQueryMetaFileBits(HMF,long,long,PBYTE);
long  __pascal __far GpiQueryMetaFileLength(HMF);
BOOL  __pascal __far GpiSaveMetaFile(HMF,PSZ);
BOOL  __pascal __far GpiSetMetaFileBits(HMF,long,long,PBYTE);





BOOL  __pascal __far GpiQueryDefArcParams(HPS,PARCPARAMS);
BOOL  __pascal __far GpiQueryDefAttrs(HPS,long,ULONG,PBUNDLE);
BOOL  __pascal __far GpiQueryDefTag(HPS,PLONG);
BOOL  __pascal __far GpiQueryDefViewingLimits(HPS,PRECTL);
BOOL  __pascal __far GpiSetDefArcParams(HPS,PARCPARAMS);
BOOL  __pascal __far GpiSetDefAttrs(HPS,long,ULONG,PBUNDLE);
BOOL  __pascal __far GpiSetDefTag(HPS,long);
BOOL  __pascal __far GpiSetDefViewingLimits(HPS,PRECTL);






















 #pragma read_only_file


























 #pragma read_only_file





    




























































































typedef PSZ __far *PDEVOPENDATA;

HMF  __pascal __far DevCloseDC(HDC);
HDC  __pascal __far DevOpenDC(HAB,long,PSZ,long,PDEVOPENDATA,HDC);
BOOL __pascal __far DevQueryCaps(HDC,long,long,PLONG);






































typedef char STR16[16], __far *PSTR16;
typedef char STR32[32], __far *PSTR32;
typedef char STR64[64], __far *PSTR64;

typedef struct _VIOSIZECOUNT {
    long maxcount;
    long count;
} VIOSIZECOUNT, __far * PVIOSIZECOUNT;

typedef struct _VIOFONTCELLSIZE {
    long cx;
    long cy;
} VIOFONTCELLSIZE, __far * PVIOFONTCELLSIZE;

typedef struct _ESCGETCP {
    ULONG  options;
    USHORT codepage;
    USHORT fontid;
} ESCGETCP, __far * PESCGETCP;

typedef struct _SFACTORS {
    long x;
    long y;
} SFACTORS, __far * PSFACTORS;

typedef struct _BANDRECT {
    long xleft;
    long ybottom;
    long xright;
    long ytop;
} BANDRECT, __far * PBANDRECT;

typedef struct _HCINFO {
    char szFormname[32];
    long cx;
    long cy;
    long xLeftClip;
    long yBottomClip;
    long xRightClip;
    long yTopClip;
    long xPels;
    long yPels;
    long flAttributes;
} HCINFO, __far *PHCINFO;

typedef struct _ESCSETMODE {
    ULONG  mode;
    USHORT codepage;
} ESCSETMODE, __far * PESCSETMODE;

long  __pascal __far DevEscape(HDC,long,long,PBYTE,PLONG,PBYTE);
long  __pascal __far DevPostDeviceModes(HAB,PDRIVDATA,PSZ,PSZ,PSZ,ULONG);
BOOL  __pascal __far DevQueryDeviceNames(HAB,PSZ,PLONG,PSTR32,PSTR64,PLONG,PSTR16);
long  __pascal __far DevQueryHardcopyCaps(HDC,long,long,PHCINFO);























 #pragma read_only_file











typedef struct _BITMAPFILEHEADER {
    USHORT usType;
    ULONG  cbSize;
    short  xHotspot;
    short  yHotspot;
    ULONG  offBits;
    BITMAPINFOHEADER bmp;
} BITMAPFILEHEADER, __far *PBITMAPFILEHEADER;

typedef struct _BITMAPARRAYFILEHEADER {
    USHORT usType;
    ULONG  cbSize;
    ULONG  offNext;
    USHORT cxDisplay;
    USHORT cyDisplay;
    BITMAPFILEHEADER bfh;
} BITMAPARRAYFILEHEADER, __far *PBITMAPARRAYFILEHEADER;






