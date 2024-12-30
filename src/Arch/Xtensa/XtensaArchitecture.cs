#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Arch.Xtensa
{
    public class XtensaArchitecture : ProcessorArchitecture
    {

        private static readonly RegisterStorage[] aregs = new[]
        {
            Registers.a0 ,
            Registers.a1 ,
            Registers.a2 ,
            Registers.a3 ,
            Registers.a4 ,
            Registers.a5 ,
            Registers.a6 ,
            Registers.a7 ,
            Registers.a8 ,
            Registers.a9 ,
            Registers.a10,
            Registers.a11,
            Registers.a12,
            Registers.a13,
            Registers.a14,
            Registers.a15,
        };

        private static readonly RegisterStorage[] bregs = new[]
        {
            Registers.b0 ,
            Registers.b1 ,
            Registers.b2 ,
            Registers.b3 ,
            Registers.b4 ,
            Registers.b5 ,
            Registers.b6 ,
            Registers.b7 ,
            Registers.b8 ,
            Registers.b9 ,
            Registers.b10,
            Registers.b11,
            Registers.b12,
            Registers.b13,
            Registers.b14,
            Registers.b15,
        };

        private static readonly RegisterStorage[] fregs = new[]
        {
            Registers.f0 ,
            Registers.f1 ,
            Registers.f2 ,
            Registers.f3 ,
            Registers.f4 ,
            Registers.f5 ,
            Registers.f6 ,
            Registers.f7 ,
            Registers.f8 ,
            Registers.f9 ,
            Registers.f10,
            Registers.f11,
            Registers.f12,
            Registers.f13,
            Registers.f14,
            Registers.f15,
        };

        private static readonly RegisterStorage[] mac16regs = new RegisterStorage[4]
        {
            Registers.mr0,
            Registers.mr1,
            Registers.mr2,
            Registers.mr3,
        };

        private static readonly RegisterStorage[] allRegs =
            aregs.Concat(bregs).Concat(fregs).ToArray();


        private static readonly Dictionary<int, RegisterStorage> sregs = new Dictionary<int, RegisterStorage>
        {
            { 0x00, Registers.LBEG },
            { 0x01, Registers.LEND },
            { 0x02, Registers.LCOUNT },
            { 0x03, Registers.SAR },
            { 0x05, RegisterStorage.Reg32("LITBASE", 0x105) },
            { 0x0C, Registers.SCOMPARE1 },
            { 0x10, Registers.ACCLO },
            { 0x11, Registers.ACCHI },

            { 0xB1, new RegisterStorage("EPC1", 0x1B1, 0, PrimitiveType.Ptr32) },
            { 0xB2, new RegisterStorage("EPC2", 0x1B2, 0, PrimitiveType.Ptr32) },
            { 0xB3, new RegisterStorage("EPC3", 0x1B3, 0, PrimitiveType.Ptr32) },
            { 0xC0, RegisterStorage.Reg32("DEPC", 0x1C0) },
            { 0xD1, RegisterStorage.Reg32("EXCSAVE1", 0x1D1) },
            { 0xD2, RegisterStorage.Reg32("EXCSAVE2", 0x1D2) },
            { 0xD3, RegisterStorage.Reg32("EXCSAVE3", 0x1D3) },
            { 0xD4, RegisterStorage.Reg32("EXCSAVE4", 0x1D4) },
            { 0xD5, RegisterStorage.Reg32("EXCSAVE5", 0x1D5) },
            { 0xD6, RegisterStorage.Reg32("EXCSAVE6", 0x1D6) },
            { 0xD7, RegisterStorage.Reg32("EXCSAVE7", 0x1D7) },
            { 0xE2, RegisterStorage.Reg32("INTSET", 0x1E2) },
            { 0xE3, RegisterStorage.Reg32("INTCLEAR", 0x1E3) },
            { 0xE4, RegisterStorage.Reg32("INTENABLE", 0x1E4) },
            { 0xE6, new RegisterStorage("PS", 0x1E6, 0, PrimitiveType.Ptr32) },
            { 0xE7, new RegisterStorage("VECBASE", 0x1E7, 0, PrimitiveType.Ptr32) },
            { 0xE8, new RegisterStorage("EXCCAUSE", 0x1E8, 0, PrimitiveType.Ptr32) },
            { 0xEA, RegisterStorage.Reg32("CCOUNT", 0x1EA) },
            { 0xEB, new RegisterStorage("PRID", 0x1EB, 0, PrimitiveType.Ptr32) },
            { 0xEE, new RegisterStorage("EXCVADDR", 0x1EE, 0, PrimitiveType.Ptr32) },
            { 0xF0, RegisterStorage.Reg32("CCOMPARE0", 0x1F0) },
        };

        private static readonly RegisterStorage[] uregs = Enumerable.Range(0, 0x100)
            .Select(n => new RegisterStorage($"user{n}", 0x800 + n, 0, PrimitiveType.Word32))
            .ToArray();

        public XtensaArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, allRegs.ToDictionary(r => r.Name), allRegs.ToDictionary(r => r.Domain))
        {
            //$TODO: Xtensa is bi-endian, but we're assuming little-endian here.
            // Fix this if encountering a big-endian binary.
            this.Endianness = EndianServices.Little;

            this.InstructionBitSize = 8;        // Instruction alignment, really.
            this.FramePointerType = PrimitiveType.Ptr32;
            this.PointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            this.StackRegister = Registers.a1;
        }


        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new XtensaDisassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new XtensaProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new XtensaRewriter(this, rdr, state, binder, host);
        }

        // Xtensa uses a link register
        public override int ReturnAddressOnStack => 0;

        public RegisterStorage GetAluRegister(int i)
        {
            return aregs[i];
        }

        public RegisterStorage GetBoolRegister(int i)
        {
            return bregs[i];
        }

        public RegisterStorage GetFpuRegister(int i)
        {
            return fregs[i];
        }

        public RegisterStorage? GetMac16Register(int i)
        {
            if (0 <= i && i < mac16regs.Length)
                return mac16regs[i];
            else
                return null;
        }

        public RegisterStorage? GetSpecialRegister(int sr)
        {
            if (sregs.TryGetValue(sr, out var sreg))
                return sreg;
            else
                return null;
        }

        public RegisterStorage GetUserRegister(int ur)
        {
            return uregs[ur];
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => Enum.GetName(typeof(Mnemonic), v)!.Replace('_','.'),
                    v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name.Replace('.', '_'), true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            var ireg = domain - StorageDomain.Register;
            if (0 <= ireg && ireg < allRegs.Length)
                return allRegs[ireg];
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            return Address.Ptr32(uAddr);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }
}
