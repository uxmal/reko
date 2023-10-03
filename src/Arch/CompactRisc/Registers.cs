#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Reko.Arch.CompactRisc
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters { get; }
        public static MachineOperand[] RegisterPairs { get; }
        public static RegisterStorage r12L { get; }
        public static RegisterStorage r13L { get; }

        public static RegisterStorage[] ProcessorRegisters { get; }
        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg(16, i => $"r{i}", PrimitiveType.Word16);
            GpRegisters[12] = RegisterStorage.Reg32("r12", 12);
            GpRegisters[13] = RegisterStorage.Reg32("r13", 13);
            GpRegisters[14] = RegisterStorage.Reg32("ra", 14);
            GpRegisters[15] = RegisterStorage.Reg32("sp", 15);

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

            factory = new StorageFactory(StorageDomain.SystemRegister);

            var DBS = factory.Reg16("DBS");
            var DSR = factory.Reg16("DSR");
            var DCRL = factory.Reg16("DCRL");
            var DCRH = factory.Reg16("DCRH");
            var CAR0L = factory.Reg16("CAR0L");
            var CAR0H = factory.Reg16("CAR0H");
            var CAR1L = factory.Reg16("CAR1L");
            var CAR1H = factory.Reg16("CAR1H");
            var CFG = factory.Reg16("CFG");
            var PSR = factory.Reg16("PSR");
            var INTBASEL = factory.Reg16("INTBASEL");
            var INTBASEH = factory.Reg16("INTBASEH");
            var ISPL = factory.Reg16("ISPL");
            var ISPH = factory.Reg16("ISPH");
            var USPL = factory.Reg16("USPL");
            var USP = factory.Reg16("USP");

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
            ByName = GpRegisters
                .Concat(ProcessorRegisters)
                .ToDictionary(r => r.Name);
            ByDomain = factory.DomainsToRegisters;
        }
    }
}
