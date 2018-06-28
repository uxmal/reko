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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public class Pdp11Instruction : MachineInstruction
    {
        private static Dictionary<Opcode, InstrClass> classOf;

        public Opcode Opcode;
        public PrimitiveType DataWidth;
        public MachineOperand op1;
        public MachineOperand op2;

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

        public override InstrClass InstructionClass
        {
            get
            {
                InstrClass ct;
                if (!classOf.TryGetValue(Opcode, out ct))
                {
                    ct = InstrClass.Linear;
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
                    writer.WriteString(",");
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
                writer.WriteString("#" + op.ToString());
            }
            else
            {
                op.Write(writer, options);
            }
        }

        static Pdp11Instruction()
        {
            classOf = new Dictionary<Opcode, InstrClass>
            {
                { Opcode.bcc,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bcs,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.beq,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bge,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bgt,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bhi,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.ble,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.blos,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.blt,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bmi,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bne,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bpl,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bpt,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.br,    InstrClass.Transfer },
                { Opcode.bvc,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bvs,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.halt,  InstrClass.Transfer },
                { Opcode.jmp,   InstrClass.Transfer },
                { Opcode.jsr,   InstrClass.Transfer },
                { Opcode.reset, InstrClass.Transfer },
                { Opcode.rti,   InstrClass.Transfer },
                { Opcode.rtt,   InstrClass.Transfer },
                { Opcode.rts,   InstrClass.Transfer },
                { Opcode.trap,  InstrClass.Transfer },
            };
        }
    }
}
