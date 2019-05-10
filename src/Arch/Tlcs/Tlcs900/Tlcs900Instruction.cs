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
using System.Linq;
using System.Text;
using Opcode = Reko.Arch.Tlcs.Tlcs900.Tlcs900Opcode;

namespace Reko.Arch.Tlcs.Tlcs900
{
    public class Tlcs900Instruction : MachineInstruction
    {
        public Opcode Opcode;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

        public override int OpcodeAsInteger => (int) Opcode;

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 != null)
            {
                writer.Tab();
                op1.Write(writer, options);
                if (op2 != null)
                {
                    writer.WriteString(",");
                    op2.Write(writer, options);
                    if (op3 != null)
                    {
                        writer.WriteString(",");
                        op3.Write(writer, options);
                    }
                }
            }
        }
    }
}
