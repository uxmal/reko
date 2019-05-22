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

namespace Reko.Arch.Vax
{
    public class VaxInstruction : MachineInstruction
    {
        public Opcode Opcode { get; internal set; }

        public MachineOperand[] Operands;

        public override int OpcodeAsInteger => (int)Opcode;

        public override MachineOperand GetOperand(int i)
        {
            if (i >= Operands.Length)
                return null;
            return Operands[i];
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(this.Opcode.ToString());
            writer.Tab();
            bool sep = false; 
            foreach (var op in Operands)
            {
                if (sep)
                    writer.WriteChar(',');
                sep = true;
                if (op is ImmediateOperand)
                {
                    writer.WriteChar('#');
                    op.Write(writer, options);
                }
                else if (op is MemoryOperand mop && mop.Base == Registers.pc)
                {
                    var addr = this.Address + this.Length;
                    if (mop.Offset != null)
                    {
                        addr += mop.Offset.ToInt32();
                    } 
                    if ((options & MachineInstructionWriterOptions.ResolvePcRelativeAddress) != 0)
                    {
                        writer.WriteAddress(addr.ToString(), addr);
                        writer.AddAnnotation(op.ToString());
                    }
                    else
                    {
                        op.Write(writer, options);
                        writer.AddAnnotation(addr.ToString());
                    }
                }
                else
                {
                    op.Write(writer, options);
                }
            }
        }
    }
}
