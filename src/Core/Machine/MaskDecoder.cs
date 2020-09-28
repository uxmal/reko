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
    public class MaskDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
        where TInstr : MachineInstruction
    {
        private readonly Bitfield bitfield;
        private readonly Decoder<TDasm, TMnemonic, TInstr>[] decoders;
        private readonly string tag;

        public MaskDecoder(int bitPos, int bitSize, string tag, params Decoder<TDasm, TMnemonic, TInstr>[] decoders)
        {
            this.bitfield = new Bitfield(bitPos, bitSize);
            Debug.Assert(decoders.Length == (1 << bitSize), $"Inconsistent number of decoders {decoders.Length} (bitPos {bitPos} bitSize{bitSize:X})");
            this.decoders = decoders;
            this.tag = tag;
        }

        public MaskDecoder(int bitPos, int bitSize, string tag, params (int, Decoder<TDasm, TMnemonic, TInstr>) [] decoders)
        {
            this.bitfield = new Bitfield(bitPos, bitSize);
            Decoder<TDasm, TMnemonic, TInstr> nyiDecoder = new NyiDecoder<TDasm, TMnemonic, TInstr>(tag);
            this.decoders = Enumerable.Range(0, 1 << bitSize)
               .Select(n => nyiDecoder)
               .ToArray();
            foreach (var (value, decoder) in decoders)
            {
                this.decoders[value] = decoder;
            }
            foreach (var (value, decoder) in decoders)
            {
                Debug.Assert(this.decoders[value] == null, $"Duplicate value {value}");
                this.decoders[value] = decoder;
            }
            this.tag = tag;
        }

        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            TraceDecoder(wInstr);
            uint op = bitfield.Read(wInstr);
            return decoders[op].Decode(wInstr, dasm);
        }

        [Conditional("DEBUG")]
        public void TraceDecoder(uint wInstr)
        {
            var shMask = this.bitfield.Mask << this.bitfield.Position;
            DumpMaskedInstruction(wInstr, shMask, tag);
        }
    }
}

