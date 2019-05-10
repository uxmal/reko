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

namespace Reko.Arch.Sparc
{
    public class SparcInstruction : MachineInstruction
    {
        public Opcode Opcode;
        public MachineOperand Op1;
        public MachineOperand Op2;
        public MachineOperand Op3;

        public override int OpcodeAsInteger => (int)Opcode; 

        public bool Annul => (InstructionClass & InstrClass.Annul) != 0;

        public override MachineOperand GetOperand(int i)
        {
            switch (i)
            {
            case 0: return Op1;
            case 1: return Op2;
            case 2: return Op3;
            default: return null; 
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(
                string.Format("{0}{1}",
                Opcode.ToString(),
                Annul ? ",a" : ""));

            if (Op1 != null)
            {
                writer.Tab();
                Write(Op1, writer, options);
                if (Op2 != null)
                {
                    writer.WriteChar(',');
                    Write(Op2, writer, options);
                    if (Op3 != null)
                    {
                        writer.WriteChar(',');
                        Write(Op3, writer, options);
                    }
                }
            }
        }

        private void Write(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var reg = op as RegisterOperand;
            if (reg != null)
            {
                writer.WriteFormat("%{0}", reg.Register.Name);
                return;
            }
            var imm = op as ImmediateOperand;
            if (imm != null)
            {
                writer.WriteString(imm.Value.ToString());
                return;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                mem.Write(writer, options);
                return;
            }
            var idx = op as IndexedMemoryOperand;
            if (idx != null)
            {
                idx.Write(writer, options);
                return;
            }
            writer.WriteString(op.ToString());
        }

        [Flags]
        public enum Flags
        {
            Annul = 1,
        }
    }
}
