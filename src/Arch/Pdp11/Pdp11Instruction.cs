#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        private static Dictionary<Opcodes, InstructionClass> classOf;

        public Opcodes Opcode;
        public PrimitiveType DataWidth;
        public MachineOperand op1;
        public MachineOperand op2;

        public override bool IsValid { get { return Opcode != Opcodes.illegal; } }

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

        public override void Render(MachineInstructionWriter writer)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 != null)
            {
                writer.Tab();
                OpToString(op1, writer);
                if (op2 != null)
                {
                    writer.Write(",");
                    OpToString(op2, writer);
                }
            }
        }

        private void OpToString(MachineOperand op, MachineInstructionWriter writer)
        {
            if (op is ImmediateOperand)
            {
                writer.Write("#" + op.ToString());
            }
            else
            {
                writer.Write(op.ToString());
            }
        }

        static Pdp11Instruction()
        {
            classOf = new Dictionary<Opcodes, InstructionClass>
            {
                { Opcodes.bcc,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bcs,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.beq,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bge,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bgt,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bhi,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.ble,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.blos,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.blt,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bmi,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bne,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bpl,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bpt,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.br,    InstructionClass.Transfer },
                { Opcodes.bvc,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.bvs,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcodes.halt,  InstructionClass.Transfer },
                { Opcodes.jmp,   InstructionClass.Transfer },
                { Opcodes.jsr,   InstructionClass.Transfer },
                { Opcodes.reset, InstructionClass.Transfer },
                { Opcodes.rti,   InstructionClass.Transfer },
                { Opcodes.rtt,   InstructionClass.Transfer },
                { Opcodes.rts,   InstructionClass.Transfer },
                { Opcodes.trap,  InstructionClass.Transfer },
            };
        }
    }

    public enum Opcodes
    {
        illegal = -1,

        adc,
        add,
        addb,
        asl,
        asr,
        bcc,
        bcs,
        beq,
        bge,
        bgt,
        bhi,
        bic,
        bis,
        bisb,
        bit,
        bitb,
        ble,
        blos,
        blt,
        bmi,
        bne,
        bpl,
        bpt,
        br,
        bvc,
        bvs,
        clr,
        clrflags,
        cmp,
        com,
        dec,
        div,

        emt,
        halt,
        inc,
        iot,
        ash,
        clrb,
        ashc,
        jmp,
        jsr,
        mark,
        mov,
        movb,
        mul,
        mfpd,
        mfpi,
        mfps,
        mtps,
        mtpi,
        mtpd,
        neg,
        nop,

        reset,
        rol,
        ror,

        rti,
        rtt,
        rts,
        sbc,
        setflags,
        sob,
        spl,
        sub,
        swab,
        sxt,
        trap,
        tst,
        tstb,
        wait,
        xor,
    }
}
