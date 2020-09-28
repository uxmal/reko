#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
 // This code was translated from the DCC decompiler project:
/*
 * Code to check for library functions. If found, replaces procNNNN with the
 * library function name. Also checks startup code for correct DS, and the
 * address of main()
 * (C) Mike van Emmerik
  */
#endregion

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Loading
{
    public class DccSignatureLoader
    {
        PerfectHash g_pattern_hasher = new PerfectHash();

        const int NIL = -1;                   /* Used like NULL, but 0 is valid */
        const byte WILD = 0xF4;

        const int SYMLEN = 16;          /* Number of chars in the symbol name, incl null */
        const int PATLEN = 23;          /* Number of bytes in the pattern part */

        private Program program;
        private ProcessorState state;

        /* Hash table structure */
        public class HT
        {
            public string htSym;
            public byte[] htPat;
        }

        //typedef struct HT_tag
        //{
        //    char htSym[SYMLEN];
        //    uint8_t htPat[PATLEN];
        //}
        //HT;

        public enum hlType : ushort
        {
            TYPE_UNKNOWN = 0,   /* unknown so far      		*/
            TYPE_BYTE_SIGN,     /* signed byte (8 bits) 	*/
            TYPE_BYTE_UNSIGN,   /* unsigned byte 			*/
            TYPE_WORD_SIGN,     /* signed word (16 bits) 	*/
            TYPE_WORD_UNSIGN,   /* unsigned word (16 bits)	*/
            TYPE_LONG_SIGN,     /* signed long (32 bits)	*/
            TYPE_LONG_UNSIGN,   /* unsigned long (32 bits)	*/
            TYPE_RECORD,        /* record structure			*/
            TYPE_PTR,           /* pointer (32 bit ptr) 	*/
            TYPE_STR,           /* string               	*/
            TYPE_CONST,         /* constant (any type)		*/
            TYPE_FLOAT,         /* floating point			*/
            TYPE_DOUBLE,        /* double precision float	*/
        }

/* Structure of the prototypes table. Same as the struct in parsehdr.h,
    except here we don't need the "next" index (the elements are already
    sorted by function name) */
class PH_FUNC_STRUCT
        {
            public string name; // [SYMLEN]  /* Name of function or arg */
            public hlType typ;               /* Return type */
            public int numArg;               /* Number of args */
            public int firstArg;             /* Index of first arg in chain */
            //  int     next;                /* Index of next function in chain */
            public bool bVararg;             /* True if variable arguements */
        }


        const int NUM_PLIST = 64;            /* Number of entries to increase allocation by */

/* statics */
static char [] buf = new char[100];          /* A general purpose buffer */
        int numKeys;                         /* Number of hash table entries (keys) */
        ushort numVert;                      /* Number of vertices in the graph (also size of g[]) */
        ushort PatLen;                       /* Size of the keys (pattern length) */
        ushort SymLen;                       /* Max size of the symbols, including null */
        static string sSigName;              /* Full path name of .sig file */
                                             
        static ushort[] T1base, T2base;      /* Pointers to start of T1, T2 */
        static short[]  g;                   /* g[] */
        static HT [] ht;                     /* The hash table */
        static PH_FUNC_STRUCT [] pFunc;      /* Points to the array of func names */
        static hlType [] pArg = null;        /* Points to the array of param types */
        static int numFunc;                  /* Number of func names actually stored */
        static int numArg;                   /* Number of param names actually stored */
//#define DCCLIBS "dcclibs.dat"              /* Name of the prototypes data file */

        /* prototypes */
        //void checkHeap(string msg);        /* For debugging */

        //void fixWildCards(uint8_t pat[]);  /* In fixwild.c */

        /*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   *\
        *                                                            *
        *   S t a r t   P a t t e r n s   ( V e n d o r    i d )     *
        *                                                            *
        \*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   */
        static byte [] pattMsC5Start = new byte[] {
            0xB4, 0x30,         /* Mov ah, 30 */
            0xCD, 0x21,         /* int 21 (dos version number) */
            0x3C, 0x02,         /* cmp al, 2 */
            0x73, 0x02,         /* jnb $+4 */
            0xCD, 0x20,         /* int 20 (exit) */
            0xBF                /* Mov di, DSEG */
        };

        static byte [] pattMsC8Start = new byte[] 
        {
            0xB4, 0x30,         /* Mov ah, 30 */
            0xCD, 0x21,         /* int 21 */
            0x3C, 0x02,         /* cmp al,2 */
            0x73, 0x05,         /* jnb $+7 */
            0x33, 0xC0,         /* xor ax, ax */
            0x06, 0x50,         /* push es:ax */
            0xCB,               /* retf */
            0xBF                /* mov di, DSEG */
        };

        static byte [] pattMsC8ComStart = new byte[]
        {
            0xB4, 0x30,         /* Mov ah, 30 */
            0xCD, 0x21,         /* int 21 (dos version number) */
            0x3C, 0x02,         /* cmp al, 2 */
            0x73, 0x01,         /* jnb $+3 */
            0xC3,               /* ret */
            0x8C, 0xDF          /* Mov di, ds */
        };

        static byte[]  pattBorl2Start = new byte[]
        {
            0xBA, WILD, WILD,       /* Mov dx, dseg */
            0x2E, 0x89, 0x16,       /* mov cs:[], dx */
            WILD, WILD,
            0xB4, 0x30,             /* mov ah, 30 */
            0xCD, 0x21,             /* int 21 (dos version number) */
            0x8B, 0x2E, 0x02, 0,    /* mov bp, [2] */
            0x8B, 0x1E, 0x2C, 0,    /* mov bx, [2C] */
            0x8E, 0xDA,             /* mov ds, dx */
            0xA3, WILD, WILD,       /* mov [xx], ax */
            0x8C, 0x06, WILD, WILD, /* mov [xx], es */
            0x89, 0x1E, WILD, WILD, /* mov [xx], bx */
            0x89, 0x2E, WILD, WILD, /* mov [xx], bp */
            0xC7                    /* mov [xx], -1 */
        };

        static byte[] pattBorl3Start = new byte[]
        {
            0xBA, WILD, WILD,   	/* Mov dx, dseg */
            0x2E, 0x89, 0x16,   	/* mov cs:[], dx */
            WILD, WILD,
            0xB4, 0x30,         	/* mov ah, 30 */
            0xCD, 0x21,         	/* int 21 (dos version number) */
            0x8B, 0x2E, 0x02, 0,	/* mov bp, [2] */
            0x8B, 0x1E, 0x2C, 0,	/* mov bx, [2C] */
            0x8E, 0xDA,         	/* mov ds, dx */
            0xA3, WILD, WILD,       /* mov [xx], ax */
            0x8C, 0x06, WILD, WILD, /* mov [xx], es */
            0x89, 0x1E, WILD, WILD, /* mov [xx], bx */
            0x89, 0x2E, WILD, WILD, /* mov [xx], bp */
            0xE8                    /* call ... */
        };

        static byte[] pattBorl4on = new byte[]
        {
            0x9A, 0, 0, WILD, WILD	/* Call init (offset always 0) */
        };

        static byte[] pattBorl4Init = new byte[]
        {
            0xBA, WILD, WILD,		/* Mov dx, dseg */
            0x8E, 0xDA,         	/* mov ds, dx */
            0x8C, 0x06, WILD, WILD, /* mov [xx], es */
            0x8B, 0xC4,				/* mov ax, sp */
            0x05, 0x13, 0,			/* add ax, 13h */
            0xB1, 0x04,				/* mov cl, 4 */
            0xD3, 0xE8,				/* shr ax, cl */
            0x8C, 0xD2				/* mov dx, ss */
        };

        static byte[] pattBorl5Init =
        {
            0xBA, WILD, WILD,		/* Mov dx, dseg */
            0x8E, 0xDA,         	/* mov ds, dx */
            0x8C, 0x06, 0x30, 0,	/* mov [0030], es */
            0x33, 0xED,				/* xor bp, bp <----- */
            0x8B, 0xC4,				/* mov ax, sp */
            0x05, 0x13, 0,			/* add ax, 13h */
            0xB1, 0x04,				/* mov cl, 4 */
            0xD3, 0xE8,				/* shr ax, cl */
            0x8C, 0xD2				/* mov dx, ss */
        };

        static byte[] pattBorl7Init =
        {
            0xBA, WILD, WILD,		/* Mov dx, dseg */
            0x8E, 0xDA,         	/* mov ds, dx */
            0x8C, 0x06, 0x30, 0,	/* mov [0030], es */
            0xE8, WILD, WILD,		/* call xxxx */
            0xE8, WILD, WILD,		/* call xxxx... offset always 00A0? */
            0x8B, 0xC4,				/* mov ax, sp */
            0x05, 0x13, 0,			/* add ax, 13h */
            0xB1, 0x04,				/* mov cl, 4 */
            0xD3, 0xE8,				/* shr ax, cl */
            0x8C, 0xD2				/* mov dx, ss */
        };


        static byte[] pattLogiStart =
        {
            0xEB, 0x04,         /* jmp short $+6 */
            WILD, WILD,         /* Don't know what this is */
            WILD, WILD,         /* Don't know what this is */
            0xB8, WILD, WILD,   /* mov ax, dseg */
            0x8E, 0xD8          /* mov ds, ax */
        };

        static byte[] pattTPasStart =
        {
            0xE9, 0x79, 0x2C    /* Jmp 2D7C - Turbo pascal 3.0 */
        };



        /*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   *\
        *                                                            *
        *       M a i n   P a t t e r n s   ( M o d e l    i d )     *
        *                                                            *
        \*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   */


        /* This pattern works for MS and Borland, small and tiny model */
        static byte[]  pattMainSmall =
        {
            0xFF, 0x36, WILD, WILD,                 /* Push environment pointer */
            0xFF, 0x36, WILD, WILD,                 /* Push argv */
            0xFF, 0x36, WILD, WILD,                 /* Push argc */
            0xE8, WILD, WILD						/* call _main */
            //  0x50,                                   /* push ax... not in Borland V3 */
            //  0xE8                                    /* call _exit */
        };
        /* Num bytes from start pattern to the relative offset of main() */
        const int OFFMAINSMALL = 13;

        /* This pattern works for MS and Borland, medium model */
        static byte[] pattMainMedium =
        {
            0xFF, 0x36, WILD, WILD,                 /* Push environment pointer */
            0xFF, 0x36, WILD, WILD,                 /* Push argv */
            0xFF, 0x36, WILD, WILD,                 /* Push argc */
            0x9A, WILD, WILD, WILD, WILD            /* call far _main */
            //  0x50                                /* push ax */
            //  0x0E,                               /* push cs NB not tested Borland */
            //  0xE8                                /* call _exit */
        };
        /* Num bytes from start pattern to the relative offset of main() */
        const int OFFMAINMEDIUM =13;

        /* This pattern works for MS and Borland, compact model */
        static byte[] pattMainCompact =
        {
            0xFF, 0x36, WILD, WILD,                 /* Push environment pointer lo */
            0xFF, 0x36, WILD, WILD,                 /* Push environment pointer hi */
            0xFF, 0x36, WILD, WILD,                 /* Push argv lo */
            0xFF, 0x36, WILD, WILD,                 /* Push argv hi */
            0xFF, 0x36, WILD, WILD,                 /* Push argc */
            0xE8, WILD, WILD,                       /* call _main */
            //  0x50,                                   /* push ax */
            //  0xE8                                    /* call _exit */
        };
        /* Num bytes from start pattern to the relative offset of main() */
        const int OFFMAINCOMPACT= 21;

        /* This pattern works for MS and Borland, large model */
        static byte[] pattMainLarge =
        {
            0xFF, 0x36, WILD, WILD,                 /* Push environment pointer lo */
            0xFF, 0x36, WILD, WILD,                 /* Push environment pointer hi */
            0xFF, 0x36, WILD, WILD,                 /* Push argv lo */
            0xFF, 0x36, WILD, WILD,                 /* Push argv hi */
            0xFF, 0x36, WILD, WILD,                 /* Push argc */
            0x9A, WILD, WILD, WILD, WILD            /* call far _main */
            //  0x50                                    /* push ax */
            //  0x0E,                                   /* push cs */
            //  0xE8                                    /* call _exit */
        };
        /* Num bytes from start pattern to the relative offset of main() */
        const int OFFMAINLARGE = 21;


        /*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   *\
        *                                                            *
        *       	M i s c e l l a n e o u s    P a t t e r n s	 *
        *                                                            *
        \*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   */

        /* This pattern is for the stack check code in Microsoft compilers */
        static byte [] pattMsChkstk =
        {
            0x59,					/* pop cx		*/
            0x8B, 0xDC,          	/* mov bx, sp	*/
            0x2B, 0xD8,				/* sub bx, ax	*/
            0x72, 0x0A,				/* jb bad		*/
            0x3B, 0x1E, WILD, WILD,	/* cmp bx, XXXX */
            0x72, 0x04,				/* jb bad		*/
            0x8B, 0xE3,				/* mov sp, bx	*/
            0xFF, 0xE1,				/* jmp [cx]		*/
            0x33, 0xC0,				/* xor ax, ax	*/
            0xE9					/* jmp XXXX		*/
        };

        // This procedure is called to initialise the library check code 
        public bool SetupLibCheck(IServiceProvider services)
        {
            var diag = services.RequireService<IDiagnosticsService>();
            var cfgSvc = services.RequireService<IConfigurationService>();
            string fpath = cfgSvc.GetInstallationRelativePath("msdos", sSigName);
            var fsSvc = services.RequireService<IFileSystemService>();
            if (!fsSvc.FileExists(fpath))
            {
                diag.Warn(string.Format("Can't open signature file {0}.", fpath));
                return false;
            }
            var bytes = fsSvc.ReadAllBytes(fpath);
            return SetupLibCheck(services, fpath, bytes);
        }

        public bool SetupLibCheck(IServiceProvider services, string fpath, byte[] bytes)
        {
            var diag = services.RequireService<IDiagnosticsService>();
            var rdr = new LeImageReader(bytes);
            ushort w, len;
            int i;

            //readProtoFile();

            /* Read the parameters */
            uint fileSig;
            if (!rdr.TryReadLeUInt32(out fileSig) || fileSig != 0x73636364) // "dccs"
            {
                diag.Warn(string.Format("{0} is not a DCC signature file.", fpath));
                return false;
            }

            numKeys = rdr.ReadLeUInt16();
            numVert = rdr.ReadLeUInt16();
            PatLen = rdr.ReadLeUInt16();
            SymLen = rdr.ReadLeUInt16();
            if ((PatLen != PATLEN) || (SymLen != SYMLEN))
            {
                diag.Warn(string.Format("Can't use signature file with sym and pattern lengths of {0} and {1}.", SymLen, PatLen));
                return false;
            }

            // Initialise the perfhlib stuff. Also allocates T1, T2, g, etc
            // Set the parameters for the hash table
            g_pattern_hasher.setHashParams(
                            numKeys,                // The number of symbols
                            PatLen,                 // The length of the pattern to be hashed
                            256,                    // The character set of the pattern (0-FF)
                            (char)0,                // Minimum pattern character value
                            numVert);               // Specifies c, the sparseness of the graph. See Czech, Havas and Majewski for details
            T1base = g_pattern_hasher.readT1();
            T2base = g_pattern_hasher.readT2();
            g = g_pattern_hasher.readG();

            /* Read T1 and T2 tables */
            ushort ww;
            if (!rdr.TryReadLeUInt16(out ww) || ww != 0x3154)    // "T1"
            {
                Debug.Print("Expected 'T1'");
                diag.Warn(string.Format("{0} is not a valid DCCS file.", fpath));
                return false;
            }
            len = (ushort) (PatLen * 256u * 2);        // 2 = sizeof ushort
            w = rdr.ReadLeUInt16();
            if (w != len)
            {
                Debug.Print("Problem with size of T1: file {0}, calc {1}", w, len);
                diag.Warn(string.Format("{0} is not a valid DCCS file.", fpath));
                return false;
            }
            readFileSection(T1base, len, rdr);

            if (!rdr.TryReadLeUInt16(out ww) || ww != 0x3254)    // "T2"
            {
                Debug.Print("Expected 'T2'");
                return false;
            }
            w = rdr.ReadLeUInt16();
            if (w != len)
            {
                Debug.Print("Problem with size of T2: file %d, calc %d\n", w, len);
                diag.Warn(string.Format("{0} is not a valid DCCS file.", fpath));
                return false;
            }
            readFileSection(T2base, len, rdr);

            /* Now read the function g[] */
            if (!rdr.TryReadLeUInt16(out ww) || ww != 0x6767)    // "gg"
            {
                Debug.Print("Expected 'gg'");
                diag.Warn(string.Format("{0} is not a valid DCCS file.", fpath));
                return false;
            }
            len = (ushort) (numVert * 2); //  sizeof(uint16_t));
            w = rdr.ReadLeUInt16();
            if (w != len)
            {
                Debug.Print("Problem with size of g[]: file {0}, calc {1}", w, len);
                diag.Warn(string.Format("{0} is not a valid DCCS file.", fpath));
                return false;
            }
            readFileSection(g, len, rdr);

            /* This is now the hash table */
            /* First allocate space for the table */
            ht = new HT[numKeys];
            if (!rdr.TryReadLeUInt16(out ww) || ww != 0x7468)    // "ht"
            {
                Debug.Print("Expected 'ht'");
                diag.Warn(string.Format("{0} is not a valid DCCS file.", fpath));
                return false;
            }
            w = rdr.ReadLeUInt16();
            if (w != numKeys * (SymLen + PatLen + 2)) // sizeof(uint16_t)))
            {
                Debug.Print("Problem with size of hash table: file {0}, calc {1}", w, len);
                diag.Warn(string.Format("{0} is not a valid DCCS file.", fpath));
                return false;
            }

            ht = new HT[numKeys];
            for (i = 0; i < numKeys; i++)
            {
                var aSym = rdr.ReadBytes(SymLen)
                    .TakeWhile(b => b != 0).ToArray();

                ht[i] = new HT
                {
                    htSym = Encoding.ASCII.GetString(aSym),
                    htPat = rdr.ReadBytes(PatLen)
                };
            }
            return true;
        }

        /// <summary>
        /// Check this function to see if it is a library function. Return true if
        /// it is, and copy its name to pProc.Name
        /// </summary>
        public bool LibCheck(IServiceProvider services, Program program, Procedure proc, Address addr)
        {
            var diagSvc = services.RequireService<IDiagnosticsService>();
            long fileOffset;
            int h;
            // i, j, arg;
            uint Idx;

            if (false) //  && program.bSigs == false)
            {
                /* No signatures... can't rely on hash parameters to be initialised
                so always return false */
                return false;
            }

            ImageSegment segment;
            if (!program.SegmentMap.TryFindSegment(addr, out segment))
                return false;
            

            fileOffset = addr - segment.MemoryArea.BaseAddress;              /* Offset into the image */
            //if (fileOffset == program.offMain)
            //{
            //    /* Easy - this function is called main! */
            //    pProc.Name = "main";
            //    return false;
            //}

            byte[] pat = new byte[PATLEN];
            if (!segment.MemoryArea.TryReadBytes(fileOffset, PATLEN, pat))
                return false;

            fixWildCards(pat);                                   /* Fix wild cards in the copy */
            h = g_pattern_hasher.hash(pat);                      /* Hash the found proc */
                                                                 /* We always have to compare keys, because the hash function will always return a valid index */
            if (MemoryArea.CompareArrays(ht[h].htPat, 0, pat, PATLEN))
            {
#if NOT_YET
                /* We have a match. Save the name, if not already set */
                if (string.IsNullOrEmpty(pProc.Name))     /* Don't overwrite existing name */
                {
                    /* Give proc the new name */
                    pProc.Name = ht[h].htSym;
                }
                /* But is it a real library function? */
                i = NIL;
                if ((numFunc == 0) || (i = searchPList(ht[h].htSym)) != NIL)
                {
                    pProc.flg |= PROC_ISLIB;        /* It's a lib function */
                    pProc.callingConv(CConv::C);
                    if (i != NIL)
                    {
                        /* Allocate space for the arg struct, and copy the hlType to
                            the appropriate field */
                        arg = pFunc[i].firstArg;
                        pProc.args.numArgs = pFunc[i].numArg;
                        pProc.args.resize(pFunc[i].numArg);
                        for (j = 0; j < pFunc[i].numArg; j++)
                        {
                            pProc.args[j].type = pArg[arg++];
                        }
                        if (pFunc[i].typ != hlType.TYPE_UNKNOWN)
                        {
                            pProc.retVal.type = pFunc[i].typ;
                            pProc.flg |= PROC_IS_FUNC;
                            switch (pProc.retVal.type)
                            {
                            case hlType.TYPE_LONG_SIGN:
                            case hlType.TYPE_LONG_UNSIGN:
                                pProc.liveOut.setReg(rDX).addReg(rAX);
                                break;
                            case hlType.TYPE_WORD_SIGN:
                            case hlType.TYPE_WORD_UNSIGN:
                                pProc.liveOut.setReg(rAX);
                                break;
                            case hlType.TYPE_BYTE_SIGN:
                            case hlType.TYPE_BYTE_UNSIGN:
                                pProc.liveOut.setReg(rAL);
                                break;
                            case hlType.TYPE_STR:
                            case hlType.TYPE_PTR:
                                fprintf(stderr, "Warning assuming Large memory model\n");
                                pProc.liveOut.setReg(rAX).addReg(rDS);
                                break;
                            default:
                                diagSvc.Warn(string.Format("Unknown retval type {0} for {1} in LibCheck.",
                                           pProc.retVal.type,
                                           pProc.Name);
                                break;
                                /*** other types are not considered yet ***/
                            }
                        }
                        pProc.getFunctionType()->m_vararg = pFunc[i].bVararg;
                    }
                }
                else if (i == NIL)
                {
                    /* Have a symbol for it, but does not appear in a header file.
                        Treat it as if it is not a library function */
                    pProc.flg |= PROC_RUNTIME;      /* => is a runtime routine */
                }
#endif
            }
            if (locatePattern(segment.MemoryArea.Bytes, (uint) fileOffset,
                              (uint) (fileOffset + pattMsChkstk.Length),
                              pattMsChkstk, out Idx))
            {
                /* Found _chkstk */
                //pProc.Name = "chkstk";
                //pProc.flg |= PROC_ISLIB;        /* We'll say its a lib function */
                //pProc.args.numArgs = 0;     /* With no args */
            }

            return true;  // ((pProc.flg & PROC_ISLIB) != 0);
        }

        private void fixWildCards(byte[] pat)
        {
            throw new NotImplementedException();
        }

        // Read a section of the file, considering endian issues
        void readFileSection(ushort[] p, int len, EndianImageReader rdr)
        {
            int pp = 0;
            for (int i = 0; i < len; i += 2)
            {
                p[pp++] = rdr.ReadLeUInt16();
            }
        }

        // Read a section of the file, considering endian issues
        void readFileSection(short[] p, int len, EndianImageReader rdr)
        {
            int pp = 0;
            for (int i = 0; i < len; i += 2)
            {
                p[pp++] = rdr.ReadLeInt16();
            }
        }

        /* The following two functions are dummies, since we don't call map() */
        void getKey(int i, byte[] keys, out int n)
        {
            n = 0;
        }

        void dispKey(int i)
        {

        }

        /*  Search the source array between limits iMin and iMax for the pattern (length
            iPatLen). The pattern can contain wild bytes; if you really want to match
            for the pattern that is used up by the WILD uint8_t, tough - it will match with
            everything else as well. */
        static bool locatePattern(
            byte[] source, uint iMin, uint iMax,
            byte[] pattern,
            out uint index)
        {
            index = 0;
            uint i, j;
            uint pSrc = 0;                             /* Pointer to start of considered source */
            uint iLast;

            iLast = iMax - (uint)pattern.Length;                 /* Last source uint8_t to consider */

            for (i = iMin; i <= iLast; i++)
            {
                pSrc = i;                  /* Start of current part of source */
                                           /* i is the index of the start of the moving pattern */
                for (j = 0; j < pattern.Length; j++)
                {
                    /* j is the index of the uint8_t being considered in the pattern. */
                    if ((source[pSrc] != pattern[j]) && (pattern[j] != WILD))
                    {
                        /* A definite mismatch */
                        break;                      /* Break to outer loop */
                    }
                    pSrc++;
                }
                if (j >= pattern.Length)
                {
                    /* Pattern has been found */
                    index = i;                     /* Pass start of pattern */
                    return true;                       /* Indicate success */
                }
                /* Else just try next value of i */
            }
            /* Pattern was not found */
            index = ~0u;                            /* Invalidate index */
            return false;                               /* Indicate failure */
        }


        void STATE_checkStartup(Program program, ProcessorState state)
        {
            Address addrEntry;
            /* This function checks the startup code for various compilers' way of
            loading DS. If found, it sets DS. This may not be needed in the future if
            pushing and popping of registers is implemented.
            Also sets program.offMain and program.segMain if possible */

            uint init;
            uint i;
            //uint startoff; 
            //ushort rel, para;
            char chModel = 'x';
            char chVendor = 'x';
            char chVersion = 'x';
            char[] temp = new char[4];

            //$TODO: get msdos entry.
            Address start = program.EntryPoints.Keys.First();

            /* Check the Turbo Pascal signatures first, since they involve only the
                        first 3 bytes, and false positives may be founf with the others later */
            ImageSegment segment;
            program.SegmentMap.TryFindSegment(start, out segment);
            var image = segment.MemoryArea;
            var startOff = (uint)(start - image.BaseAddress);   /* Offset into the Image of the initial CS:IP */
            if (locatePattern(image.Bytes,
                startOff, startOff + 5, pattBorl4on, out i))
            {
                /* The first 5 bytes are a far call. Follow that call and
                                determine the version from that */
                var addrNew = ReadSegPtr(image, startOff + 1);
                init = (uint)(addrNew - image.BaseAddress);
                if (locatePattern(image.Bytes, init, init + 26, pattBorl4Init, out i))
                {
                    setState("ds", image.ReadLeUInt16(i + 1));
                    Debug.Print("Borland Pascal v4 detected\n");
                    chVendor = 't';                     /* Trubo */
                    chModel = 'p';						/* Pascal */
                    chVersion = '4';                    /* Version 4 */
                    addrEntry = start;                  /* Code starts immediately */
                                             			/* At the 5 byte jump */
                    goto gotVendor;                     /* Already have vendor */
                }
                else if (locatePattern(image.Bytes, init, init + 26, pattBorl5Init, out i))
                {
                    setState("ds", image.ReadLeUInt16(i + 1));
                    Debug.Print("Borland Pascal v5.0 detected");
                    chVendor = 't';                     /* Trubo */
                    chModel = 'p';                      /* Pascal */
                    chVersion = '5';                    /* Version 5 */
                    addrEntry = start;                  /* Code starts immediately */
                    goto gotVendor;                     /* Already have vendor */
                }
                else if (locatePattern(image.Bytes, init, init + 26, pattBorl7Init, out i))
                {
                    setState("ds", image.ReadLeUInt16(i + 1));
                    Debug.Print("Borland Pascal v7 detected");
                    chVendor = 't';                     /* Trubo */
                    chModel = 'p';                      /* Pascal */
                    chVersion = '7';                    /* Version 7 */
                    addrEntry = start;                  /* Code starts immediately */
                    goto gotVendor;                     /* Already have vendor */
                }

            }

            /* Search for the call to main pattern. This is compiler independant,
                but decides the model required. Note: must do the far data models
                (large and compact) before the others, since they are the same pattern
                as near data, just more pushes at the start. */
            if (image.Length > startOff + 0x180 + pattMainLarge.Length)
            {
                if (locatePattern(image.Bytes, startOff, startOff + 0x180, pattMainLarge, out i))
                {
                    addrEntry = ReadSegPtr(image, i + OFFMAINLARGE);
                    chModel = 'l';                          /* Large model */
                }
                else if (locatePattern(image.Bytes, startOff, startOff + 0x180, pattMainCompact, out i))
                {
                    short srel = image.ReadLeInt16(i + OFFMAINCOMPACT);/* This is the rel addr of main */
                    addrEntry = image.BaseAddress + i + OFFMAINCOMPACT + 2 + srel; /* Save absolute image offset */
                    chModel = 'c';                          /* Compact model */
                }
                else if (locatePattern(image.Bytes, startOff, startOff + 0x180, pattMainMedium,
                                      out i))
                {
                    addrEntry = ReadSegPtr(image, i + OFFMAINMEDIUM);  /* This is abs off of main */
                    chModel = 'm';                          /* Medium model */
                }
                else if (locatePattern(image.Bytes, startOff, startOff + 0x180, pattMainSmall,
                                       out i))
                {
                    var srel = image.ReadLeInt16(i + OFFMAINSMALL); /* This is rel addr of main */
                    addrEntry = image.BaseAddress + i + OFFMAINSMALL + 2 + srel;    /* Save absolute image offset */
                    chModel = 's';                          /* Small model */
                }
                else if (MemoryArea.CompareArrays(image.Bytes, (int) startOff, pattTPasStart, pattTPasStart.Length))
                {
                    var srel = image.ReadLeInt16(startOff + 1);     /* Get the jump offset */
                    addrEntry = start + srel + 3;          /* Save absolute image offset */
                    addrEntry += 0x20;                   /* These first 32 bytes are setting up */
                    chVendor = 't';                         /* Turbo.. */
                    chModel = 'p';                          /* ...Pascal... (only 1 model) */
                    chVersion = '3';                        /* 3.0 */
                    Debug.Print("Turbo Pascal 3.0 detected");
                    Debug.Print("Main at {0}", addrEntry);
                    goto gotVendor;                         /* Already have vendor */
                }
                else
                {
                    Debug.Print("Main could not be located!");
                    addrEntry = null;
                }
            }
            else
            {
                Debug.Print("Main could not be located!");
                addrEntry = null;
            }

            Debug.Print("Model: {0}", chModel);
            //program.addressingMode = chModel;

            /* Now decide the compiler vendor and version number */
            if (MemoryArea.CompareArrays(image.Bytes, (int)startOff, pattMsC5Start, pattMsC5Start.Length))
            {
                /* Yes, this is Microsoft startup code. The DS is sitting right here
                    in the next 2 bytes */
                setState("ds", image.ReadLeUInt16((uint)(startOff + pattMsC5Start.Length)));
                chVendor = 'm';                     /* Microsoft compiler */
                chVersion = '5';                    /* Version 5 */
                Debug.Print("MSC 5 detected");
            }

            /* The C8 startup pattern is different from C5's */
            else if (MemoryArea.CompareArrays(image.Bytes, (int)startOff, pattMsC8Start, pattMsC8Start.Length))
            {
                setState("ds", image.ReadLeUInt16((uint)(startOff + pattMsC8Start.Length)));
                chVendor = 'm';                     /* Microsoft compiler */
                chVersion = '8';                    /* Version 8 */
                Debug.Print("MSC 8 detected");
            }

            /* The C8 .com startup pattern is different again! */
            else if (MemoryArea.CompareArrays(
                image.Bytes,
                (int)startOff,
                pattMsC8ComStart,
                pattMsC8ComStart.Length))
            {
                Debug.Print("MSC 8 .com detected");
                chVendor = 'm';                     /* Microsoft compiler */
                chVersion = '8';                    /* Version 8 */
            }

            else if (locatePattern(image.Bytes, startOff, startOff + 0x30, pattBorl2Start,
                                   out i))
            {
                /* Borland startup. DS is at the second uint8_t (offset 1) */
                setState("ds", image.ReadLeUInt16(i + 1));
                Debug.Print("Borland v2 detected\n");
                chVendor = 'b';                     /* Borland compiler */
                chVersion = '2';                    /* Version 2 */
            }

            else if (locatePattern(image.Bytes, startOff, startOff + 0x30, pattBorl3Start,
                                   out i))
            {
                /* Borland startup. DS is at the second uint8_t (offset 1) */
                setState("ds", image.ReadLeUInt16(i + 1));
                Debug.Print("Borland v3 detected\n");
                chVendor = 'b';                     /* Borland compiler */
                chVersion = '3';                    /* Version 3 */
            }

            else if (locatePattern(image.Bytes, startOff, startOff + 0x30, pattLogiStart,
                                   out i))
            {
                /* Logitech modula startup. DS is 0, despite appearances */
                Debug.Print("Logitech modula detected");
                chVendor = 'l';                     /* Logitech compiler */
                chVersion = '1';                    /* Version 1 */
            }

            /* Other startup idioms would go here */
            else
            {
                Debug.Print("Warning - compiler not recognised");
            }

            gotVendor:

            sSigName = string.Format("dcc{0}{1}{2}.sig",
                    chVendor, /* Add vendor */
                    chVersion, /* Add version */
                    chModel); /* Add model */
            Debug.Print("Signature file: {0}", sSigName);
        }

        private Address ReadSegPtr(MemoryArea mem, uint offset)
        {
            var off = mem.ReadLeUInt16(offset);
            var seg = mem.ReadLeUInt16(offset+2);
            return Address.SegPtr(seg, off);
        }

        const string DCCLIBS = "dclibs.lst";

        /*  DCCLIBS.DAT is a data file sorted on function name containing names and
            return types of functions found in include files, and the names and types
            of arguements. Only functions in this list will be considered library
            functions; others (like LXMUL@) are helper files, and need to be analysed
            by dcc, rather than considered as known functions. When a prototype is
            found (in searchPList()), the parameter info is written to the proc struct.
        */
        void readProtoFile(IServiceProvider services)
        {
            var diagSvc = services.RequireService<IDiagnosticsService>();
            var cfgSvc = services.RequireService<IConfigurationService>();
            var szProFName = cfgSvc.GetInstallationRelativePath("msdos", DCCLIBS); /* Full name of dclibs.lst */
            var fsSvc = services.RequireService<IFileSystemService>();
            if (fsSvc.FileExists(szProFName))
            {
                diagSvc.Warn(string.Format("Cannot open library prototype data file {0}.", szProFName));
                return;
            }
            var bytes = fsSvc.ReadAllBytes(szProFName);
            var fProto = new LeImageReader(bytes);
            int i;

            uint fileSig = fProto.ReadLeUInt32();
            if (fileSig != 0x70636364)      // "dccp"
            {
                diagSvc.Warn(string.Format("{0} is not a dcc prototype file.", szProFName));
                return;
            }

            ushort sectionID = fProto.ReadLeUInt16();
            if (sectionID != 0x4E46)        // "FN"
            {
                Debug.Print("FN (Function) subsection expected in {0}", szProFName);
                diagSvc.Warn(string.Format("{0} is not a dcc prototype file.", szProFName));
                return;
            }
            numFunc = fProto.ReadLeUInt16();    /* Num of entries to allocate */

            /* Allocate exactly correct # entries */
            pFunc = new PH_FUNC_STRUCT[numFunc];

            for (i = 0; i < numFunc; i++)
            {
                var symbuf = fProto.ReadBytes(SYMLEN);
                if (symbuf.Length != SYMLEN)
                    break;
                pFunc[i].typ = (hlType)fProto.ReadLeUInt16();
                pFunc[i].numArg = fProto.ReadLeUInt16();
                pFunc[i].firstArg = fProto.ReadLeUInt16();
                int c = fProto.ReadByte();
                pFunc[i].bVararg = (c != 0); //fread(&pFunc[i].bVararg, 1, 1, fProto);
            }

            sectionID = fProto.ReadLeUInt16();
            if (sectionID != 0x4D50)    // "PM"
            {
                Debug.Print("PM (Parameter) subsection expected in {0}", szProFName);
                return;
            }

            numArg = fProto.ReadLeUInt16();     /* Num of entries to allocate */

            /* Allocate exactly correct # entries */
            pArg = new hlType[numArg];

            for (i = 0; i < numArg; i++)
            {
                //      fread(&pArg[i], 1, SYMLEN, fProto);     /* No names to read as yet */
                pArg[i] = (hlType)fProto.ReadLeUInt16();
            }
        }

        void setState(string regName, ushort val)
        {
            state.SetRegister(
                program.Architecture.GetRegister(regName),
                Constant.Word16(val));

        }

        int searchPList(string name)
        {
            /* Search through the symbol names for the name */
            /* Use binary search */
            int mx, mn, i, res;


            mx = numFunc;
            mn = 0;

            while (mn < mx)
            {
                i = mn + (mx - mn) / 2;
                res = pFunc[i].name.CompareTo(name);
                if (res == 0)
                {
                    return i;            /* Found! */
                }
                else
                {
                    if (res < 0)
                    {
                        mn = i + 1;
                    }
                    else
                    {
                        mx = i - 1;
                    }
                }
            }

            /* Still could be the case that mn == mx == required record */
            res = string.Compare(pFunc[mn].name, name);
            if (res == 0)
            {
                return mn;            /* Found! */
            }
            return NIL;
        }
    }
}
