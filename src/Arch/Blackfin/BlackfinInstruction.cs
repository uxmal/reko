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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Blackfin
{
    public class BlackfinInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            (string? prefix, string? infix, string? suffix) aaOpcode;
            if (!mapMnemonics.TryGetValue(Mnemonic, out aaOpcode))
            {
                aaOpcode.prefix = Mnemonic.ToString();
            }
            if (aaOpcode.infix is not null)
            {
                renderer.WriteString(aaOpcode.prefix!);
                Operands[0].Render(renderer, options);
                if (Operands.Length > 2)
                {
                    renderer.WriteString(" = ");
                    Operands[1].Render(renderer, options);
                    renderer.WriteString(aaOpcode.infix);
                    Operands[2].Render(renderer, options);
                }
                else if (Operands.Length > 1)
                {
                    renderer.WriteString(aaOpcode.infix);
                    Operands[1].Render(renderer, options);
                }
                else
                {
                    renderer.WriteString(aaOpcode.infix);
                }
            }
            else
            {
                renderer.WriteMnemonic(aaOpcode.prefix!);
                var sep = " ";
                if (Operands is null)
                    return;
                foreach (var op in Operands)
                {
                    renderer.WriteString(sep);
                    sep = ",";
                    op.Render(renderer, options);
                }
            }
            if (aaOpcode.suffix is not null)
            {
                renderer.WriteString(aaOpcode.suffix);
            }
            renderer.WriteString(";");
        }

        private static readonly Dictionary<Mnemonic, (string?,string?,string?)> mapMnemonics = 
            new Dictionary<Mnemonic, (string?, string?, string?)>
        {
            { Mnemonic.add, (null, " += ", null) },
            { Mnemonic.add3, (null, " + ", null) },
            { Mnemonic.add_sh1, (null, " + ", " << 1") },
            { Mnemonic.add_sh2, (null, " + ", " << 2") },
            { Mnemonic.and3, (null, " & ", null) },
            { Mnemonic.or3, (null, " | ", null) },
            { Mnemonic.sub3, (null, " - ", null) },
            { Mnemonic.xor3, (null, " ^ ", null) },
            { Mnemonic.asr, (null, " >>>= ", null) },       // SIC: this is opposite to the Java syntax.
            { Mnemonic.asr3, (null, " >>> ", null) },
            { Mnemonic.DIVQ, ("DIVQ (", null, ")") },
            { Mnemonic.lsl, (null, " <<= ", null) },
            { Mnemonic.lsl3, (null, " << ", null) },
            { Mnemonic.lsr, (null, " >>= ", null) },
            { Mnemonic.lsr3, (null, " >> ", null) },
            { Mnemonic.bitset, ( "BITSET(", ",", ")") },
            { Mnemonic.bittgl, ( "BITTGL(", ",", ")") },
            { Mnemonic.bitclr, ( "BITCLR(", ",", ")") },
            { Mnemonic.if_cc_jump, ("IF CC JUMP", null, null) },
            { Mnemonic.if_cc_jump_bp, ("IF CC JUMP", null, " (BP)") },
            { Mnemonic.if_cc_mov, ("IF CC ", " = ", null) },
            { Mnemonic.if_ncc_mov, ("IF !CC ", " = ", null) },
            { Mnemonic.if_ncc_jump, ("IF !CC JUMP", null, null) },
            { Mnemonic.if_ncc_jump_bp, ("IF !CC JUMP", null, " (BP)") },
            { Mnemonic.mov, (null, " = ", null) },
            { Mnemonic.mov_cc_eq, ("CC = ", " == ", null) },
            { Mnemonic.mov_cc_le, ("CC = ", " <= ", null) },
            { Mnemonic.mov_cc_lt, ("CC = ", " < ", null) },
            { Mnemonic.mov_cc_ule, ("CC = ", " <= ", null) },
            { Mnemonic.mov_cc_ult, ("CC = ", " < ", null) },
            { Mnemonic.mov_cc_bittest, ( "CC = BITTEST(", ",", ")" )},
            { Mnemonic.mov_cc_n_bittest, ( "CC = !BITTEST(", ",", ")" )},
            { Mnemonic.mov_r_cc, (null, " = ", "CC") },
            { Mnemonic.mov_xb, (null, " = ", ".B (X)") },
            { Mnemonic.mov_xl, (null, " = ", ".L (X)") },
            { Mnemonic.mov_zb, (null, " = ", ".B (Z)") },
            { Mnemonic.mov_zl, (null, " = ", ".L (Z)") },
            { Mnemonic.mov_x, (null, " = ", " (X)") },
            { Mnemonic.mov_z, (null, " = ", " (Z)") },
            { Mnemonic.mul, (null, " *= ", null) },
            { Mnemonic.neg, (null, " = -", null) },
            { Mnemonic.neg_cc, ("CC = !CC", null, null)},
            { Mnemonic.not, (null, " = ~", null) },
            { Mnemonic.JUMP_S, ("JUMP.S", null, null) },
            { Mnemonic.JUMP_L, ("JUMP.L", null, null) },
            { Mnemonic.sub, (null, " -= ", null) },

        };
    }
}
