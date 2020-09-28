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
    /// <summary>
    /// Abstract base class for instruction decoders
    /// </summary>
    /// <typeparam name="TDasm">Disassembler</typeparam>
    /// <typeparam name="TInstr">Instruction type</typeparam>
    public abstract class Decoder<TDasm, TMnemonic, TInstr> 
        where TInstr : MachineInstruction
    {
        public abstract TInstr Decode(uint wInstr, TDasm dasm);

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction(uint wInstr, uint shMask, TMnemonic mnemonic)
        {
            DumpMaskedInstruction(wInstr, shMask, mnemonic.ToString());
        }

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction(uint wInstr, Bitfield[] bitfields, string tag)
        {
            var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
            DumpMaskedInstruction(wInstr, shMask, tag);
        }

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction(uint wInstr, uint shMask, string tag)
        {
            return;
            var hibit = 0x80000000u;
            var sb = new StringBuilder();
            for (int i = 0; i < 32; ++i)
            {
                if ((shMask & hibit) != 0)
                {
                    sb.Append((wInstr & hibit) != 0 ? '1' : '0');
                }
                else
                {
                    sb.Append((wInstr & hibit) != 0 ? ':' : '.');
                }
                shMask <<= 1;
                wInstr <<= 1;
            }
            if (!string.IsNullOrEmpty(tag))
            {
                sb.AppendFormat(" {0}", tag);
            }
            Debug.Print(sb.ToString());
        }
    }

    public abstract class WideDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
    {
        public sealed override TInstr Decode(uint wInstr, TDasm dasm)
        {
            throw new InvalidOperationException("32-bit decoding is not allowed with wide decoders.");
        }

        public abstract TInstr Decode(ulong ulInstr, TDasm dasm);
    }
}
