#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
#endregion

namespace Reko.Arch.NatSemi;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using Reko.Core;

public static class Registers
{
    static Registers()
    {
        var factory = new StorageFactory();
        GpRegisters = factory.RangeOfReg32(8, "r{0}");
        FpRegisters = factory.RangeOfReg64(8, "f{0}");

        PC = factory.Reg32("pc");
        SP1 = factory.Reg32("SP1");
        SP0 = factory.Reg32("SP0");
        FSR = factory.Reg16("FSR");
        TOS = factory.Reg32("tos");

        var sysregs = new StorageFactory(StorageDomain.SystemRegister);

        UPSR = sysregs.Reg32("upsr");
        DCR = sysregs.Reg32("dcr");
        BPC = sysregs.Reg32("bpc");
        DSR = sysregs.Reg32("dsr");
        CAR = sysregs.Reg32("car");
        FP = sysregs.Reg32("fp");
        SP = sysregs.Reg32("sp");
        SB = sysregs.Reg32("sb");
        USP = sysregs.Reg32("usp");
        CFG = sysregs.Reg32("cfg");
        PSR = sysregs.Reg32("psr");
        INTBASE = sysregs.Reg32("intbase");
        MOD = sysregs.Reg32("mod");

        MCR = sysregs.Reg32("mcr");
        MSR = sysregs.Reg32("msr");
        TEAR = sysregs.Reg32("tear");
        PTB0 = sysregs.Reg32("ptb0");
        PTB1 = sysregs.Reg32("ptb1");
        IVAR0 = sysregs.Reg32("ivar0");
        IVAR1 = sysregs.Reg32("ivar1");

        I = new FlagGroupStorage(PSR, (uint) FlagM.IF, "I");
        P = new FlagGroupStorage(PSR, (uint) FlagM.PF, "P");
        S = new FlagGroupStorage(PSR, (uint) FlagM.SF, "S");
        U = new FlagGroupStorage(PSR, (uint) FlagM.UF, "U");
        N = new FlagGroupStorage(PSR, (uint) FlagM.NF, "N");
        Z = new FlagGroupStorage(PSR, (uint) FlagM.ZF, "Z");
        F = new FlagGroupStorage(PSR, (uint) FlagM.FF, "F");
        V = new FlagGroupStorage(PSR, (uint) FlagM.VF, "V");
        L = new FlagGroupStorage(PSR, (uint) FlagM.LF, "L");
        T = new FlagGroupStorage(PSR, (uint) FlagM.TF, "T");
        C = new FlagGroupStorage(PSR, (uint) FlagM.CF, "C");
        CF = new FlagGroupStorage(PSR, (uint) (FlagM.CF | FlagM.FF), "CF");
        LNZ = new FlagGroupStorage(PSR, (uint) (FlagM.LF | FlagM.NF | FlagM.ZF), "LNZ");
        LZ = new FlagGroupStorage(PSR, (uint) (FlagM.LF | FlagM.ZF), "LZ");
        NZ = new FlagGroupStorage(PSR, (uint) (FlagM.NF | FlagM.ZF), "NZ");

        RegistersByName = factory.NamesToRegisters;
        RegistersByDomain = factory.DomainsToRegisters;
    }

    public static RegisterStorage PC { get; }   //  Program Counter
    public static RegisterStorage SP1 { get; }  // User Stack Pointer
    public static RegisterStorage SP0 { get; }  // Interrupt Stack Pointer
    public static RegisterStorage FSR { get; }  // Floating Point Status Register
    public static RegisterStorage TOS { get; }  // Top of Stack Register pseudo-register

    // System registers
    public static RegisterStorage UPSR { get; } // User PSR
    public static RegisterStorage DCR { get; } // Debug Condition Register
    public static RegisterStorage BPC { get; } // Breakpoint Program Counter
    public static RegisterStorage DSR { get; } // Debug Status Register
    public static RegisterStorage CAR { get; } // Compare Address Register
    public static RegisterStorage FP { get; } // Frame Pointer
    public static RegisterStorage SP { get; } // Stack Pointer
    public static RegisterStorage SB { get; } // Static Base Register
    public static RegisterStorage USP { get; } // User Stack Pointer(SP1)
    public static RegisterStorage CFG { get; } // Configuration Register
    public static RegisterStorage PSR { get; } // Processor Status Register
    public static RegisterStorage INTBASE { get; } // Interrupt Base Register
    public static RegisterStorage MOD { get; } // Module Register

    // MMU registers
    public static RegisterStorage MCR { get; }  //     Memory Management Control Register
    public static RegisterStorage MSR { get; }  // Memory Management Status Register
    public static RegisterStorage TEAR { get; }  // Translation Exception Address Register
    public static RegisterStorage PTB0 { get; }  // Page Table Base Register 0
    public static RegisterStorage PTB1 { get; }  // Page Table Base Register 1
    public static RegisterStorage IVAR0 { get; }  // Invalidate Virtual Address 0
    public static RegisterStorage IVAR1 { get; }  // Invalidate Virtual Address 1

    public static RegisterStorage[] GpRegisters { get; }
    public static RegisterStorage[] FpRegisters { get; }
    
    public static FlagGroupStorage I { get; }
    public static FlagGroupStorage P { get; }
    public static FlagGroupStorage S { get; }
    public static FlagGroupStorage U { get; }
    public static FlagGroupStorage N { get; }
    public static FlagGroupStorage Z { get; }
    public static FlagGroupStorage F { get; }
    public static FlagGroupStorage V { get; }
    public static FlagGroupStorage L { get; }
    public static FlagGroupStorage T { get; }
    public static FlagGroupStorage C { get; }

    public static FlagGroupStorage CF { get; }
    public static FlagGroupStorage LNZ { get; }
    public static FlagGroupStorage LZ { get; }
    public static FlagGroupStorage NZ { get; }


    public static Dictionary<string, RegisterStorage> RegistersByName { get; }
    public static Dictionary<StorageDomain, RegisterStorage> RegistersByDomain { get; }
}

[Flags]
public enum FlagM
{
    IF = 0x800,
    PF = 0x400,
    SF = 0x200,
    UF = 0x100,
    NF = 0x80,
    ZF = 0x40,
    FF = 0x20,
    VF = 0x10,
    LF = 4,
    TF = 2,
    CF = 1,
}
