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
using System.Threading.Tasks;

namespace Reko.Arch.Blackfin
{
    public class BlackfinInstruction : MachineInstruction
    {
        public InstrClass IClass;
        public Opcode Opcode;
        public MachineOperand[] Operands;

        public override InstrClass InstructionClass => IClass;

        public override int OpcodeAsInteger => (int) Opcode;

        public override MachineOperand GetOperand(int i)
        {
            return (0 <= i && i < Operands.Length)
                ? Operands[i]
                : null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (!mapOpcodes.TryGetValue(Opcode, out var sOpcode))
            {
                sOpcode = Opcode.ToString();
            }
            writer.WriteOpcode(sOpcode);
            var sep = " ";
            if (Operands == null)
                return;
            foreach (var op in Operands)
            {
                writer.WriteString(sep);
                sep = ",";
                op.Write(writer, options);
            }
            writer.WriteString(";");
        }

        private static readonly Dictionary<Opcode, string> mapOpcodes = new Dictionary<Opcode, string>
        {
            { Opcode.JUMP_S, "JUMP.S" },
        };
    }
}
