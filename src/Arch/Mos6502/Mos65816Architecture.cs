#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Mos6502
{
    public class Mos65816Architecture : ProcessorArchitecture
    {
        public static readonly RegisterStorage X;
        public static readonly RegisterStorage Y;
        public static readonly RegisterStorage D;
        public static readonly RegisterStorage K;
        public static readonly RegisterStorage B;
        public static readonly PrimitiveType Word24;

        public Mos65816Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options)
        {

            CarryFlagMask = (uint) FlagM.CF;
            Endianness = EndianServices.Little;
            InstructionBitSize = 8;
            FramePointerType = PrimitiveType.Byte;       // Yup, stack pointer is a byte register (!)
            PointerType = PrimitiveType.Ptr16;
            StackRegister = Registers.s;
            WordWidth = PrimitiveType.Byte;       // 8-bit, baby!
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Mos65816Disassembler(this, rdr);
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
            return new Mos65816ProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Mos65816Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            throw new NotImplementedException();
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }

        static Mos65816Architecture()
        {
            var factory = new StorageFactory();
            X = factory.Reg("x", PrimitiveType.Word16);
            Y = factory.Reg("y", PrimitiveType.Word16);
            D = factory.Reg("d", PrimitiveType.Word16);
            K = factory.Reg("k", PrimitiveType.Byte);
            B = factory.Reg("b", PrimitiveType.Byte);
            Word24 = PrimitiveType.CreateWord(24);
        }
    }
}
