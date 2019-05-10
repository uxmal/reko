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
using System.Collections.Generic;
using System;

namespace Reko.Arch.Mips
{
    public class MipsInstruction : MachineInstruction
    {
        private static readonly Dictionary<Opcode, string> instrNames = new Dictionary<Opcode, string>
        {
            { Opcode.add_d,     "add.d" },
            { Opcode.add_s,     "add.s" },
            { Opcode.c_eq_d,    "c.eq.d" },
            { Opcode.c_eq_s,    "c.eq.s" },
            { Opcode.c_le_d,    "c.le.d" },
            { Opcode.c_le_s,    "c.le.s" },
            { Opcode.c_lt_d,    "c.lt.d" },
            { Opcode.c_lt_s,    "c.lt.s" },
            { Opcode.cvt_d_l,   "cvt.d.l" },
            { Opcode.cvt_s_d,   "cvt.s.d" },
            { Opcode.cvt_s_l,   "cvt.s.l" },
            { Opcode.cvt_w_d,   "cvt.w.d" },
            { Opcode.div_d,     "div.d" },
            { Opcode.div_s,     "div.s" },
            { Opcode.mov_d,     "mov.d" },
            { Opcode.mov_s,     "mov.s" },
            { Opcode.mul_d,     "mul.d" },
            { Opcode.mul_s,     "mul.s" },
            { Opcode.neg_d,     "neg.d" },
            { Opcode.neg_s,     "neg.s" },
            { Opcode.sub_d,     "sub.d" },
            { Opcode.sub_s,     "sub.s" },
            { Opcode.trunc_l_d, "trunc.l.d" },
        };

        public Opcode opcode;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

        public override int OpcodeAsInteger { get { return (int)opcode; } }

        public override MachineOperand GetOperand(int i)
        {
            switch (i)
            {
            case 0: return op1;
            case 1: return op2;
            case 2: return op3;
            default: return null;
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (!instrNames.TryGetValue(opcode, out string name))
            {
                name = opcode.ToString();
            }
            writer.WriteOpcode(name);

            if (op1 != null)
            {
                writer.Tab();
                op1.Write(writer, options);
                if (op2 != null)
                {
                    writer.WriteChar(',');
                    op2.Write(writer, options);
                    if (op3 != null)
                    {
                        writer.WriteChar(',');
                        op3.Write(writer, options);
                    }
                }
            }
        }
    }
}
