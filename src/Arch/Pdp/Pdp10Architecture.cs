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

using Reko.Arch.Pdp.Memory;
using Reko.Arch.Pdp.Pdp10;
using Reko.Arch.Pdp.Pdp10.Rewriter;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Pdp
{
    public class Pdp10Architecture : ProcessorArchitecture
    {
        public Pdp10Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.CarryFlagMask = 0; //$TODO:
            this.DefaultBase = 8;
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PdpTypes.Ptr18;
            this.InstructionBitSize = 36;
            this.MemoryGranularity = 36;
            this.PointerType = PdpTypes.Ptr18;
            this.StackRegister = Registers.Accumulators[15];
            this.WordWidth = PdpTypes.Word36;
        }

        public override int ReturnAddressOnStack => 1;


        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Pdp10.Disassembler.Pdp10Disassembler(this, (Word36BeImageReader) imageReader);
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new DefaultProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Pdp10Rewriter(this, (Word36BeImageReader)rdr, state, binder, host);
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Word32;
            var flagregister = Registers.Psw;
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf), dt);
            return fl;
        }

        public override FlagGroupStorage? GetFlagGroup(string name)
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

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.Accumulators[(int) domain];
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint)FlagM.C0) != 0) yield return Registers.C0;
            if ((grf & (uint)FlagM.C1) != 0) yield return Registers.C1;
            if ((grf & (uint)FlagM.V) != 0) yield return Registers.V;
            if ((grf & (uint)FlagM.T) != 0) yield return Registers.T;
            if ((grf & (uint)FlagM.ND) != 0) yield return Registers.ND;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var s = new StringBuilder();
            if ((grf & (uint) FlagM.C0) != 0) s.Append("C0");
            if ((grf & (uint) FlagM.C1) != 0) s.Append("C1");
            if ((grf & (uint) FlagM.V)  != 0) s.Append("V");
            if ((grf & (uint) FlagM.T) != 0) s.Append("T");
            if ((grf & (uint) FlagM.ND) != 0) s.Append("ND");
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return new Address18((uint)c.ToUInt64());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override string RenderInstructionOpcode(MachineInstruction instr, EndianImageReader rdr)
        {
            var reader = (Word36BeImageReader) rdr;
            if (!reader.TryReadBeUInt36(out ulong uInstr))
                return "";
            return Convert.ToString((long) uInstr, 8).PadLeft(12, '0');
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            var r = Registers.Accumulators.FirstOrDefault(r => r.Name == name);
            if (r is not null)
            {
                reg = r;
                return true;
            }
            else
            {
                reg = default!;
                return false;
            }
        }

        public override bool TryParseAddress(string? txtAddr, out Address addr)
        {
            if (txtAddr is null)
            {
                addr = default!;
                return false;
            }
            uint uAddr = 0;
            foreach (var c in txtAddr)
            {
                var u = (uint) (c - '0');
                if (u > 8)
                {
                    addr = default!;
                    return false;
                }
                uAddr = (uAddr << 3) | u;
            }
            addr = new Address18(uAddr);
            return true;
        } 

        public static ulong OctalStringToWord(string octalWord)
        {
            int c = Math.Min(octalWord.Length, 12);
            ulong word = 0;
            for (int i = 0; i < c; ++i)
            {
                var digit = (uint)(octalWord[i] - '0');
                if (digit > 7)
                    break;
                word = (word << 3) | digit;
            }
            return word;
        }

        public static ulong Ascii7(string ascii)
        {
            if (ascii.Length > 5)
                throw new ArgumentException("No more than 5 ASCII characters can be packed into a 36-bit word.");
            byte[] bytes = new byte[5];
            Encoding.ASCII.GetBytes(ascii, bytes);
            int shift = 36 - 8;
            ulong value = 0;
            foreach (byte b in bytes)
            {
                value |= (ulong) b << shift;
                shift -= 7;
            }
            return value;
        }
    }
}
