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
#endregion

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.Xtensa
{
    public class XtensaArchitecture : ProcessorArchitecture
    {
        public XtensaArchitecture(string archId)  : base(archId)
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

        private static RegisterStorage[] aregs = new[]
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

        private static RegisterStorage[] bregs = new[]
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

        private static RegisterStorage[] fregs = new[]
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

        private static RegisterStorage[] allRegs =
            aregs.Concat(bregs).Concat(fregs).ToArray();

        private static Dictionary<int, RegisterStorage> sregs = new Dictionary<int, RegisterStorage>
        {
            { 0x03, Registers.SAR },
            { 0xA2, new RegisterStorage("CCOUNT", 0x1A2, 0, PrimitiveType.Word32) },
            { 0xA3, new RegisterStorage("INTENABLE", 0x1A3, 0, PrimitiveType.Word32) },
            { 0xB1, new RegisterStorage("EPC1", 0x1B1, 0, PrimitiveType.Ptr32) },
            { 0xB2, new RegisterStorage("EPC2", 0x1B2, 0, PrimitiveType.Ptr32) },
            { 0xB3, new RegisterStorage("EPC3", 0x1B3, 0, PrimitiveType.Ptr32) },
            { 0xC0, new RegisterStorage("DEPC", 0x1C0, 0, PrimitiveType.Word32) },
            { 0xD1, new RegisterStorage("EXCSAVE1", 0x1D1, 0, PrimitiveType.Word32) },
            { 0xD2, new RegisterStorage("EXCSAVE2", 0x1D2, 0, PrimitiveType.Word32) },
            { 0xD3, new RegisterStorage("EXCSAVE3", 0x1D3, 0, PrimitiveType.Word32) },
            { 0xD4, new RegisterStorage("EXCSAVE4", 0x1D4, 0, PrimitiveType.Word32) },
            { 0xD5, new RegisterStorage("EXCSAVE5", 0x1D5, 0, PrimitiveType.Word32) },
            { 0xD6, new RegisterStorage("EXCSAVE6", 0x1D6, 0, PrimitiveType.Word32) },
            { 0xD7, new RegisterStorage("EXCSAVE7", 0x1D7, 0, PrimitiveType.Word32) },
            { 0xE2, new RegisterStorage("INTSET", 0x1E2, 0, PrimitiveType.Word32) },
            { 0xE3, new RegisterStorage("INTCLEAR", 0x1E3, 0, PrimitiveType.Word32) },
            { 0xE6, new RegisterStorage("PS", 0x1E6, 0, PrimitiveType.Ptr32) },
            { 0xE7, new RegisterStorage("VECBASE", 0x1E7, 0, PrimitiveType.Ptr32) },
            { 0xE8, new RegisterStorage("EXCCAUSE", 0x1E8, 0, PrimitiveType.Ptr32) },
            { 0xEE, new RegisterStorage("EXCVADDR", 0x1EE, 0, PrimitiveType.Ptr32) },
            { 0xF0, new RegisterStorage("CCOMPARE0", 0x1F0, 0, PrimitiveType.Word32) },
        };

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new XtensaDisassembler(this, rdr);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
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

        public RegisterStorage GetSpecialRegister(int sr)
        {
            return sregs[sr];
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
            .Cast<Mnemonic>()
            .ToSortedList(
                v => Enum.GetName(typeof(Mnemonic), v).Replace('_','.'),
                v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            if (!Enum.TryParse(name.Replace('.', '_'), true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return allRegs[domain - StorageDomain.Register];
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

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }
}
