/*
 *$Log:	fixwild.c,v $
 * Revision 1.10  93/10/28  11:10:10  emmerik
 * Addressing mode [reg+nnnn] is now wildcarded
 * 
 * Revision 1.9  93/10/26  13:40:11  cifuente
 * op0F(byte pat[])
 * 
 * Revision 1.8  93/10/26  13:01:29  emmerik
 * Completed the odd opcodes, like 0F XX and F7. Result: some library
 * functions that were not recognised before are recognised now.
 * 
 * Revision 1.7  93/10/11  11:37:01  cifuente
 * First walk of HIGH_LEVEL icodes.
 * 
 * Revision 1.6  93/10/01  14:36:21  emmerik
 * Added $ log, and made independant of dcc.h
 * 
 *
 */

/*  *   *   *   *   *   *   *   *   *   *   *  *\
*                                               *
*           Fix Wild Cards Code                 *
*                                               *
\*  *   *   *   *   *   *   *   *   *   *   *  */

class FixWild
{
    private const int PATLEN = 23;
    private const byte WILD = 0xF4;

    private const bool TRUE = true;
    private const bool FALSE = false;

    int pc;                              /* Indexes into pat[] */

    /* prototypes */
    //static bool ModRM(byte[] pat);              /* Handle the mod/rm byte */
    //static bool TwoWild(byte[] pat);            /* Make the next 2 bytes wild */
    //static bool FourWild(byte[] pat);           /* Make the next 4 bytes wild */
    //void fixWildCards(byte[] pat);       /* Main routine */


    /* Handle the mod/rm case. Returns true if pattern exhausted */
    bool ModRM(byte[] pat)
    {
        byte op;

        /* A standard mod/rm byte follows opcode */
        op = pat[pc++];                         /* The mod/rm byte */
        if (pc >= PATLEN) return TRUE;          /* Skip Mod/RM */
        switch (op & 0xC0)
        {
        case 0x00:                          /* [reg] or [nnnn] */
            if ((op & 0xC7) == 6)
            {
                /* Uses [nnnn] address mode */
                pat[pc++] = WILD;
                if (pc >= PATLEN) return TRUE;
                pat[pc++] = WILD;
                if (pc >= PATLEN) return TRUE;
            }
            break;
        case 0x40:                          /* [reg + nn] */
            if ((pc += 1) >= PATLEN) return TRUE;
            break;
        case 0x80:                          /* [reg + nnnn] */
            /* Possibly just a long constant offset from a register,
                but often will be an index from a variable */
            pat[pc++] = WILD;
            if (pc >= PATLEN) return TRUE;
            pat[pc++] = WILD;
            if (pc >= PATLEN) return TRUE;
            break;
        case 0xC0:                          /* reg */
            break;
        }
        return FALSE;
    }

    /* Change the next two bytes to wild cards */
    bool TwoWild(byte[] pat)
    {
        pat[pc++] = WILD;
        if (pc >= PATLEN) return TRUE;      /* Pattern exhausted */
        pat[pc++] = WILD;
        if (pc >= PATLEN) return TRUE;
        return FALSE;
    }

    /* Change the next four bytes to wild cards */
    bool FourWild(byte[] pat)
    {
        TwoWild(pat);
        return TwoWild(pat);
    }

    /* Chop from the current point by wiping with zeroes. Can't rely on anything
        after this point */
    void chop(byte[] pat)
    {
        if (pc >= PATLEN) return;               /* Could go negative otherwise */
        for (int i = pc; i < pat.Length; ++i)
            pat[i] = 0;
    }

    bool op0F(byte[] pat)
    {
        /* The two byte opcodes */
        byte op = pat[pc++];
        switch (op & 0xF0)
        {
        case 0x00:              /* 00 - 0F */
            if (op >= 0x06)     /* Clts, Invd, Wbinvd */
                return false;
            else
            {
                /* Grp 6, Grp 7, LAR, LSL */
                return ModRM(pat);
            }
        case 0x20:              /* Various funnies, all with Mod/RM */
            return ModRM(pat);

        case 0x80:
            pc += 2;            /* Word displacement cond jumps */
            return false;

        case 0x90:              /* Byte set on condition */
            return ModRM(pat);

        case 0xA0:
            switch (op)
            {
            case 0xA0:      /* Push FS */
            case 0xA1:      /* Pop  FS */
            case 0xA8:      /* Push GS */
            case 0xA9:      /* Pop  GS */
                return false;

            case 0xA3:      /* Bt  Ev,Gv */
            case 0xAB:      /* Bts Ev,Gv */
                return ModRM(pat);

            case 0xA4:      /* Shld EvGbIb */
            case 0xAC:      /* Shrd EvGbIb */
                if (ModRM(pat))
                    return true;
                pc++;       /* The #num bits to shift */
                return false;

            case 0xA5:      /* Shld EvGb CL */
            case 0xAD:      /* Shrd EvGb CL */
                return ModRM(pat);

            default:        /* CmpXchg, Imul */
                return ModRM(pat);
            }

        case 0xB0:
            if (op == 0xBA)
            {
                /* Grp 8: bt/bts/btr/btc Ev,#nn */
                if (ModRM(pat))
                    return true;
                pc++;           /* The #num bits to shift */
                return false;
            }
            return ModRM(pat);

        case 0xC0:
            if (op <= 0xC1)
            {
                /* Xadd */
                return ModRM(pat);
            }
            /* Else BSWAP */
            return false;

        default:
            return false;       /* Treat as double byte opcodes */
        }
    }

    /* Scan through the instructions in pat[], looking for opcodes that may
        have operands that vary with different instances. For example, load and
        store from statics, calls to other procs (even relative calls; they may
        call procs loaded in a different order, etc).
        Note that this procedure is architecture specific, and assumes the
        processor is in 16 bit address mode (real mode).
        PATLEN bytes are scanned.
    */
    public void fixWildCards(byte[] pat)
    {
        byte op;
        int quad;
        byte intArg;

        pc = 0;
        while (pc < PATLEN)
        {
            op = pat[pc++];
            if (pc >= PATLEN) return;

            quad = op & 0xC0;                   /* Quadrant of the opcode map */
            if (quad == 0)
            {
                /* Arithmetic group 00-3F */

                if ((op & 0xE7) == 0x26)        /* First check for the odds */
                {
                    /* Segment prefix: treat as 1 byte opcode */
                    continue;
                }
                if (op == 0x0F)                 /* 386 2 byte opcodes */
                {
                    if (op0F(pat)) return;
                    continue;
                }

                if ((op & 0x04) != 0)
                {
                    /* All these are constant. Work out the instr length */
                    if ((op & 2) != 0)
                    {
                        /* Push, pop, other 1 byte opcodes */
                        continue;
                    }
                    else
                    {
                        if ((op & 1) != 0)
                        {
                            /* Word immediate operands */
                            pc += 2;
                            continue;
                        }
                        else
                        {
                            /* Byte immediate operands */
                            pc++;
                            continue;
                        }
                    }
                }
                else
                {
                    /* All these have mod/rm bytes */
                    if (ModRM(pat)) return;
                    continue;
                }
            }
            else if (quad == 0x40)
            {
                if ((op & 0x60) == 0x40)
                {
                    /* 0x40 - 0x5F -- these are inc, dec, push, pop of general
                        registers */
                    continue;
                }
                else
                {
                    /* 0x60 - 0x70 */
                    if ((op & 0x10) != 0)
                    {
                        /* 70-7F 2 byte jump opcodes */
                        pc++;
                        continue;
                    }
                    else
                    {
                        /* Odds and sods */
                        switch (op)
                        {
                        case 0x60:  /* pusha */
                        case 0x61:  /* popa */
                        case 0x64:  /* overrides */
                        case 0x65:
                        case 0x66:
                        case 0x67:
                        case 0x6C:  /* insb DX */
                        case 0x6E:  /* outsb DX */
                            continue;

                        case 0x62:  /* bound */
                            pc += 4;
                            continue;

                        case 0x63:  /* arpl */
                            if (TwoWild(pat)) return;
                            continue;

                        case 0x68:  /* Push byte */
                        case 0x6A:  /* Push byte */
                        case 0x6D:  /* insb port */
                        case 0x6F:  /* outsb port */
                            /* 2 byte instr, no wilds */
                            pc++;
                            continue;

                        }
                    }

                }
            }
            else if (quad == 0x80)
            {
                switch (op & 0xF0)
                {
                case 0x80:          /* 80 - 8F */
                    /* All have a mod/rm byte */
                    if (ModRM(pat)) return;
                    /* These also have immediate values */
                    switch (op)
                    {
                    case 0x80:
                    case 0x83:
                        /* One byte immediate */
                        pc++;
                        continue;

                    case 0x81:
                        /* Immediate 16 bit values might be constant, but
                            also might be relocatable. Have to make them
                            wild */
                        if (TwoWild(pat)) return;
                        continue;
                    }
                    continue;
                case 0x90:          /* 90 - 9F */
                    if (op == 0x9A)
                    {
                        /* far call */
                        if (FourWild(pat)) return;
                        continue;
                    }
                    /* All others are 1 byte opcodes */
                    continue;
                case 0xA0:          /* A0 - AF */
                    if ((op & 0x0C) == 0)
                    {
                        /* mov al/ax to/from [nnnn] */
                        if (TwoWild(pat)) 
                            return;
                    }
                    else if ((op & 0xFE) == 0xA8)
                    {
                        /* test al,#byte or test ax,#word */
                        if ((op & 1)!=0)
                            pc += 2;
                        else
                            pc += 1;
                    }
                    continue;
                case 0xB0:          /* B0 - BF */
                    {
                        if ((op & 8) != 0)
                        {
                            /* mov reg, #16 */
                            /* Immediate 16 bit values might be constant, but also
                                might be relocatable. For now, make them wild */
                            if (TwoWild(pat)) return;
                        }
                        else
                        {
                            /* mov reg, #8 */
                            pc++;
                        }
                        continue;
                    }
                }
            }
            else
            {
                /* In the last quadrant of the op code table */
                switch (op)
                {
                case 0xC0:          /* 386: Rotate group 2 ModRM, byte, #byte */
                case 0xC1:          /* 386: Rotate group 2 ModRM, word, #byte */
                    if (ModRM(pat)) return;
                    /* Byte immediate value follows ModRM */
                    pc++;
                    continue;

                case 0xC3:          /* Return */
                case 0xCB:          /* Return far */
                    chop(pat);
                    return;
                case 0xC2:          /* Ret nnnn */
                case 0xCA:          /* Retf nnnn */
                    pc += 2;
                    chop(pat);
                    return;

                case 0xC4:          /* les Gv, Mp */
                case 0xC5:          /* lds Gv, Mp */
                    if (ModRM(pat)) return;
                    continue;

                case 0xC6:          /* Mov ModRM, #nn */
                    if (ModRM(pat)) return;
                    /* Byte immediate value follows ModRM */
                    pc++;
                    continue;
                case 0xC7:          /* Mov ModRM, #nnnn */
                    if (ModRM(pat)) return;
                    /* Word immediate value follows ModRM */
                    /* Immediate 16 bit values might be constant, but also
                        might be relocatable. For now, make them wild */
                    if (TwoWild(pat)) return;
                    continue;

                case 0xC8:          /* Enter Iw, Ib */
                    pc += 3;        /* Constant word, byte */
                    continue;
                case 0xC9:          /* Leave */
                    continue;

                case 0xCC:          /* Int 3 */
                    continue;

                case 0xCD:          /* Int nn */
                    intArg = pat[pc++];
                    if ((intArg >= 0x34) && (intArg <= 0x3B))
                    {
                        /* Borland/Microsoft FP emulations */
                        if (ModRM(pat)) return;
                    }
                    continue;

                case 0xCE:          /* Into */
                    continue;

                case 0xCF:          /* Iret */
                    continue;

                case 0xD0:          /* Group 2 rotate, byte, 1 bit */
                case 0xD1:          /* Group 2 rotate, word, 1 bit */
                case 0xD2:          /* Group 2 rotate, byte, CL bits */
                case 0xD3:          /* Group 2 rotate, word, CL bits */
                    if (ModRM(pat)) return;
                    continue;

                case 0xD4:          /* Aam */
                case 0xD5:          /* Aad */
                case 0xD7:          /* Xlat */
                    continue;

                case 0xD8:
                case 0xD9:
                case 0xDA:
                case 0xDB:          /* Esc opcodes */
                case 0xDC:          /* i.e. floating point */
                case 0xDD:          /* coprocessor calls */
                case 0xDE:
                case 0xDF:
                    if (ModRM(pat)) return;
                    continue;

                case 0xE0:          /* Loopne */
                case 0xE1:          /* Loope */
                case 0xE2:          /* Loop */
                case 0xE3:          /* Jcxz */
                    pc++;           /* Short jump offset */
                    continue;

                case 0xE4:          /* in  al,nn */
                case 0xE6:          /* out nn,al */
                    pc++;
                    continue;

                case 0xE5:          /* in ax,nn */
                case 0xE7:          /* in nn,ax */
                    pc += 2;
                    continue;

                case 0xE8:          /* Call rel */
                    if (TwoWild(pat)) return;
                    continue;
                case 0xE9:          /* Jump rel, unconditional */
                    if (TwoWild(pat)) return;
                    chop(pat);
                    return;
                case 0xEA:          /* Jump abs */
                    if (FourWild(pat)) return;
                    chop(pat);
                    return;
                case 0xEB:          /* Jmp short unconditional */
                    pc++;
                    chop(pat);
                    return;

                case 0xEC:          /* In  al,dx */
                case 0xED:          /* In  ax,dx */
                case 0xEE:          /* Out dx,al */
                case 0xEF:          /* Out dx,ax */
                    continue;

                case 0xF0:          /* Lock */
                case 0xF2:          /* Repne */
                case 0xF3:          /* Rep/repe */
                case 0xF4:          /* Halt */
                case 0xF5:          /* Cmc */
                case 0xF8:          /* Clc */
                case 0xF9:          /* Stc */
                case 0xFA:          /* Cli */
                case 0xFB:          /* Sti */
                case 0xFC:          /* Cld */
                case 0xFD:          /* Std */
                    continue;

                case 0xF6:          /* Group 3 byte test/not/mul/div */
                case 0xF7:          /* Group 3 word test/not/mul/div */
                case 0xFE:          /* Inc/Dec group 4 */
                    if (ModRM(pat)) return;
                    continue;

                case 0xFF:          /* Group 5 Inc/Dec/Call/Jmp/Push */
                    /* Most are like standard ModRM */
                    if (ModRM(pat)) return;
                    continue;

                default:            /* Rest are single byte opcodes */
                    continue;
                }
            }
        }
    }
}

