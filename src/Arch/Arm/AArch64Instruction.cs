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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public class AArch64Instruction : MachineInstruction
    {
        public static AArch64Instruction Create(AA64Opcode opcode, List<MachineOperand> ops)
        {
            var instr = new AArch64Instruction();
            instr.Opcode = opcode;
            if (ops.Count < 1)
                return instr;
            instr.op1 = ops[0];
            if (ops.Count < 2)
                return instr;
            instr.op2 = ops[1];
            if (ops.Count < 3)
                return instr;
            instr.op3 = ops[2];
            return instr;
        }

        public AA64Opcode Opcode { get; private set; }
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

        public override int OpcodeAsInteger { get { return (int)Opcode; } }

        public override void Render(MachineInstructionWriter writer)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 == null)
                return;
            writer.Tab();
            op1.Write(false, writer);
            if (op2 == null)
                return;
            writer.Write(",");
            op2.Write(false, writer);
            if (op3 == null)
                return;
            writer.Write(",");
            op3.Write(false, writer);
        }
    }
}
