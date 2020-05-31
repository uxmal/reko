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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Machine
{
    public class BitfieldDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
    {
        private readonly Bitfield[] bitfields;
        private readonly Decoder<TDasm, TMnemonic, TInstr>[] decoders;
        private readonly string tag;

        public BitfieldDecoder(Bitfield[] bitfields, string tag, Decoder<TDasm, TMnemonic, TInstr>[] decoders)
        {
            Debug.Assert(1 << bitfields.Sum(b => b.Length) == decoders.Length,
                $"Expected {1 << bitfields.Sum(b => b.Length)} decoders but found {decoders.Length}.");
            this.bitfields = bitfields;
            this.decoders = decoders;
            this.tag = tag;
        }

        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            TraceDecoder(wInstr, tag);
            uint n = Bitfield.ReadFields(bitfields, wInstr);
            return this.decoders[n].Decode(wInstr, dasm);
        }

        [Conditional("DEBUG")]
        public void TraceDecoder(uint wInstr, string tag = "")
        {
            var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
            DumpMaskedInstruction(wInstr, shMask, tag);
        }
    }
}
