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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public class SparcArchitecture : ProcessorArchitecture
    {
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public SparcArchitecture(string archId, PrimitiveType wordWidth) : base(archId)
        {
            this.Endianness = EndianServices.Big;
            this.WordWidth = wordWidth;
            this.PointerType = PrimitiveType.Create(Domain.Pointer, wordWidth.BitSize);
            this.StackRegister = Registers.sp;
            this.FramePointerType = PointerType;
            this.InstructionBitSize = 32;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        #region IProcessorArchitecture Members

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new SparcDisassembler(this, imageReader);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new SparcInstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new SparcProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new SparcRewriter(this, rdr, (SparcProcessorState)state, binder, host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        // Sparc uses a link register
        public override int ReturnAddressOnStack => 0;

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
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.GetRegister(domain);
        }

        public override RegisterStorage GetRegister(string name)
        {
            if (Registers.TryGetRegister(name, out var reg))
                return reg;
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return
                Registers.IntegerRegisters
                .Concat(Registers.FloatRegisters).ToArray();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.TryGetRegister(name, out reg);
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0)
                return reg;
            else
                return null;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out FlagGroupStorage fl))
                return fl;

            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            fl = new FlagGroupStorage(Registers.psr, grf, GrfToString(flagRegister, "", grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            FlagM grf = 0;
            for (int i = 0; i < name.Length; ++i)
            {
                switch (name[i])
                {
                case 'N': grf |= FlagM.NF; break;
                case 'C': grf |= FlagM.CF; break;
                case 'Z': grf |= FlagM.ZF; break;
                case 'V': grf |= FlagM.VF; break;

                case 'E': grf |= FlagM.EF; break;
                case 'L': grf |= FlagM.LF; break;
                case 'G': grf |= FlagM.GF; break;
                case 'U': grf |= FlagM.UF; break;
                default: return null;
                }
            }
            return GetFlagGroup(Registers.psr, (uint)grf);
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            if (flags.FlagRegister != Registers.psr)
                yield break;
            var grf = flags.FlagGroupBits;
            if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;

            if ((grf & Registers.E.FlagGroupBits) != 0) yield return Registers.E;
            if ((grf & Registers.L.FlagGroupBits) != 0) yield return Registers.L;
            if ((grf & Registers.G.FlagGroupBits) != 0) yield return Registers.G;
            if ((grf & Registers.U.FlagGroupBits) != 0) yield return Registers.U;
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }


        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & Registers.N.FlagGroupBits) != 0) s.Append(Registers.N.Name);
            if ((grf & Registers.Z.FlagGroupBits) != 0) s.Append(Registers.Z.Name);
            if ((grf & Registers.V.FlagGroupBits) != 0) s.Append(Registers.V.Name);
            if ((grf & Registers.C.FlagGroupBits) != 0) s.Append(Registers.C.Name);

            if ((grf & Registers.E.FlagGroupBits) != 0) s.Append(Registers.E.Name);
            if ((grf & Registers.L.FlagGroupBits) != 0) s.Append(Registers.L.Name);
            if ((grf & Registers.G.FlagGroupBits) != 0) s.Append(Registers.G.Name);
            if ((grf & Registers.U.FlagGroupBits) != 0) s.Append(Registers.U.Name);

            return s.ToString();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        #endregion
    }

    public class SparcArchitecture32 : SparcArchitecture
    {
        public SparcArchitecture32(string archId) : base(archId, PrimitiveType.Word32)
        {
        }
    }

    public class SparcArchitecture64 : SparcArchitecture
    {
        public SparcArchitecture64(string archId) : base(archId, PrimitiveType.Word64)
        {
        }
    }
}