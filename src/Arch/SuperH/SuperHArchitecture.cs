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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using BindingFlags = System.Reflection.BindingFlags;

namespace Reko.Arch.SuperH
{
    // NetBSD for dreamcast? http://ftp.netbsd.org/pub/pkgsrc/packages/NetBSD/dreamcast/7.0/All/
    // RaymondC says: https://devblogs.microsoft.com/oldnewthing/20190820-00/?p=102792
    public class SuperHArchitecture : ProcessorArchitecture
    {
        private readonly Dictionary<uint, FlagGroupStorage> grfs;
        private Decoder<SuperHDisassembler, Mnemonic, SuperHInstruction> rootDecoder;

        public SuperHArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.Endianness = EndianServices.Big;   // Pick one at random.
            this.FramePointerType = PrimitiveType.Ptr32;
            this.InstructionBitSize = 16;
            this.PointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            // No architecture-defined stack register -- defined by platform.
            this.grfs = new Dictionary<uint, FlagGroupStorage>();
            this.rootDecoder = default!;
            LoadUserOptions(options);
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new SuperHDisassembler(this, rootDecoder, rdr);
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            return null;
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new SuperHState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new SuperHRewriter(this, rootDecoder, rdr, (SuperHState)state, binder, host);
        }

        // SuperH uses a link register
        public override int ReturnAddressOnStack => 0;


        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagRegister == Registers.sr)
            {
                if (!grfs.TryGetValue(grf, out FlagGroupStorage? fl))
                {
                    PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
                    fl = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf));
                    grfs.Add(grf, fl);
                }
                return fl;
            }
            return null;
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
            var regField = typeof(Registers).GetField(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public)!;
            return (RegisterStorage?)regField.GetValue(null);
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            if (domain == Registers.fr0.Domain)
            {
                // Special case the floating point numbers.
                if (range.Extent == 32)
                {
                    return Registers.fpregs[range.Lsb / 32];
                }
                if (range.Extent == 64)
                {
                    return Registers.dfpregs[range.Lsb / 64];
                }
                throw new NotImplementedException("GetRegister: FP registers not done yet.");
            }
            return Registers.RegistersByDomain.TryGetValue(domain, out var reg)
                ? reg 
                : null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.gpregs;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
            if ((grf & Registers.T.FlagGroupBits) != 0) yield return Registers.T;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var s = new StringBuilder();
            if (flagRegister == Registers.sr)
            {
                if ((Registers.S.FlagGroupBits & grf) != 0) s.Append(Registers.S.Name);
                if ((Registers.T.FlagGroupBits & grf) != 0) s.Append(Registers.T.Name);
            }
            return s.ToString();
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            Endianness = (options is not null && options.TryGetValue(ProcessorOption.Endianness, out var oEndian)
                && oEndian is string sEndian
                && string.Compare(sEndian, "be") == 0)
                ? EndianServices.Big
                : EndianServices.Little;
            rootDecoder = new SuperHDisassembler.InstructionSet(options).CreateDecoder();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~1u;
            return Address.Ptr32(uAddr);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, object>? SaveUserOptions()
        {
            return Options;
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
