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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using System.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Services;

namespace Reko.Environments.Msdos
{
    /// <summary>
    /// This class identifies the "real" entry point of the binary based 
    /// on pattern recognition from various MS-DOS compilers
    /// </summary>
    public class StartupFinder
    {
        const byte WILD = 0xF4;

        private IServiceProvider services;
        private IProcessorArchitecture arch;
        private Program program;
        private Address start;
        private ProcessorState state;

        public StartupFinder(IServiceProvider services, Program program, Address addrStart)
        {
            this.services = services;
            this.program = program;
            this.arch = program.Architecture;
            this.start = addrStart;
            this.state = program.Architecture.CreateProcessorState();
        }

        /// <summary>
        /// Tries to identify the main program of various MS-DOS compilers, and if 
        /// successful, sets the DS register to the appropriate value.
        /// </summary>
        /// <returns></returns>
        public ImageSymbol FindMainAddress()
        {
            var diagSvc = services.RequireService<IDiagnosticsService>();
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

            program.SegmentMap.TryFindSegment(start, out ImageSegment segment);
            var image = segment.MemoryArea;
            var startOff = (uint)(start - image.BaseAddress);   // Offset into the Image of the initial CS:IP

            // Check the Turbo Pascal signatures first, since they involve only the
            // first 3 bytes, and false positives may be found with the others later
            if (locatePattern(image.Bytes,
                startOff, startOff + 5, pattBorl4on, out i))
            {
                // The first 5 bytes are a far call. Follow that call and
                // determine the version from that */
                var addrNew = ReadSegPtr(image, startOff + 1);
                init = (uint)(addrNew - image.BaseAddress);
                if (locatePattern(image.Bytes, init, init + 26, pattBorl4Init, out i))
                {
                    setState("ds", image.ReadLeUInt16(i + 1));
                    Debug.Print("Borland Pascal v4 detected\n");
                    chVendor = 't';                     /* Turbo */
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
                    chVendor = 't';                     /* Turbo */
                    chModel = 'p';                      /* Pascal */
                    chVersion = '5';                    /* Version 5 */
                    addrEntry = start;                  /* Code starts immediately */
                    goto gotVendor;                     /* Already have vendor */
                }
                else if (locatePattern(image.Bytes, init, init + 26, pattBorl7Init, out i))
                {
                    setState("ds", image.ReadLeUInt16(i + 1));
                    Debug.Print("Borland Pascal v7 detected");
                    chVendor = 't';                     /* Turbo */
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
                else if (MemoryArea.CompareArrays(image.Bytes, (int)startOff, pattTPasStart, pattTPasStart.Length))
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
                diagSvc.Inform("Borland C v3 runtime detected");
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
                return null;
            }

            gotVendor:
            ;
            //sSigName = string.Format("dcc{0}{1}{2}.sig",
            //        chVendor, /* Add vendor */
            //        chVersion, /* Add version */
            //        chModel); /* Add model */
            //Debug.Print("Signature file: {0}", sSigName);
            return ImageSymbol.Procedure(arch, addrEntry, "main", state: this.state);
        }

        private Address ReadSegPtr(MemoryArea mem, uint offset)
        {
            var off = mem.ReadLeUInt16(offset);
            var seg = mem.ReadLeUInt16(offset + 2);
            return Address.SegPtr(seg, off);
        }

        /* Search the source array between limits iMin and iMax for the pattern (length
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

        void setState(string regName, ushort val)
        {
            state.SetRegister(
                program.Architecture.GetRegister(regName),
                Constant.Word16(val));

        }

/*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   *\
*                                                            *
*   S t a r t   P a t t e r n s   ( V e n d o r    i d )     *
*                                                            *
\*  *   *   *   *   *   *   *   *   *   *   *   *   *   *   */
        static byte[] pattMsC5Start = new byte[] {
            0xB4, 0x30,         /* Mov ah, 30 */
            0xCD, 0x21,         /* int 21 (dos version number) */
            0x3C, 0x02,         /* cmp al, 2 */
            0x73, 0x02,         /* jnb $+4 */
            0xCD, 0x20,         /* int 20 (exit) */
            0xBF                /* Mov di, DSEG */
        };

        static byte[] pattMsC8Start = new byte[]
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

        static byte[] pattMsC8ComStart = new byte[]
        {
            0xB4, 0x30,         /* Mov ah, 30 */
            0xCD, 0x21,         /* int 21 (dos version number) */
            0x3C, 0x02,         /* cmp al, 2 */
            0x73, 0x01,         /* jnb $+3 */
            0xC3,               /* ret */
            0x8C, 0xDF          /* Mov di, ds */
        };

        static byte[] pattBorl2Start = new byte[]
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
        static byte[] pattMainSmall =
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
        const int OFFMAINMEDIUM = 13;

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
        const int OFFMAINCOMPACT = 21;

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
    }
}
