#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Collections.Generic;
using System;

namespace Reko.Arch.Mips
{
    public class MipsInstruction : MachineInstruction
    {
        private const InstructionClass LinkCondTransfer = InstructionClass.Conditional | InstructionClass.Call | InstructionClass.Transfer | InstructionClass.Delay;
        private const InstructionClass CondTransfer = InstructionClass.Conditional | InstructionClass.Transfer | InstructionClass.Delay;
        private const InstructionClass Linear = InstructionClass.Linear;
        private const InstructionClass Transfer = InstructionClass.Transfer | InstructionClass.Delay;
        private const InstructionClass LinkTransfer = InstructionClass.Transfer | InstructionClass.Call | InstructionClass.Delay;

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

        private static Dictionary<Opcode, InstructionClass> classOf;

        public Opcode opcode;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

        public override bool IsValid { get { return opcode != Opcode.illegal; } }

        public override int OpcodeAsInteger { get { return (int)opcode; } }

        public override InstructionClass InstructionClass
        {
            get
            {
                InstructionClass ct;
                if (!classOf.TryGetValue(opcode, out ct))
                {
                    ct = Linear;
                }
                return ct;
            }
        }

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
            string name;
            if (!instrNames.TryGetValue(opcode, out name))
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
                    writer.Write(',');
                    op2.Write(writer, options);
                    if (op3 != null)
                    {
                        writer.Write(',');
                        op3.Write(writer, options);
                    }
                }
            }
        }

 

        static MipsInstruction()
        {
            classOf = new Dictionary<Opcode, InstructionClass>
            {
                { Opcode.illegal, InstructionClass.Invalid },

                { Opcode.beq,     CondTransfer },
                { Opcode.beql,    LinkCondTransfer },
                { Opcode.bgez,    CondTransfer },
                { Opcode.bgezal,  LinkCondTransfer },
                { Opcode.bgezall, LinkCondTransfer },
                { Opcode.bgezl,   LinkCondTransfer },
                { Opcode.bgtz,    CondTransfer },
                { Opcode.bgtzl,   LinkCondTransfer },
                { Opcode.blez,    CondTransfer },
                { Opcode.blezl,   LinkCondTransfer },
                { Opcode.bltz,    CondTransfer },
                { Opcode.bltzal,  LinkCondTransfer },
                { Opcode.bltzall, LinkCondTransfer },
                { Opcode.bltzl,   CondTransfer },
                { Opcode.bne,     CondTransfer },
                { Opcode.bnel,    LinkCondTransfer },
                { Opcode.@break,  Transfer },
                { Opcode.j,       Transfer },
                { Opcode.jal,     LinkTransfer },
                { Opcode.jalr,    LinkTransfer },
                { Opcode.jr,      Transfer },
                { Opcode.syscall, LinkTransfer },
                { Opcode.teq,     LinkCondTransfer },
                { Opcode.tlt,     LinkCondTransfer },
                { Opcode.tltu,    LinkCondTransfer },
                { Opcode.tge,     LinkCondTransfer },
                { Opcode.tgeu,    LinkCondTransfer },
                { Opcode.tne,     LinkCondTransfer },
            };
        }
    }
}
