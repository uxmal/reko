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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.MilStd1750
{
    public class MilStd1750Architecture : ProcessorArchitecture
    {
        private const double FLOATING_TWO_TO_THE_TWENTYTHREE = 8388608.0;
        private const double FLOATING_TWO_TO_THE_THIRTYNINE = 549755813888.0;


        private readonly Dictionary<uint, FlagGroupStorage> flagGroups;

        public static PrimitiveType Real48 { get; } 

        public MilStd1750Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options)
        {
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.InstructionBitSize = 16;
            this.MemoryGranularity = 16;        // Memory is organized as 16-bit words, not 8-bit bytes
            this.PointerType = PrimitiveType.Ptr16;
            this.StackRegister = Registers.GpRegs[15];
            this.WordWidth = PrimitiveType.Word16;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new MilStd1750Disassembler(this, rdr);
        }

        public override MemoryArea CreateMemoryArea(Address addr, byte[] bytes)
        {
            // NOTE: assumes the bytes are provided in big-endian form.
            var words = new ushort[bytes.Length / 2];
            for (int i = 0; i < words.Length; ++i)
            {
                words[i] = (ushort)((bytes[i * 2] << 8) | bytes[i * 2 + 1]); 
            }
            return new Word16MemoryArea(addr, words);
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
            return new MilStd1750State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new MilStd1750Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out FlagGroupStorage f))
                return f;

            PrimitiveType dt = PrimitiveType.Byte;
            var fl = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
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
            return Registers.ByName.TryGetValue(name, out var reg) ? reg : null;
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.ByDomain.TryGetValue(domain, out var reg) ? reg : null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.CF) != 0) yield return Registers.C;
            if ((grf & (uint) FlagM.PF) != 0) yield return Registers.P;
            if ((grf & (uint) FlagM.ZF) != 0) yield return Registers.Z;
            if ((grf & (uint) FlagM.NF) != 0) yield return Registers.N;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            StringBuilder sb = new StringBuilder();
            if ((grf & (uint) FlagM.CF) != 0) sb.Append('C');
            if ((grf & (uint) FlagM.PF) != 0) sb.Append('P');
            if ((grf & (uint) FlagM.ZF) != 0) sb.Append('Z');
            if ((grf & (uint) FlagM.NF) != 0) sb.Append('N');
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override Constant ReinterpretAsFloat(Constant rawBits)
        {
            if (rawBits.DataType.BitSize == 32)
            {
                var n = ConstantReal.Create(rawBits.DataType, Real32ToIEEE(rawBits.ToUInt32()));
                return n;
            }
            else if (rawBits.DataType.BitSize == 48)
            {
                var n = ConstantReal.Create(rawBits.DataType, Real48ToIEEE(rawBits.ToUInt64()));
                return n;
            }
            throw new NotSupportedException($"Floating point bit size {rawBits.DataType.BitSize} is not supported on MIL-STD-1750A.");
        }

        public static double Real32ToIEEE(uint rawBits32)
        {
            var int_exp = (sbyte) rawBits32;
            var int_mant = (int) rawBits32 >> 8;
            var flt_mant = (double) int_mant / FLOATING_TWO_TO_THE_TWENTYTHREE;
            var flt_exp = Constant.IntPow(2, int_exp);
            return flt_mant * flt_exp;
        }

        private static readonly Bitfield bfHiReal48 = new Bitfield(24, 24);

        public static double Real48ToIEEE(ulong rawBits48)
        {
            var int_exp = (sbyte) (rawBits48 >> 16);
            var int_mant_hi = (int) bfHiReal48.ReadSigned(rawBits48);
            var int_mant_lo = (short) rawBits48;

            var flt_mant = 
                (double) int_mant_hi / FLOATING_TWO_TO_THE_TWENTYTHREE +
                (double) int_mant_lo / FLOATING_TWO_TO_THE_THIRTYNINE;
            var flt_exp = Constant.IntPow(2, int_exp);
            return flt_mant * flt_exp;
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.ByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string? txtAddr, out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }

        public override bool TryRead(EndianImageReader rdr, PrimitiveType dt, out Constant value)
        {
            if (dt.Domain == Domain.Real)
            {
                if (dt.BitSize == 32)
                {
                    if (rdr.TryReadBeUInt32(out uint uValue))
                    {
                        value = ConstantReal.Create(dt, Real32ToIEEE(uValue));
                        return true;
                    }
                }
                if (dt.BitSize == 48)
                {
                    if (rdr.TryReadBeUInt32(out uint uHiwords) &&
                        rdr.TryReadBeUInt16(out ushort uLoword))
                    {
                        ulong uValue = ((ulong) uHiwords << 16) | uLoword;
                        value = ConstantReal.Create(dt, Real48ToIEEE(uValue));
                        return true;
                    }
                }
            }
            return base.TryRead(rdr, dt, out value);
        }

        static MilStd1750Architecture()
        {
            Real48 = PrimitiveType.Create(Domain.Real, 48);
        }
    }
}
