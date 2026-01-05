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

using Reko.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Motorola.M88k;

public static class Registers
{
    public static readonly RegisterStorage[] GpRegisters;
    public static readonly RegisterStorage?[] CrRegisters;
    public static readonly RegisterStorage?[] FcrRegisters;

    public static Dictionary<string, RegisterStorage> ByName { get; }
    public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }
    public static RegisterStorage PID { get; }
    public static RegisterStorage PSR { get; }
    public static RegisterStorage EPSR { get; }
    public static RegisterStorage SSBR { get; }
    public static RegisterStorage SXIP { get; }
    public static RegisterStorage SNIP { get; }
    public static RegisterStorage SFIP { get; }
    public static RegisterStorage VBR { get; }
    public static RegisterStorage DMTO { get; }
    public static RegisterStorage DMD0 { get; }
    public static RegisterStorage DMA0 { get; }
    public static RegisterStorage DMT1 { get; }
    public static RegisterStorage DMD1 { get; }
    public static RegisterStorage DMA1 { get; }
    public static RegisterStorage DMT2 { get; }
    public static RegisterStorage DMD2 { get; }
    public static RegisterStorage DMA2 { get; }
    public static RegisterStorage SRO { get; }
    public static RegisterStorage SRl { get; }
    public static RegisterStorage SR2 { get; }
    public static RegisterStorage SR3 { get; }
    public static RegisterStorage FPECR { get; }
    public static RegisterStorage FPHS1 { get; }
    public static RegisterStorage FPLS1 { get; }
    public static RegisterStorage FPHS2 { get; }
    public static RegisterStorage FPLS2 { get; }
    public static RegisterStorage FPPT { get; }
    public static RegisterStorage FPRH { get; }
    public static RegisterStorage FPRL { get; }
    public static RegisterStorage FPIT { get; }
    public static RegisterStorage FPSR { get; }
    public static RegisterStorage FPCR { get; }

    public static FlagGroupStorage C { get; }


    static Registers()
    {
        var factory = new StorageFactory();
        //rO ZERO
        //rl SUBROUTINE RETURN POINTER 
        //r2 I
        //            CALLED PROCEDURE PARAMETER REGISTERS 
        //r9 



        //rl0 - r13: CALLED PROCEDURE TEMPORARY REGISTERS 
        //r14 - Vector256: CAlliNG PROCEDURE  RESERVED REGISTERS 
        //r26 LINKER 
        //r27 LINKER 
        //r2B LINKER
        //r29 LINKER
        //r30 FRAME POINTER
        //r31 STACK POINTER

        GpRegisters = factory.RangeOfReg32(32, "r{0}");
        var crFactory = new StorageFactory(StorageDomain.SystemRegister);
        CrRegisters = new RegisterStorage[64];
        CrRegisters[0] = PID = crFactory.Reg32("PID"); // cr0 - PROCESSOR IDENTIFICATION REGISTER,
        CrRegisters[1] = PSR = crFactory.Reg32("PSR"); // cr1 - PROCESSOR STATUS REGISTER
        CrRegisters[2] = EPSR = crFactory.Reg32("EPSR"); // cr2 - EXCEPTION TIME PROCESSOR STATUS REGISTER
        CrRegisters[3] = SSBR = crFactory.Reg32("SSBR"); // cr3 - SHADOW SCOREBOARD REGISTER,
        CrRegisters[4] = SXIP = crFactory.Reg32("SXIP"); // cr4 - SHADOW EXECUTE INSTRUCTION POINTER I
        CrRegisters[5] = SNIP = crFactory.Reg32("SNIP"); // cr5 - SHADOW NEXT INSTRUCTION POINTER I
        CrRegisters[6] = SFIP = crFactory.Reg32("SFIP"); // cr6 - SHADOW FETCHED INSTRUCTION POINTER
        CrRegisters[7] = VBR = crFactory.Reg32("VBR"); // cr7 - VECTOR BASE REGISTER
        CrRegisters[8] = DMTO = crFactory.Reg32("DMTO"); // cr8 - TRANSACTION REGISTER 0
        CrRegisters[9] = DMD0 = crFactory.Reg32("DMD0"); // cr9 - DATA REGISTER 0
        CrRegisters[10] = DMA0 = crFactory.Reg32("DMA0"); // cr10 - ADDRESS REGISTER 0
        CrRegisters[11] = DMT1 = crFactory.Reg32("DMT1"); // cr11 - TRANSACTION REGISTER 1
        CrRegisters[12] = DMD1 = crFactory.Reg32("DMD1"); // cr12 - DATA REGISTER 1
        CrRegisters[13] = DMA1 = crFactory.Reg32("DMAl"); // cr13 - ADDRESS REGISTER 1
        CrRegisters[14] = DMT2 = crFactory.Reg32("DMT2"); // cr14 - TRANSACTION REGISTER 2
        CrRegisters[15] = DMD2 = crFactory.Reg32("DMD2"); // cr15 - DATA REGISTER Z
        CrRegisters[16] = DMA2 = crFactory.Reg32("DMA2"); // cr16 - ADDRESS REGISTER 2
        CrRegisters[17] = SRO = crFactory.Reg32("SRO"); // cr17 - SUPERVISOR STORAGE REGISTER 0
        CrRegisters[18] = SRl = crFactory.Reg32("SRl"); // cr18 - 2 SUPERVISOR STORAGE REGISTER 1
        CrRegisters[19] = SR2 = crFactory.Reg32("SR2"); // cr19 - SUPERVISOR STORAGE REGISTER 2
        CrRegisters[20] = SR3 = crFactory.Reg32("SR3"); // cr20 - SUPERVISOR STORAGE REGISTER 3

        FcrRegisters = new RegisterStorage[64];
        FcrRegisters[0] = FPECR = crFactory.Reg32("FPECR"); // ferO - FLOATING - POINT EXCEPTION CAUSE REGISTER
        FcrRegisters[1] = FPHS1 = crFactory.Reg32("FPHS1"); // fer1 - F.P.SOURCE 1 OPERAND HIGH REGISTER
        FcrRegisters[2] = FPLS1 = crFactory.Reg32("FPLS1"); // fer2 - F.P.SOURCE 1 OPERAND LOW REGISTER
        FcrRegisters[3] = FPHS2 = crFactory.Reg32("FPHS2"); // fer3 - F.P.SOURCE Z OPERAND HIGH REGISTER
        FcrRegisters[4] = FPLS2 = crFactory.Reg32("FPLS2"); // lcr4 - F.P.SOURCE Z OPERAND LOW REGISTER
        FcrRegisters[5] = FPPT = crFactory.Reg32("FPPT"); // fer5 - F.P.PRECISE OPERAliON TYPE REGISTER
        FcrRegisters[6] = FPRH = crFactory.Reg32("FPRH"); // fer6 - F.P.RESULT HIGH REGISTER er9 DMDO DATA REGISTER 0
        FcrRegisters[7] = FPRL = crFactory.Reg32("FPRL"); // fer7 - F.P.RESULT LOW REGISTER
        FcrRegisters[8] = FPIT = crFactory.Reg32("FPIT"); // fcr8 - F.P.IMPRECISE OPERATION TYPE REGISTER cr11 DMTlCO
        FcrRegisters[62] = FPSR = crFactory.Reg32("FPSR"); // fer62 - F.P.USER STATUS REGISTER 
        FcrRegisters[63] = FPCR = crFactory.Reg32("FPCR"); // fer63 - F.P.USER CONTROL REGISTER 

        C = new FlagGroupStorage(PSR, (uint) FlagM.CF, "C");

        ByName = factory.NamesToRegisters.Concat(crFactory.NamesToRegisters)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
        ByDomain = factory.DomainsToRegisters.Concat(crFactory.DomainsToRegisters)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}

[Flags]
public enum FlagM
{
    CF = 1 << 28,
}
