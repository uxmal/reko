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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Machine
{
    /// <summary>
    /// This decoder extracts a value from one or more bitfields and selects one
    /// of two sub-decoders depending on the result of evaluating a provided
    /// predicate with the bitfield value.
    /// </summary>
    public class ConditionalDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TMnemonic : struct
    {
        private readonly Bitfield[] bitfields;
        private readonly Predicate<uint> predicate;
        private readonly Decoder<TDasm, TMnemonic, TInstr> trueDecoder;
        private readonly Decoder<TDasm, TMnemonic, TInstr> falseDecoder;
        private readonly string tag;

        public ConditionalDecoder(
            Bitfield[] bitfields, 
            Predicate<uint> predicate, 
            string tag,
            Decoder<TDasm, TMnemonic, TInstr> trueDecoder,
            Decoder<TDasm, TMnemonic, TInstr> falseDecoder)
        {
            this.bitfields = bitfields;
            this.predicate = predicate;
            this.trueDecoder = trueDecoder;
            this.falseDecoder = falseDecoder;
            this.tag = tag;
        }

        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            DumpMaskedInstruction(32, wInstr, this.bitfields, tag);
            uint n = Bitfield.ReadFields(bitfields, wInstr);
            var decoder = predicate(n) ? trueDecoder : falseDecoder;
            return decoder.Decode(wInstr, dasm);
        }
    }

    /// <summary>
    /// This decoder extracts a value from one or more bitfields and selects one
    /// of two sub-decoders depending on the result of evaluating a provided
    /// predicate with the bitfield value.
    /// </summary>
    public class WideConditionalDecoder<TDasm, TMnemonic, TInstr> : WideDecoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TMnemonic : struct
    {
        private readonly Bitfield[] bitfields;
        private readonly Predicate<ulong> predicate;
        private readonly WideDecoder<TDasm, TMnemonic, TInstr> trueDecoder;
        private readonly WideDecoder<TDasm, TMnemonic, TInstr> falseDecoder;
        private readonly string tag;

        public WideConditionalDecoder(
            Bitfield[] bitfields,
            Predicate<ulong> predicate,
            string tag,
            WideDecoder<TDasm, TMnemonic, TInstr> trueDecoder,
            WideDecoder<TDasm, TMnemonic, TInstr> falseDecoder)
        {
            this.bitfields = bitfields;
            this.predicate = predicate;
            this.trueDecoder = trueDecoder;
            this.falseDecoder = falseDecoder;
            this.tag = tag;
        }

        public override TInstr Decode(ulong wInstr, TDasm dasm)
        {
            DumpMaskedInstruction(64, wInstr, this.bitfields, tag);
            ulong n = Bitfield.ReadFields(bitfields, wInstr);
            var decoder = predicate(n) ? trueDecoder : falseDecoder;
            return decoder.Decode(wInstr, dasm);
        }
    }

}
