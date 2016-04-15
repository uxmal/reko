#line 1 "windows.h"






















































































#pragma option -a-      
#line 89 "windows.h"













#line 103 "windows.h"



#line 107 "windows.h"

#line 109 "windows.h"




#line 114 "windows.h"




























#line 143 "windows.h"















typedef int                 BOOL;



typedef unsigned char       BYTE;
typedef unsigned short      WORD;
typedef unsigned long       DWORD;

typedef unsigned int        UINT;





#line 173 "windows.h"












#line 186 "windows.h"


#line 189 "windows.h"
#line 190 "windows.h"


typedef UINT WPARAM;
typedef long LPARAM;
typedef long LRESULT;









#line 205 "windows.h"

#line 207 "windows.h"
#line 208 "windows.h"

typedef char _near*          PSTR;
typedef char _near*          NPSTR;


typedef char _far*           LPSTR;
typedef const char _far*     LPCSTR;

typedef BYTE _near*          PBYTE;
typedef BYTE _far*           LPBYTE;

typedef int _near*           PINT;
typedef int _far*            LPINT;

typedef WORD _near*          PWORD;
typedef WORD _far*           LPWORD;

typedef long _near*          PLONG;
typedef long _far*           LPLONG;

typedef DWORD _near*         PDWORD;
typedef DWORD _far*          LPDWORD;

typedef void _far*           LPVOID;
















typedef UINT                    HANDLE;


#line 252 "windows.h"

typedef HANDLE*             PHANDLE;
typedef HANDLE _near*        SPHANDLE;
typedef HANDLE _far*         LPHANDLE;

typedef HANDLE              HGLOBAL;
typedef HANDLE              HLOCAL;

typedef HANDLE              GLOBALHANDLE;
typedef HANDLE              LOCALHANDLE;

typedef UINT                ATOM;





typedef int (_far _pascal*      FARPROC)();
typedef int (_near _pascal*   NEARPROC)();
#line 272 "windows.h"

typedef UINT HSTR;



typedef UINT HINSTANCE;
typedef HINSTANCE HMODULE;  








#line 288 "windows.h"
int     _pascal WinMain(HANDLE, HANDLE, LPSTR, int);
int _far _pascal LibMain(HANDLE, WORD, WORD, LPSTR);
#line 291 "windows.h"
int _far _pascal   WEP(int);



DWORD   _far _pascal GetVersion(void);

DWORD   _far _pascal GetFreeSpace(UINT);
UINT    _far _pascal GetCurrentPDB(void);

UINT    _far _pascal GetWindowsDirectory(LPSTR, UINT);
UINT    _far _pascal GetSystemDirectory(LPSTR, UINT);


UINT    _far _pascal GetFreeSystemResources(UINT);



#line 309 "windows.h"

DWORD   _far _pascal GetWinFlags(void);

















LPSTR   _far _pascal GetDOSEnvironment(void);

DWORD   _far _pascal GetCurrentTime(void);
DWORD   _far _pascal GetTickCount(void);
DWORD   _far _pascal GetTimerResolution(void);






void    _far _pascal LogError(UINT err, void _far* lpInfo);
void    _far _pascal LogParamError(UINT err, FARPROC lpfn, void _far* param);



























































































typedef struct tagWINDEBUGINFO
{
    UINT    flags;
    DWORD   dwOptions;
    DWORD   dwFilter;
    char    achAllocModule[8];
    DWORD   dwAllocBreak;
    DWORD   dwAllocCount;
} WINDEBUGINFO;

BOOL    _far _pascal GetWinDebugInfo(WINDEBUGINFO _far* lpwdi, UINT flags);
BOOL    _far _pascal SetWinDebugInfo(const WINDEBUGINFO _far* lpwdi);

void    _far _cdecl DebugOutput(UINT flags, LPCSTR lpsz, ...);






































#line 485 "windows.h"
#line 486 "windows.h"

void    _far _pascal FatalExit(int);
void    _far _pascal FatalAppExit(UINT, LPCSTR);

BOOL    _far _pascal ExitWindows(DWORD dwReturnCode, UINT wReserved);





BOOL    _far _pascal ExitWindowsExec(LPCSTR, LPCSTR);
#line 498 "windows.h"

void    _far _pascal DebugBreak(void);
void    _far _pascal OutputDebugString(LPCSTR);






UINT    _far _pascal SetErrorMode(UINT);





BOOL    _far _pascal IsBadReadPtr(const void _far* lp, UINT cb);
BOOL    _far _pascal IsBadWritePtr(void _far* lp, UINT cb);
BOOL    _far _pascal IsBadHugeReadPtr(const void _huge* lp, DWORD cb);
BOOL    _far _pascal IsBadHugeWritePtr(void _huge* lp, DWORD cb);
BOOL    _far _pascal IsBadCodePtr(FARPROC lpfn);
BOOL    _far _pascal IsBadStringPtr(const void _far* lpsz, UINT cchMax);
#line 520 "windows.h"





int     _far _pascal ProfInsChk(void);
void    _far _pascal ProfSetup(int,int);
void    _far _pascal ProfSampRate(int,int);
void    _far _pascal ProfStart(void);
void    _far _pascal ProfStop(void);
void    _far _pascal ProfClear(void);
void    _far _pascal ProfFlush(void);
void    _far _pascal ProfFinish(void);
#line 534 "windows.h"



typedef int CATCHBUF[9];
typedef int _far* LPCATCHBUF;

int     _far _pascal Catch(int _far*);
void    _far _pascal Throw(const int _far*, int);

void    _far _pascal SwitchStackBack(void);
void    _far _pascal SwitchStackTo(UINT, UINT, UINT);






HINSTANCE   _far _pascal LoadModule(LPCSTR, LPVOID);
BOOL        _far _pascal FreeModule(HINSTANCE);

HINSTANCE   _far _pascal LoadLibrary(LPCSTR);
void        _far _pascal FreeLibrary(HINSTANCE);

UINT    _far _pascal WinExec(LPCSTR, UINT);

HMODULE _far _pascal GetModuleHandle(LPCSTR);

int     _far _pascal GetModuleUsage(HINSTANCE);
int     _far _pascal GetModuleFileName(HINSTANCE, LPSTR, int);

FARPROC _far _pascal GetProcAddress(HINSTANCE, LPCSTR);

int     _far _pascal GetInstanceData(HINSTANCE, PBYTE, int);

HGLOBAL _far _pascal GetCodeHandle(FARPROC);

typedef struct tagSEGINFO
{
    UINT offSegment;
    UINT cbSegment;
    UINT flags;
    UINT cbAlloc;
    HGLOBAL h;
    UINT alignShift;
    UINT reserved[2];
} SEGINFO;
typedef SEGINFO _far* LPSEGINFO;

void    _far _pascal GetCodeInfo(FARPROC lpProc, SEGINFO _far* lpSegInfo);

FARPROC _far _pascal MakeProcInstance(FARPROC, HINSTANCE);
void    _far _pascal FreeProcInstance(FARPROC);






long    _far _pascal SetSwapAreaSize(UINT);
void    _far _pascal SwapRecording(UINT);
void    _far _pascal ValidateCodeSegments(void);







#line 603 "windows.h"

typedef UINT HTASK;



UINT    _far _pascal GetNumTasks(void);


BOOL    _far _pascal IsTask(HTASK);
#line 613 "windows.h"

HTASK   _far _pascal GetCurrentTask(void);

void    _far _pascal Yield(void);
void    _far _pascal DirectedYield(HTASK);


























HGLOBAL _far _pascal GlobalAlloc(UINT, DWORD);
HGLOBAL _far _pascal GlobalReAlloc(HGLOBAL, DWORD, UINT);
HGLOBAL _far _pascal GlobalFree(HGLOBAL);

DWORD   _far _pascal GlobalDosAlloc(DWORD);
UINT    _far _pascal GlobalDosFree(UINT);




char _far* _far _pascal GlobalLock(HGLOBAL);
#line 656 "windows.h"

BOOL    _far _pascal GlobalUnlock(HGLOBAL);

DWORD   _far _pascal GlobalSize(HGLOBAL);
DWORD   _far _pascal GlobalHandle(UINT);




UINT    _far _pascal GlobalFlags(HGLOBAL);




char _far* _far _pascal GlobalWire(HGLOBAL);
#line 672 "windows.h"

BOOL    _far _pascal GlobalUnWire(HGLOBAL);

UINT    _far _pascal GlobalPageLock(HGLOBAL);
UINT    _far _pascal GlobalPageUnlock(HGLOBAL);

void    _far _pascal GlobalFix(HGLOBAL);
void    _far _pascal GlobalUnfix(HGLOBAL);

HGLOBAL _far _pascal GlobalLRUNewest(HGLOBAL);
HGLOBAL _far _pascal GlobalLRUOldest(HGLOBAL);

DWORD   _far _pascal GlobalCompact(DWORD);




typedef FARPROC GNOTIFYPROC;
#line 691 "windows.h"

void    _far _pascal GlobalNotify(GNOTIFYPROC);

HGLOBAL _far _pascal LockSegment(UINT);
void    _far _pascal UnlockSegment(UINT);




UINT    _far _pascal AllocSelector(UINT);
UINT    _far _pascal FreeSelector(UINT);
UINT    _far _pascal AllocDStoCSAlias(UINT);
UINT    _far _pascal PrestoChangoSelector(UINT sourceSel, UINT destSel);
DWORD   _far _pascal GetSelectorBase(UINT);
UINT    _far _pascal SetSelectorBase(UINT, DWORD);
DWORD   _far _pascal GetSelectorLimit(UINT);
UINT    _far _pascal SetSelectorLimit(UINT, DWORD);

void    _far _pascal LimitEmsPages(DWORD);

void    _far _pascal ValidateFreeSpaces(void);

























HLOCAL  _far _pascal LocalAlloc(UINT, UINT);
HLOCAL  _far _pascal LocalReAlloc(HLOCAL, UINT, UINT);
HLOCAL  _far _pascal LocalFree(HLOCAL);




char _near* _far _pascal LocalLock(HLOCAL);
#line 746 "windows.h"

BOOL    _far _pascal LocalUnlock(HLOCAL);

UINT    _far _pascal LocalSize(HLOCAL);



HLOCAL  _far _pascal LocalHandle(UINT);
#line 755 "windows.h"





UINT    _far _pascal LocalFlags(HLOCAL);

BOOL    _far _pascal LocalInit(UINT, UINT, UINT);
UINT    _far _pascal LocalCompact(UINT);
UINT    _far _pascal LocalShrink(HLOCAL, UINT);

#line 767 "windows.h"





typedef int HFILE;      






typedef struct tagOFSTRUCT
{
    BYTE cBytes;
    BYTE fFixedDisk;
    UINT nErrCode;
    BYTE reserved[4];
    char szPathName[128];
} OFSTRUCT;
typedef OFSTRUCT*       POFSTRUCT;
typedef OFSTRUCT _near* NPOFSTRUCT;
typedef OFSTRUCT _far*  LPOFSTRUCT;




















HFILE   _far _pascal OpenFile(LPCSTR, OFSTRUCT _far*, UINT);

#line 813 "windows.h"






HFILE   _far _pascal _lopen(LPCSTR, int);
HFILE   _far _pascal _lcreat(LPCSTR, int);

HFILE   _far _pascal _lclose(HFILE);

long    _far _pascal _llseek(HFILE, long, int);






UINT    _far _pascal _lread(HFILE, void _huge*, UINT);
UINT    _far _pascal _lwrite(HFILE, const void _huge*, UINT);


long    _far _pascal _hread(HFILE, void _huge*, long);
long    _far _pascal _hwrite(HFILE, const void _huge*, long);
#line 838 "windows.h"


#line 841 "windows.h"




int     _far _pascal GetTempFileName(BYTE, LPCSTR, UINT, LPSTR);
BYTE    _far _pascal GetTempDrive(char);





UINT    _far _pascal GetDriveType(int);

UINT    _far _pascal SetHandleCount(UINT);


UINT _far _pascal WNetAddConnection(LPSTR, LPSTR, LPSTR);
UINT _far _pascal WNetGetConnection(LPSTR, LPSTR, UINT _far*);
UINT _far _pascal WNetCancelConnection(LPSTR, BOOL);



























typedef UINT HRSRC;

HRSRC   _far _pascal FindResource(HINSTANCE, LPCSTR, LPCSTR);
HGLOBAL _far _pascal LoadResource(HINSTANCE, HRSRC);
BOOL    _far _pascal FreeResource(HGLOBAL);




char _far* _far _pascal LockResource(HGLOBAL);
#line 898 "windows.h"



DWORD   _far _pascal SizeofResource(HINSTANCE, HRSRC);

int     _far _pascal AccessResource(HINSTANCE, HRSRC);

HGLOBAL _far _pascal AllocResource(HINSTANCE, HRSRC, DWORD);




typedef FARPROC RSRCHDLRPROC;
#line 912 "windows.h"

RSRCHDLRPROC _far _pascal SetResourceHandler(HINSTANCE, LPCSTR, RSRCHDLRPROC);



#line 918 "windows.h"

#line 920 "windows.h"


















#line 939 "windows.h"




































































#line 1008 "windows.h"

#line 1010 "windows.h"



BOOL    _far _pascal InitAtomTable(int);
ATOM    _far _pascal AddAtom(LPCSTR);
ATOM    _far _pascal DeleteAtom(ATOM);
ATOM    _far _pascal FindAtom(LPCSTR);
UINT    _far _pascal GetAtomName(ATOM, LPSTR, int);
ATOM    _far _pascal GlobalAddAtom(LPCSTR);
ATOM    _far _pascal GlobalDeleteAtom(ATOM);
ATOM    _far _pascal GlobalFindAtom(LPCSTR);
UINT    _far _pascal GlobalGetAtomName(ATOM, LPSTR, int);
HLOCAL  _far _pascal GetAtomHandle(ATOM);

#line 1025 "windows.h"




UINT    _far _pascal GetProfileInt(LPCSTR, LPCSTR, int);
int     _far _pascal GetProfileString(LPCSTR, LPCSTR, LPCSTR, LPSTR, int);
BOOL    _far _pascal WriteProfileString(LPCSTR, LPCSTR, LPCSTR);

UINT    _far _pascal GetPrivateProfileInt(LPCSTR, LPCSTR, int, LPCSTR);
int     _far _pascal GetPrivateProfileString(LPCSTR, LPCSTR, LPCSTR, LPSTR, int, LPCSTR);
BOOL    _far _pascal WritePrivateProfileString(LPCSTR, LPCSTR, LPCSTR, LPCSTR);





void    _far _pascal AnsiToOem(const char _huge*, char _huge*);
void    _far _pascal OemToAnsi(const char _huge*, char _huge*);

void    _far _pascal AnsiToOemBuff(LPCSTR, LPSTR, UINT);
void    _far _pascal OemToAnsiBuff(LPCSTR, LPSTR, UINT);

LPSTR   _far _pascal AnsiNext(LPCSTR);
LPSTR   _far _pascal AnsiPrev(LPCSTR, LPCSTR);

LPSTR   _far _pascal AnsiUpper(LPSTR);
LPSTR   _far _pascal AnsiLower(LPSTR);

UINT    _far _pascal AnsiUpperBuff(LPSTR, UINT);
UINT    _far _pascal AnsiLowerBuff(LPSTR, UINT);



BOOL    _far _pascal IsCharAlpha(char);
BOOL    _far _pascal IsCharAlphaNumeric(char);
BOOL    _far _pascal IsCharUpper(char);
BOOL    _far _pascal IsCharLower(char);
#line 1063 "windows.h"


int     _far _pascal lstrcmp(LPCSTR, LPCSTR);
int     _far _pascal lstrcmpi(LPCSTR, LPCSTR);
LPSTR   _far _pascal lstrcpy(LPSTR, LPCSTR);
LPSTR   _far _pascal lstrcat(LPSTR, LPCSTR);
int     _far _pascal lstrlen(LPCSTR);

LPSTR   _far _pascal lstrcpyn(LPSTR, LPCSTR, int);
void    _far _pascal hmemcpy(void _huge*, const void _huge*, long);
#line 1074 "windows.h"
#line 1075 "windows.h"



BOOL    _far _pascal IsDBCSLeadByte(BYTE);
#line 1080 "windows.h"
#line 1081 "windows.h"

int     _far _pascal LoadString(HINSTANCE, UINT, LPSTR, int);





DWORD   _far _pascal OemKeyScan(UINT);
UINT    _far _pascal VkKeyScan(UINT);
int     _far _pascal GetKeyboardType(int);
UINT    _far _pascal MapVirtualKey(UINT, UINT);
int     _far _pascal GetKBCodePage(void);
int     _far _pascal GetKeyNameText(long, LPSTR, int);
int     _far _pascal ToAscii(UINT wVirtKey, UINT wScanCode, BYTE _far* lpKeyState, DWORD _far* lpChar, UINT wFlags);

#line 1097 "windows.h"

#line 1099 "windows.h"



typedef UINT HDC;






typedef UINT HGDIOBJ;
#line 1111 "windows.h"

#line 1113 "windows.h"

typedef UINT HBITMAP;
typedef UINT HPEN;
typedef UINT HBRUSH;
typedef UINT HRGN;
typedef UINT HPALETTE;
typedef UINT HFONT;

typedef struct tagRECT
{
    int left;
    int top;
    int right;
    int bottom;
} RECT;
typedef RECT*      PRECT;
typedef RECT _near* NPRECT;
typedef RECT _far*  LPRECT;

typedef struct tagPOINT
{
    int x;
    int y;
} POINT;
typedef POINT*       PPOINT;
typedef POINT _near* NPPOINT;
typedef POINT _far*  LPPOINT;


typedef struct tagSIZE
{
    int cx;
    int cy;
} SIZE;
typedef SIZE*       PSIZE;
typedef SIZE _near* NPSIZE;
typedef SIZE _far*  LPSIZE;
#line 1151 "windows.h"







HDC     _far _pascal CreateDC(LPCSTR, LPCSTR, LPCSTR, const void _far*);
HDC     _far _pascal CreateIC(LPCSTR, LPCSTR, LPCSTR, const void _far*);
HDC     _far _pascal CreateCompatibleDC(HDC);

BOOL    _far _pascal DeleteDC(HDC);

DWORD   _far _pascal GetDCOrg(HDC);

int     _far _pascal SaveDC(HDC);
BOOL    _far _pascal RestoreDC(HDC, int);

int     _far _pascal SetEnvironment(LPCSTR, const void _far*, UINT);
int     _far _pascal GetEnvironment(LPCSTR, void _far*, UINT);

int     _far _pascal MulDiv(int, int, int);



UINT    _far _pascal SetBoundsRect(HDC hDC, const RECT _far* lprcBounds, UINT flags);
UINT    _far _pascal GetBoundsRect(HDC hDC, RECT _far* lprcBounds, UINT flags);







#line 1186 "windows.h"



int _far _pascal GetDeviceCaps(HDC, int);

























































































































#line 1312 "windows.h"



int     _far _pascal SetMapMode(HDC, int);
int     _far _pascal GetMapMode(HDC);











DWORD   _far _pascal SetWindowOrg(HDC, int, int);
DWORD   _far _pascal GetWindowOrg(HDC);

DWORD   _far _pascal SetWindowExt(HDC, int, int);
DWORD   _far _pascal GetWindowExt(HDC);

DWORD   _far _pascal OffsetWindowOrg(HDC, int, int);
DWORD   _far _pascal ScaleWindowExt(HDC, int, int, int, int);

DWORD   _far _pascal SetViewportOrg(HDC, int, int);
DWORD   _far _pascal GetViewportOrg(HDC);

DWORD   _far _pascal SetViewportExt(HDC, int, int);
DWORD   _far _pascal GetViewportExt(HDC);

DWORD   _far _pascal OffsetViewportOrg(HDC, int, int);
DWORD   _far _pascal ScaleViewportExt(HDC, int, int, int, int);


BOOL    _far _pascal SetWindowOrgEx(HDC, int, int, POINT _far*);
BOOL    _far _pascal GetWindowOrgEx(HDC, POINT _far*);

BOOL    _far _pascal SetWindowExtEx(HDC, int, int, SIZE _far*);
BOOL    _far _pascal GetWindowExtEx(HDC, SIZE _far*);

BOOL    _far _pascal OffsetWindowOrgEx(HDC, int, int, POINT _far*);
BOOL    _far _pascal ScaleWindowExtEx(HDC, int, int, int, int, SIZE _far*);

BOOL    _far _pascal SetViewportExtEx(HDC, int, int, SIZE _far*);
BOOL    _far _pascal GetViewportExtEx(HDC, SIZE _far*);

BOOL    _far _pascal SetViewportOrgEx(HDC, int, int, POINT _far*);
BOOL    _far _pascal GetViewportOrgEx(HDC, POINT _far*);

BOOL    _far _pascal OffsetViewportOrgEx(HDC, int, int, POINT _far*);
BOOL    _far _pascal ScaleViewportExtEx(HDC, int, int, int, int, SIZE _far*);
#line 1365 "windows.h"

BOOL    _far _pascal DPtoLP(HDC, POINT _far*, int);
BOOL    _far _pascal LPtoDP(HDC, POINT _far*, int);








typedef DWORD COLORREF;







COLORREF _far _pascal GetNearestColor(HDC, COLORREF);



COLORREF _far _pascal GetSysColor(int);
void    _far _pascal SetSysColors(int, const int _far*, const COLORREF _far*);























#line 1414 "windows.h"

#line 1416 "windows.h"







HGDIOBJ _far _pascal GetStockObject(int);

BOOL    _far _pascal IsGDIObject(HGDIOBJ);

BOOL    _far _pascal DeleteObject(HGDIOBJ);
HGDIOBJ _far _pascal SelectObject(HDC, HGDIOBJ);
int     _far _pascal GetObject(HGDIOBJ, int, void _far*);
BOOL    _far _pascal UnrealizeObject(HGDIOBJ);




typedef FARPROC GOBJENUMPROC;
#line 1437 "windows.h"




int     _far _pascal EnumObjects(HDC, int, GOBJENUMPROC, LPSTR);
#line 1443 "windows.h"








typedef struct tagLOGPEN
{
    UINT    lopnStyle;
    POINT   lopnWidth;
    COLORREF lopnColor;
} LOGPEN;
typedef LOGPEN*       PLOGPEN;
typedef LOGPEN _near* NPLOGPEN;
typedef LOGPEN _far*  LPLOGPEN;










HPEN    _far _pascal CreatePen(int, int, COLORREF);
HPEN    _far _pascal CreatePenIndirect(LOGPEN _far*);


























typedef struct tagLOGBRUSH
{
    UINT     lbStyle;
    COLORREF lbColor;
    int      lbHatch;
} LOGBRUSH;
typedef LOGBRUSH*       PLOGBRUSH;
typedef LOGBRUSH _near* NPLOGBRUSH;
typedef LOGBRUSH _far*  LPLOGBRUSH;

typedef LOGBRUSH            PATTERN;
typedef PATTERN*       PPATTERN;
typedef PATTERN _near* NPPATTERN;
typedef PATTERN _far*  LPPATTERN;

HBRUSH  _far _pascal CreateSolidBrush(COLORREF);
HBRUSH  _far _pascal CreateHatchBrush(int, COLORREF);
HBRUSH  _far _pascal CreatePatternBrush(HBITMAP);
HBRUSH  _far _pascal CreateDIBPatternBrush(HGLOBAL, UINT);
HBRUSH  _far _pascal CreateBrushIndirect(LOGBRUSH _far*);










DWORD   _far _pascal SetBrushOrg(HDC, int, int);
DWORD   _far _pascal GetBrushOrg(HDC);


BOOL    _far _pascal GetBrushOrgEx(HDC, POINT _far*);
#line 1534 "windows.h"
#line 1535 "windows.h"



HRGN    _far _pascal CreateRectRgn(int, int, int, int);
HRGN    _far _pascal CreateRectRgnIndirect(const RECT _far*);
HRGN    _far _pascal CreateEllipticRgnIndirect(const RECT _far*);
HRGN    _far _pascal CreateEllipticRgn(int, int, int, int);
HRGN    _far _pascal CreatePolygonRgn(const POINT _far*, int, int);
HRGN    _far _pascal CreatePolyPolygonRgn(const POINT _far*, const int _far*, int, int);
HRGN    _far _pascal CreateRoundRectRgn(int, int, int, int, int, int);







void    _far _pascal SetRectRgn(HRGN, int, int, int, int);

int     _far _pascal CombineRgn(HRGN, HRGN, HRGN, int);








BOOL    _far _pascal EqualRgn(HRGN, HRGN);
int     _far _pascal OffsetRgn(HRGN, int, int);

int     _far _pascal GetRgnBox(HRGN, RECT _far*);

BOOL    _far _pascal RectInRegion(HRGN, const RECT _far*);
BOOL    _far _pascal PtInRegion(HRGN, int, int);






typedef struct tagPALETTEENTRY
{
    BYTE    peRed;
    BYTE    peGreen;
    BYTE    peBlue;
    BYTE    peFlags;
} PALETTEENTRY;
typedef PALETTEENTRY _far* LPPALETTEENTRY;







typedef struct tagLOGPALETTE
{
    WORD    palVersion;
    WORD    palNumEntries;
    PALETTEENTRY palPalEntry[1];
} LOGPALETTE;
typedef LOGPALETTE*       PLOGPALETTE;
typedef LOGPALETTE _near* NPLOGPALETTE;
typedef LOGPALETTE _far*  LPLOGPALETTE;

HPALETTE _far _pascal CreatePalette(const LOGPALETTE _far*);

HPALETTE _far _pascal SelectPalette(HDC, HPALETTE, BOOL);

UINT    _far _pascal RealizePalette(HDC);

int     _far _pascal UpdateColors(HDC);
void    _far _pascal AnimatePalette(HPALETTE, UINT, UINT, const PALETTEENTRY _far*);

UINT    _far _pascal SetPaletteEntries(HPALETTE, UINT, UINT, const PALETTEENTRY _far*);
UINT    _far _pascal GetPaletteEntries(HPALETTE, UINT, UINT, PALETTEENTRY _far*);

UINT    _far _pascal GetNearestPaletteIndex(HPALETTE, COLORREF);

BOOL    _far _pascal ResizePalette(HPALETTE, UINT);

UINT    _far _pascal GetSystemPaletteEntries(HDC, UINT, UINT, PALETTEENTRY _far*);

UINT    _far _pascal GetSystemPaletteUse(HDC);
UINT    _far _pascal SetSystemPaletteUse(HDC, UINT);












int     _far _pascal SelectClipRgn(HDC, HRGN);
int     _far _pascal GetClipBox(HDC, RECT _far*);

int     _far _pascal IntersectClipRect(HDC, int, int, int, int);
int     _far _pascal OffsetClipRgn(HDC, int, int);
int     _far _pascal ExcludeClipRect(HDC, int, int, int, int);

BOOL    _far _pascal PtVisible(HDC, int, int);
BOOL    _far _pascal RectVisible(HDC, const RECT _far*);




DWORD   _far _pascal MoveTo(HDC, int, int);
DWORD   _far _pascal GetCurrentPosition(HDC);


BOOL    _far _pascal MoveToEx(HDC, int, int, POINT _far*);
BOOL    _far _pascal GetCurrentPositionEx(HDC, POINT _far*);
#line 1653 "windows.h"

BOOL    _far _pascal LineTo(HDC, int, int);
BOOL    _far _pascal Polyline(HDC, const POINT _far*, int);




typedef FARPROC LINEDDAPROC;
#line 1662 "windows.h"

void    _far _pascal LineDDA(int, int, int, int, LINEDDAPROC, LPARAM);

BOOL    _far _pascal Rectangle(HDC, int, int, int, int);
BOOL    _far _pascal RoundRect(HDC, int, int, int, int, int, int);

BOOL    _far _pascal Ellipse(HDC, int, int, int, int);
BOOL    _far _pascal Arc(HDC, int, int, int, int, int, int, int, int);
BOOL    _far _pascal Chord(HDC, int, int, int, int, int, int, int, int);
BOOL    _far _pascal Pie(HDC, int, int, int, int, int, int, int, int);

BOOL    _far _pascal Polygon(HDC, const POINT _far*, int);
BOOL    _far _pascal PolyPolygon(HDC, const POINT _far*, int _far*, int);





int     _far _pascal SetPolyFillMode(HDC, int);
int     _far _pascal GetPolyFillMode(HDC);

BOOL    _far _pascal FloodFill(HDC, int, int, COLORREF);
BOOL    _far _pascal ExtFloodFill(HDC, int, int, COLORREF, UINT);





BOOL    _far _pascal FillRgn(HDC, HRGN, HBRUSH);
BOOL    _far _pascal FrameRgn(HDC, HRGN, HBRUSH, int, int);
BOOL    _far _pascal InvertRgn(HDC, HRGN);
BOOL    _far _pascal PaintRgn(HDC, HRGN);


int     _far _pascal FillRect(HDC, const RECT _far*, HBRUSH);
int     _far _pascal FrameRect(HDC, const RECT _far*, HBRUSH);
void    _far _pascal InvertRect(HDC, const RECT _far*);

void    _far _pascal DrawFocusRect(HDC, const RECT _far*);




BOOL    _far _pascal TextOut(HDC, int, int, LPCSTR, int);
long    _far _pascal TabbedTextOut(HDC, int, int, LPCSTR, int, int, int _far*, int);
BOOL    _far _pascal ExtTextOut(HDC, int, int, UINT, const RECT _far*, LPCSTR, UINT, int _far*);





DWORD   _far _pascal GetTextExtent(HDC, LPCSTR, int);
DWORD   _far _pascal GetTabbedTextExtent(HDC, LPCSTR, int, int, int _far*);


BOOL    _far _pascal GetTextExtentPoint(HDC, LPCSTR, int, SIZE _far*);
#line 1719 "windows.h"



















int     _far _pascal DrawText(HDC, LPCSTR, int, RECT _far*, UINT);
#line 1740 "windows.h"




typedef FARPROC GRAYSTRINGPROC;
#line 1746 "windows.h"

BOOL    _far _pascal GrayString(HDC, HBRUSH, GRAYSTRINGPROC, LPARAM, int, int, int, int, int);

BOOL    _far _pascal GetCharWidth(HDC, UINT, UINT, int _far*);

COLORREF _far _pascal SetTextColor(HDC, COLORREF);
COLORREF _far _pascal GetTextColor(HDC);

COLORREF _far _pascal SetBkColor(HDC, COLORREF);
COLORREF _far _pascal GetBkColor(HDC);

int     _far _pascal SetBkMode(HDC, int);
int     _far _pascal GetBkMode(HDC);





UINT    _far _pascal SetTextAlign(HDC, UINT);
UINT    _far _pascal GetTextAlign(HDC);











int     _far _pascal SetTextCharacterExtra(HDC, int);
int     _far _pascal GetTextCharacterExtra(HDC);

int     _far _pascal SetTextJustification(HDC, int, int);






typedef struct tagLOGFONT
{
    int     lfHeight;
    int     lfWidth;
    int     lfEscapement;
    int     lfOrientation;
    int     lfWeight;
    BYTE    lfItalic;
    BYTE    lfUnderline;
    BYTE    lfStrikeOut;
    BYTE    lfCharSet;
    BYTE    lfOutPrecision;
    BYTE    lfClipPrecision;
    BYTE    lfQuality;
    BYTE    lfPitchAndFamily;
    char    lfFaceName[32];
} LOGFONT;
typedef LOGFONT*       PLOGFONT;
typedef LOGFONT _near* NPLOGFONT;
typedef LOGFONT _far*  LPLOGFONT;






































#line 1846 "windows.h"










#line 1857 "windows.h"



















HFONT   _far _pascal CreateFont(int, int, int, int, int, BYTE, BYTE, BYTE, BYTE, BYTE, BYTE, BYTE, BYTE, LPCSTR);
HFONT   _far _pascal CreateFontIndirect(const LOGFONT _far*);











DWORD   _far _pascal SetMapperFlags(HDC, DWORD);


int     _far _pascal AddFontResource(LPCSTR);
BOOL    _far _pascal RemoveFontResource(LPCSTR);



int     _far _pascal GetTextFace(HDC, int, LPSTR);

DWORD   _far _pascal GetAspectRatioFilter(HDC);

BOOL    _far _pascal GetAspectRatioFilterEx(HDC, SIZE _far*);
#line 1903 "windows.h"

#line 1905 "windows.h"



typedef struct tagTEXTMETRIC
{
    int     tmHeight;
    int     tmAscent;
    int     tmDescent;
    int     tmInternalLeading;
    int     tmExternalLeading;
    int     tmAveCharWidth;
    int     tmMaxCharWidth;
    int     tmWeight;
    BYTE    tmItalic;
    BYTE    tmUnderlined;
    BYTE    tmStruckOut;
    BYTE    tmFirstChar;
    BYTE    tmLastChar;
    BYTE    tmDefaultChar;
    BYTE    tmBreakChar;
    BYTE    tmPitchAndFamily;
    BYTE    tmCharSet;
    int     tmOverhang;
    int     tmDigitizedAspectX;
    int     tmDigitizedAspectY;
} TEXTMETRIC;
typedef TEXTMETRIC*       PTEXTMETRIC;
typedef TEXTMETRIC _near* NPTEXTMETRIC;
typedef TEXTMETRIC _far*  LPTEXTMETRIC;







#line 1942 "windows.h"

BOOL    _far _pascal GetTextMetrics(HDC, TEXTMETRIC _far*);




typedef struct tagPANOSE
{
    BYTE    bFamilyType;
    BYTE    bSerifStyle;
    BYTE    bWeight;
    BYTE    bProportion;
    BYTE    bContrast;
    BYTE    bStrokeVariation;
    BYTE    bArmStyle;
    BYTE    bLetterform;
    BYTE    bMidline;
    BYTE    bXHeight;
} PANOSE, _far* LPPANOSE;

typedef struct tagOUTLINETEXTMETRIC
{
    UINT    otmSize;
    TEXTMETRIC otmTextMetrics;
    BYTE    otmFiller;
    PANOSE  otmPanoseNumber;
    UINT    otmfsSelection;
    UINT    otmfsType;
    int     otmsCharSlopeRise;
    int     otmsCharSlopeRun;
    int     otmItalicAngle;
    UINT    otmEMSquare;
    int     otmAscent;
    int     otmDescent;
    UINT    otmLineGap;
    UINT    otmsCapEmHeight;
    UINT    otmsXHeight;
    RECT    otmrcFontBox;
    int     otmMacAscent;
    int     otmMacDescent;
    UINT    otmMacLineGap;
    UINT    otmusMinimumPPEM;
    POINT   otmptSubscriptSize;
    POINT   otmptSubscriptOffset;
    POINT   otmptSuperscriptSize;
    POINT   otmptSuperscriptOffset;
    UINT    otmsStrikeoutSize;
    int     otmsStrikeoutPosition;
    int     otmsUnderscorePosition;
    int     otmsUnderscoreSize;
    PSTR    otmpFamilyName;
    PSTR    otmpFaceName;
    PSTR    otmpStyleName;
    PSTR    otmpFullName;
} OUTLINETEXTMETRIC, _far* LPOUTLINETEXTMETRIC;

WORD    _far _pascal GetOutlineTextMetrics(HDC, UINT, OUTLINETEXTMETRIC _far*);

#line 2001 "windows.h"
#line 2002 "windows.h"






typedef struct tagNEWTEXTMETRIC
{
    int     tmHeight;
    int     tmAscent;
    int     tmDescent;
    int     tmInternalLeading;
    int     tmExternalLeading;
    int     tmAveCharWidth;
    int     tmMaxCharWidth;
    int     tmWeight;
    BYTE    tmItalic;
    BYTE    tmUnderlined;
    BYTE    tmStruckOut;
    BYTE    tmFirstChar;
    BYTE    tmLastChar;
    BYTE    tmDefaultChar;
    BYTE    tmBreakChar;
    BYTE    tmPitchAndFamily;
    BYTE    tmCharSet;
    int     tmOverhang;
    int     tmDigitizedAspectX;
    int     tmDigitizedAspectY;
    DWORD   ntmFlags;
    UINT    ntmSizeEM;
    UINT    ntmCellHeight;
    UINT    ntmAvgWidth;
} NEWTEXTMETRIC;
typedef NEWTEXTMETRIC*       PNEWTEXTMETRIC;
typedef NEWTEXTMETRIC _near* NPNEWTEXTMETRIC;
typedef NEWTEXTMETRIC _far*  LPNEWTEXTMETRIC;









typedef struct tagENUMLOGFONT
{
    LOGFONT elfLogFont;
    char    elfFullName[64];
    char    elfStyle[32];
} ENUMLOGFONT, _far* LPENUMLOGFONT;

#line 2055 "windows.h"
#line 2056 "windows.h"




















typedef FARPROC OLDFONTENUMPROC;
typedef FARPROC FONTENUMPROC;

int     _far _pascal EnumFonts(HDC, LPCSTR, OLDFONTENUMPROC, LPSTR);


int     _far _pascal EnumFontFamilies(HDC, LPCSTR, FONTENUMPROC, LPSTR);
#line 2084 "windows.h"

#line 2086 "windows.h"






#line 2093 "windows.h"

#line 2095 "windows.h"




DWORD   _far _pascal GetFontData(HDC, DWORD, DWORD, void _far*, DWORD);
BOOL    _far _pascal CreateScalableFontResource(UINT, LPCSTR, LPCSTR, LPCSTR);

typedef struct tagGLYPHMETRICS
{
    UINT    gmBlackBoxX;
    UINT    gmBlackBoxY;
    POINT   gmptGlyphOrigin;
    int     gmCellIncX;
    int     gmCellIncY;
} GLYPHMETRICS, _far* LPGLYPHMETRICS;

typedef struct tagFIXED
{
    UINT    fract;
    int     value;
} FIXED, _far* LPFIXED;

typedef struct tagMAT2
{
    FIXED  eM11;
    FIXED  eM12;
    FIXED  eM21;
    FIXED  eM22;
} MAT2, _far* LPMAT2;

DWORD   _far _pascal GetGlyphOutline(HDC, UINT, UINT, GLYPHMETRICS _far*, DWORD, void _far*, const MAT2 _far*);











typedef struct tagPOINTFX
{
    FIXED x;
    FIXED y;
} POINTFX, _far* LPPOINTFX;

typedef struct tagTTPOLYCURVE
{
    UINT    wType;
    UINT    cpfx;
    POINTFX apfx[1];
} TTPOLYCURVE, _far* LPTTPOLYCURVE;

typedef struct tagTTPOLYGONHEADER
{
    DWORD   cb;
    DWORD   dwType;
    POINTFX pfxStart;
} TTPOLYGONHEADER, _far* LPTTPOLYGONHEADER;

typedef struct tagABC
{
    int   abcA;
    UINT  abcB;
    int   abcC;
} ABC;
typedef ABC _far* LPABC;

BOOL    _far _pascal GetCharABCWidths(HDC, UINT, UINT, ABC _far*);

typedef struct tagKERNINGPAIR
{
    WORD wFirst;
    WORD wSecond;
    int  iKernAmount;
} KERNINGPAIR, _far* LPKERNINGPAIR;

int     _far _pascal GetKerningPairs(HDC, int, KERNINGPAIR _far*);

typedef struct tagRASTERIZER_STATUS
{
    int   nSize;
    int   wFlags;
    int   nLanguageID;
} RASTERIZER_STATUS;
typedef RASTERIZER_STATUS _far* LPRASTERIZER_STATUS;





BOOL    _far _pascal GetRasterizerCaps(RASTERIZER_STATUS _far*, int);

#line 2191 "windows.h"
#line 2192 "windows.h"




typedef struct tagBITMAP
{
    int     bmType;
    int     bmWidth;
    int     bmHeight;
    int     bmWidthBytes;
    BYTE    bmPlanes;
    BYTE    bmBitsPixel;
    void _far* bmBits;
} BITMAP;
typedef BITMAP*       PBITMAP;
typedef BITMAP _near* NPBITMAP;
typedef BITMAP _far*  LPBITMAP;


typedef struct tagRGBTRIPLE
{
    BYTE    rgbtBlue;
    BYTE    rgbtGreen;
    BYTE    rgbtRed;
} RGBTRIPLE;
typedef RGBTRIPLE _far* LPRGBTRIPLE;

typedef struct tagRGBQUAD
{
    BYTE    rgbBlue;
    BYTE    rgbGreen;
    BYTE    rgbRed;
    BYTE    rgbReserved;
} RGBQUAD;
typedef RGBQUAD _far* LPRGBQUAD;


typedef struct tagBITMAPCOREHEADER
{
    DWORD   bcSize;
    short   bcWidth;
    short   bcHeight;
    WORD    bcPlanes;
    WORD    bcBitCount;
} BITMAPCOREHEADER;
typedef BITMAPCOREHEADER*      PBITMAPCOREHEADER;
typedef BITMAPCOREHEADER _far* LPBITMAPCOREHEADER;

typedef struct tagBITMAPINFOHEADER
{
    DWORD   biSize;
    long    biWidth;
    long    biHeight;
    WORD    biPlanes;
    WORD    biBitCount;
    DWORD   biCompression;
    DWORD   biSizeImage;
    long    biXPelsPerMeter;
    long    biYPelsPerMeter;
    DWORD   biClrUsed;
    DWORD   biClrImportant;
} BITMAPINFOHEADER;
typedef BITMAPINFOHEADER*      PBITMAPINFOHEADER;
typedef BITMAPINFOHEADER _far* LPBITMAPINFOHEADER;






typedef struct tagBITMAPINFO
{
    BITMAPINFOHEADER bmiHeader;
    RGBQUAD          bmiColors[1];
} BITMAPINFO;
typedef BITMAPINFO*     PBITMAPINFO;
typedef BITMAPINFO _far* LPBITMAPINFO;

typedef struct tagBITMAPCOREINFO
{
    BITMAPCOREHEADER bmciHeader;
    RGBTRIPLE        bmciColors[1];
} BITMAPCOREINFO;
typedef BITMAPCOREINFO*      PBITMAPCOREINFO;
typedef BITMAPCOREINFO _far* LPBITMAPCOREINFO;

typedef struct tagBITMAPFILEHEADER
{
    UINT    bfType;
    DWORD   bfSize;
    UINT    bfReserved1;
    UINT    bfReserved2;
    DWORD   bfOffBits;
} BITMAPFILEHEADER;
typedef BITMAPFILEHEADER*      PBITMAPFILEHEADER;
typedef BITMAPFILEHEADER _far* LPBITMAPFILEHEADER;


HBITMAP _far _pascal CreateBitmap(int, int, UINT, UINT, const void _far*);
HBITMAP _far _pascal CreateBitmapIndirect(BITMAP _far* );
HBITMAP _far _pascal CreateCompatibleBitmap(HDC, int, int);
HBITMAP _far _pascal CreateDiscardableBitmap(HDC, int, int);
HBITMAP _far _pascal CreateDIBitmap(HDC, BITMAPINFOHEADER _far*, DWORD, const void _far*, BITMAPINFO _far*, UINT);

HBITMAP _far _pascal LoadBitmap(HINSTANCE, LPCSTR);







#line 2305 "windows.h"






































#line 2344 "windows.h"


BOOL    _far _pascal BitBlt(HDC, int, int, int, int, HDC, int, int, DWORD);

BOOL    _far _pascal PatBlt(HDC, int, int, int, int, DWORD);

BOOL    _far _pascal StretchBlt(HDC, int, int, int, int, HDC, int, int, int, int, DWORD);
int     _far _pascal StretchDIBits(HDC, int, int, int, int, int,
                        int, int, int, const void _far*, LPBITMAPINFO, UINT, DWORD);

COLORREF _far _pascal SetPixel(HDC, int, int, COLORREF);
COLORREF _far _pascal GetPixel(HDC, int, int);











int     _far _pascal SetStretchBltMode(HDC, int);
int     _far _pascal GetStretchBltMode(HDC);

DWORD   _far _pascal SetBitmapDimension(HBITMAP, int, int);
DWORD   _far _pascal GetBitmapDimension(HBITMAP);

BOOL    _far _pascal SetBitmapDimensionEx(HBITMAP, int, int, SIZE _far*);
BOOL    _far _pascal GetBitmapDimensionEx(HBITMAP, SIZE _far*);
#line 2376 "windows.h"
int     _far _pascal SetROP2(HDC, int);
int     _far _pascal GetROP2(HDC);

long    _far _pascal SetBitmapBits(HBITMAP, DWORD, const void _far*);
long    _far _pascal GetBitmapBits(HBITMAP, long, void _far*);

int     _far _pascal SetDIBits(HDC, HBITMAP, UINT, UINT, const void _far*, BITMAPINFO _far*, UINT);
int     _far _pascal GetDIBits(HDC, HBITMAP, UINT, UINT, void _far*, BITMAPINFO _far*, UINT);

int     _far _pascal SetDIBitsToDevice(HDC, int, int, int, int, int, int, UINT, UINT,
                    void _far*, BITMAPINFO _far*, UINT);
#line 2388 "windows.h"





typedef UINT HMETAFILE;

HDC     _far _pascal CreateMetaFile(LPCSTR);
HMETAFILE _far _pascal CloseMetaFile(HDC);

HMETAFILE _far _pascal GetMetaFile(LPCSTR);
BOOL      _far _pascal DeleteMetaFile(HMETAFILE);
HMETAFILE _far _pascal CopyMetaFile(HMETAFILE, LPCSTR);

BOOL    _far _pascal PlayMetaFile(HDC, HMETAFILE);

HGLOBAL _far _pascal GetMetaFileBits(HMETAFILE);
HMETAFILE _far _pascal SetMetaFileBits(HGLOBAL);

HMETAFILE _far _pascal SetMetaFileBitsBetter(HGLOBAL);
#line 2409 "windows.h"


typedef struct tagMETAFILEPICT
{
    int     mm;
    int     xExt;
    int     yExt;
    HMETAFILE hMF;
} METAFILEPICT;
typedef METAFILEPICT _far* LPMETAFILEPICT;

typedef struct tagMETAHEADER
{
    UINT    mtType;
    UINT    mtHeaderSize;
    UINT    mtVersion;
    DWORD   mtSize;
    UINT    mtNoObjects;
    DWORD   mtMaxRecord;
    UINT    mtNoParameters;
} METAHEADER;

typedef struct tagHANDLETABLE
{
    HGDIOBJ objectHandle[1];
} HANDLETABLE;
typedef HANDLETABLE*      PHANDLETABLE;
typedef HANDLETABLE _far* LPHANDLETABLE;

typedef struct tagMETARECORD
{
    DWORD   rdSize;
    UINT    rdFunction;
    UINT    rdParm[1];
} METARECORD;
typedef METARECORD*      PMETARECORD;
typedef METARECORD _far* LPMETARECORD;























































































void    _far _pascal PlayMetaFileRecord(HDC, HANDLETABLE _far*, METARECORD _far*, UINT);




typedef FARPROC MFENUMPROC;
#line 2540 "windows.h"

BOOL    _far _pascal EnumMetaFile(HDC, HMETAFILE, MFENUMPROC, LPARAM);

#line 2544 "windows.h"






typedef FARPROC ABORTPROC;
#line 2552 "windows.h"



typedef struct
{
    int     cbSize;
    LPCSTR  lpszDocName;
    LPCSTR  lpszOutput;
}   DOCINFO;
typedef DOCINFO _far* LPDOCINFO;

int     _far _pascal StartDoc(HDC, DOCINFO _far*);
int     _far _pascal StartPage(HDC);
int     _far _pascal EndPage(HDC);
int     _far _pascal EndDoc(HDC);
int     _far _pascal AbortDoc(HDC);

int     _far _pascal SetAbortProc(HDC, ABORTPROC);
HANDLE  _far _pascal SpoolFile(LPSTR, LPSTR, LPSTR, LPSTR);
#line 2572 "windows.h"

BOOL    _far _pascal QueryAbort(HDC, int);











#line 2586 "windows.h"








int     _far _pascal Escape(HDC, int, int, LPCSTR, void _far*);










































































#line 2670 "windows.h"



typedef UINT HWND;



typedef UINT HMENU;

typedef UINT HICON;
typedef HICON HCURSOR;      





int _far _pascal GetSystemMetrics(int);















































#line 2735 "windows.h"



#line 2739 "windows.h"

UINT    _far _pascal GetDoubleClickTime(void);
void    _far _pascal SetDoubleClickTime(UINT);









BOOL    _far _pascal SystemParametersInfo(UINT, UINT, void _far*, UINT);







































#line 2792 "windows.h"
#line 2793 "windows.h"



void    _far _pascal SetRect(RECT _far*, int, int, int, int);
void    _far _pascal SetRectEmpty(RECT _far*);

void    _far _pascal CopyRect(RECT _far*, const RECT _far*);

BOOL    _far _pascal IsRectEmpty(const RECT _far*);

BOOL    _far _pascal EqualRect(const RECT _far*, const RECT _far*);

BOOL    _far _pascal IntersectRect(RECT _far*, const RECT _far*, const RECT _far*);
BOOL    _far _pascal UnionRect(RECT _far*, const RECT _far*, const RECT _far*);
BOOL    _far _pascal SubtractRect(RECT _far*, const RECT _far*, const RECT _far*);

void    _far _pascal OffsetRect(RECT _far*, int, int);
void    _far _pascal InflateRect(RECT _far*, int, int);

BOOL    _far _pascal PtInRect(const RECT _far*, POINT);



UINT _far _pascal RegisterWindowMessage(LPCSTR);









typedef struct tagMSG
{
    HWND        hwnd;
    UINT        message;
    WPARAM      wParam;
    LPARAM      lParam;
    DWORD       time;
    POINT       pt;
} MSG;
typedef MSG* PMSG;
typedef MSG _near* NPMSG;
typedef MSG _far* LPMSG;

BOOL    _far _pascal GetMessage(MSG _far*, HWND, UINT, UINT);
BOOL    _far _pascal PeekMessage(MSG _far*, HWND, UINT, UINT, UINT);






void    _far _pascal WaitMessage(void);

DWORD   _far _pascal GetMessagePos(void);
long    _far _pascal GetMessageTime(void);

LPARAM  _far _pascal GetMessageExtraInfo(void);
#line 2854 "windows.h"

BOOL    _far _pascal TranslateMessage(const MSG _far*);
long    _far _pascal DispatchMessage(const MSG _far*);

BOOL    _far _pascal SetMessageQueue(int);

BOOL    _far _pascal GetInputState(void);


DWORD   _far _pascal GetQueueStatus(UINT flags);












#line 2877 "windows.h"

#line 2879 "windows.h"

BOOL    _far _pascal PostMessage(HWND, UINT, WPARAM, LPARAM);
LRESULT _far _pascal SendMessage(HWND, UINT, WPARAM, LPARAM);



BOOL    _far _pascal PostAppMessage(HTASK, UINT, WPARAM, LPARAM);

void    _far _pascal ReplyMessage(LRESULT);
BOOL    _far _pascal InSendMessage(void);




BOOL _far _pascal CallMsgFilter(MSG _far*, int);

















#line 2912 "windows.h"





















#line 2934 "windows.h"








void    _far _pascal PostQuitMessage(int);





typedef LRESULT (_far _pascal* WNDPROC)(HWND, UINT, WPARAM, LPARAM);

typedef struct tagWNDCLASS
{
    UINT        style;
    WNDPROC     lpfnWndProc;
    int         cbClsExtra;
    int         cbWndExtra;
    HINSTANCE   hInstance;
    HICON       hIcon;
    HCURSOR     hCursor;
    HBRUSH      hbrBackground;
    LPCSTR      lpszMenuName;
    LPCSTR      lpszClassName;
} WNDCLASS;
typedef WNDCLASS* PWNDCLASS;
typedef WNDCLASS _near* NPWNDCLASS;
typedef WNDCLASS _far* LPWNDCLASS;

ATOM    _far _pascal RegisterClass(const WNDCLASS _far*);
BOOL    _far _pascal UnregisterClass(LPCSTR, HINSTANCE);

BOOL    _far _pascal GetClassInfo(HINSTANCE, LPCSTR, WNDCLASS _far*);
int     _far _pascal GetClassName(HWND, LPSTR, int);
























#line 2997 "windows.h"



WORD    _far _pascal GetClassWord(HWND, int);
WORD    _far _pascal SetClassWord(HWND, int, WORD);
long    _far _pascal GetClassLong(HWND, int);
long    _far _pascal SetClassLong(HWND, int, long);














#line 3019 "windows.h"

#line 3021 "windows.h"



















































#line 3073 "windows.h"








#line 3082 "windows.h"




BOOL    _far _pascal IsWindow(HWND);

HWND    _far _pascal CreateWindowEx(DWORD, LPCSTR, LPCSTR, DWORD, int, int, int, int, HWND, HMENU, HINSTANCE, void _far*);
HWND    _far _pascal CreateWindow(LPCSTR, LPCSTR, DWORD, int, int, int, int, HWND, HMENU, HINSTANCE, void _far*);





typedef struct tagCREATESTRUCT
{
    void _far* lpCreateParams;
    HINSTANCE hInstance;
    HMENU     hMenu;
    HWND      hwndParent;
    int       cy;
    int       cx;
    int       y;
    int       x;
    long      style;
    LPCSTR    lpszName;
    LPCSTR    lpszClass;
    DWORD     dwExStyle;
} CREATESTRUCT;
typedef CREATESTRUCT _far* LPCREATESTRUCT;

BOOL    _far _pascal DestroyWindow(HWND);






HTASK   _far _pascal GetWindowTask(HWND);

BOOL    _far _pascal IsChild(HWND, HWND);

HWND    _far _pascal GetParent(HWND);
HWND    _far _pascal SetParent(HWND, HWND);

BOOL    _far _pascal IsWindowVisible(HWND);

BOOL    _far _pascal ShowWindow(HWND, int);



































#line 3165 "windows.h"




BOOL    _far _pascal EnableWindow(HWND,BOOL);
BOOL    _far _pascal IsWindowEnabled(HWND);




void    _far _pascal SetWindowText(HWND, LPCSTR);
int     _far _pascal GetWindowText(HWND, LPSTR, int);
int     _far _pascal GetWindowTextLength(HWND);






WORD    _far _pascal GetWindowWord(HWND, int);
WORD    _far _pascal SetWindowWord(HWND, int, WORD);
long    _far _pascal GetWindowLong(HWND, int);
long    _far _pascal SetWindowLong(HWND, int, long);









#line 3198 "windows.h"





void    _far _pascal GetClientRect(HWND, RECT _far*);
void    _far _pascal GetWindowRect(HWND, RECT _far*);



typedef struct tagWINDOWPLACEMENT
{
    UINT  length;
    UINT  flags;
    UINT  showCmd;
    POINT ptMinPosition;
    POINT ptMaxPosition;
    RECT  rcNormalPosition;
} WINDOWPLACEMENT;
typedef WINDOWPLACEMENT     *PWINDOWPLACEMENT;
typedef WINDOWPLACEMENT _far* LPWINDOWPLACEMENT;




BOOL    _far _pascal GetWindowPlacement(HWND, WINDOWPLACEMENT _far*);
BOOL    _far _pascal SetWindowPlacement(HWND, const WINDOWPLACEMENT _far*);
#line 3226 "windows.h"

BOOL    _far _pascal SetWindowPos(HWND, HWND, int, int, int, int, UINT);



























typedef UINT HDWP;

HDWP    _far _pascal BeginDeferWindowPos(int);
HDWP    _far _pascal DeferWindowPos(HDWP, HWND, HWND, int, int, int, int, UINT);
BOOL    _far _pascal EndDeferWindowPos(HDWP);

#line 3262 "windows.h"

BOOL    _far _pascal MoveWindow(HWND, int, int, int, int, BOOL);
BOOL    _far _pascal BringWindowToTop(HWND);







typedef struct tagWINDOWPOS
{
    HWND    hwnd;
    HWND    hwndInsertAfter;
    int     x;
    int     y;
    int     cx;
    int     cy;
    UINT    flags;
} WINDOWPOS;
typedef WINDOWPOS _far* LPWINDOWPOS;
#line 3284 "windows.h"




















LRESULT _far _pascal DefWindowProc(HWND, UINT, WPARAM, LPARAM);




LRESULT _far _pascal CallWindowProc(FARPROC, HWND, UINT, WPARAM, LPARAM);
#line 3311 "windows.h"



void    _far _pascal AdjustWindowRect(RECT _far*, DWORD, BOOL);
void    _far _pascal AdjustWindowRectEx(RECT _far*, DWORD, BOOL, DWORD);





typedef struct tagMINMAXINFO
{
    POINT ptReserved;
    POINT ptMaxSize;
    POINT ptMaxPosition;
    POINT ptMinTrackSize;
    POINT ptMaxTrackSize;
} MINMAXINFO;



BOOL    _far _pascal FlashWindow(HWND, BOOL);

void    _far _pascal ShowOwnedPopups(HWND, BOOL);


BOOL    _far _pascal OpenIcon(HWND);
void    _far _pascal CloseWindow(HWND);
BOOL    _far _pascal AnyPopup(void);
BOOL    _far _pascal IsIconic(HWND);
BOOL    _far _pascal IsZoomed(HWND);



void    _far _pascal ClientToScreen(HWND, POINT _far*);
void    _far _pascal ScreenToClient(HWND, POINT _far*);


void    _far _pascal MapWindowPoints(HWND hwndFrom, HWND hwndTo, POINT _far* lppt, UINT cpt);
#line 3351 "windows.h"

HWND    _far _pascal WindowFromPoint(POINT);
HWND    _far _pascal ChildWindowFromPoint(HWND, POINT);



HWND    _far _pascal GetDesktopWindow(void);

HWND    _far _pascal FindWindow(LPCSTR, LPCSTR);




typedef FARPROC WNDENUMPROC;
#line 3366 "windows.h"

BOOL    _far _pascal EnumWindows(WNDENUMPROC, LPARAM);
BOOL    _far _pascal EnumChildWindows(HWND, WNDENUMPROC, LPARAM);
BOOL    _far _pascal EnumTaskWindows(HTASK, WNDENUMPROC, LPARAM);

HWND    _far _pascal GetTopWindow(HWND);

HWND    _far _pascal GetWindow(HWND, UINT);
HWND    _far _pascal GetNextWindow(HWND, UINT);












BOOL    _far _pascal SetProp(HWND, LPCSTR, HANDLE);
HANDLE  _far _pascal GetProp(HWND, LPCSTR);
HANDLE  _far _pascal RemoveProp(HWND, LPCSTR);




typedef FARPROC PROPENUMPROC;
#line 3396 "windows.h"

int     _far _pascal EnumProps(HWND, PROPENUMPROC);



HDC     _far _pascal GetDC(HWND);
int     _far _pascal ReleaseDC(HWND, HDC);

HDC     _far _pascal GetWindowDC(HWND);


HDC     _far _pascal GetDCEx(register HWND hwnd, HRGN hrgnClip, DWORD flags);
















#line 3425 "windows.h"








typedef struct tagPAINTSTRUCT
{
    HDC         hdc;
    BOOL        fErase;
    RECT        rcPaint;
    BOOL        fRestore;
    BOOL        fIncUpdate;
    BYTE        rgbReserved[16];
} PAINTSTRUCT;
typedef PAINTSTRUCT* PPAINTSTRUCT;
typedef PAINTSTRUCT _near* NPPAINTSTRUCT;
typedef PAINTSTRUCT _far* LPPAINTSTRUCT;

HDC     _far _pascal BeginPaint(HWND, PAINTSTRUCT _far*);
void    _far _pascal EndPaint(HWND, const PAINTSTRUCT _far*);

void    _far _pascal UpdateWindow(HWND);

int     _far _pascal ExcludeUpdateRgn(HDC, HWND);


BOOL    _far _pascal LockWindowUpdate(HWND hwndLock);
#line 3456 "windows.h"

BOOL    _far _pascal GetUpdateRect(HWND, RECT _far*, BOOL);
int     _far _pascal GetUpdateRgn(HWND, HRGN, BOOL);

void    _far _pascal InvalidateRect(HWND, const RECT _far*, BOOL);
void    _far _pascal ValidateRect(HWND, const RECT _far*);

void    _far _pascal InvalidateRgn(HWND, HRGN, BOOL);
void    _far _pascal ValidateRgn(HWND, HRGN);


BOOL    _far _pascal RedrawWindow(HWND hwnd, const RECT _far* lprcUpdate, HRGN hrgnUpdate, UINT flags);


















#line 3487 "windows.h"



void    _far _pascal ScrollWindow(HWND, int, int, const RECT _far*, const RECT _far*);
BOOL    _far _pascal ScrollDC(HDC, int, int, const RECT _far*, const RECT _far*, HRGN, RECT _far*);



int     _far _pascal ScrollWindowEx(HWND hwnd, int dx, int dy,
                const RECT _far* prcScroll, const RECT _far* prcClip,
                HRGN hrgnUpdate, RECT _far* prcUpdate, UINT flags);






#line 3505 "windows.h"




















typedef struct tagNCCALCSIZE_PARAMS
{
    RECT           rgrc[3];
    WINDOWPOS _far* lppos;
} NCCALCSIZE_PARAMS;





#line 3536 "windows.h"
typedef NCCALCSIZE_PARAMS _far* LPNCCALCSIZE_PARAMS;




































HWND    _far _pascal SetActiveWindow(HWND);
HWND    _far _pascal GetActiveWindow(void);

HWND    _far _pascal GetLastActivePopup(HWND);












HWND    _far _pascal SetFocus(HWND);
HWND    _far _pascal GetFocus(void);

int     _far _pascal GetKeyState(int);
int     _far _pascal GetAsyncKeyState(int);

void    _far _pascal GetKeyboardState(BYTE _far* );
void    _far _pascal SetKeyboardState(BYTE _far* );











































































































#line 3705 "windows.h"







HWND    _far _pascal SetCapture(HWND);
void    _far _pascal ReleaseCapture(void);
HWND    _far _pascal GetCapture(void);

BOOL    _far _pascal SwapMouseButton(BOOL);
























#line 3742 "windows.h"






















#line 3765 "windows.h"





typedef struct tagMOUSEHOOKSTRUCT
{
    POINT   pt;
    HWND    hwnd;
    UINT    wHitTestCode;
    DWORD   dwExtraInfo;
} MOUSEHOOKSTRUCT;
typedef MOUSEHOOKSTRUCT  _far* LPMOUSEHOOKSTRUCT;
#line 3779 "windows.h"







HWND    _far _pascal GetSysModalWindow(void);
HWND    _far _pascal SetSysModalWindow(HWND);






typedef FARPROC TIMERPROC;
#line 3796 "windows.h"

UINT    _far _pascal SetTimer(HWND, UINT, UINT, TIMERPROC);

BOOL    _far _pascal KillTimer(HWND, UINT);





typedef UINT HACCEL;

HACCEL  _far _pascal LoadAccelerators(HINSTANCE, LPCSTR);


int     _far _pascal TranslateAccelerator(HWND, HACCEL, MSG _far*);
#line 3812 "windows.h"






typedef struct
{
    UINT    versionNumber;
    UINT    offset;
} MENUITEMTEMPLATEHEADER;


typedef struct
{
    UINT    mtOption;
    UINT    mtID;
    char    mtString[1];
} MENUITEMTEMPLATE;


BOOL    _far _pascal IsMenu(HMENU);
#line 3835 "windows.h"

HMENU   _far _pascal CreateMenu(void);
HMENU   _far _pascal CreatePopupMenu(void);
HMENU   _far _pascal LoadMenu(HINSTANCE, LPCSTR);
HMENU   _far _pascal LoadMenuIndirect(const void _far*);

BOOL    _far _pascal DestroyMenu(HMENU);

HMENU   _far _pascal GetMenu(HWND);
BOOL    _far _pascal SetMenu(HWND, HMENU);

HMENU   _far _pascal GetSystemMenu(HWND, BOOL);

void    _far _pascal DrawMenuBar(HWND);

BOOL    _far _pascal HiliteMenuItem(HWND, HMENU, UINT, UINT);

BOOL    _far _pascal InsertMenu(HMENU, UINT, UINT, UINT, LPCSTR);
BOOL    _far _pascal AppendMenu(HMENU, UINT, UINT, LPCSTR);
BOOL    _far _pascal ModifyMenu(HMENU, UINT, UINT, UINT, LPCSTR);
BOOL    _far _pascal RemoveMenu(HMENU, UINT, UINT);
BOOL    _far _pascal DeleteMenu(HMENU, UINT, UINT);

BOOL    _far _pascal ChangeMenu(HMENU, UINT, LPCSTR, UINT, UINT);







































BOOL    _far _pascal EnableMenuItem(HMENU, UINT, UINT);
BOOL    _far _pascal CheckMenuItem(HMENU, UINT, UINT);

HMENU   _far _pascal GetSubMenu(HMENU, int);

int     _far _pascal GetMenuItemCount(HMENU);
UINT    _far _pascal GetMenuItemID(HMENU, int);

int     _far _pascal GetMenuString(HMENU, UINT, LPSTR, int, UINT);
UINT    _far _pascal GetMenuState(HMENU, UINT, UINT);

BOOL    _far _pascal SetMenuItemBitmaps(HMENU, UINT, UINT, HBITMAP, HBITMAP);
DWORD   _far _pascal GetMenuCheckMarkDimensions(void);

BOOL    _far _pascal TrackPopupMenu(HMENU, UINT, int, int, int, HWND, const RECT _far*);








#line 3922 "windows.h"

#line 3924 "windows.h"










#line 3935 "windows.h"


































int     _far _pascal SetScrollPos(HWND, int, int, BOOL);
int     _far _pascal GetScrollPos(HWND, int);
void    _far _pascal SetScrollRange(HWND, int, int, int, BOOL);
void    _far _pascal GetScrollRange(HWND, int, int _far*, int _far*);
void    _far _pascal ShowScrollBar(HWND, int, BOOL);
BOOL    _far _pascal EnableScrollBar(HWND, int, UINT);














#line 3990 "windows.h"

































BOOL    _far _pascal OpenClipboard(HWND);
BOOL    _far _pascal CloseClipboard(void);
BOOL    _far _pascal EmptyClipboard(void);


HWND    _far _pascal GetOpenClipboardWindow(void);
#line 4030 "windows.h"

HWND    _far _pascal GetClipboardOwner(void);

HWND    _far _pascal SetClipboardViewer(HWND);
HWND    _far _pascal GetClipboardViewer(void);

HANDLE  _far _pascal SetClipboardData(UINT, HANDLE);
HANDLE  _far _pascal GetClipboardData(UINT);

BOOL    _far _pascal IsClipboardFormatAvailable(UINT);
int     _far _pascal GetPriorityClipboardFormat(UINT _far*, int);

UINT    _far _pascal RegisterClipboardFormat(LPCSTR);
int     _far _pascal CountClipboardFormats(void);
UINT    _far _pascal EnumClipboardFormats(UINT);
int     _far _pascal GetClipboardFormatName(UINT, LPSTR, int);

BOOL    _far _pascal ChangeClipboardChain(HWND, HWND);






















#line 4071 "windows.h"



HCURSOR _far _pascal LoadCursor(HINSTANCE, LPCSTR);
HCURSOR _far _pascal CreateCursor(HINSTANCE, int, int, int, int, const void _far*, const void _far*);
BOOL    _far _pascal DestroyCursor(HCURSOR);


HCURSOR _far _pascal CopyCursor(HINSTANCE, HCURSOR);
#line 4081 "windows.h"

int     _far _pascal ShowCursor(BOOL);

void    _far _pascal SetCursorPos(int, int);
void    _far _pascal GetCursorPos(POINT _far*);

HCURSOR _far _pascal SetCursor(HCURSOR);


HCURSOR _far _pascal GetCursor(void);
#line 4092 "windows.h"

void    _far _pascal ClipCursor(const RECT _far*);

void    _far _pascal GetClipCursor(RECT _far*);
#line 4097 "windows.h"


















HICON   _far _pascal LoadIcon(HINSTANCE, LPCSTR);
HICON   _far _pascal CreateIcon(HINSTANCE, int, int, BYTE, BYTE, const void _far*, const void _far*);
BOOL    _far _pascal DestroyIcon(HICON);


HICON   _far _pascal CopyIcon(HINSTANCE, HICON);
#line 4122 "windows.h"

BOOL    _far _pascal DrawIcon(HDC, int, int, HICON);










#line 4135 "windows.h"





int     _far _pascal MessageBox(HWND, LPCSTR, LPCSTR, UINT);
void    _far _pascal MessageBeep(UINT);































#line 4174 "windows.h"



void    _far _pascal CreateCaret(HWND, HBITMAP, int, int);
void    _far _pascal DestroyCaret(void);

void    _far _pascal SetCaretPos(int, int);
void    _far _pascal GetCaretPos(POINT _far*);

void    _far _pascal HideCaret(HWND);
void    _far _pascal ShowCaret(HWND);

UINT    _far _pascal GetCaretBlinkTime(void);
void    _far _pascal SetCaretBlinkTime(UINT);






























#line 4219 "windows.h"






typedef struct tagCLIENTCREATESTRUCT
{
    HMENU  hWindowMenu;
    UINT   idFirstChild;
} CLIENTCREATESTRUCT;
typedef CLIENTCREATESTRUCT _far* LPCLIENTCREATESTRUCT;




#line 4236 "windows.h"















typedef struct tagMDICREATESTRUCT
{
    LPCSTR  szClass;
    LPCSTR  szTitle;
    HINSTANCE hOwner;
    int     x;
    int     y;
    int     cx;
    int     cy;
    DWORD   style;
    LPARAM  lParam;
} MDICREATESTRUCT;
typedef MDICREATESTRUCT _far*  LPMDICREATESTRUCT;






#line 4271 "windows.h"



LRESULT _far _pascal DefFrameProc(HWND, HWND, UINT, WPARAM, LPARAM);
LRESULT _far _pascal DefMDIChildProc(HWND, UINT, WPARAM, LPARAM);


BOOL    _far _pascal TranslateMDISysAccel(HWND, MSG _far*);
#line 4280 "windows.h"

UINT    _far _pascal ArrangeIconicWindows(HWND);

#line 4284 "windows.h"


























#line 4311 "windows.h"















typedef FARPROC DLGPROC;
#line 4328 "windows.h"







BOOL    _far _pascal IsDialogMessage(HWND, MSG _far*);
#line 4337 "windows.h"

LRESULT _far _pascal DefDlgProc(HWND, UINT, WPARAM, LPARAM);

HWND    _far _pascal CreateDialog(HINSTANCE, LPCSTR, HWND, DLGPROC);
HWND    _far _pascal CreateDialogIndirect(HINSTANCE, const void _far*, HWND, DLGPROC);
HWND    _far _pascal CreateDialogParam(HINSTANCE, LPCSTR, HWND, DLGPROC, LPARAM);
HWND    _far _pascal CreateDialogIndirectParam(HINSTANCE, const void _far*, HWND, DLGPROC, LPARAM);

int     _far _pascal DialogBox(HINSTANCE, LPCSTR, HWND, DLGPROC);
int     _far _pascal DialogBoxIndirect(HINSTANCE, HGLOBAL, HWND, DLGPROC);
int     _far _pascal DialogBoxParam(HINSTANCE, LPCSTR, HWND, DLGPROC, LPARAM);
int     _far _pascal DialogBoxIndirectParam(HINSTANCE, HGLOBAL, HWND, DLGPROC, LPARAM);

void    _far _pascal EndDialog(HWND, int);

int     _far _pascal GetDlgCtrlID(HWND);
HWND    _far _pascal GetDlgItem(HWND, int);
LRESULT _far _pascal SendDlgItemMessage(HWND, int, UINT, WPARAM, LPARAM);

void    _far _pascal SetDlgItemInt(HWND, int, UINT, BOOL);
UINT    _far _pascal GetDlgItemInt(HWND, int, BOOL _far* , BOOL);

void    _far _pascal SetDlgItemText(HWND, int, LPCSTR);
int     _far _pascal GetDlgItemText(HWND, int, LPSTR, int);

void    _far _pascal CheckDlgButton(HWND, int, UINT);
void    _far _pascal CheckRadioButton(HWND, int, int, int);
UINT    _far _pascal IsDlgButtonChecked(HWND, int);

HWND    _far _pascal GetNextDlgGroupItem(HWND, HWND, BOOL);
HWND    _far _pascal GetNextDlgTabItem(HWND, HWND, BOOL);

void    _far _pascal MapDialogRect(HWND, RECT _far*);
DWORD   _far _pascal GetDialogBaseUnits(void);






























#line 4402 "windows.h"
































typedef struct tagDRAWITEMSTRUCT
{
    UINT        CtlType;
    UINT        CtlID;
    UINT        itemID;
    UINT        itemAction;
    UINT        itemState;
    HWND        hwndItem;
    HDC         hDC;
    RECT        rcItem;
    DWORD       itemData;
} DRAWITEMSTRUCT;
typedef DRAWITEMSTRUCT _near* PDRAWITEMSTRUCT;
typedef DRAWITEMSTRUCT _far* LPDRAWITEMSTRUCT;



typedef struct tagMEASUREITEMSTRUCT
{
    UINT        CtlType;
    UINT        CtlID;
    UINT        itemID;
    UINT        itemWidth;
    UINT        itemHeight;
    DWORD       itemData;
} MEASUREITEMSTRUCT;
typedef MEASUREITEMSTRUCT _near* PMEASUREITEMSTRUCT;
typedef MEASUREITEMSTRUCT _far* LPMEASUREITEMSTRUCT;



typedef struct tagDELETEITEMSTRUCT
{
    UINT       CtlType;
    UINT       CtlID;
    UINT       itemID;
    HWND       hwndItem;
    DWORD      itemData;
} DELETEITEMSTRUCT;
typedef DELETEITEMSTRUCT _near* PDELETEITEMSTRUCT;
typedef DELETEITEMSTRUCT _far* LPDELETEITEMSTRUCT;



typedef struct tagCOMPAREITEMSTRUCT
{
    UINT        CtlType;
    UINT        CtlID;
    HWND        hwndItem;
    UINT        itemID1;
    DWORD       itemData1;
    UINT        itemID2;
    DWORD       itemData2;
} COMPAREITEMSTRUCT;
typedef COMPAREITEMSTRUCT _near* PCOMPAREITEMSTRUCT;
typedef COMPAREITEMSTRUCT _far* LPCOMPAREITEMSTRUCT;

























#line 4516 "windows.h"
#line 4517 "windows.h"

#line 4519 "windows.h"


































#line 4554 "windows.h"





















#line 4576 "windows.h"
#line 4577 "windows.h"


































#line 4612 "windows.h"
#line 4613 "windows.h"


typedef int   (_far _pascal* EDITWORDBREAKPROC)(LPSTR lpch, int ichCurrent, int cch, int code);





#line 4622 "windows.h"











#line 4634 "windows.h"



















#line 4654 "windows.h"

#line 4656 "windows.h"





















#line 4678 "windows.h"

#line 4680 "windows.h"




































#line 4717 "windows.h"

#line 4719 "windows.h"






















int     _far _pascal DlgDirList(HWND, LPSTR, int, int, UINT);
BOOL    _far _pascal DlgDirSelect(HWND, LPSTR, int);

int     _far _pascal DlgDirListComboBox(HWND, LPSTR, int, int, UINT);
BOOL    _far _pascal DlgDirSelectComboBox(HWND, LPSTR, int);


BOOL    _far _pascal DlgDirSelectEx(HWND, LPSTR, int, int);
BOOL    _far _pascal DlgDirSelectComboBoxEx(HWND, LPSTR, int, int);
#line 4751 "windows.h"














#line 4766 "windows.h"



















#line 4786 "windows.h"
#line 4787 "windows.h"





























#line 4817 "windows.h"

#line 4819 "windows.h"














#line 4834 "windows.h"






#line 4841 "windows.h"





typedef DWORD HHOOK;




typedef FARPROC HOOKPROC;
#line 4853 "windows.h"





HOOKPROC _far _pascal SetWindowsHook(int, HOOKPROC);
LRESULT _far _pascal DefHookProc(int, WPARAM, LPARAM, HOOKPROC _far*);
#line 4861 "windows.h"
BOOL    _far _pascal UnhookWindowsHook(int, HOOKPROC);



HHOOK   _far _pascal SetWindowsHookEx(int idHook, HOOKPROC lpfn, HINSTANCE hInstance, HTASK hTask);
BOOL    _far _pascal UnhookWindowsHookEx(HHOOK hHook);
LRESULT _far _pascal CallNextHookEx(HHOOK hHook, int code, WPARAM wParam, LPARAM lParam);

#line 4870 "windows.h"










#line 4881 "windows.h"























typedef struct tagCBT_CREATEWND
{
    CREATESTRUCT _far* lpcs;
    HWND    hwndInsertAfter;
} CBT_CREATEWND;
typedef CBT_CREATEWND _far* LPCBT_CREATEWND;


typedef struct tagCBTACTIVATESTRUCT
{
    BOOL    fMouse;
    HWND    hWndActive;
} CBTACTIVATESTRUCT;

#line 4919 "windows.h"
#line 4920 "windows.h"







typedef struct tagHARDWAREHOOKSTRUCT
{
    HWND    hWnd;
    UINT    wMessage;
    WPARAM  wParam;
    LPARAM  lParam;
} HARDWAREHOOKSTRUCT;
#line 4935 "windows.h"
#line 4936 "windows.h"












#line 4949 "windows.h"
#line 4950 "windows.h"
















typedef struct tagEVENTMSG
{
    UINT    message;
    UINT    paramL;
    UINT    paramH;
    DWORD   time;
} EVENTMSG;
typedef EVENTMSG *PEVENTMSG;
typedef EVENTMSG _near* NPEVENTMSG;
typedef EVENTMSG _far* LPEVENTMSG;

BOOL    _far _pascal EnableHardwareInput(BOOL);

#line 4980 "windows.h"








typedef struct tagDEBUGHOOKINFO
{
    HMODULE     hModuleHook;
    LPARAM      reserved;
    LPARAM      lParam;
    WPARAM      wParam;
    int         code;
} DEBUGHOOKINFO;
typedef DEBUGHOOKINFO _far* LPDEBUGHOOKINFO;


BOOL _far _pascal QuerySendMessage(HANDLE h1, HANDLE h2, HANDLE h3, LPMSG lpmsg);
#line 5001 "windows.h"

BOOL _far _pascal LockInput(HANDLE h1, HWND hwndInput, BOOL fLock);

long _far _pascal GetSystemDebugState(void);







#line 5013 "windows.h"





BOOL _far _pascal WinHelp(HWND hwndMain, LPCSTR lpszHelp, UINT usCommand, DWORD ulData);

















typedef struct tagMULTIKEYHELP
{
    UINT    mkSize;
    BYTE    mkKeylist;
    BYTE    szKeyphrase[1];
} MULTIKEYHELP;


typedef struct
{
    int  wStructSize;
    int  x;
    int  y;
    int  dx;
    int  dy;
    int  wMax;
    char rgchMember[2];
} HELPWININFO;
typedef HELPWININFO _near* PHELPWININFO;
typedef HELPWININFO _far* LPHELPWININFO;

#line 5058 "windows.h"





int     _far _pascal OpenSound(void);
void    _far _pascal CloseSound(void);

int     _far _pascal StartSound(void);
int     _far _pascal StopSound(void);

int     _far _pascal SetVoiceQueueSize(int, int);
int     _far _pascal SetVoiceNote(int, int, int, int);
int     _far _pascal SetVoiceAccent(int, int, int, int, int);
int     _far _pascal SetVoiceEnvelope(int, int, int);
int     _far _pascal SetVoiceSound(int, DWORD, int);

int     _far _pascal SetVoiceThreshold(int, int);
int _far* _far _pascal GetThresholdEvent(void);
int     _far _pascal GetThresholdStatus(void);

int     _far _pascal SetSoundNoise(int, int);











int     _far _pascal WaitSoundState(int);






int     _far _pascal SyncAllVoices(void);
int     _far _pascal CountVoiceNotes(int);
























#line 5125 "windows.h"


































































































#line 5224 "windows.h"

typedef struct tagDCB
{
    BYTE Id;
    UINT BaudRate;
    BYTE ByteSize;
    BYTE Parity;
    BYTE StopBits;
    UINT RlsTimeout;
    UINT CtsTimeout;
    UINT DsrTimeout;

    UINT fBinary        :1;
    UINT fRtsDisable    :1;
    UINT fParity        :1;
    UINT fOutxCtsFlow   :1;
    UINT fOutxDsrFlow   :1;
    UINT fDummy         :2;
    UINT fDtrDisable    :1;

    UINT fOutX          :1;
    UINT fInX           :1;
    UINT fPeChar        :1;
    UINT fNull          :1;
    UINT fChEvt         :1;
    UINT fDtrflow       :1;
    UINT fRtsflow       :1;
    UINT fDummy2        :1;

    char XonChar;
    char XoffChar;
    UINT XonLim;
    UINT XoffLim;
    char PeChar;
    char EofChar;
    char EvtChar;
    UINT TxDelay;
} DCB;
typedef DCB _far* LPDCB;



typedef struct tagCOMSTAT
{
    BYTE status;
    UINT cbInQue;
    UINT cbOutQue;
} COMSTAT;

























#line 5298 "windows.h"

int     _far _pascal BuildCommDCB(LPCSTR, DCB _far*);

int     _far _pascal OpenComm(LPCSTR, UINT, UINT);
int     _far _pascal CloseComm(int);

int     _far _pascal ReadComm(int, void _far*, int);
int     _far _pascal WriteComm(int, const void _far*, int);
int     _far _pascal UngetCommChar(int, char);
int     _far _pascal FlushComm(int, int);
int     _far _pascal TransmitCommChar(int, char);

int     _far _pascal SetCommState(const DCB _far*);
int     _far _pascal GetCommState(int, DCB _far*);
int     _far _pascal GetCommError(int, COMSTAT _far* );

int     _far _pascal SetCommBreak(int);
int     _far _pascal ClearCommBreak(int);

UINT _far* _far _pascal SetCommEventMask(int, UINT);
UINT    _far _pascal GetCommEventMask(int, int);

long    _far _pascal EscapeCommFunction(int, int);


BOOL    _far _pascal EnableCommNotification(int, HWND, int, int);


#line 5327 "windows.h"

#line 5329 "windows.h"



int     _far _pascal wvsprintf(LPSTR lpszOut, LPCSTR lpszFmt, const void _far* lpParams);


int     _far _cdecl WSPRINTF(LPSTR lpszOut, LPCSTR lpszFmt, ...);








typedef UINT HDRVR;

typedef LRESULT (_far _pascal* DRIVERPROC)(DWORD, HDRVR, UINT, LPARAM, LPARAM);




















typedef struct tagDRVCONFIGINFO
{
    DWORD   dwDCISize;
    LPCSTR  lpszDCISectionName;
    LPCSTR  lpszDCIAliasName;
} DRVCONFIGINFO;
typedef DRVCONFIGINFO _near* PDRVCONFIGINFO;
typedef DRVCONFIGINFO _far* LPDRVCONFIGINFO;










LRESULT _far _pascal DefDriverProc(DWORD dwDriverIdentifier, HDRVR driverID, UINT message, LPARAM lParam1, LPARAM lParam2);

HDRVR   _far _pascal OpenDriver(LPCSTR szDriverName, LPCSTR szSectionName, LPARAM lParam2);
LRESULT _far _pascal CloseDriver(HDRVR hDriver, LPARAM lParam1, LPARAM lParam2);

LRESULT _far _pascal SendDriverMessage(HDRVR hDriver, UINT message, LPARAM lParam1, LPARAM lParam2);

HINSTANCE _far _pascal GetDriverModuleHandle(HDRVR hDriver);

HDRVR   _far _pascal GetNextDriver(HDRVR, DWORD);







typedef struct tagDRIVERINFOSTRUCT
{
    UINT    length;
    HDRVR   hDriver;
    HINSTANCE hModule;
    char    szAliasName[128];
} DRIVERINFOSTRUCT;
typedef DRIVERINFOSTRUCT _far* LPDRIVERINFOSTRUCT;

BOOL    _far _pascal GetDriverInfo(HDRVR, DRIVERINFOSTRUCT _far*);

#line 5414 "windows.h"
#line 5415 "windows.h"
#line 5416 "windows.h"







#pragma option -a.      
#line 5425 "windows.h"

#line 5427 "windows.h"



























































































































































































#line 5615 "windows.h"

