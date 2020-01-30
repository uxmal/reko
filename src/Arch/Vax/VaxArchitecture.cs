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
using System.Threading.Tasks;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System.Globalization;
using Reko.Core.Lib;

namespace Reko.Arch.Vax
{
    public class VaxArchitecture : ProcessorArchitecture
    {
        private static RegisterStorage[] regs = new[]
        {
            Registers.r0,
            Registers.r1,
            Registers.r2,
            Registers.r3,

            Registers.r4,
            Registers.r5,
            Registers.r6,
            Registers.r7,

            Registers.r8,
            Registers.r9,
            Registers.r10,
            Registers.r11,

            Registers.ap,
            Registers.fp,
            Registers.sp,
            Registers.pc,
        };

        private static Dictionary<string, RegisterStorage> regsByName = regs
            .ToDictionary(r => r.Name);

        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public VaxArchitecture(string name) : base(name)
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 8;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            this.PointerType = PrimitiveType.Ptr32;
            this.StackRegister = Registers.sp;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new VaxDisassembler(this, imageReader);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new VaxInstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new VaxProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new VaxRewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out var f))
            {
                return f;
            }

            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(flagRegister, grf, GrfToString(Registers.psw, "", grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(v => Enum.GetName(typeof(Mnemonic), v), v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            int i = domain - StorageDomain.Register;
            return GetRegister(i);
        }

        public RegisterStorage GetRegister(int i)
        {
            if (0 <= i && i < regs.Length)
                return regs[i];
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regs;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            foreach (var flag in flagRegisters)
            {
                if ((flags.FlagGroupBits & flag.FlagGroupBits) != 0)
                    yield return flag;
            }
        }

        //$REVIEW: shouldn't this be flaggroup?
        private static readonly FlagGroupStorage[] flagRegisters = {
            new FlagGroupStorage(Registers.psw, (uint)FlagM.CF, "C", PrimitiveType.Bool),
            new FlagGroupStorage(Registers.psw, (uint)FlagM.VF, "V", PrimitiveType.Bool),
            new FlagGroupStorage(Registers.psw, (uint)FlagM.ZF, "Z", PrimitiveType.Bool),
            new FlagGroupStorage(Registers.psw, (uint)FlagM.NF, "N", PrimitiveType.Bool),
        };

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder();
            for (int r = 0; grf != 0; ++r, grf >>= 1)
            {
                if ((grf & 1) != 0)
                    s.Append(flagRegisters[r].Name);
            }
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return regsByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            if (!uint.TryParse(txtAddr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint uAddr))
            {
                addr = null;
                return false;
            }
            else
            {
                addr = Address.Ptr32(uAddr);
                return true;
            }
        }
    }
}
