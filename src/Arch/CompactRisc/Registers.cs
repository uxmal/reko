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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.CompactRisc
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters { get; }
        public static MachineOperand[] RegisterPairs { get; }

        public static RegisterStorage ra { get; }
        public static RegisterStorage r12L { get; }
        public static RegisterStorage r13L { get; }

        public static RegisterStorage[] ProcessorRegisters { get; }


        public static RegisterStorage PSR { get; }
        public static FlagGroupStorage I { get; }
        public static FlagGroupStorage P { get; }
        public static FlagGroupStorage E { get; }
        public static FlagGroupStorage N { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage F { get; }
        public static FlagGroupStorage U { get; }
        public static FlagGroupStorage L { get; }
        public static FlagGroupStorage T { get; }
        public static FlagGroupStorage C { get; }

        public static FlagGroupStorage CF { get; }
        public static FlagGroupStorage LNZ { get; }
        public static FlagGroupStorage LZ { get; }
        public static FlagGroupStorage NZ { get; }


        public static RegisterStorage SVC { get; }
        public static RegisterStorage DVZ { get; }
        public static RegisterStorage FLG { get; }
        public static RegisterStorage BPT { get; }
        public static RegisterStorage TRC { get; }
        public static RegisterStorage UND { get; }
        public static RegisterStorage IAD { get; }
        public static RegisterStorage DBG { get; }
        public static RegisterStorage ISE { get; }

        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg(16, i => $"r{i}", PrimitiveType.Word16);
            GpRegisters[12] = factory.Replace(GpRegisters[12], RegisterStorage.Reg32("r12", 12));
            GpRegisters[13] = factory.Replace(GpRegisters[13], RegisterStorage.Reg32("r13", 13));
            GpRegisters[14] = ra = factory.Replace(GpRegisters[14], RegisterStorage.Reg32("ra", 14));
            GpRegisters[15] = factory.Replace(GpRegisters[15], RegisterStorage.Reg32("sp", 15));

            r12L = new RegisterStorage("r12_l", GpRegisters[12].Number, 0, PrimitiveType.Word16);
            r13L = new RegisterStorage("r13_l", GpRegisters[13].Number, 0, PrimitiveType.Word16);

            RegisterPairs = new MachineOperand[]
            {
                new SequenceStorage(GpRegisters[1],GpRegisters[0]),
                new SequenceStorage(GpRegisters[2],GpRegisters[1]),
                new SequenceStorage(GpRegisters[3],GpRegisters[2]),
                new SequenceStorage(GpRegisters[4],GpRegisters[3]),

                new SequenceStorage(GpRegisters[5],GpRegisters[4]),
                new SequenceStorage(GpRegisters[6],GpRegisters[5]),
                new SequenceStorage(GpRegisters[7],GpRegisters[6]),
                new SequenceStorage(GpRegisters[8],GpRegisters[7]),

                new SequenceStorage(GpRegisters[9],GpRegisters[8]),
                new SequenceStorage(GpRegisters[10],GpRegisters[9]),
                new SequenceStorage(GpRegisters[11],GpRegisters[10]),
                new SequenceStorage(r12L, GpRegisters[11]),

                GpRegisters[12],
                GpRegisters[13],
                GpRegisters[14],
                GpRegisters[15],
            };

            var sysFactory = new StorageFactory(StorageDomain.SystemRegister);

            var DBS = sysFactory.Reg16("DBS");
            var DSR = sysFactory.Reg16("DSR");
            var DCRL = sysFactory.Reg16("DCRL");
            var DCRH = sysFactory.Reg16("DCRH");
            var CAR0L = sysFactory.Reg16("CAR0L");
            var CAR0H = sysFactory.Reg16("CAR0H");
            var CAR1L = sysFactory.Reg16("CAR1L");
            var CAR1H = sysFactory.Reg16("CAR1H");
            var CFG = sysFactory.Reg16("CFG");
            PSR = sysFactory.Reg16("PSR");
            var INTBASEL = sysFactory.Reg16("INTBASEL");
            var INTBASEH = sysFactory.Reg16("INTBASEH");
            var ISPL = sysFactory.Reg16("ISPL");
            var ISPH = sysFactory.Reg16("ISPH");
            var USPL = sysFactory.Reg16("USPL");
            var USP = sysFactory.Reg16("USP");

            SVC = sysFactory.Reg("SVC", PrimitiveType.Byte);
            DVZ = sysFactory.Reg("DVZ", PrimitiveType.Byte);
            FLG = sysFactory.Reg("FLG", PrimitiveType.Byte);
            BPT = sysFactory.Reg("BPT", PrimitiveType.Byte);
            TRC = sysFactory.Reg("TRC", PrimitiveType.Byte);
            UND = sysFactory.Reg("UND", PrimitiveType.Byte);
            IAD = sysFactory.Reg("IAD", PrimitiveType.Byte);
            DBG = sysFactory.Reg("DBG", PrimitiveType.Byte);
            ISE = sysFactory.Reg("ISE", PrimitiveType.Byte);

            ProcessorRegisters = new RegisterStorage[16]
            {
                DBS,
                DSR,
                DCRL,
                DCRH,
                CAR0L,
                CAR0H,
                CAR1L,
                CAR1H,
                CFG,
                PSR,
                INTBASEL,
                INTBASEH,
                ISPL,
                ISPH,
                USPL,
                USP,
            };

            I = new FlagGroupStorage(PSR, (uint) FlagM.IF, nameof(I));
            P = new FlagGroupStorage(PSR, (uint) FlagM.PF, nameof(P));
            E = new FlagGroupStorage(PSR, (uint) FlagM.EF, nameof(E));
            N = new FlagGroupStorage(PSR, (uint) FlagM.NF, nameof(N));
            Z = new FlagGroupStorage(PSR, (uint) FlagM.ZF, nameof(Z));
            F = new FlagGroupStorage(PSR, (uint) FlagM.FF, nameof(F));
            U = new FlagGroupStorage(PSR, (uint) FlagM.UF, nameof(U));
            L = new FlagGroupStorage(PSR, (uint) FlagM.LF, nameof(L));
            T = new FlagGroupStorage(PSR, (uint) FlagM.TF, nameof(T));
            C = new FlagGroupStorage(PSR, (uint) FlagM.CF, nameof(C));

            CF = new FlagGroupStorage(PSR, (uint) (FlagM.CF | FlagM.FF), nameof(CF));
            LNZ = new FlagGroupStorage(PSR, (uint) (FlagM.LF | FlagM.NF | FlagM.ZF), nameof(LNZ));
            LZ = new FlagGroupStorage(PSR, (uint) (FlagM.LF | FlagM.ZF), nameof(LZ));
            NZ = new FlagGroupStorage(PSR, (uint) (FlagM.NF | FlagM.ZF), nameof(NZ));

            ByName = GpRegisters
                    .Concat(ProcessorRegisters)
                    .ToDictionary(r => r.Name);
            ByDomain = factory.DomainsToRegisters;
        }
    }

    [Flags]
    public enum FlagM
    {
        IF = 0x800,
        PF = 0x400,
        EF = 0x200,
        NF = 0x080,
        ZF = 0x040,
        FF = 0x020,
        UF = 0x008,
        LF = 0x004,
        TF = 0x002,
        CF = 0x001,
    }
}
