typedef unsigned long   HEV,  *PHEV;

int __far __pascal FSH_ADDSHARE(char *pName,
                             unsigned short mode,
                             unsigned short hVPB,
                             unsigned long *phShare);

int __far __pascal FSH_CALLDRIVER(void *pPkt,
                               unsigned short hVPB,
                               unsigned short fControl);

int __far __pascal FSH_CANONICALIZE(char *pPathName,
                                 unsigned short cbPathBuf,
                                 char *pPathBuf,
                                 unsigned short *pFl);

int __far __pascal FSH_CHECKEANAME(unsigned short iLevel,
                                unsigned long cbEAName,
                                char *szEAN);

int __far __pascal FSH_CRITERROR(unsigned short cbMessage,
                              char *pMessage,
                              unsigned short nSubs,
                              char *pSubs,
                              unsigned short fAll);

int __far __pascal FSH_DEVIOCTL(unsigned short flag,
                             unsigned long hDev,
                             unsigned short sfn,
                             unsigned short cat,
                             unsigned short func,
                             char *pParm,
                             unsigned short cbParm,
                             char *pData,
                             unsigned short cbData);

int __far __pascal FSH_DOVOLIO(unsigned short operation,
                            unsigned short fAllowed,
                            unsigned short hVPB,
                            char *pData,
                            unsigned short *pcSec,
                            unsigned long iSec);


int __far __pascal FSH_DOVOLIO2(unsigned long hDev,
                             unsigned short sfn,
                             unsigned short cat,
                             unsigned short func,
                             char *pParm,
                             unsigned short cbParm,
                             char *pData,
                             unsigned short cbData);

int __far __pascal FSH_FINDCHAR(unsigned short nChars,
                             char *pChars,
                             char **ppStr);

int __far __pascal FSH_FINDDUPHVPB(unsigned short hVPB,
                                unsigned short *phVPB);

int __far __pascal FSH_FORCENOSWAP(unsigned short sel);

int __far __pascal FSH_GETPRIORITY(void);

int __far __pascal FSH_GETOVERLAPBUF(unsigned short hVPB,
    unsigned long iSec,
    unsigned long iSec2,     // (?)
    unsigned long *pisecBuf,
    char **ppBuf);

void  __far __pascal  FSH_GETVOLPARM(unsigned short hVPB,
                               struct vpfsi **ppVPBfsi,
                               struct vpfsd **ppVPBfsd);

void  __far __pascal  FSH_INTERR(char *pMsg,
                           unsigned short cbMsg);

void  __far __pascal  FSH_IOBOOST(void);

int __far __pascal FSH_IOSEMCLEAR(HEV pSem);

int __far __pascal FSH_ISCURDIRPREFIX(char *pMsg);

void  __far __pascal FSH_LOADCHAR(char **ppStr,
                             unsigned short *pChar);

int __far __pascal FSH_NAMEFROMSFN(unsigned short sfn,
                                char *pName,
                                unsigned short *pcbName);

void  __far __pascal FSH_PREVCHAR(char *pBeg,
                             char **ppStr);

int __far __pascal FSH_PROBEBUF(unsigned short operation,
                             char *pData,
                             unsigned short cbData);

int __far __pascal FSH_QSYSINFO(unsigned short index,
                             char *pData,
                             unsigned short cbData);

int __far __pascal FSH_REGISTERPERFCTRS(void *pDataBlk,
                                     void * pTextBlk,
                                     unsigned short fsFlags);

void  __far __pascal FSH_REMOVESHARE(unsigned long hShare);

int __far __pascal FSH_SEGALLOC(unsigned short flags,
                             unsigned long cbSeg,
                             unsigned short *pSel);

int __far __pascal FSH_SEGFREE(unsigned short sel);

int __far __pascal FSH_SEGREALLOC(unsigned short sel,
                               unsigned long cbSeg);

int __far __pascal FSH_SEMCLEAR(void *pSem);

int __far __pascal FSH_SEMREQUEST(void *pSem,
                               unsigned long cmsTimeout);

int __far __pascal FSH_SEMSET(void *pSem);

int __far __pascal FSH_SEMSETWAIT(void *pSem,
                               unsigned long cmsTimeout);

int __far __pascal FSH_SEMWAIT(void *pSem,
                            unsigned long cmsTimeout);

int __far __pascal FSH_SETVOLUME(unsigned short hVPB,
                              unsigned short fControl);

int __far __pascal FSH_STORECHAR(unsigned short chDBCS,
                              char **ppStr);

int __far __pascal FSH_UPPERCASE(char *szPathName,
                              unsigned short cbPathBuf,
                              char *pPathBuf);

int __far __pascal FSH_WILDMATCH(char *pPat,
                              char *pStr);

void  __far __pascal FSH_YIELD(void);