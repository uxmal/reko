#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core;
using System;
using Reko.Core.Types;

namespace Reko.Arch.Msp430
{
    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public short Offset;
        public bool PostIncrement;

        public MemoryOperand(PrimitiveType dataWidth) : base(dataWidth) { }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (Offset > 0)
            {
                if (Base == Registers.pc && (options & MachineInstructionWriterOptions.ResolvePcRelativeAddress) != 0)
                {
                    var addr = writer.Address + 4 + Offset;
                    writer.WriteAddress(addr.ToString(), addr);
                }
                else
                {
                    writer.WriteFormat("{0:X4}({1})", Offset, Base.Name);
                }
            }
            else if (Offset < 0)
            {
                if (Base == Registers.pc && (options & MachineInstructionWriterOptions.ResolvePcRelativeAddress) != 0)
                {
                    var addr = writer.Address + 4 + Offset;
                    writer.WriteAddress(addr.ToString(), addr);
                }
                else
                {
                    writer.WriteFormat("-{0:X4}({1})", -Offset, Base.Name);
                }
            }
            else
            {
                writer.WriteChar('@');
                writer.WriteString(Base.Name);
                if (PostIncrement)
                    writer.WriteChar('+');
            }
        }
    }
}