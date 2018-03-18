#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Libraries.Microchip;

namespace Reko.Arch.Microchip.PIC18
{
    using Common;

    internal class PIC18LegacyMode : PICProcessorMode
    {
        public override PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr)
            => PIC18DisassemblerBase.Create(arch, rdr);

        public override void CreateRegisters(PIC pic)
            => PIC18LegacyRegisters.Create(pic);

        public override PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            => PIC18Rewriter.Create(arch, dasm, state, binder, host);

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override PICProcessorState CreateProcessorState(PICArchitecture arch)
            => new PIC18State(arch);

    }

    internal class PIC18EggMode : PICProcessorMode
    {
        public override PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr)
            => PIC18DisassemblerBase.Create(arch, rdr);

        public override void CreateRegisters(PIC pic)
            => PIC18EggRegisters.Create(pic);

        public override PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            => PIC18Rewriter.Create(arch, dasm, state, binder, host);

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override PICProcessorState CreateProcessorState(PICArchitecture arch)
            => new PIC18State(arch);

    }

    internal class PIC18EnhancedMode : PICProcessorMode
    {
        public override PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr)
            => PIC18DisassemblerBase.Create(arch, rdr);

        public override void CreateRegisters(PIC pic)
            => PIC18EnhancedRegisters.Create(pic);

        public override PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            => PIC18Rewriter.Create(arch, dasm, state, binder, host);

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override PICProcessorState CreateProcessorState(PICArchitecture arch)
            => new PIC18State(arch);

    }

}
