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
using Reko.Core.Rtl;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Serialization;
using Reko.Core.Operators;

namespace Reko.Arch.Mos6502
{
    public class Mos6502Architecture : ProcessorArchitecture
    {
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Mos6502Architecture(string archId) : base(archId)
        {
            CarryFlagMask = (uint)FlagM.CF;
            Endianness = EndianServices.Little;
            InstructionBitSize = 8;
            FramePointerType = PrimitiveType.Byte;       // Yup, stack pointer is a byte register (!)
            PointerType = PrimitiveType.Ptr16;
            StackRegister = Registers.s;
            WordWidth = PrimitiveType.Byte;       // 8-bit, baby!
            flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Disassembler((LeImageReader)imageReader);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            return new Mos6502Emulator(this, segmentMap, envEmulator);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Mos6502ProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Rewriter(this, rdr.Clone(), state, binder, host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => v.ToString(),
                    v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            Mnemonic result;
            if (!Enum.TryParse(name, true, out result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.All[(int) domain];
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.All;
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            return reg;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            foreach (var flag in Registers.Flags)
            {
                if ((flags.FlagGroupBits & flag.FlagGroupBits) != 0)
                    yield return flag;
            }
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out var fstg))
                return fstg;
            var sb = new StringBuilder();
            foreach (var flag in Registers.Flags)
            {
                if ((grf & flag.FlagGroupBits) != 0)
                    sb.Append(flag.Name);
            }
            fstg = new FlagGroupStorage(flagRegister, grf, sb.ToString(), PrimitiveType.Byte);
            this.flagGroups.Add(grf, fstg);
            return fstg;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            //$TODO: the M6502 stack pointer is an 8-bit offset into the fixed memory area $0100-$01FF.
            return new MemoryAccess(new BinaryExpression(
                Operator.IAdd,
                PrimitiveType.Ptr16,
                binder.EnsureRegister(Registers.s),
                Constant.Int16((short) cbOffset)),
                dataType);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            if (rdr.TryReadLeUInt16(out var uaddr))
            {
                return Address.Ptr16(uaddr);
            }
            else
            {
                return null;
            }
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            var sb = new StringBuilder();
            foreach (var flag in Registers.Flags)
            {
                if ((flag.FlagGroupBits & grf) != 0)
                    sb.Append(flag.Name);
            }
            return sb.ToString();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }
    }

    public static class Registers
    {
        public static readonly RegisterStorage a = RegisterStorage.Reg8("a", 0);
        public static readonly RegisterStorage x = RegisterStorage.Reg8("x", 1);
        public static readonly RegisterStorage y = RegisterStorage.Reg8("y", 2);
        public static readonly RegisterStorage s = RegisterStorage.Reg8("s", 3);

        public static readonly RegisterStorage p = new RegisterStorage("p", 10, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage pc = new RegisterStorage("pc", 11, 0, PrimitiveType.Word16);

        public static readonly FlagGroupStorage N = new FlagGroupStorage(p, (uint)FlagM.NF, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage V = new FlagGroupStorage(p, (uint)FlagM.VF, "V", PrimitiveType.Bool);
        public static readonly FlagGroupStorage I = new FlagGroupStorage(p, (uint)FlagM.IF, "I", PrimitiveType.Bool);
        public static readonly FlagGroupStorage D = new FlagGroupStorage(p, (uint)FlagM.DF, "D", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(p, (uint)FlagM.ZF, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage C = new FlagGroupStorage(p, (uint)FlagM.CF, "C", PrimitiveType.Bool);

        internal static RegisterStorage[] All;

        internal static FlagGroupStorage[] Flags;

        public static RegisterStorage GetRegister(int reg)
        {
            return All[reg];
        }

        static Registers()
        {
            All = new RegisterStorage[]
            {
                a,
                x,
                y,
                s,
            };
            Flags = new FlagGroupStorage[]
            {
                N,
                V,
                I,
                D,
                Z,
                C,
            };
        }
    }
}
