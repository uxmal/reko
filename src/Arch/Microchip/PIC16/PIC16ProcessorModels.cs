#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    internal abstract class PIC16ProcessorModel : PICProcessorModel
    {
        public override void CreateMemoryDescriptor()
            => PIC16MemoryDescriptor.Create(PICDescriptor);

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override PICProcessorState CreateProcessorState(PICArchitecture arch)
            => new PIC16State(arch);

        public override Address CreateBankedAddress(byte bsrReg, byte offset)
            => Address.Ptr16((ushort)((bsrReg << 7) + offset));

        public override PICPointerScanner CreatePointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            => new PIC16PointerScanner(rdr, knownLinAddresses, flags);

    }

    internal class PIC16BasicModel : PIC16ProcessorModel
    {
        public override PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr)
            => PIC16BasicDisasm.Create(arch, rdr);

        public override void CreateRegisters()
            => PIC16BasicRegisters.Create(PICDescriptor);

        public override PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            => PIC16BasicRewriter.Create(arch, dasm, state, binder, host);

    }

    internal class PIC16EnhancedModel : PIC16ProcessorModel
    {
        public override PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr)
            => PIC16EnhancedDisasm.Create(arch, rdr);

        public override void CreateRegisters()
            => PIC16EnhancedRegisters.Create(PICDescriptor);

        public override PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            => PIC16EnhancedRewriter.Create(arch, dasm, state, binder, host);

    }

    internal class PIC16FullFeatureModel : PIC16ProcessorModel
    {
        public override PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr)
            => PIC16FullDisasm.Create(arch, rdr);

        public override void CreateRegisters()
            => PIC16FullRegisters.Create(PICDescriptor);

        public override PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            => PIC16FullRewriter.Create(arch, dasm, state, binder, host);

    }

}
