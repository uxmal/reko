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

namespace Reko.Arch.Z80
{
    public class Z80Instruction : MachineInstruction
    {
        public Opcode Code;
        public MachineOperand Op1;
        public MachineOperand Op2;

        public override int OpcodeAsInteger => (int)Code;

        public override MachineOperand GetOperand(int i)
        {
            if (i == 0)
                return Op1;
            else if (i == 1)
                return Op2;
            else
                return null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (Code == Opcode.ex_af)
            {
                writer.WriteOpcode("ex");
                writer.Tab();
                writer.WriteString("af,af'");
                return;
            }
            writer.WriteOpcode(Code.ToString());
            if (Op1 != null)
            {
                writer.Tab();
                Op1.Write(writer, options);
                if (Op2 != null)
                {
                    writer.WriteString(",");
                    Op2.Write(writer, options);
                }
            }
        }
    }
}
