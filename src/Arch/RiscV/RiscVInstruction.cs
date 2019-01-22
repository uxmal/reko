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

namespace Reko.Arch.RiscV
{
    public class RiscVInstruction : MachineInstruction
    {
        private static Dictionary<Opcode, string> opcodeNames;

        internal Opcode opcode;
        internal InstrClass iclass;
        internal MachineOperand op1;
        internal MachineOperand op2;
        internal MachineOperand op3;
        internal MachineOperand op4;

        static RiscVInstruction()
        {
            opcodeNames = new Dictionary<Opcode, string>
            {
                { Opcode.c_addi4spn, "c.addi4spn" },
                { Opcode.c_slli, "c.slli" },
                { Opcode.fadd_s, "fadd.s" },
                { Opcode.fadd_d, "fadd.d" },
                { Opcode.fcvt_d_s, "fcvt.d.s" },
                { Opcode.fcvt_s_d, "fcvt.s.d" },
                { Opcode.feq_s, "feq.s" },
                { Opcode.fmadd_s, "fmadd.s" },
                { Opcode.fmv_d_x, "fmv.d.x" },
                { Opcode.fmv_s_x, "fmv.s.x" },
            };
        }

        public override InstrClass InstructionClass => iclass;

        public override int OpcodeAsInteger { get { return (int)opcode; } }

        public override MachineOperand GetOperand(int i)
        {
            if (i == 0)
                return op1;
            else if (i == 1)
                return op2;
            else if (i == 2)
                return op3;
            else if (i == 3)
                return op4;
            return null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (!opcodeNames.TryGetValue(opcode, out string name))
            {
                name = opcode.ToString();
            }
            writer.WriteOpcode(name);
            if (op1 == null)
                return;
            writer.Tab();
            WriteOp(op1, writer);
            if (op2 == null)
                return;
            writer.WriteChar(',');
            WriteOp(op2, writer);
            if (op3 == null)
                return;
            writer.WriteChar(',');
            WriteOp(op3, writer);
            if (op4 == null)
                return;
            writer.WriteChar(',');
            WriteOp(op4, writer);
        }

        private void WriteOp(MachineOperand op, MachineInstructionWriter writer)
        {
            switch (op)
            {
            case RegisterOperand rop:
                writer.WriteString(rop.Register.Name);
                return;
            case ImmediateOperand immop:
                writer.WriteString(immop.Value.ToString());
                return;
            case AddressOperand addrop:
                //$TODO: 32-bit?
                writer.WriteAddress(string.Format("{0:X16}", addrop.Address.ToLinear()), addrop.Address);
                return;
            }
            throw new NotImplementedException($"Risc-V operand type {op.GetType().Name} not implemented yet.");
        }
    }
}