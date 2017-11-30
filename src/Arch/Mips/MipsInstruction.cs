#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
            writer.WriteOpcode(GetOpcodeString(opcode));
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

        private string GetOpcodeString(Opcode opcode)
        {
            switch (opcode)
            {
            case Opcode.add_d: return "add.d";
            case Opcode.c_le_d: return "c.le.d";
            case Opcode.cvt_w_d: return "cvt.w.d";
            default: return opcode.ToString();
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
