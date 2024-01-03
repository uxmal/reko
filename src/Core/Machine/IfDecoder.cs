#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Machine
{
    /// <summary>
    /// This decoder tests a predicate. If true, it will evaluate
    /// the given subdecoder. If false, the disassembler's CreateInvalidInstruction
    /// metod is called.
    /// </summary>
    public class IfDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
        where TInstr : MachineInstruction
        where TMnemonic : struct
    {
        private readonly Bitfield bitfield;
        private readonly Predicate<uint> predicate;
        private readonly Decoder<TDasm, TMnemonic, TInstr> trueDecoder;

        public IfDecoder(
            Bitfield bf,
            Predicate<uint> predicate,
            Decoder<TDasm, TMnemonic, TInstr> trueDecoder)
        {
            this.bitfield = bf;
            this.predicate = predicate;
            this.trueDecoder = trueDecoder;
        }

        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            DumpMaskedInstruction(32, wInstr, bitfield.Mask << bitfield.Position, "");
            var u = bitfield.Read(wInstr);
            if (predicate(u))
                return trueDecoder.Decode(wInstr, dasm);
            else
                return dasm.CreateInvalidInstruction();
        }
    }
}
