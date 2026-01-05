#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Core.Machine
{
    /// <summary>
    /// This decoder tests a predicate. If true, it will evaluate
    /// the given subdecoder. If false, the disassembler's 
    /// <see cref="DisassemblerBase{TInstr,TMnemonic}.CreateInvalidInstruction" />
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

        /// <summary>
        /// Constructs a conditional decoder.
        /// </summary>
        /// <param name="bf">Bitfield to extract a value from.</param>
        /// <param name="predicate">Predicate used to test the extracted value.</param>
        /// <param name="trueDecoder">Decoder to invoke when predicate tests true.</param>
        public IfDecoder(
            in Bitfield bf,
            Predicate<uint> predicate,
            Decoder<TDasm, TMnemonic, TInstr> trueDecoder)
        {
            this.bitfield = bf;
            this.predicate = predicate;
            this.trueDecoder = trueDecoder;
        }

        /// <inheritdoc/>
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
