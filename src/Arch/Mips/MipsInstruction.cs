#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
        private const InstructionClass CondTransfer = InstructionClass.Conditional | InstructionClass.Transfer | InstructionClass.Delay;
        private const InstructionClass Linear = InstructionClass.Linear;
        private const InstructionClass Transfer = InstructionClass.Transfer | InstructionClass.Delay;

        private static Dictionary<Opcode, InstructionClass> classOf;

        public Opcode opcode;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

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

        public override void Render(MachineInstructionWriter writer)
        {
            writer.WriteOpcode(opcode.ToString());
            if (op1 != null)
            {
                writer.Tab();
                op1.Write(true, writer);
                if (op2 != null)
                {
                    writer.Write(',');
                    op2.Write(true, writer);
                    if (op3 != null)
                    {
                        writer.Write(',');
                        op3.Write(true, writer);
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
                { Opcode.beql,    CondTransfer },
                { Opcode.bgez,    CondTransfer },
                { Opcode.bgezal,  CondTransfer },
                { Opcode.bgezall, CondTransfer },
                { Opcode.bgezl,   CondTransfer },
                { Opcode.bgtz,    CondTransfer },
                { Opcode.bgtzl,   CondTransfer },
                { Opcode.blez,    CondTransfer },
                { Opcode.blezl,   CondTransfer },
                { Opcode.bltz,    CondTransfer },
                { Opcode.bltzal,  CondTransfer },
                { Opcode.bltzall, CondTransfer },
                { Opcode.bltzl,   CondTransfer },
                { Opcode.bne,     CondTransfer },
                { Opcode.bnel,    CondTransfer },
                { Opcode.@break,  Transfer },
                { Opcode.j,       Transfer },
                { Opcode.jal,     Transfer },
                { Opcode.jalr,    Transfer },
                { Opcode.jr,      Transfer },
                { Opcode.syscall, Transfer },
                { Opcode.teq,     CondTransfer },
                { Opcode.tlt,     CondTransfer },
                { Opcode.tltu,    CondTransfer },
                { Opcode.tge,     CondTransfer },
                { Opcode.tgeu,    CondTransfer },
                { Opcode.tne,     CondTransfer },
            };
        }
    }
}
