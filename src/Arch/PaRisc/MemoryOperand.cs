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

using System;
using System.Text;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.PaRisc
{
    public class MemoryOperand : MachineOperand
    {
        public int Offset;
        public RegisterStorage Base;
        public RegisterStorage Index;
        public RegisterStorage Space;

        public MemoryOperand(PrimitiveType dt, int disp, RegisterStorage baseReg, RegisterStorage idxReg, RegisterStorage spaceReg) :
            base(dt)
        {
            this.Offset = disp;
            this.Base = baseReg;
            this.Index = idxReg;
            this.Space= spaceReg;
        }

        public static MemoryOperand Indirect(PrimitiveType dt, int offset, RegisterStorage baseReg, RegisterStorage spaceReg = null)
        {
            return new MemoryOperand(dt, offset, baseReg, null, spaceReg);
        }

        public static MemoryOperand Indexed(PrimitiveType dt, RegisterStorage baseReg, RegisterStorage idxReg, RegisterStorage spaceReg = null)
        {
            return new MemoryOperand(dt, 0, baseReg, idxReg, spaceReg);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var sb = new StringBuilder();
            if (Index != null)
            {
                sb.AppendFormat("{0}(", Index.Name, Base.Name);
            }
            else
            {
                sb.AppendFormat("{0}(", Offset, Base.Name);
            }
            if (Space != null && Space != Registers.SpaceRegs[0])
                sb.AppendFormat("{0},", Space.Name);
            sb.AppendFormat("{0})", Base.Name);
            writer.WriteString(sb.ToString());
        }
    }
}