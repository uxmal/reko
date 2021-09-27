#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
        where TMnemonic : struct
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
            DumpMaskedInstruction(32, wInstr, shMask, tag);
        }

        public override string ToString()
        {
            return $"{{Mask {bitfield} {tag}}}";
        }
    }

    public class WideMaskDecoder<TDasm, TMnemonic, TInstr> : WideDecoder<TDasm, TMnemonic, TInstr>
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
        where TMnemonic : struct
        where TInstr : MachineInstruction
    {
        private readonly Bitfield bitfield;
        private readonly WideDecoder<TDasm, TMnemonic, TInstr>[] decoders;
        private readonly string tag;

        public WideMaskDecoder(int bitPos, int bitSize, string tag, params WideDecoder<TDasm, TMnemonic, TInstr>[] decoders)
        {
            this.bitfield = new Bitfield(bitPos, bitSize);
            Debug.Assert(decoders.Length == (1 << bitSize), $"Inconsistent number of decoders {decoders.Length} (bitPos {bitPos} bitSize{bitSize:X})");
            this.decoders = decoders;
            this.tag = tag;
        }

        public override TInstr Decode(ulong wInstr, TDasm dasm)
        {
            TraceDecoder(wInstr);
            uint op = bitfield.Read(wInstr);
            return decoders[op].Decode(wInstr, dasm);
        }

        [Conditional("DEBUG")]
        public void TraceDecoder(uint wInstr)
        {
            var shMask = this.bitfield.Mask << this.bitfield.Position;
            DumpMaskedInstruction(32, wInstr, shMask, tag);
        }

        [Conditional("DEBUG")]
        public void TraceDecoder(ulong wInstr)
        {
            var shMask = (ulong)this.bitfield.Mask << this.bitfield.Position;
            DumpMaskedInstruction(64, wInstr, shMask, tag);
        }

        public override string ToString()
        {
            return $"{{Mask {bitfield}}}";
        }
    }
}

