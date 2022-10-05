#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Avr.Avr32
{
    public static class Registers
    {
        static Registers()
        {
            var factory = new StorageFactory();
            var regs = factory.RangeOfReg32(16, "r{0}");
            regs[13] = RegisterStorage.Reg32("sp", regs[13].Number);
            regs[14] = RegisterStorage.Reg32("lr", regs[14].Number);
            regs[15] = RegisterStorage.Reg32("pc", regs[15].Number);
            GpRegisters = regs;
            ByName = regs.ToDictionary(r => r.Name);
            sp = regs[13];
            pc = regs[15];
            ByDomain = regs.ToDictionary(r => r.Domain);

            factory = new StorageFactory(StorageDomain.Register + 0x100);
            sr = factory.Reg32(nameof(sr));
            evba = factory.Reg32(nameof(evba));
            acba = factory.Reg32(nameof(acba));
            /*
            0 0 SR Status Register RA RB
            1 4 EVBA Exception Vector Base Address RA RB
            2 8 ACBA Application Call Base Address RA RB
            3 12 CPUCR CPU Control Register RA RB
            4 16 ECR Exception Cause Register OA OB
            5 20 RSR_SUP Return Status Register for Supervisor context UA RB
            6 24 RSR_INT0 Return Status Register for INT 0 context UA RB
            7 28 RSR_INT1 Return Status Register for INT 1 context UA RB
            8 32 RSR_INT2 Return Status Register for INT 2 context UA RB
            9 36 RSR_INT3 Return Status Register for INT 3 context UA RB
            10 40 RSR_EX Return Status Register for Exception context UA RB
            11 44 RSR_NMI Return Status Register for NMI context UA RB
            12 48 RSR_DBG Return Status Register for Debug Mode OA OB
            13 52 RAR_SUP Return Address Register for Supervisor context UA RB
            14 56 RAR_INT0 Return Address Register for INT 0 context UA RB
            15 60 RAR_INT1 Return Address Register for INT 1 context UA RB
            16 64 RAR_INT2 Return Address Register for INT 2 context UA RB
            17 68 RAR_INT3 Return Address Register for INT 3 context UA RB
            18 72 RAR_EX Return Address Register for Exception context UA RB
            19 76 RAR_NMI Return Address Register for NMI context UA RB
            20 80 RAR_DBG Return Address Register for Debug Mode OA OB
            21 84 JECR Java Exception Cause Register OA OB
            22 88 JOSP Java Operand Stack Pointer OA OB
            23 92 JAVA_LV0 Java Local Variable 0 OA OB
            16
            32000D–04/2011
            AVR32
            24 96 JAVA_LV1 Java Local Variable 1 OA OB
            25 100 JAVA_LV2 Java Local Variable 2 OA OB
            26 104 JAVA_LV3 Java Local Variable 3 OA OB
            27 108 JAVA_LV4 Java Local Variable 4 OA OB
            28 112 JAVA_LV5 Java Local Variable 5 OA OB
            29 116 JAVA_LV6 Java Local Variable 6 OA OB
            30 120 JAVA_LV7 Java Local Variable 7 OA OB
            31 124 JTBA Java Trap Base Address OA OB
            32 128 JBCR Java Write Barrier Control Register OA OB
            33-63 132-252 Reserved Reserved for future use - -
            64 256 CONFIG0 Configuration register 0 RA RB
            65 260 CONFIG1 Configuration register 1 RA RB
            66 264 COUNT Cycle Counter register RA RB
            67 268 COMPARE Compare register RA RB
            68 272 TLBEHI MMU TLB Entry High OA OB
            69 276 TLBELO MMU TLB Entry Low OA OB
            70 280 PTBR MMU Page Table Base Register OA OB
            71 284 TLBEAR MMU TLB Exception Address Register OA OB
            72 288 MMUCR MMU Control Register OA OB
            73 292 TLBARLO MMU TLB Accessed Register Low OA OB
            74 296 TLBARHI MMU TLB Accessed Register High OA OB
            75 300 PCCNT Performance Clock Counter OA OB
            76 304 PCNT0 Performance Counter 0 OA OB
            77 308 PCNT1 Performance Counter 1 OA OB
            78 312 PCCR Performance Counter Control Register OA OB
            79 316 BEAR Bus Error Address Register OA OB
            80 320 MPUAR0 MPU Address Register region 0 OA OB
            81 324 MPUAR1 MPU Address Register region 1 OA OB
            82 328 MPUAR2 MPU Address Register region 2 OA OB
            83 332 MPUAR3 MPU Address Register region 3 OA OB
            84 336 MPUAR4 MPU Address Register region 4 OA OB
            85 340 MPUAR5 MPU Address Register region 5 OA OB
            86 344 MPUAR6 MPU Address Register region 6 OA OB
            87 348 MPUAR7 MPU Address Register region 7 OA OB
            88 352 MPUPSR0 MPU Privilege Select Register region 0 OA OB
            89 356 MPUPSR1 MPU Privilege Select Register region 1 OA OB
            Table 2-7. System Registers (Continued)
            Reg # Address Name Function Compliance
            17
            32000D–04/2011
            AVR32
            SR- Status Register
            The Status Register is mapped into the system register space. This allows it to be loaded into
            the register file to be modified, or to be stored to memory. The Status Register is described in
            detail in Section 2.10 “The Status Register” on page 11.
            EVBA - Exception Vector Base Address
            This register contains a pointer to the exception routines. All exception routines start at this
            address, or at a defined offset relative to the address. Special alignment requirements may
            apply for EVBA, depending on the implementation of the interrupt controller. Exceptions are
            described in detail in Section 8. “Event Processing” on page 63.
            ACBA - Application Call Base Address
            Pointer to the start of a table of function pointers. Subroutines can thereby be called by the compact acall instruction. This facilitates efficient reuse of code. Keeping this pointer as a register
            facilitates multiple function pointer tables. ACBA is a full 32 bit register, but the lowest two bits
            90 360 MPUPSR2 MPU Privilege Select Register region 2 OA OB
            91 364 MPUPSR3 MPU Privilege Select Register region 3 OA OB
            92 368 MPUPSR4 MPU Privilege Select Register region 4 OA OB
            93 372 MPUPSR5 MPU Privilege Select Register region 5 OA OB
            94 376 MPUPSR6 MPU Privilege Select Register region 6 OA OB
            95 380 MPUPSR7 MPU Privilege Select Register region 7 OA OB
            96 384 MPUCRA MPU Cacheable Register A OA OB
            97 388 MPUCRB MPU Cacheable Register B OA OB
            98 392 MPUBRA MPU Bufferable Register A OA OB
            99 396 MPUBRB MPU Bufferable Register B OA OB
            100 400 MPUAPRA MPU Access Permission Register A OA OB
            101 404 MPUAPRB MPU Access Permission Register B OA OB
            102 408 MPUCR MPU Control Register OA OB
            103 412 SS_STATUS Secure State Status Register OA OB
            104 416 SS_ADRF Secure State Address Flash Register OA OB
            105 420 SS_ADRR Secure State Address RAM Register OA OB
            106 424 SS_ADR0 Secure State Address 0 Register OA OB
            107 428 SS_ADR1 Secure State Address 1 Register OA OB
            108 432 SS_SP_SYS Secure State Stack Pointer System Register OA OB
            109 436 SS_SP_APP Secure State Stack Pointer Application Register OA OB
            110 440 SS_RAR Secure State Return Address Register OA OB
            111 444 SS_RSR Secure State Return Status Register OA OB
            112-191 448-764 Reserved Reserved for future use - -
            192-255 768-1020 IMPL IMPLEMENTATION DEFINED - -
             */
        }

        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }
        public static RegisterStorage[] GpRegisters { get; }
        public static RegisterStorage sp { get; }
        public static RegisterStorage pc { get; }
        public static RegisterStorage sr { get; }
        public static RegisterStorage evba { get; }
        public static RegisterStorage acba { get; }
    }

    public enum FlagM
    {
        LF = 0x20,
        QF = 0x10,
        VF = 0x08,
        NF = 0x04,
        ZF = 0x02,
        CF = 0x01
    }
}
