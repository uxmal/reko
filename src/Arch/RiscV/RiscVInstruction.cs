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
        internal MachineOperand op1;
        internal MachineOperand op2;
        internal MachineOperand op3;
        internal MachineOperand op4;

        static RiscVInstruction()
        {
            opcodeNames = new Dictionary<Opcode, string>
            {
                { Opcode.c_add, "c.add" },
                { Opcode.c_addi, "c.addi" },
                { Opcode.c_addiw, "c.addiw" },
                { Opcode.c_addw, "c.addw" },
                { Opcode.c_and, "c.and" },
                { Opcode.c_andi, "c.andi" },
                { Opcode.c_beqz,  "c.beqz" },
                { Opcode.c_bnez,  "c.bnez" },
                { Opcode.c_addi16sp, "c.addi16sp" },
                { Opcode.c_addi4spn, "c.addi4spn" },
                { Opcode.c_fld, "c.fld" },
                { Opcode.c_fldsp, "c.fldsp" },
                { Opcode.c_flw, "c.flw" },
                { Opcode.c_flwsp, "c.flwsp" },
                { Opcode.c_fsd, "c.fsd" },
                { Opcode.c_fsdsp, "c.fsdsp" },
                { Opcode.c_j, "c.j" },
                { Opcode.c_jr, "c.jr" },
                { Opcode.c_jalr, "c.jalr" },
                { Opcode.c_ld, "c.ld" },
                { Opcode.c_ldsp, "c.ldsp" },
                { Opcode.c_lwsp, "c.lwsp" },
                { Opcode.c_li, "c.li" },
                { Opcode.c_lui, "c.lui" },
                { Opcode.c_lw, "c.lw" },
                { Opcode.c_mv, "c.mv" },
                { Opcode.c_or, "c.or" },
                { Opcode.c_sdsp, "c.sdsp" },
                { Opcode.c_swsp, "c.swsp" },
                { Opcode.c_slli, "c.slli" },
                { Opcode.c_sd, "c.sd" },
                { Opcode.c_srai, "c.srai" },
                { Opcode.c_srli, "c.srli" },
                { Opcode.c_sub, "c.sub" },
                { Opcode.c_subw, "c.subw" },
                { Opcode.c_sw, "c.sw" },
                { Opcode.c_xor, "c.xor" },
                { Opcode.fadd_s, "fadd.s" },
                { Opcode.fadd_d, "fadd.d" },
                { Opcode.fcvt_d_s, "fcvt.d.s" },
                { Opcode.fcvt_s_d, "fcvt.s.d" },
                { Opcode.feq_s, "feq.s" },
                { Opcode.fmadd_s, "fmadd.s" },
                { Opcode.fmsub_s,  "fmsub.s" },
                { Opcode.fnmsub_s, "fnmsub.s" },
                { Opcode.fnmadd_s, "fnmadd.s" },
                { Opcode.fmv_d_x, "fmv.d.x" },
                { Opcode.fmv_s_x, "fmv.s.x" },
            };
        }

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
            WriteOp(op1, writer, options);
            if (op2 == null)
                return;
            writer.WriteChar(',');
            WriteOp(op2, writer, options);
            if (op3 == null)
                return;
            writer.WriteChar(',');
            WriteOp(op3, writer, options);
            if (op4 == null)
                return;
            writer.WriteChar(',');
            WriteOp(op4, writer, options);
        }

        private void WriteOp(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case RegisterOperand rop:
                writer.WriteString(rop.Register.Name);
                return;
            case ImmediateOperand immop:
                immop.Write(writer, options);
                return;
            case AddressOperand addrop:
                //$TODO: 32-bit?
                writer.WriteAddress(string.Format("{0:X16}", addrop.Address.ToLinear()), addrop.Address);
                return;
            case MemoryOperand memop:
                memop.Write(writer, options);
                return;
            }
            throw new NotImplementedException($"Risc-V operand type {op.GetType().Name} not implemented yet.");
        }
    }
}