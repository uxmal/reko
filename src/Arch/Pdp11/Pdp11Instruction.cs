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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public class Pdp11Instruction : MachineInstruction
    {
        private static Dictionary<Opcode, InstructionClass> classOf;

        public Opcode Opcode;
        public PrimitiveType DataWidth;
        public MachineOperand op1;
        public MachineOperand op2;

        public override bool IsValid { get { return Opcode != Opcode.illegal; } }

        public override int OpcodeAsInteger { get { return (int)Opcode; } }

        public override MachineOperand GetOperand(int i)
        {
            if (i == 0)
                return op1;
            else if (i == 1)
                return op2;
            else
                return null;
        }

        public override InstructionClass InstructionClass
        {
            get
            {
                InstructionClass ct;
                if (!classOf.TryGetValue(Opcode, out ct))
                {
                    ct = InstructionClass.Linear;
                }
                return ct;
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 != null)
            {
                writer.Tab();
                OpToString(op1, options, writer);
                if (op2 != null)
                {
                    writer.Write(",");
                    OpToString(op2, options, writer);
                }
            }
        }

        private void OpToString(
            MachineOperand op,
            MachineInstructionWriterOptions options,
            MachineInstructionWriter writer)
        {
            if (op is ImmediateOperand)
            {
                writer.Write("#" + op.ToString());
            }
            else
            {
                op.Write(writer, options);
            }
        }

        static Pdp11Instruction()
        {
            classOf = new Dictionary<Opcode, InstructionClass>
            {
                { Opcode.bcc,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bcs,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.beq,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bge,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bgt,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bhi,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.ble,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.blos,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.blt,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bmi,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bne,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bpl,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bpt,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.br,    InstructionClass.Transfer },
                { Opcode.bvc,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bvs,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.halt,  InstructionClass.Transfer },
                { Opcode.jmp,   InstructionClass.Transfer },
                { Opcode.jsr,   InstructionClass.Transfer },
                { Opcode.reset, InstructionClass.Transfer },
                { Opcode.rti,   InstructionClass.Transfer },
                { Opcode.rtt,   InstructionClass.Transfer },
                { Opcode.rts,   InstructionClass.Transfer },
                { Opcode.trap,  InstructionClass.Transfer },
            };
        }
    }
}
