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

using Reko.Core.Lib;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Instruction decoder that extracts a value from an ordered list 
    /// of bit fields and uses that value to select one of a set of sub-decoders.
    /// </summary>
    public class BitfieldDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TMnemonic : struct
    {
        private readonly Bitfield[] bitfields;
        private readonly Decoder<TDasm, TMnemonic, TInstr>[] decoders;
        private readonly string tag;

        /// <summary>
        /// Constructs an instance of <see cref="BitfieldDecoder{TDasm, TMnemonic, TInstr}"/>.
        /// </summary>
        /// <param name="bitfields">Bit fields to extract, ordered most to least siginificant position.</param>
        /// <param name="tag">Tag used for debugging output.</param>
        /// <param name="decoders">Sub-decoders.</param>
        public BitfieldDecoder(Bitfield[] bitfields, string tag, Decoder<TDasm, TMnemonic, TInstr>[] decoders)
        {
            Debug.Assert(1 << bitfields.Sum(b => b.Length) == decoders.Length,
                $"Expected {1 << bitfields.Sum(b => b.Length)} decoders but found {decoders.Length}.");
            this.bitfields = bitfields;
            this.decoders = decoders;
            this.tag = tag;
        }

        /// <inheritdoc/>
        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            TraceDecoder(wInstr, tag);
            uint n = Bitfield.ReadFields(bitfields, wInstr);
            return this.decoders[n].Decode(wInstr, dasm);
        }

        /// <summary>
        /// Dumps a masked instruction to the debugger output and console.
        /// </summary>
        /// <param name="wInstr"></param>
        /// <param name="tag"></param>
        [Conditional("DEBUG")]
        public void TraceDecoder(uint wInstr, string tag = "")
        {
            var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
            DumpMaskedInstruction(32, wInstr, shMask, tag);
        }
    }
}
