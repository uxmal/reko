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

using Reko.Core.Machine;

namespace Reko.Arch.PowerPC
{
    using Decoder = Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>;
    using static Reko.Arch.PowerPC.PowerPcDisassembler;

    public class PPC750clInstructionSet : InstructionSet
    {
        public override Decoder Ext4Decoder()
        {
            var decoder = Mask(26, 5,
                Sparse(21, 5,
                    (1, Instr(Mnemonic.ps_cmpo0, c1, f2, f3))
                    ),
                Nyi("0b00001"),
                Nyi("0b00010"),
                Nyi("0b00011"),

                Nyi("0b00100"),
                Nyi("0b00101"),
                Mask(25, 1,
                    Instr(Mnemonic.psq_lx, f1, r2, r3, u21_1, u22_3),
                    Instr(Mnemonic.psq_lux, f1, r2, r3, u21_1, u22_3)),
                Mask(25, 1,
                    Instr(Mnemonic.psq_stx, f1, r2, r3, u21_1, u22_3),
                    Instr(Mnemonic.psq_stux, f1, r2, r3, u21_1, u22_3)),

                Sparse(21, 5,
                    (1, Instr(Mnemonic.ps_neg, C, f1, f3)),
                    (2, Instr(Mnemonic.ps_mr, C, f1, f3)),
                    (4, Instr(Mnemonic.ps_nabs, C, f1, f3)),
                    (8, Instr(Mnemonic.ps_abs, C, f1, f3))),
                Nyi("0b01001"),
                Instr(Mnemonic.ps_sum0, C, f1, f2, f4, f3),
                Instr(Mnemonic.ps_sum1, C, f1, f2, f4, f3),

                Instr(Mnemonic.ps_muls0, C, f1, f2, f4),
                Instr(Mnemonic.ps_muls1, C, f1, f2, f4),
                Instr(Mnemonic.ps_madds0, C, f1, f2, f4, f3),
                Instr(Mnemonic.ps_madds1, C, f1, f2, f4, f3),

                Sparse(21, 5,
                    (0b10000, Instr(Mnemonic.ps_merge00, C, f1, f2, f3)),
                    (0b10001, Instr(Mnemonic.ps_merge01, C, f1, f2, f3)),
                    (0b10010, Instr(Mnemonic.ps_merge10, C, f1, f2, f3)),
                    (0b10011, Instr(Mnemonic.ps_merge11, C, f1, f2, f3))),
                Nyi("0b10001"),
                Instr(Mnemonic.ps_div, C, f1, f2, f3),
                Nyi("0b10011"),

                Instr(Mnemonic.ps_sub, C, f1, f2, f3),
                Instr(Mnemonic.ps_add, C, f1, f2, f3),
                Nyi("0b10110"),
                Instr(Mnemonic.ps_sel, C, f1, f2, f4, f3),

                Instr(Mnemonic.ps_res, C, f1, f3),
                Instr(Mnemonic.ps_mul, C, f1, f2, f4),
                Instr(Mnemonic.ps_rsqrte, C, f1, f3),
                Nyi("0b11011"),

                Instr(Mnemonic.ps_msub, C, f1, f2, f4, f3),
                Instr(Mnemonic.ps_madd, C, f1, f2, f4, f3),
                Instr(Mnemonic.ps_nmsub, C, f1, f2, f4, f3),
                Instr(Mnemonic.ps_nmadd, C, f1, f2, f4, f3));
            return decoder;
        }

        public override Decoder Ext38Decoder()
        {
            return Instr(Mnemonic.psq_l, f1, r2, s0_12, u21_1, u22_3);
        }

        public override Decoder Ext39Decoder()
        {
            return Instr(Mnemonic.psq_lu, f1, r2, s0_12, u21_1, u22_3);
        }

        public override Decoder Ext3CDecoder()
        {
            return Instr(Mnemonic.psq_st, f1, r2, s0_12, u21_1, u22_3);
        }

        public override Decoder Ext3DDecoder()
        {
            return Instr(Mnemonic.psq_stu, f1, r2, s0_12, u21_1, u22_3);
        }
    }
}