#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
        public InstrClass IClass;
        public Opcode Opcode;
        public MachineOperand[] Operands;

        public override InstrClass InstructionClass => IClass;

        public override int OpcodeAsInteger => (int) Opcode;

        public override MachineOperand GetOperand(int i)
        {
            return (0 <= i && i < Operands.Length)
                ? Operands[i]
                : null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            (string prefix, string infix, string suffix) aaOpcode;
            if (!mapOpcodes.TryGetValue(Opcode, out aaOpcode))
            {
                aaOpcode.prefix = Opcode.ToString();
            }
            if (aaOpcode.infix != null)
            {
                writer.WriteString(aaOpcode.prefix);
                Operands[0].Write(writer, options);
                if (Operands.Length > 2)
                {
                    writer.WriteString(" = ");
                    Operands[1].Write(writer, options);
                    writer.WriteString(aaOpcode.infix);
                    Operands[2].Write(writer, options);
                }
                else if (Operands.Length > 1)
                {
                    writer.WriteString(aaOpcode.infix);
                    Operands[1].Write(writer, options);
                }
                else
                {
                    writer.WriteString(aaOpcode.infix);
                }
            }
            else
            {
                writer.WriteOpcode(aaOpcode.prefix);
                var sep = " ";
                if (Operands == null)
                    return;
                foreach (var op in Operands)
                {
                    writer.WriteString(sep);
                    sep = ",";
                    op.Write(writer, options);
                }
            }
            if (aaOpcode.suffix != null)
            {
                writer.WriteString(aaOpcode.suffix);
            }
            writer.WriteString(";");
        }

        private static readonly Dictionary<Opcode, (string,string,string)> mapOpcodes = 
            new Dictionary<Opcode, (string, string, string)>
        {
            { Opcode.add, (null, " += ", null) },
            { Opcode.add3, (null, " + ", null) },
            { Opcode.add_sh1, (null, " + ", " << 1") },
            { Opcode.add_sh2, (null, " + ", " << 2") },
            { Opcode.and3, (null, " & ", null) },
            { Opcode.or3, (null, " | ", null) },
            { Opcode.sub3, (null, " - ", null) },
            { Opcode.xor3, (null, " ^ ", null) },
            { Opcode.asr, (null, " >>>= ", null) },
            { Opcode.asr3, (null, " >>> ", null) },
            { Opcode.DIVQ, ("DIVQ(", null, ")") },
            { Opcode.lsl, (null, " <<= ", null) },
            { Opcode.lsl3, (null, " << ", null) },
            { Opcode.lsr, (null, " >>= ", null) },
            { Opcode.lsr3, (null, " >> ", null) },
            { Opcode.bitset, ( "BITSET(", ",", ")") },
            { Opcode.bittgl, ( "BITTGL(", ",", ")") },
            { Opcode.bitclr, ( "BITCLR(", ",", ")") },
            { Opcode.if_cc_jump, ("IF CC JUMP", null, null) },
            { Opcode.if_cc_jump_bp, ("IF CC JUMP", null, " (BP)") },
            { Opcode.if_cc_mov, ("IF CC ", " = ", null) },
            { Opcode.if_ncc_mov, ("IF !CC ", " = ", null) },
            { Opcode.if_ncc_jump, ("IF !CC JUMP", null, null) },
            { Opcode.if_ncc_jump_bp, ("IF !CC JUMP", null, " (BP)") },
            { Opcode.mov, (null, " = ", null) },
            { Opcode.mov_cc_eq, ("CC = ", " == ", null) },
            { Opcode.mov_cc_le, ("CC = ", " <= ", null) },
            { Opcode.mov_cc_lt, ("CC = ", " < ", null) },
            { Opcode.mov_cc_ule, ("CC = ", " <= ", null) },
            { Opcode.mov_cc_ult, ("CC = ", " < ", null) },
            { Opcode.mov_cc_bittest, ( "CC = BITTEST(", ",", ")" )},
            { Opcode.mov_cc_n_bittest, ( "CC = !BITTEST(", ",", ")" )},
            { Opcode.mov_r_cc, (null, " = ", "CC") },
            { Opcode.mov_xb, (null, " = ", ".B (X)") },
            { Opcode.mov_xl, (null, " = ", ".L (X)") },
            { Opcode.mov_zb, (null, " = ", ".B (Z)") },
            { Opcode.mov_zl, (null, " = ", ".L (Z)") },
            { Opcode.mov_x, (null, " = ", " (X)") },
            { Opcode.mov_z, (null, " = ", " (Z)") },
            { Opcode.mul, (null, " *= ", null) },
            { Opcode.neg, (null, " = -", null) },
            { Opcode.neg_cc, ("CC = !CC", null, null)},
            { Opcode.not, (null, " = ~", null) },
            { Opcode.JUMP_S, ("JUMP.S", null, null) },
            { Opcode.JUMP_L, ("JUMP.L", null, null) },
            { Opcode.sub, (null, " -= ", null) },

        };
    }
}
