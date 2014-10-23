#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public class SparcInstruction : MachineInstruction
    {
        public Opcode Opcode;
        public bool Annul;

        public MachineOperand Op1;
        public MachineOperand Op2;
        public MachineOperand Op3;

        public override void Render(MachineInstructionWriter writer)
        {
            writer.Opcode(
                string.Format("{0}{1}",
                Opcode.ToString(),
                Annul ? ",a" : ""));

            if (Op1 != null)
            {
                writer.Tab();
                Write(Op1, writer);
                if (Op2 != null)
                {
                    writer.Write(',');
                    Write(Op2, writer);
                    if (Op3 != null)
                    {
                        writer.Write(',');
                        Write(Op3, writer);
                    }
                }
            }
        }

        private void Write(MachineOperand op, MachineInstructionWriter writer)
        {
            var reg = op as RegisterOperand;
            if (reg != null)
            {
                writer.Write("%{0}", reg.Register.Name);
                return;
            }
            var imm = op as ImmediateOperand;
            if (imm != null)
            {
                writer.Write(imm.Value.ToString());
                return;
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                writer.Write("[%{0}", mem.Base.Name);
                if (!mem.Offset.IsNegative)
                {
                    writer.Write("+");
                }
                writer.Write(mem.Offset.ToString());
                writer.Write("]");
                return;
            }
            var idx = op as IndexedMemoryOperand;
            if (idx != null)
            {
                writer.Write("[%{0}+%{1}]", idx.Base, idx.Index);
                return;
            }
            writer.Write(op.ToString());
        }

        [Flags]
        public enum Flags
        {
            Annul = 1,
        }
    }
}
