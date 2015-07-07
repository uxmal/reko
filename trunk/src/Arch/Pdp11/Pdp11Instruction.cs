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

using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Pdp11Instruction : MachineInstruction
    {
        public Opcodes Opcode;
        public PrimitiveType DataWidth;
        public MachineOperand op1;
        public MachineOperand op2;

        public override int OpcodeAsInteger { get { return (int)Opcode; } }

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
    }

    public enum Opcodes
    {
        illegal = -1,

        adc,
        add,
        asl,
        asr,
        bic,
        bis,
        bit,
        cmp,
        dec,
        clr,
        com,
        div,
        inc,
        mark,
        mfpd,
        mfpi,
        mfps,
        mtps,
        mtpi,
        mtpd,
        mov,
        movb,
        mul,
        neg,
        rol,
        ror,
        sbc,
        sub,
        swab,
        sxt,
        tst,
        br,
        bne,
        bcs,
        bcc,
        bvs,
        bvc,
        blos,
        bhi,
        bmi,
        bgt,
        blt,
        bge,
        beq,
        ble,
        bpl,
        xor,
        halt,
        wait,
        emt,
        trap,
        reset,
        iot,
        bpt,
        rti,
        rtt,
        rts,
        sob,
        ashc,
        jmp,
        jsr,
        ash,
        spl,
        clrb,
    }
}
