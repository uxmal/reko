#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using TypeDomain = Reko.Core.Types.Domain;

namespace Reko.ImageLoaders.LLVM
{
    /// <summary>
    /// This implementation of <see cref="IProcessorArchitecture"/> simulates a
    /// real architecture using the <see cref="DataLayout"/> properties extracted
    /// from an LLVM source file.
    /// </summary>
    public class LLVMArchitecture : ProcessorArchitecture
    {
        private readonly DataLayout layout;

        public LLVMArchitecture(IServiceProvider services, DataLayout layout) : base(services, "llvm", [], [], [])
        {
            this.layout = layout;
            this.Endianness = layout.Endianness;
            this.PointerType = PrimitiveType.Create(TypeDomain.Pointer,layout.PointerLayouts[0].BitSize);
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            throw new NotSupportedException();
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotSupportedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            throw new NotSupportedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotSupportedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotSupportedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotSupportedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotSupportedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotSupportedException();
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            throw new NotSupportedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotSupportedException();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            throw new NotSupportedException();
        }

        public override Address MakeAddressFromConstant(Core.Expressions.Constant c, bool codeAlign)
        {
            throw new System.NotImplementedException();
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotSupportedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            throw new NotImplementedException();
        }
    }
}